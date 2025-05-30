using System;
using System.Collections.Generic;
using Match3d.Gameplay.Item;

namespace Gameplay.Level
{
    [Serializable]
    public struct LevelModel
    {
        public int seconds;
        public List<LevelGoals> itemOrders;
        public List<LevelLayout> itemLayouts;
    }
    
    [Serializable]
    public struct LevelGoals
    {
        public ItemData.ItemType type;
        public int count;
    }

    [Serializable]
    public struct LevelLayout
    {
        public ItemData.ItemType type;
        public float minVolume;
        public float maxVolume;
        public int count;
    }
}