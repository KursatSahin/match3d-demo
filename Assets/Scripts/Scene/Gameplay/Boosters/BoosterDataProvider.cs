using System.Collections.Generic;
using UnityEngine;

namespace Match3d.Gameplay.Boosters
{
    public class BoosterDataProvider : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private BoosterData[] _boosterDataList;

        #endregion

        public Dictionary<BoosterData.BoosterType, BoosterData> boosterData { get; } = new();

        private void Awake()
        {
            foreach (var boosterPrototype in _boosterDataList)
            {
                boosterData.Add(boosterPrototype.Type, boosterPrototype);
            }
        }
        
        public BoosterData GetBoosterData(BoosterData.BoosterType type)
        {
            return boosterData.TryGetValue(type, out var data) ? data : null;
        }
    }
}