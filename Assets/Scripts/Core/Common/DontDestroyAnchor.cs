using UnityEngine;

namespace Match3d.Core.Common
{
    public class DontDestroyAnchor : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}