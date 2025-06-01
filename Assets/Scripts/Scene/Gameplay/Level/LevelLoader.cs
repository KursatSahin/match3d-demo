using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Match3d.Core.DataManager;
using Match3d.Core.Utils;
using Match3d.Gameplay;
using Match3d.Gameplay.Item;
using Match3d.Scene;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using Random = UnityEngine.Random;
using Timer = Match3d.Core.Utils.Timer;

namespace Match3d.Gameplay.Level
{
    public class LevelLoader
    {
        private const string levelPrefix = "Level"; // Prefix for level asset names
        
        // Dependencies injected via VContainer
        [Inject] private IDataManager _dataManager;
        [Inject] private Timer _levelTimer;
        [Inject] private MonoGameObjectPool _gameObjectPool;
        [Inject] private ItemDataProvider _itemDataProvider;
        [Inject] private GameDataContainer _gameDataContainer;
        
        // Loads the current level based on player data
        public async UniTask LoadLevelAsync(CancellationToken token)
        {
            var levelNumber = _dataManager.Load().currentLevel;
            await LoadLevelAsync(levelNumber.ToString(), token);
        }
        
        // Loads a specific level by name/number
        public async UniTask LoadLevelAsync(string levelNameVariable, CancellationToken token)
        {
            var playerData = _dataManager.Load();
            
            string levelPath = $"{levelPrefix}{levelNameVariable}";
            bool isValidLevel = await AddressableKeyChecker.KeyExistsAsync(levelPath);
            if (!isValidLevel)
            {
                // If level does not exist, fallback to previous level
                playerData.currentLevel--;
                _dataManager.Save(playerData);
                
                levelPath = $"{levelPrefix}{playerData.currentLevel}";
            }
            
            // Load level data asset asynchronously
            var handle = Addressables.LoadAssetAsync<LevelData>(levelPath);
            (var isCanceled, var levelData) = await handle.ToUniTask(cancellationToken: token).SuppressCancellationThrow();
            
            if (isCanceled)
            {
                Debug.LogError("Level loading was canceled.");
                return;
            }

            // Add goal items to the game data container
            foreach (var goalItem in levelData.Data.goalItems)
            {
                _gameDataContainer.AddGoal(goalItem.type, goalItem.count);
            }

            // Instantiate and place all layout items for the level
            foreach (var gameItem in levelData.Data.layoutItems)
            {
                if (!_itemDataProvider.itemData.TryGetValue(gameItem.type, out var itemData))
                {
                    continue;
                }

                var scale = Random.Range(gameItem.minVolume, gameItem.maxVolume);
                var itemCount = gameItem.count;
                for (int i = 0; i < itemCount; i++)
                {
                    var recycledItem = _gameObjectPool.GetItem(); // Get item from pool
                    recycledItem.SetItemData(itemData);
                    recycledItem.gameObject.SetActive(true);
                    // Randomize position, rotation, and scale
                    recycledItem.transform.position = new Vector3(Random.Range(-3, 3), 3 + Random.Range(gameItem.minVolume, gameItem.maxVolume), Random.Range(-5, 5));
                    recycledItem.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                    recycledItem.transform.localScale = new Vector3(scale, scale, scale);
                    
                    _gameDataContainer.Items.Add(recycledItem);
                }
                
                _gameDataContainer.AddItemCount(gameItem.type, itemCount);
                    
            }
            
            _levelTimer.SetTimer(levelData.Data.seconds); // Set the level timer
            
            // Simulate physics to resolve overlapping items
            for (var i = 0; i < 10; i++)
            {
                Physics.Simulate(Time.fixedDeltaTime);
            }
        }
    }
}