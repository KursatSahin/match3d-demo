using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Level;
using Match3d.Core.DataManager;
using Match3d.Core.Utils;
using Match3d.Gameplay;
using Match3d.Gameplay.Item;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using Random = UnityEngine.Random;
using Timer = Match3d.Core.Utils.Timer;

namespace Match3d.Scene
{
    public class LevelLoader
    {
        private const string levelPrefix = "Level";
        
        [Inject] private IDataManager _dataManager;
        [Inject] private Timer _levelTimer;
        [Inject] private MonoGameObjectPool _gameObjectPool;
        [Inject] private ItemDataProvider _itemDataProvider;
        
        public async UniTask LoadLevelAsync(CancellationToken token)
        {
            var levelNumber = _dataManager.Load().currentLevel;
            await LoadLevelAsync(levelNumber.ToString(), token);
        }
        
        public async UniTask LoadLevelAsync(string levelNameVariable, CancellationToken token)
        {
            var playerData = _dataManager.Load();
            
            string levelPath = $"{levelPrefix}{levelNameVariable}";
            bool isValidLevel = await AddressableKeyChecker.KeyExistsAsync(levelPath);
            if (!isValidLevel)
            {
                // you can create a level loop calculation here
                playerData.currentLevel--;
                _dataManager.Save(playerData);
                
                levelPath = $"{levelPrefix}{playerData.currentLevel}";
            }
            
            var handle = Addressables.LoadAssetAsync<LevelData>(levelPath);
            (var isCanceled, var levelData) = await handle.ToUniTask(cancellationToken: token).SuppressCancellationThrow();
            
            if (isCanceled)
            {
                Debug.LogError("Level loading was canceled.");
                return;
            }

            foreach (var goalItem in levelData.Data.goalItems)
            {
                // TODO : Add all goals to a list
            }

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
                    var recycledItem = _gameObjectPool.GetItem();
                    recycledItem.SetItemData(itemData);
                    recycledItem.gameObject.SetActive(true);
                    recycledItem.transform.position = new Vector3(Random.Range(-3, 3), 3 + Random.Range(gameItem.minVolume, gameItem.maxVolume), Random.Range(-5, 5));
                    recycledItem.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                    recycledItem.transform.localScale = new Vector3(scale, scale, scale);
                    
                    // TODO : Add all game items to a list
                }
            }
            
            _levelTimer.SetTimer(levelData.Data.seconds);
            
            // Since we create positions randomly, some items might overlap. We simulate physics to make these items move away from each other.
            for (var i = 0; i < 10; i++)
            {
                Physics.Simulate(Time.fixedDeltaTime);
            }
        }
    }
}