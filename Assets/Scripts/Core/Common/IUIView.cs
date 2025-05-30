using UnityEngine;

namespace Match3d.Core.Common
{
    public interface IUIView
    {
        public GameObject Go { get; }
        public void SetUICamera(Camera cam);
    }
}