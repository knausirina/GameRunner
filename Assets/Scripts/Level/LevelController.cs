using Player;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace Level
{
    public class LevelController : ITickable, IDisposable
    {
		private const int DISTANCE_MIN_DRAW = 30;
        public const int COLUMNS = 3;
        public const float CELL = 4f / COLUMNS;

        private readonly LevelPools _levelPools;
        private readonly Vector3 _startPosition;
        private readonly IList<LevelSegment> _segments;

        private bool _isPlaying = false;
        private float _beginPos = 0;

        private GameObjectPool _coinsPool;
        private GameObjectPool _segmentsPool;
        private GameObjectPool _obstaclesSimplePool;
        private GameObjectPool _obstaclesComplexPool;

        private Transform _playerTransform;

		public LevelSegment _lastSegment;

        public LevelController(LevelPools levelPools, Vector3 startPosition)
        {
            _levelPools = levelPools;
            _startPosition = startPosition;
            _segments = new List<LevelSegment>();
        }

        public void SetPlayer(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }

        public void Build()
        {
            Clear();
            _isPlaying = true;
            _beginPos = _playerTransform.position.z;
            GenerateSegment(true);
            GenerateSegment(true);
        }

        public void Stop()
        {
            _isPlaying = false;
        }

        public void Resume()
        {
            _isPlaying = true;
        }

        public void Tick()
        {
            if (!_isPlaying)
            {
                return;
            }

            ClearUnVisible();
            if (!NeedBuild())
            {
                return;
            }

            GenerateSegment(false);
        }

        private void Clear()
        {
            foreach (var segment in _segments)
            {
                segment.Clear();
            }

            _segments.Clear();
        }

        private void ClearUnVisible()
        {
            foreach (var segment in _segments)
            {
                if ((_playerTransform.position.z - segment.Position.z) > (segment.LengthSegment / 2))
                {
                    segment.Dispose();
                    _segments.Remove(segment);
                    break;
                }
            }
        }

        private bool NeedBuild()
        {
            return _beginPos - _playerTransform.position.z < DISTANCE_MIN_DRAW;
        }

        private void GenerateSegment(bool empty)
        {
            var position = _startPosition;
            position.z += _beginPos;
            Debug.Log(position);
            _lastSegment = new LevelSegment(_levelPools, position);
            _lastSegment.Generate(empty);
            _beginPos += _lastSegment.LengthSegment;
            _segments.Add(_lastSegment);
        }

        void IDisposable.Dispose()
        {
            foreach (var segment in _segments)
            {
                segment.Dispose();
            }

            _segments.Clear();
        }
    }
}