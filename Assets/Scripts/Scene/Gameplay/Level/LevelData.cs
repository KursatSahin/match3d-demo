using UnityEngine;

namespace Match3d.Gameplay.Level
{
    [CreateAssetMenu(fileName = "Assets/Data/Levels/LevelDataXX.asset", menuName = "Match3D/Create/Level Data", order = 0)]
    public class LevelData : ScriptableObject
    {
        #region Inspector

        [SerializeField] private LevelModel _data;

        #endregion

        public LevelModel Data => _data;
    }
}