using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Match3d.Core.DataManager;
using VContainer;

namespace Match3d.Scene
{
    public class LevelLoader
    {
        private const string levelSuffix = "Level";
        
        [Inject] private IDataManager _dataManager;
        
        public async UniTask LoadLevelAsync(CancellationToken token)
        {
            var levelName = _dataManager.Load().currentLevel;
            await LoadLevelAsync(levelName.ToString(), token);
        }
        
        public async UniTask LoadLevelAsync(string levelName, CancellationToken token)
        {
            // TODO : Logic to load the level
            
            // This is a placeholder for the actual implementation
            await UniTask.Delay(1000, cancellationToken: token);
            Console.WriteLine($"Level {levelName} loaded.");
        }
    }
}