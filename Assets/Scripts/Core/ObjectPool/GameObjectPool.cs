using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Match3d.Core
{
    public class GameObjectPool<T> where T : MonoBehaviour
    {
        private readonly IObjectPool<T> _objectPool;
        private readonly Transform _parent;
        private readonly T _prefab;
        private readonly HashSet<T> _pooledObjects;

        public IReadOnlyCollection<T> PooledObjects => _pooledObjects;

        public GameObjectPool(T prefab, Transform parent = null, int initialCapacity = 10, int maxSize = 100)
        {
            _prefab = prefab;
            _parent = parent;
            _pooledObjects = new HashSet<T>();
            _objectPool = new ObjectPool<T>(
                createFunc: CreatePooledItem,
                actionOnGet: OnTakeFromPool,
                actionOnRelease: OnReturnedToPool,
                actionOnDestroy: OnDestroyPoolObject,
                collectionCheck: false,
                defaultCapacity: initialCapacity,
                maxSize: maxSize);
        }

        public T Get()
        {
            var item = _objectPool.Get();
            _pooledObjects.Add(item);
            return item;
        }

        public void Release(T obj)
        {
            _objectPool.Release(obj);
        }

        public void Preload(int count)
        {
            var temp = new T[count];
            for (var i = 0; i < count; i++)
            {
                temp[i] = _objectPool.Get();
            }

            for (var i = 0; i < count; i++)
            {
                _objectPool.Release(temp[i]);
            }
        }

        private void OnDestroyPoolObject(T item)
        {
            Object.Destroy(item.gameObject);
        }

        private void OnReturnedToPool(T item)
        {
            item.gameObject.SetActive(false);
        }

        private void OnTakeFromPool(T item)
        {
            item.gameObject.SetActive(true);
        }

        private T CreatePooledItem()
        {
            var item = Object.Instantiate(_prefab, _parent);
            item.gameObject.SetActive(false);
            return item;
        }    
    }
}