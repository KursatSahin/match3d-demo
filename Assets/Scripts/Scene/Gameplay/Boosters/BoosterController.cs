using System.Collections.Generic;
using Match3d.Core.DataManager;
using VContainer;

namespace Match3d.Gameplay.Boosters
{
    public class BoosterController
    {
        [Inject] private readonly IDataManager _dataManager;
        [Inject] private readonly FanBooster _fanBooster;
        [Inject] private readonly TimeFreeze _timeFreeze;
        
        public IReadOnlyList<IBooster> GetAvailableBoosters()
        {
            var playerData = _dataManager.Load();
            var availableBoosters = new List<IBooster>();
            
            if (playerData.boosters != null && playerData.boosters.Length > 0)
            {
                foreach (var boosterData in playerData.boosters)
                {
                    if (boosterData.qty > 0 && boosterData.powerUps != null)
                    {
                        availableBoosters.Add(boosterData.powerUps);
                    }
                }
            }
            
            // If no boosters are available in player data, return default boosters
            if (availableBoosters.Count == 0)
            {
                availableBoosters.Add(_fanBooster);
                availableBoosters.Add(_timeFreeze);
            }
            
            return availableBoosters;
        }
    }
}