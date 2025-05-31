using Match3d.Core.Common;
using Match3d.Core.DataManager;
using Match3d.Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Match3d.Scene
{
    public class GameUIView : BaseUiView
    {
        #region Inspector

        [SerializeField] private TextMeshProUGUI _level;
        [SerializeField] private TextMeshProUGUI _timer;
        [SerializeField] private Image _timerState;
        [SerializeField] private Button _closeButton;

        [SerializeField] private Transform[] _Slots;

        #endregion

        [Inject] private Timer _levelTimer;
        [Inject] private IDataManager _dataManager;
        [Inject] private GameLogicManager _gameLogicManager;

        [Inject]
        private void Initialize()
        {
            //itemSlotsController.SetSlotTransforms(_Slots);

            _level.SetText($"Level {_dataManager.Load().currentLevel}");
            OnTimerTick(_levelTimer.RemainingSeconds);
        }

        private void OnEnable()
        {
            _levelTimer.TimerTick += OnTimerTick;
            _levelTimer.TimerStateChanged += OnTimerStateChanged;

            _closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        private void OnDisable()
        {
            _levelTimer.TimerTick -= OnTimerTick;
            _levelTimer.TimerStateChanged -= OnTimerStateChanged;

            _closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        }

        private void OnTimerStateChanged(bool isRunning)
        {
            _timerState.color = isRunning ? Color.white : Color.red;
        }

        private void OnTimerTick(int seconds)
        {
            _timer.SetText($"{seconds / 60}:{seconds % 60:00}");
        }

        private void OnCloseButtonClicked()
        {
            //_gameplayManager.CloseButtonClicked();
        }
    }
}