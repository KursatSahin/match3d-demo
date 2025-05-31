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

    public bool IsFull => _items != null && _items[_slots.Length - 1];

    public void SetSlotTransforms(Transform[] slots)
    {
        _slots = slots;
        _items = new ItemBase[_slots.Length];
    }

    /// <summary>
    /// Finds an available slot for the given item, handling matching item grouping.
    /// </summary>
    /// <param name="item">The item to place in a slot</param>
    /// <returns>MatchData containing position and matched items information</returns>
    public MatchData GetAvailableSlot(ItemBase item)
    {
        int matchingItemsCount = 0;
        _lastMatchedItems = (0, 0);

        for (int i = 0; i < _items.Length; i++)
        {
            // Handle existing item in the current slot
            if (_items[i] != null)
            {
                bool isMatchingType = _items[i].Type == item.Type;

                // Found a matching item
                if (isMatchingType)
                {
                    if (matchingItemsCount == 0)
                    {
                        _lastMatchedItems.offset = i; // Remember where matching sequence starts
                    }

                    matchingItemsCount++;
                    continue;
                }

                // Not a matching item, but no matches found yet, so keep looking
                if (matchingItemsCount == 0)
                {
                    continue;
                }
            }

            // Found an empty slot after matching items or an empty slot with no matches yet
            if (matchingItemsCount > 0)
            {
                ShiftItemsToMakeRoom(i);
            }

            // Place the item in the current slot
            _items[i] = item;
            _lastMatchedItems.count = matchingItemsCount + 1;

            // Return the match data with the current slot position and all matched items
            return CreateMatchData(i, _lastMatchedItems.offset, _lastMatchedItems.count);
        }

        // No available slot was found
        return new MatchData(Vector3.zero, null);
    }

    /// <summary>
    /// Shifts items to make room for a new item at the specified index
    /// </summary>
    private void ShiftItemsToMakeRoom(int index)
    {
        for (int j = _items.Length - 1; j > index; j--)
        {
            _items[j] = _items[j - 1];
            if (_items[j] != null)
            {
                _items[j].ShiftNextSlot(_slots[j].position);
            }
        }
    }

    /// <summary>
    /// Creates match data for the given slot index and matching items
    /// </summary>
    private MatchData CreateMatchData(int slotIndex, int matchOffset, int matchCount)
    {
        return new MatchData(
            _slots[slotIndex].position,
            new ArraySegment<ItemBase>(_items, matchOffset, matchCount)
        );
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

    public class MatchData
    {
        public IReadOnlyList<ItemBase> MatchedItems { get; }
        public Vector3 AvailableSlot { get; }
        public int Count => MatchedItems.Count;

        public MatchData(Vector3 availableSlot, IReadOnlyList<ItemBase> matchedItems)
        {
            AvailableSlot = availableSlot;
            MatchedItems = matchedItems;
        }
    }
}