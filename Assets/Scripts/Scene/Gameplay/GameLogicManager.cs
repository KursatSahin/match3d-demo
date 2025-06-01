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
    public class GameLogicManager : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Transform _sceneEnvironment; // Main environment object for the scene

        #endregion
        
        // Dependencies injected via VContainer
        [Inject] private IObjectResolver _container;
        [Inject] private IUIViewFactory _viewFactory;
        [Inject] private MonoGameObjectPool _gameItemPool;
        [Inject] private IDataManager _dataManager;
        [Inject] private Timer _levelTimer;
        [Inject] private SlotContainer _slotContainer;
        [Inject] private GameDataContainer _gameDataContainer;

        private const int MatchCount = 3; // Number of items needed for a match

        private static int _itemsLayerMask; // Layer mask for item selection
        
        private GameObject _currentlySelectedItem; // Currently selected item (if any)

        private Camera _camera;
        private bool _isInputDisabled = false; // Prevents input during animations or popups
        private bool _isGamePaused = false; // Indicates if the game is paused

        private void Awake()
        {
            _camera = Camera.main;
            _itemsLayerMask = LayerMask.GetMask("Items"); // Set up layer mask for item raycasts
        }

        private void Start()
        {
            _levelTimer.IsRunning = true; // Start the level timer
        }

        private void OnEnable()
        {
            // Subscribe to timer and goal completion events
            _levelTimer.TimerEnd += OnTimerEnd;
            _gameDataContainer.OnAllGoalsCompleted += OnAllGoalCompleted;
        }

        

        private void OnDisable()
        {
            // Unsubscribe from events
            _levelTimer.TimerEnd -= OnTimerEnd;
            _gameDataContainer.OnAllGoalsCompleted -= OnAllGoalCompleted;
        }

        private void FixedUpdate()
        {
            if (_isGamePaused)
            {
                return;
            }

            Physics.Simulate(Time.fixedDeltaTime); // Manual physics simulation
        }

        public void Update()
        {
            // If slots are full and game is not paused, trigger fail popup
            if (_slotContainer.IsFull && !_isInputDisabled && !_isGamePaused)
            {
                OpenEndGamePopupFail().Forget();
                return;
            }
            
            // On left mouse click, check for item selection
            if (Input.GetMouseButtonDown(0) && !_isInputDisabled && !_isGamePaused)
            {
                CheckItem();
            }
        }

        // Handles item selection and matching logic
        private void CheckItem()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out var hitInfo, 50, _itemsLayerMask);
            
            if (!hitInfo.collider) return;
            
            var hitItemBase = hitInfo.collider.GetComponent<ItemBase>();
            
            var matchData = _slotContainer.GetAvailableSlot(hitItemBase);

            var remaining = _gameDataContainer.GetCount(hitItemBase.Type);
            
            // Determine if a match is made or all items of this type are used
            var isMatched = _isInputDisabled = (matchData.Count == MatchCount || matchData.Count == remaining);
            
            // Move item to slot, and if matched, destroy matched items
            hitItemBase.JumpToSlot(matchData.AvailableSlot, isMatched ? () => DestroyMatchedItems(matchData.MatchedItems) : null);
        }

        // Handles destruction and animation of matched items
        private void DestroyMatchedItems(IReadOnlyList<ItemBase> items)
        {
            const float offset = 1f;
            var firstPositionX = items[0].transform.position.x;
            var lastPositionX = items[^1].transform.position.x;
            var mergePointX = (firstPositionX + lastPositionX) / 2;

            foreach (var item in items)
            {
                // Animate item to merge point, then release it to the pool
                item.MoveToMergePoint(new Vector3(mergePointX, 0, item.transform.position.z + offset), () => _gameItemPool.ReleaseItem(item));
            }

            _gameDataContainer.RemoveItemCount(items[0].Type, items.Count); // Update item count
            _slotContainer.ReleaseLastMatchedSlots(); // Free up slots
            
            DOVirtual.DelayedCall(0.5f, () => _isInputDisabled = false); // Re-enable input after animation
        }

        // Called when all level goals are completed
        private void OnAllGoalCompleted()
        {
            var playerData = _dataManager.Load();
            playerData.currentLevel++;
            playerData.coins += 100;
            playerData.boosters.ToList().ForEach((x)=> x.qty++);
            _dataManager.Save(playerData);

            OpenEndGamePopupSuccess().Forget(); // Show success popup
        }
        
        // Called when the timer ends
        private void OnTimerEnd()
        {
            var playerData = _dataManager.Load();
            
            if (playerData.lives > 0) playerData.lives--;
            
            _dataManager.Save(playerData);
            
            OpenEndGamePopupFail().Forget(); // Show fail popup
        }

        // Shows the end game success popup
        private async UniTaskVoid OpenEndGamePopupSuccess()
        {
            _isGamePaused = true;
            _levelTimer.IsRunning = false;
            
            var popup = await _viewFactory.CreateAsync("EndGamePopupSuccess", _sceneEnvironment, _container, this.GetCancellationTokenOnDestroy());
            popup.SetUICamera(_camera);
            popup.GameObject.SetActive(true);
        }

        // Shows the end game fail popup
        private async UniTaskVoid OpenEndGamePopupFail()
        {
            _isGamePaused = true;
            _levelTimer.IsRunning = false;
            
            var popup = await _viewFactory.CreateAsync("EndGamePopupFail", _sceneEnvironment, _container, this.GetCancellationTokenOnDestroy());
            popup.SetUICamera(_camera);
            popup.GameObject.SetActive(true);
        }
        
        // Ends gameplay, cleans up, and loads the home scene
        public void EndGameplay()
        {
            DOTween.KillAll(); // Stop all tweens
            _gameItemPool.ReleaseAll(); // Release all pooled items

            SceneLoader.LoadSceneAsync(GameConstants.SceneNames.Loader, this.GetCancellationTokenOnDestroy(), options: new LoaderSceneOptions { SceneName = GameConstants.SceneNames.Home }).Forget();
        }
    }
}