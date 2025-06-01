using System.Collections.Generic;
using Match3d.Gameplay.Item;
using UnityEngine;
using VContainer;

namespace Match3d.Scene.Goal
{
    public class GoalManager : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private GoalCardView _goalCardPrefab;

        #endregion

        [Inject] private GameDataContainer _gameDataContainer;
        [Inject] private ItemDataProvider _itemDataProvider;

        private readonly Dictionary<ItemData.ItemType, GoalCardView> _orderViews = new();

        private void Awake()
        {
            var index = 0;
            foreach (var goal in _gameDataContainer.Goals)
            {
                var goalCardView = Instantiate(_goalCardPrefab, transform);
                _orderViews.Add(goal.Key, goalCardView);
                var viewTransform = (RectTransform)goalCardView.transform;
                viewTransform.anchoredPosition = new Vector2(viewTransform.rect.width * index + (20 * index), 0);
                index++;

                if (!_itemDataProvider.itemData.TryGetValue(goal.Key, out var itemData))
                {
                    continue;
                }

                goalCardView.Initialize(goal.Value, itemData.Sprite);
            }
        }

        private void OnEnable()
        {
            _gameDataContainer.OnGoalProgressUpdated += OnGoalProgressUpdated;
        }

        private void OnDisable()
        {
            _gameDataContainer.OnGoalProgressUpdated -= OnGoalProgressUpdated;
        }

        private void OnGoalProgressUpdated(ItemData.ItemType type, int amount)
        {
            _orderViews[type].SetOrderCount(amount);
        }
    }
}