using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Match3d.Gameplay.Item;

namespace Match3d.Gameplay.Item
{
    public class ItemBase : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshCollider _meshCollider;
        [SerializeField] private Rigidbody _rigidbody;

        #endregion

        public ItemData.ItemType Type { get; private set; }

        public bool IsColliderEnabled => _meshCollider.enabled;

        void OnEnable()
        {

        }

        void OnDisable()
        {

        }

        public void SetItemData(ItemData data)
        {
            _meshFilter.sharedMesh = data.Filter;
            _meshCollider.sharedMesh = data.Collider;
            Type = data.Type;
        }

        public void ShiftNextSlot(Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public void ShiftPreviousSlot(Vector3 position, int count)
        {
            throw new System.NotImplementedException();
        }
    }

}