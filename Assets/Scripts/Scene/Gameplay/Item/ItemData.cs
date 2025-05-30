using UnityEngine;

namespace Match3d.Gameplay.Item
{
    [CreateAssetMenu(fileName = "Assets/Data/Items/ItemDataXX.asset", menuName = "Match3D/Create/Item Data", order = 0)]
    public class ItemData : ScriptableObject 
    {
        #region Inspector

        [SerializeField] private ItemType _type;
        [SerializeField] private Mesh _meshFilter;
        [SerializeField] private Mesh _meshCollider;
        [SerializeField] private Sprite _sprite;

        #endregion

        public ItemType Type => _type;
        public Mesh Filter => _meshFilter;
        public Mesh Collider => _meshCollider;
        public Sprite Sprite => _sprite;
        
        public enum ItemType
        {
            Banana, BasketballBall, ChristmasBoot, Crown, Dumbell, LipStick, Perfume, Snowman, SoccerBall, TopHat
        }
    }
}