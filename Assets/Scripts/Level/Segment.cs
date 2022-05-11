using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public class Segment
    {
        private IList<Transform> _coins;
        private IList<Transform> _obstaclesSimple;
        private IList<Transform> _obstaclesComplex;

        private GameObjectPool _segmentsPool;
        private GameObjectPool _coinsPool;
        private GameObjectPool _obstaclesSimplePool;
        private GameObjectPool _obstaclesComplexPool;

        private Transform _transform;

        public Vector3 Position
        {
            get
            {
                return _transform.position;
            }
        }

        public Segment(GameObjectPool segmentsPool, GameObjectPool coinsPool, GameObjectPool obstaclesSimplePool, GameObjectPool obstaclesComplexPool, Vector3 position)
        {
            _coins = new List<Transform>();
            _obstaclesSimple = new List<Transform>();
            _obstaclesComplex = new List<Transform>();

            _segmentsPool = segmentsPool;
            _coinsPool = coinsPool;
            _obstaclesSimplePool = obstaclesSimplePool;
            _obstaclesComplexPool = obstaclesComplexPool;

            _transform = _segmentsPool.Rent();
            _transform.position = position;
        }

        public void AddObstacleSimple(Vector3 position)
        {
            var obstacle = _obstaclesSimplePool.Rent();
            _obstaclesSimple.Add(obstacle);

            obstacle.position = Position + position;

            var animation = obstacle.gameObject.GetComponentInChildren<Animation>();
            animation.clip.SampleAnimation(obstacle.gameObject, 0);
        }

        public void AddObstacleComplex(Vector3 position)
        {   
            var obstacle = _obstaclesComplexPool.Rent();
            _obstaclesComplex.Add(obstacle);

            var animation = obstacle.gameObject.GetComponentInChildren<Animation>();
            animation.clip.SampleAnimation(obstacle.gameObject, 0);

            var anim = obstacle.gameObject.GetComponentInChildren<Animation>();
            anim.Rewind();
            anim.Play();
            anim.Sample();
            anim.Stop();

            obstacle.position = Position + position;
        }

        public Transform AddCoin(Vector3 position)
        {
            var coin = _coinsPool.Rent();
            coin.transform.position = _transform.position + position;

            _coins.Add(coin);

            return coin;
        }

        public void RemoveCoin(Transform coinTransform)
        {
            _coins.Remove(coinTransform);

            _coinsPool.Return(coinTransform);
        }

        public void Clear()
        {
            foreach (var coin in _coins)
            {
                _coinsPool.Return(coin);
            }
            _coins.Clear();

            foreach (var obstacle in _obstaclesSimple)
            {
                _obstaclesSimplePool.Return(obstacle);
            }
            _obstaclesSimple.Clear();

            foreach (var obstacle in _obstaclesComplex)
            {
                _obstaclesComplexPool.Return(obstacle);
            }
            _obstaclesSimple.Clear();

            _segmentsPool.Return(_transform);
        }
    }
}