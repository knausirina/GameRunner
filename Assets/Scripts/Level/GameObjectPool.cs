using System.Collections;
using System.Collections.Generic;
using UniRx.Toolkit;
using UnityEngine;

namespace Level
{
    public class GameObjectPool : ObjectPool<Transform>
    {
        private GameObject _prefab;

        public GameObjectPool(GameObject prefab)
        {
            _prefab = prefab;
        }

        protected override Transform CreateInstance()
        {
            return GameObject.Instantiate(_prefab).transform;
        }
    }
}
