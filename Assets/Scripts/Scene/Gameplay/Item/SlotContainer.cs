using System;
using System.Collections;
using System.Collections.Generic;
using Match3d.Gameplay.Item;
using UnityEngine;

public class SlotContainer
{
    private Transform[] _slots;
    private ItemBase[] _items;
    private (int offset, int count) _lastMatchedItems;
    
    public bool IsFull => _items != null && _items.Length == _slots.Length;
    
    public void Initialize()
    {
        _items = new ItemBase[_slots.Length];
    }
    
    public Transform GetAvailableSlot(ItemBase item)
    {
        return null;
    }
    
    public void ClearSlots(MatchedItems matchedItems)
    {
        
    }
    
    public void ClearAllSlots()
    {
        for (var i = 0; i < _items.Length; i++)
        {
            if (_items[i] != null)
            {
                _items[i].gameObject.SetActive(false);
                _items[i] = null;
            }
        }
    }
    
    public class MatchedItems
    {
        public ItemBase[] Items { get; }
        public int MiddleSlotIndex { get; }
        public int Count => Items.Length;
        public MatchedItems(ItemBase[] items, int slotIndex)
        {
            Items = items;
            MiddleSlotIndex = slotIndex;
        }
    }
}
