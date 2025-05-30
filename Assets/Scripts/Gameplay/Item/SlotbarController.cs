using System;
using System.Collections;
using System.Collections.Generic;
using Match3d.Gameplay.Item;
using UnityEngine;

public class SlotbarController : MonoBehaviour
{
    #region Inspector
    [SerializeField] private Transform[] _slots;
    #endregion
    
    private ItemBase[] _items;
    private (int offset, int count) _lastMatchedItems;
    
    public bool IsFull => _items != null && _items.Length == _slots.Length;
    
    public void Initialize()
    {
        _items = new ItemBase[_slots.Length];
    }
    
    public (Vector3, IReadOnlyList<ItemBase>) GetAvailableSlot(ItemBase item)
    {
        var counter = 0;
        _lastMatchedItems = (0, 0);

        for (var i = 0; i < _items.Length; i++)
        {
            if (_items[i])
            {
                if (_items[i].Type == item.Type)
                {
                    if (counter == 0)
                    {
                        _lastMatchedItems.offset = i;
                    }
                    counter++;
                    continue;
                }

                if (counter == 0)
                {
                    continue;
                }
            }
            if (counter > 0)
            {
                for (var j = _items.Length - 1; j > i; j--)
                {
                    _items[j] = _items[j - 1];
                    if (_items[j])
                    {
                        _items[j].ShiftNextSlot(_slots[j].position);
                    }
                }
            }

            _items[i] = item;
            _lastMatchedItems.count = counter + 1;
            return (_slots[i].position, new ArraySegment<ItemBase>(_items, _lastMatchedItems.offset, _lastMatchedItems.count));
        }

        return (Vector3.zero, null);
    }
    
    public void ReleaseLastMatchedSlots()
    {
        for (var i = _lastMatchedItems.offset; i < _lastMatchedItems.offset + _lastMatchedItems.count; i++)
        {
            _items[i] = null;
        }

        for (var i = _lastMatchedItems.offset + _lastMatchedItems.count; i < _items.Length; i++)
        {
            if (!_items[i])
            {
                continue;
            }

            var newIndex = i - _lastMatchedItems.count;
            _items[newIndex] = _items[i];
            _items[i].ShiftPreviousSlot(_slots[newIndex].position, _lastMatchedItems.count);
            _items[i] = null;
        }
    }
}
