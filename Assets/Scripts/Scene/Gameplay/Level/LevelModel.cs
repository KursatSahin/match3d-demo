using System;
using System.Collections.Generic;
using Match3d.Gameplay.Item;
using UnityEngine.Serialization;

namespace Gameplay.Level
{
    [Serializable]
    public struct LevelModel
    {
        public int seconds;
        public List<LevelGoals> goalItems;
        public List<LevelLayout> layoutItems;
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