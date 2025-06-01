using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Match3d.Core.DataManager;
using Match3d.Core.Utils;
using Match3d.Gameplay.Item;
using Match3d.Scene;
using Match3dCore.Utils;
using UnityEngine;
using VContainer;

namespace Match3d.Gameplay.Boosters
{
    public class FanBooster : IBooster
    {
        [Inject] private IDataManager _dataManager;
        [Inject] private GameDataContainer _gameDataContainer;
        [Inject] private SlotContainer _slotContainer;
        
        public string Name { get; }
        
        public BoosterData.BoosterType Type { get; } = BoosterData.BoosterType.Fan;
        
        public async UniTask Execute(CancellationToken token)
        {
            var randomItems = GetRandomFanBoosterItems();
            await UseFanPowerUps(randomItems);
        }
        public void Complete()
        {
            throw new System.NotImplementedException();
        }
        
        private IReadOnlyList<ItemBase> GetRandomFanBoosterItems()
        {
            var items = _gameDataContainer.Items.Where(i => !_slotContainer.ContainsItem(i)).ToArray();
            
            return items.GetRandomSubarrayWithMinMax(items.Length / 2, items.Length);
        }
        
        private async UniTask UseFanPowerUps(IReadOnlyList<ItemBase> items)
        {
            var tasks = new List<UniTask>();
            foreach (var item in items)
            {
                // Add force to each item using physics instead of tweening
                var rigidbody = item.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    // Generate random direction with upward bias to prevent falling through ground
                    Vector3 forceDirection = new Vector3(
                        Random.Range(-1f, 1f),
                        Random.Range(0.5f, 1f), // Bias upward
                        Random.Range(-1f, 1f)
                    ).normalized;
            
                    // Apply appropriate force magnitude
                    float forceMagnitude = 10.0f;
                    rigidbody.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
            
                    // Wait for physics to settle
                    tasks.Add(WaitForPhysicsToSettle(rigidbody));
                }
                await UniTask.Delay(100);
            }
            await UniTask.WhenAll(tasks);
        }

        private async UniTask WaitForPhysicsToSettle(Rigidbody rigidbody)
        {
            // Create a task that completes when the object has mostly stopped moving
            float velocityThreshold = 0.1f;
            float maxWaitTime = 2.0f; // Seconds
            float startTime = Time.time;
    
            while (rigidbody != null && 
                   rigidbody.velocity.magnitude > velocityThreshold && 
                   Time.time - startTime < maxWaitTime)
            {
                await UniTask.Yield();
            }
        }
    }
}