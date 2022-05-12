using UniRx.Toolkit;
using UnityEngine;

namespace Level
{
    public class GameObjectPool : ObjectPool<Transform>
    {
        private readonly GameObject _prefab;

        public GameObjectPool(GameObject prefab)
        {
            _prefab = prefab;
        }

        protected override Transform CreateInstance()
        {
            return Object.Instantiate(_prefab).transform;
        }
    }
}