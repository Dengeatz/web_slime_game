using System.Collections.Generic;
using TestTask.Core;
using TestTask.Info;
using UnityEngine;

namespace TestTask.Managers 
{
    public class BulbManager : MonoBehaviour
    {
        [SerializeField] private List<BulbInfo> _bulbsInfo;
        private List<Bulb> _bulbs = new();
        private bool _isTranslate;
        private Bulb _selectedBulb;

        #region Unity Methods
        private void Start()
        {
            InitBulbs();
        }
        #endregion

        #region Public Methods
        public void OnInteract(Bulb bulb)
        {
            if (!_bulbs.Contains(bulb) || bulb.IsLocked || _isTranslate) return;

            if(_selectedBulb == null && !bulb.IsEmpty())
            {
                _selectedBulb = bulb;
                bulb.Select();
                return;
            }
            
            if(_selectedBulb == bulb)
            {
                _selectedBulb = null;
                bulb.Deselect(); 
                return;
            }

            if (_selectedBulb != null && bulb != null && _selectedBulb != bulb)
            {
                if(!bulb.IsFull())
                {
                    _isTranslate = true;
                    _selectedBulb.FillToAnotherBulb(bulb, EndTranslate);
                    _selectedBulb = null;
                    return;
                } 
                else
                {
                    _selectedBulb = bulb;
                    return;
                }
            }
        }
        #endregion

        #region Private Methods
        private void EndTranslate()
        {
            _isTranslate = false;
        }

        private void InitBulbs()
        {
            foreach(var bulb in _bulbsInfo)
            {
                bulb.BulbHandler.InitSlimes(bulb.StartSlimeObjects);
                _bulbs.Add(bulb.BulbHandler);
            }
        }
        #endregion
    }
}