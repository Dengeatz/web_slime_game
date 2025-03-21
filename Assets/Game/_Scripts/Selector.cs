using TestTask.Managers;
using UnityEngine;

namespace TestTask.Core
{
    public class Selector : MonoBehaviour
    {
        [SerializeField] private BulbManager _bulbManager;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private int _bulbMask;

        #region Unity Methods
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                var hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, 1 << _bulbMask);

                if (!hit) return;

                if(hit.transform.gameObject.TryGetComponent(out Bulb bulbScript))
                {
                    _bulbManager.OnInteract(bulbScript);
                }
            }
        }
        #endregion
    }
}
