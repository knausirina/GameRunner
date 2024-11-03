using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Pool
{
    [Serializable]
    public class Pools : IDisposable
    {
        [SerializeField] private GameObject _coinPrefab;
        [SerializeField] private GameObject _segmentPrefab;
        [SerializeField] private GameObject _obstacleSimple;
        [SerializeField] private GameObject _obstacleComplex;

        private GameObjectPool _coinsPool;
        private GameObjectPool _segmentsPool;
        private GameObjectPool _obstaclesSimplePool;
        private GameObjectPool _obstaclesComplexPool;

        public GameObjectPool Coins => GetPool(ref _coinsPool, _coinPrefab);
        public GameObjectPool Segments => GetPool(ref _segmentsPool, _segmentPrefab);
        public GameObjectPool ObstaclesSimple => GetPool(ref _obstaclesSimplePool, _obstacleSimple);
        public GameObjectPool ObstaclesComplex => GetPool(ref _obstaclesComplexPool, _obstacleComplex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static GameObjectPool GetPool(ref GameObjectPool pool, GameObject prefab)
        {
            if (pool == null)
            {
                if (prefab == null)
                    throw new Exception("Prefab is null");
                pool = new GameObjectPool(prefab);
            }

            return pool;
        }

        public void Dispose()
        {
            _coinsPool?.Dispose();
            _segmentsPool?.Dispose();
            _obstaclesSimplePool?.Dispose();
            _obstaclesComplexPool?.Dispose();
        }
    }
}