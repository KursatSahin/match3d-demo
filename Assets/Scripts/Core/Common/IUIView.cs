using UnityEngine;

namespace Match3d.Core.Common
{
    public interface IUIView
    {
        public GameObject GameObject { get; }
        public void SetUICamera(Camera cam);
    }
}