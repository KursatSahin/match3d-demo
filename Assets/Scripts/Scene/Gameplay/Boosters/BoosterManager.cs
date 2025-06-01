using Match3d.Gameplay.Boosters;
using UnityEngine;
using VContainer;

namespace Match3d.Gameplay.Boosters
{
    public class BoosterManager : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private BoosterView _BoosterPrefab;

        #endregion

        [Inject] private BoosterController _boosterController;
        [Inject] private BoosterDataProvider _boosterDataProvider;

        private void Awake()
        {
            var index = 0;
            foreach (var booster in _boosterController.GetAvailableBoosters())
            {
                var boosterView = Instantiate(_BoosterPrefab, transform);
                var viewTransform = (RectTransform)boosterView.transform;
                viewTransform.anchoredPosition = new Vector2(viewTransform.rect.width * index + (80 * index), 0);
                index++;

                if (!_boosterDataProvider.boosterData.TryGetValue(booster.Type, out var boosterData))
                {
                    continue;
                }

                boosterView.Initialize(booster, boosterData.Icon);
            }
        }
    }
}
