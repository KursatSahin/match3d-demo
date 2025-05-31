using Match3d.Core;
using Match3d.Gameplay.Item;
using UnityEngine;

namespace Match3d.Gameplay
{
    public class MonoGameObjectPool : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private ItemBase _gameItemPrefab;

        #endregion

        private GameObjectPool<ItemBase> _gameItemPool;

        private void Start()
        {
            _gameItemPool = new GameObjectPool<ItemBase>(_gameItemPrefab, transform);
            _gameItemPool.Preload(10);
        }

        public ItemBase GetItem()
        {
            return _gameItemPool.Get();
        }

        public void ReleaseItem(ItemBase gameItem)
        {
            _gameItemPool.Release(gameItem);
        }

        public void ReleaseAll()
        {
            foreach (var gameItem in _gameItemPool.PooledObjects)
            {
                if (gameItem.gameObject.activeInHierarchy)
                {
                    ReleaseItem(gameItem);
                }
            }
        }
    }
}