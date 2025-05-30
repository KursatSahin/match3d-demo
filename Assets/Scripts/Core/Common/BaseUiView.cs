using UnityEngine;

namespace Match3d.Core.Common
{
    public abstract class BaseUiView : MonoBehaviour, IUIView
    {
        #region Inspector

        [SerializeField] private Canvas _canvas;

        #endregion

        public GameObject Go => gameObject;

        public virtual void SetUICamera(Camera cam)
        {
            _canvas.worldCamera = cam;
        }
    }
}