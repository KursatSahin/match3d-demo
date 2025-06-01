using UnityEngine;

namespace Match3d.Gameplay.Boosters
{
    [CreateAssetMenu(fileName = "Assets/Data/Boosters/BoosterDataXX.asset", menuName = "Match3D/Create/Booster Data", order = 1)]
    public class BoosterData : ScriptableObject 
    {
        #region Inspector

        [SerializeField] private BoosterType _type;
        [SerializeField] private Sprite _icon;
        [SerializeField] private float _cooldown;
        [SerializeField] private int _defaultQuantity;

        #endregion

        public BoosterType Type => _type;
        public Sprite Icon => _icon;
        public float Cooldown => _cooldown;
        public int DefaultQuantity => _defaultQuantity;
        
        public enum BoosterType
        {
            Fan, TimeFreeze
        }
    }
}