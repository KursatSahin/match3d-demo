using System.Collections.Generic;
using UnityEngine;

namespace Match3d.Gameplay.Item
{
    public class ItemDataProvider : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private ItemData[] _itemDataList;

        #endregion

        public Dictionary<ItemData.ItemType, ItemData> itemData { get; } = new();

        private void Awake()
        {
            foreach (var itemPrototype in _itemDataList)
            {
                itemData.Add(itemPrototype.Type, itemPrototype);
            }
        }
    }
}
