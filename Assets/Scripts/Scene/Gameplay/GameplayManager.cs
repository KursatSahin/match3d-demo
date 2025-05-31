using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Match3d.Core;
using Match3d.Core.Common;
using Match3d.Core.DataManager;
using Match3d.Core.Scene;
using Match3d.Core.Utils;
using Match3d.Gameplay;
using Match3d.Gameplay.Item;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace Match3d.Scene
{
    public class GameplayManager : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Transform _sceneEnvironment;

        #endregion
        
        [Inject] private IObjectResolver _container;
        [Inject] private IUIViewFactory _viewFactory;
        [Inject] private MonoGameObjectPool _gameItemPool;
        [Inject] private IDataManager _dataManager;
        [Inject] private Timer _levelTimer;
        [Inject] private SlotContainer _slotContainer;
        [Inject] private GameplayDataContainer _gameplayDataContainer;

        private const int MatchCount = 3;

        private static int _itemsLayerMask;
        
        private GameObject _currentlySelectedItem;

        private Camera _camera;
        private bool _isInputDisabled = false;
        private bool _isGamePaused = false;

        private void Awake()
        {
            _camera = Camera.main;
            _itemsLayerMask = LayerMask.GetMask("Items");
        }

        private void Start()
        {
            _levelTimer.IsRunning = true;
        }

        private void OnEnable()
        {
            _levelTimer.TimerEnd += OnTimerEnd;
            _gameplayDataContainer.OnAllGoalsCompleted += OnAllGoalCompleted;
        }

        

        private void OnDisable()
        {
            _levelTimer.TimerEnd -= OnTimerEnd;
            _gameplayDataContainer.OnAllGoalsCompleted -= OnAllGoalCompleted;
        }

        private void FixedUpdate()
        {
            if (_isGamePaused)
            {
                return;
            }

            Physics.Simulate(Time.fixedDeltaTime);
        }

        public void Update()
        {
            if (_slotContainer.IsFull && !_isInputDisabled)
            {
                OpenEndGamePopupFail().Forget();
                return;
            }
            
            if (Input.GetMouseButtonDown(0) && !_isInputDisabled && !_isGamePaused)
            {
                CheckItem();
            }
        }

        private void CheckItem()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out var hitInfo, 50, _itemsLayerMask);
            if (!hitInfo.collider)
            {
                return;
            }
            
            var hitItemBase = hitInfo.collider.GetComponent<ItemBase>();
            
            var matchData = _slotContainer.GetAvailableSlot(hitItemBase);
            var isMatched = _isInputDisabled = (matchData.Count == MatchCount);
            
            hitItemBase.JumpToSlot(matchData.AvailableSlot, isMatched ? () => DestroyMatchedItems(matchData.MatchedItems) : null);
        }

        private void DestroyMatchedItems(IReadOnlyList<ItemBase> items)
        {
            const float offset = 1f;
            var firstPositionX = items[0].transform.position.x;
            var lastPositionX = items[^1].transform.position.x;
            var mergePointX = (firstPositionX + lastPositionX) / 2;

            foreach (var item in items)
            {
                item.MoveToMergePoint(new Vector3(mergePointX, 0, item.transform.position.z + offset), () => _gameItemPool.ReleaseItem(item));
            }
            
            _slotContainer.ReleaseLastMatchedSlots();
            DOVirtual.DelayedCall(0.5f, () => _isInputDisabled = false);
        }

        private void OnAllGoalCompleted()
        {
            var playerData = _dataManager.Load();
            playerData.currentLevel++;
            playerData.coins += 100;
            playerData.boosters.ToList().ForEach((x)=> x.qty++);
            _dataManager.Save(playerData);

            OpenEndGamePopupSuccess().Forget();
        }
        
        private void OnTimerEnd()
        {
            var playerData = _dataManager.Load();
            
            if (playerData.lives > 0) playerData.lives--;
            
            _dataManager.Save(playerData);
            
            OpenEndGamePopupFail().Forget();
        }

        private async UniTaskVoid OpenEndGamePopupSuccess()
        {
            _isGamePaused = true;
            _levelTimer.IsRunning = false;
            
            var popup = await _viewFactory.CreateAsync("EndGamePopupSuccess", _sceneEnvironment, _container, this.GetCancellationTokenOnDestroy());
            popup.SetUICamera(_camera);
            popup.Go.SetActive(true);
        }

        
        private async UniTaskVoid OpenEndGamePopupFail()
        {
            _isGamePaused = true;
            _levelTimer.IsRunning = false;
            
            var popup = await _viewFactory.CreateAsync("EndGamePopupFail", _sceneEnvironment, _container, this.GetCancellationTokenOnDestroy());
            popup.SetUICamera(_camera);
            popup.Go.SetActive(true);
        }
        
        public void EndGameplay()
        {
            DOTween.KillAll();
            _gameItemPool.ReleaseAll();

            SceneLoader.LoadSceneAsync(GameConstants.SceneNames.Home, this.GetCancellationTokenOnDestroy()).Forget();
        }
    }
}