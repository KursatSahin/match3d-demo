using UnityEngine;

namespace Match3d.Core.Common
{
    public abstract class BaseUiView : MonoBehaviour, IUIView
    {
        #region Inspector

        [SerializeField] protected Canvas _canvas;

        #endregion

        public GameObject GameObject => gameObject;

        public virtual void SetUICamera(Camera cam)
        {
            _canvas.worldCamera = cam;
        }
    }
}