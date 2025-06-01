using System;
using System.Collections.Generic;
using Match3d.Gameplay.Item;

namespace Match3d.Scene
{
    public class GameDataContainer
    {
        public event Action<ItemData.ItemType, int> OnGoalProgressUpdated;
        public event Action OnAllGoalsCompleted;

        private readonly Dictionary<ItemData.ItemType, int> _items = new();

        public Dictionary<ItemData.ItemType, int> Goals { get; } = new();

        public List<ItemBase> Items { get; } = new();

        public void AddItemCount(ItemData.ItemType type, int count)
        {
            _items.TryAdd(type, count);
        }

        public void AddGoal(ItemData.ItemType type, int count)
        {
            Goals.TryAdd(type, count);
        }

        public int GetCount(ItemData.ItemType type)
        {
            return _items.GetValueOrDefault(type, 0);
        }

        public void RemoveItemCount(ItemData.ItemType type, int count)
        {
            _items[type] -= count;

            if (!Goals.ContainsKey(type))
            {
                return;
            }

            var oldCount = Goals[type];
            var newCount = Math.Clamp(oldCount - count, 0, int.MaxValue);
            if (oldCount != newCount)
            {
                Goals[type] = newCount;
                OnGoalProgressUpdated?.Invoke(type, Goals[type]);
            }

            var remainingOrders = 0;
            foreach (var order in Goals)
            {
                remainingOrders += order.Value;
            }

            if (remainingOrders == 0)
            {
                OnAllGoalsCompleted?.Invoke();
            }
        }
    }
}