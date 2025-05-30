using UnityEngine;

namespace Match3d.Core.DataManager
{
    public class DataManager : IDataManager
    {
        private const string PlayerDataKey = "PlayerData";

        private PlayerData _cachedData;
        
        public void Save(PlayerData data)
        {
            PlayerPrefs.SetString(PlayerDataKey, JsonUtility.ToJson(data));
            _cachedData = data;
        }

        public PlayerData Load()
        {
            if (_cachedData != null && _cachedData.currentLevel != 0)
            {
                return _cachedData;
            }

            var data = PlayerPrefs.GetString(PlayerDataKey);
            _cachedData = string.IsNullOrEmpty(data) ? new PlayerData() : JsonUtility.FromJson<PlayerData>(data);

            return _cachedData;
        }

        public void Reset()
        {
            PlayerPrefs.DeleteKey(PlayerDataKey);
            _cachedData = new PlayerData();
            Save(_cachedData);
        }
    }
}