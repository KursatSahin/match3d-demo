using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Match3d.Core.DataManager;
using Match3d.Core.Scene;
using UnityEngine;
using VContainer;

namespace Match3d.Scene
{
    public class GameSceneBootstrapper : SceneBootstrapper
    {
        #region Inspector

        [SerializeField] private Transform[] _slots;

        #endregion
        
        private const string viewKey = "GameUIView";
        
        [Inject] private LevelLoader _levelLoader;
        [Inject] private GameLogicManager _gameLogicManager;
        [Inject] private IDataManager _dataManager;
        [Inject] private SlotContainer _slotContainer;
        
        public override async UniTask InitializeAsync(CancellationToken token, IProgress<float> progress = null)
        {
            try
            {
                if (_slots.Length > 0)
                {
                    _slotContainer.SetSlotTransforms(_slots);
                }
                
                var currentLevel = _dataManager.Load().currentLevel;
                var isCanceled = await _levelLoader.LoadLevelAsync(this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();
                if (isCanceled)
                {
                    throw new Exception($"Failed to load the level {currentLevel}.");
                }
                
                var view = await uiViewFactory.CreateAsync(viewKey, _sceneEnvironment, container, token);
                if (view == null)
                {
                    throw new Exception($"Couldn't create the view. key: {viewKey}");
                }
                view.SetUICamera(_uiCamera);
                view.Go.SetActive(true);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                throw;
            }
            
        }
    }
}