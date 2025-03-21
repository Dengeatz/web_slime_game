using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TestTask.Assets.Game._Scripts.Managers;
using TestTask.Enums;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace TestTask.Core
{
    public class Bulb : MonoBehaviour
    {
        private const float OFFSET_Y = 1f;
        
        [SerializeField] private int _maxCountOfSlimes = 5;
        [SerializeField] private Transform _startPosition;
        [SerializeField] private SpriteManager _spriteManager;
        [SerializeField] private SpriteRenderer _bulbSprite;
        [SerializeField] private SpriteRenderer _bulbFullySprite;
        [SerializeField] private SpriteRenderer _bulbShakeSprite;
        [SerializeField] private SpriteRenderer _closeSprite;
        public bool IsLocked;
        private List<Slime> _slimesInBulb = new();
        private Vector3 _oldPosition;
        private int _currentIndex = 0;
        private Vector3 _oldClosePosition;

        #region Unity Methods
        private void Awake()
        {
            _oldPosition = transform.position;
        }
        #endregion

        #region Public Methods
        public void InitSlimes(List<Slime> slimes)
        {
            foreach (var slime in slimes)
            {
                var gObj = UnityEngine.Object.Instantiate(slime, Vector3.zero, Quaternion.identity, null);
                AddSlime(gObj, true);
            }
        }

        public void Select()
        {
            this.transform.DOMove(_oldPosition + (this.transform.up * 0.2f), 0.15f);
        }

        public int GetSlimesCount()
        {
            return _slimesInBulb.Count;
        }

        public bool IsFull()
        {
            return _slimesInBulb.Count == _maxCountOfSlimes;
        }

        public void FillToAnotherBulb(Bulb bulb, Action callback)
        {
            StartCoroutine(FillAnimationBulb(bulb, callback));
        }

        public IEnumerator FillAnimationBulb(Bulb bulb, Action callback)
        {
            int addedSlime = 0;
            float timer = 0f;
            var needToFill = _maxCountOfSlimes - (_maxCountOfSlimes - GetSlimesCount());
            SlimeColor slimeColor = _slimesInBulb[^1].SlimeColorType;

            this.transform.DOMove(bulb.transform.position + (bulb.transform.up * 6f) + (-bulb.transform.right * 2.3f), 0.5f);
            this.transform.DORotate(new Vector3(0f, 0f, -140f), 0.5f);
            
            while (timer < 0.5f)
            {
                timer += Time.deltaTime;
                foreach (var slime in _slimesInBulb)
                {
                    slime.DisableSimulation();
                    slime.SetPosition(GetPositionInBulb(slime.BulbIndex));
                }
                yield return null;
            }
            
            if (needToFill > _maxCountOfSlimes)
            {
                needToFill = Mathf.Abs((_slimesInBulb.Count - bulb.GetSlimesCount()));
            }

            for (int i = _slimesInBulb.Count - 1; i > (_slimesInBulb.Count - needToFill) - 1; i--)
            {
                if (!_slimesInBulb[i].SlimeColorType.Equals(slimeColor)) break;
                _slimesInBulb[i].EnableSimulation();
                _slimesInBulb[i].EnableStrongGravity();
            }

            if (!bulb.IsEmpty())
            {
                foreach (var slime in bulb._slimesInBulb)
                {
                    slime.EnableStrongMass();
                    slime.EnableSimulation();
                }
            }

            yield return new WaitForSeconds(3.5f);

            _startPosition.rotation = Quaternion.identity;
            this.transform.DORotateQuaternion(Quaternion.identity, 0.5f);
            this.transform.DOMove(_oldPosition, 0.5f);


            for (int i = _slimesInBulb.Count - 1; i > (_slimesInBulb.Count - needToFill) - 1; i--)
            {
                if (!_slimesInBulb[i].SlimeColorType.Equals(slimeColor)) break;
                bulb.AddSlime(_slimesInBulb[i], false);
                addedSlime++;
                yield return null;
            }

            var slimeCount = _slimesInBulb.Count;
            
            for (int i = slimeCount - 1; i > (slimeCount - addedSlime) - 1; i--)
            {
                _slimesInBulb.RemoveAt(i);
                _currentIndex--;
            }

            foreach (var s in bulb._slimesInBulb)
            {
                s.DisableStrongMass();
            }

            timer = 0f;

            while (timer < 0.5f)
            {
                timer += Time.deltaTime;
                foreach (var slime in _slimesInBulb)
                {
                    slime.DisableSimulation();
                    slime.SetPosition(GetPositionInBulb(slime.BulbIndex));
                }
                yield return null;
            }
            
            foreach(var slime in _slimesInBulb)
            {
                slime.EnableSimulation();
            }
         
            callback?.Invoke();
        }

        public bool IsEmpty()
        {
            return _slimesInBulb.Count == 0;
        }

        public void Deselect()
        {
            this.transform.DOMove(_oldPosition, 0.15f);
        }

        public void AddSlime(Slime slime, bool setPosition)
        {
            _currentIndex++;

            if (setPosition)
            {
                var newSlimePosition = GetPositionInBulb(_currentIndex);
                slime.SetPosition(newSlimePosition);
            }
            slime.DisableStrongGravity();
            slime.BulbIndex = _currentIndex;
            _slimesInBulb.Add(slime);
            if (_slimesInBulb.Count == _maxCountOfSlimes)
            {
                var firstColor = _slimesInBulb[0].SlimeColorType;

                foreach (var s in _slimesInBulb)
                {
                    if (s.SlimeColorType != firstColor) return;
                }

                BulbFilled();
            }
        }
        #endregion

        #region Private Methods
        private void BulbFilled()
        {
            IsLocked = true;
            _closeSprite.gameObject.SetActive(true);
            _oldClosePosition = _closeSprite.transform.position;
            _closeSprite.transform.position += _closeSprite.transform.up * 0.5f;
            _bulbShakeSprite.sprite = _spriteManager.GetBulbShakeSprite(_slimesInBulb[0]);
            StartCoroutine(BulbAnimationFilled());
        }

        private IEnumerator BulbAnimationFilled()
        {
            yield return _closeSprite.transform.DOMove(_oldClosePosition, 1f).SetEase(Ease.InOutQuint).WaitForCompletion();

            yield return new WaitForSeconds(2f);

            float timer = 0f;
            Vector3 oldPos = this.transform.position;
            _bulbSprite.DOFade(0f, 2f);
            _bulbShakeSprite.DOFade(1f, 2f);

            foreach (var slime in _slimesInBulb)
            {
                slime.gameObject.GetComponent<SpriteRenderer>().DOFade(0f, 1f);
            }

            while (timer < 2f)
            {
                this.transform.position += (this.transform.up * (Mathf.Sin(timer * Mathf.Cos(timer * 50f)) * 0.2f));
                timer += Time.deltaTime;
                yield return null;
            }

            _bulbShakeSprite.DOFade(0f, 2f);
            _bulbFullySprite.DOFade(1f, 2f);
            _bulbFullySprite.sprite = _spriteManager.GetBulbFullySprite(_slimesInBulb[0]);
            this.transform.DOMove(oldPos, 0.6f);
        }

        private Vector3 GetPositionInBulb(int index)
        {
            return CalculatePositionInBulb(index);
        }

        private Vector3 CalculatePositionInBulb(int index)
        {
            var position = _startPosition.position + (this.transform.up * (index * OFFSET_Y));
            return position;
        }
        #endregion
    }
}
