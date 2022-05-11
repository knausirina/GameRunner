using Player;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace Level
{
    public class LevelController : ITickable , IDisposable
    {
        private bool _isPlaying = false;
        private float _beginPos = 0;

        private enum typePlace { none = 0, busy = 1, obstacleSimple = 2, obstacleComplex = 3 };

        private int _columns;
        private int _rows;
        private float _cell;
        private float _lenghtSegment;
        private typePlace[,] _positions;
        private IList<Segment> _segments;
        private Dictionary<Transform, Segment> _segmentsByCoins;

        private GameObjectPool _coinsPool;
        private GameObjectPool _segmentsPool;
        private GameObjectPool _obstaclesSimplePool;
        private GameObjectPool _obstaclesComplexPool;

        public Vector3 _lastCreatedSegmentPosition;
        private Transform _playerTransform;

        private const int DISTANCE_MIN_DRAW = 10;

        public LevelController (GameObject segmentPrefab, GameObject coinPrefab, GameObject simpleObstaclePrefab, GameObject complesObstaclePrefab, Vector3 lastCreatedSegmentPosition)
        {
            _lastCreatedSegmentPosition = lastCreatedSegmentPosition;

            _lenghtSegment = Math.Abs(segmentPrefab.transform.localScale.z);
            _cell = 4.0f / 3.0f;
            _columns = 3;
            _rows = (int)Math.Ceiling(_lenghtSegment / _cell);

            _positions = new typePlace[_rows, _columns];
            Array.Clear(_positions, 0, _positions.Length);

            _coinsPool = new GameObjectPool(coinPrefab);
			_segmentsPool = new GameObjectPool(segmentPrefab);
            _obstaclesSimplePool = new GameObjectPool(simpleObstaclePrefab);
            _obstaclesComplexPool = new GameObjectPool(complesObstaclePrefab);

            _segments = new List<Segment>();
            _segmentsByCoins = new Dictionary<Transform, Segment>();

            MessageBroker.Default.Receive<CoinEvent>()
                .Subscribe(x => OnPickCoin(x.Transform));
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

            _segmentsByCoins.Clear();
        }

        private void ClearUnVisible()
        {
            foreach (var segment in _segments)
            {
                if ((_playerTransform.position.z - segment.Position.z) > (_lenghtSegment / 2))
                {
                    segment.Clear();
                    _segments.Remove(segment);
                    break;
                }
            }
        }
        private bool NeedBuild()
        {
            return ((_playerTransform.position.z - _beginPos) > DISTANCE_MIN_DRAW);
        }

        private Segment GenerateSegment()
        {
            var position = new Vector3(_lastCreatedSegmentPosition.x, _lastCreatedSegmentPosition.y, _lastCreatedSegmentPosition.z + _lenghtSegment + _beginPos);
            var segment = new Segment(_segmentsPool, _coinsPool, _obstaclesSimplePool, _obstaclesComplexPool, position);

            _beginPos += _lenghtSegment;
            _segments.Add(segment);
            return segment;
        }

        private void GenerateObstacles(Segment segment)
        {
            int count = UnityEngine.Random.Range(1, 4);

            for (int i = 0; i < count; i++)
            {
                int typeObstacle = UnityEngine.Random.Range(2, 4);
                if (typeObstacle == (int)typePlace.obstacleSimple)
                {
                    int attempCount = 3;
                    var found = false;
                    int randomColumn = 0;
                    int randomRow = 0;
                    while (attempCount > 0)
                    {
                        randomColumn = UnityEngine.Random.Range(0, _columns);
                        randomRow = UnityEngine.Random.Range(0, _rows);
                        bool allowPosition = true;
                        for (int j = 0; j < _columns; j++)
                        {
                            if (_positions[randomRow, j] != typePlace.none)
                            {
                                allowPosition = false;
                                break;
                            }
                        }
                        if (allowPosition)
                        {
                            found = true;
                            break;
                        }
                        attempCount--;
                    }
                    if (found)
                    {
                        _positions[randomRow, randomColumn] = typePlace.obstacleSimple;
                        for (int j = 0; j < _columns; j++)
                        {
                            if (j != randomColumn)
                            {
                                _positions[randomRow, j] = typePlace.busy;
                            }
                            if (randomRow > 0)
                            {
                               _positions[randomRow - 1, j] = typePlace.busy;
                            }
                            if (randomRow < (_rows - 1))
                            {
                               _positions[randomRow + 1, j] = typePlace.busy;
                            }
                            if (randomRow > 1)
                            {
                               _positions[randomRow - 2, j] = typePlace.busy;
                            }
                            if (randomRow < (_rows - 2))
                            {
                               _positions[randomRow  + 2, j] = typePlace.busy;
                            }
                        }

                        segment.AddObstacleSimple(new Vector3(-1 + randomColumn, 0.502f, randomRow * _cell - _lenghtSegment / 2.0f + 0.5f));
                    }
                }
                else
                {
                    var attempCount = 3;
                    var found = false;
                    int randomRow = 0;
                    while (attempCount > 0)
                    {
                        randomRow = UnityEngine.Random.Range(1, _rows);
                        bool allowPosition = true;
                        for (int j = 0; j < _columns; j++)
                        {
                            if (_positions[randomRow, j] != typePlace.none)
                            {
                                allowPosition = false;
                                break;
                            }
                        }
                        if (allowPosition)
                        {
                            found = true;
                            break;
                        }
                        attempCount--;
                    }
                    if (found)
                    {
                        for (int j = 0; j < _columns; j++)
                        {
                            _positions[randomRow, j] = typePlace.obstacleComplex;
                            if (randomRow > 0)
                            {
                                _positions[randomRow - 1, j] = typePlace.busy;
                            }
                            if (randomRow < (_rows - 1))
                            {
                               _positions[randomRow + 1, j] = typePlace.busy;
                            }
                            if (randomRow > 1)
                            {
                               _positions[randomRow - 2, j] = typePlace.busy;
                            }
                            if (randomRow < (_rows - 2))
                            {
                               _positions[randomRow + 2, j] = typePlace.busy;
                            }
                        }
                        segment.AddObstacleComplex(new Vector3(0, 0.502f, randomRow * _cell - _lenghtSegment / 2.0f + 0.2f));
                    }
                }
            }
        }

        private void GenerateCoins(Segment segment)
        {
            int prevRow = 0;
            bool foundComplex = false;
            bool foundSimple = false;
            int columnSimple = 0;
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    if (_positions[i, j] == typePlace.obstacleComplex)
                    {
                        foundComplex = true;
                    }
                    else if (_positions[i, j] == typePlace.obstacleSimple)
                    {
                        foundSimple = true;
                        columnSimple = j;
                    }
                }
                if (foundComplex || foundSimple)
                {
                    for (int i1 = prevRow; i1 < i; i1++)
                    {
                        var coin = segment.AddCoin(new Vector3(-1 + (foundSimple ? columnSimple : 0), 0.3f, 0.5f + i1 * _cell - _lenghtSegment / 2));
                        _segmentsByCoins[coin] = segment;
                    }
                    foundSimple = false;
                    foundComplex = false;
                    prevRow = i;
                }
            }
        }

        void IDisposable.Dispose()
        {

        }
    }
}
