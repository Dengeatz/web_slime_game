using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TestTask.Enums;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace TestTask.Core
{
    public class Slime : MonoBehaviour
    {
        [SerializeField] public SlimeColor SlimeColorType;
        [SerializeField] private List<Rigidbody2D> _rigidbodies;
        [NonSerialized] public SpriteSkin SlimeSpriteSkin;
        [NonSerialized] public int BulbIndex;
        [NonSerialized] public Vector3 OldPosition;
        [NonSerialized] public Transform RootBone;
        private List<Vector3> _rootBonePositions = new();
        private List<Quaternion> _rootBoneRotations = new();
        private bool _simulationDisabled = false;

        #region Unity Methods
        private void Awake()
        {
            SlimeSpriteSkin = this.transform.GetComponent<SpriteSkin>();
            RootBone = SlimeSpriteSkin.rootBone;

            for(int i = 0; i < SlimeSpriteSkin.boneTransforms.Count(); i++)
            {
                _rootBonePositions.Add(SlimeSpriteSkin.boneTransforms[i].localPosition);
                _rootBoneRotations.Add(SlimeSpriteSkin.boneTransforms[i].localRotation);
            }
        }

        private void Update()
        {
            if (_simulationDisabled)
                ResetBones();

        }
        #endregion

        #region Public Methods

        public void DisableStrongMass()
        {
            foreach(var rb in _rigidbodies)
            {
                rb.mass = 1f;
            }
        }

        public void EnableStrongMass()
        {
            foreach(var rb in _rigidbodies)
            {
                rb.mass = 1000f;
            }
        }

        public void DisableStrongGravity()
        {
            foreach(var rb in _rigidbodies)
            {
                rb.gravityScale = 0.1f;
            }
        }

        public void EnableStrongGravity()
        {
            foreach(var rb in _rigidbodies)
            {
                rb.gravityScale = 0.5f;
            }
        }
        public void SetPosition(Vector3 position)
        {
            this.transform.position = position;
        }

        public void DisableSimulation()
        {
            _simulationDisabled = true;

            foreach (var rb in _rigidbodies)
            {
                rb.Sleep();
            }

            ResetBones();
        }

        public void EnableSimulation()
        {
            _simulationDisabled = false;

            foreach (var rb in _rigidbodies)
            {
                rb.simulated = true;
            }
        }
        #endregion

        #region Private Methods
        private void ResetBones()
        {
            for (int i = 0; i < SlimeSpriteSkin.boneTransforms.Count(); i++)
            {
                SlimeSpriteSkin.boneTransforms[i].transform.DOLocalMove(_rootBonePositions[i], 0.15f);
                SlimeSpriteSkin.boneTransforms[i].DOLocalRotate(_rootBoneRotations[i].eulerAngles, 0.15f);
            }
        }
        #endregion
    }
}
