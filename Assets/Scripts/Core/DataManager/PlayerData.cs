using System;
using Match3d.Gameplay.Boosters;

namespace Match3d.Core.DataManager
{
    [Serializable]
    public class PlayerData : IData
    {
        public int currentLevel;
        public int coins;
        public int lives;
        public string recoverLiveStart;

        public BoosterSaveData[] boosters;
        
        public PlayerData()
        {
            currentLevel = 1;
            coins = 100;
            lives = 5;
            recoverLiveStart = null;
            boosters = Array.Empty<BoosterSaveData>();
        }
    }
    

    public interface IData
    {
        
    }

    [Serializable]
    public class BoosterSaveData
    {
        public IBooster powerUps;
        public int qty;
    }
}