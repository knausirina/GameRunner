using System;
using System.Collections.Generic;
using Player.Events;
using UniRx;
using UnityEngine;
using Pool;
using TMPro.Examples;

namespace Level
{
    public class LevelSegment : IDisposable
    {
        private readonly Pools _pools;
        private readonly Transform _segmentTransform;

        private readonly List<Transform> _coins;
        private readonly List<Transform> _obstaclesSimple;
        private readonly List<Transform> _obstaclesComplex;

        private readonly CompositeDisposable _disposable;

        public readonly float LengthSegment;
        public Vector3 Position => _segmentTransform.position;

        public LevelSegment(Pools pools, Vector3 position)
        {
            _pools = pools;
            _coins = new List<Transform>();
            _obstaclesSimple = new List<Transform>();
            _obstaclesComplex = new List<Transform>();

            _disposable = new CompositeDisposable();

            _segmentTransform = _pools.Segments.Rent();
            _segmentTransform.position = position;
            LengthSegment = Math.Abs(_segmentTransform.localScale.z);

            MessageBroker.Default
                .Receive<CoinEvent>()
                .Subscribe(x => OnPickCoin(x.Transform))
                .AddTo(_disposable);
        }

        public void Generate(bool isEmptySegment)
        {
            if (isEmptySegment)
            {
                return;
            }

            var rows = (int)Math.Ceiling(LengthSegment / Config.RoadWidth);
            var cells = new TypePlace[rows, Config.RoundsCount];
            GenerateObstacles(cells);
            GenerateCoins(cells);
        }

        public void Clear()
        {
            foreach (var coin in _coins)
            {
                if (coin == null)
                {
                    Debug.Log("xxx !!! coin is null");
                }
                _pools.Coins.Return(coin);
            }
            _coins.Clear();

            foreach (var obstacle in _obstaclesSimple)
            {
                if (obstacle == null)
                {
                    Debug.Log("xxx !!! obstacle is null");
                }
                _pools.ObstaclesSimple.Return(obstacle);
            }
            _obstaclesSimple.Clear();

            foreach (var obstacle in _obstaclesComplex)
            {
                _pools.ObstaclesComplex.Return(obstacle);
            }
            _obstaclesComplex.Clear();

            if (_segmentTransform != null)
            {
                _pools.Segments.Return(_segmentTransform);
            }
        }

        private void AddObstacleSimple(Vector3 position)
        {
            var obstacle = _pools.ObstaclesSimple.Rent();
            _obstaclesSimple.Add(obstacle);

            obstacle.position = Position + position;

            var animation = obstacle.GetComponentInChildren<Animation>();
            animation.clip.SampleAnimation(obstacle.gameObject, 0);
        }

        private void AddObstacleComplex(Vector3 position)
        {
            var obstacle = _pools.ObstaclesComplex.Rent();
            _obstaclesComplex.Add(obstacle);

            var animation = obstacle.GetComponentInChildren<Animation>();
            animation.clip.SampleAnimation(obstacle.gameObject, 0);

            var anim = obstacle.GetComponentInChildren<Animation>();
            anim.Rewind();
            anim.Play();
            anim.Sample();
            anim.Stop();

            obstacle.position = Position + position;
        }

        private void AddCoin(Vector3 position)
        {
            var coin = _pools.Coins.Rent();
            coin.transform.position = _segmentTransform.position + position;
            _coins.Add(coin);
        }

        private void RemoveCoin(Transform transform)
        {
            _coins.Remove(transform);
            _pools.Coins.Return(transform);
        }

        public void Dispose()
        {
            _disposable.Dispose();
            Clear();
        }

        private void OnPickCoin(Transform coinTransform)
        {
            if (!_coins.Contains(coinTransform))
            {
                return;
            }
            RemoveCoin(coinTransform);
        }

        private void GenerateObstacles(TypePlace[,] cells)
        {
            int count = UnityEngine.Random.Range(1, 4);
            int rows = cells.GetLength(0);
            int columns = cells.GetLength(1);

            for (int i = 0; i < count; i++)
            {
                int typeObstacle = UnityEngine.Random.Range(2, 4);
                if (typeObstacle == (int)TypePlace.SimpleObstacle)
                {
                    int attempCount = 3;
                    var found = false;
                    int randomColumn = 0;
                    int randomRow = 0;
                    while (attempCount > 0)
                    {
                        randomRow = UnityEngine.Random.Range(0, cells.GetLength(0));
                        randomColumn = UnityEngine.Random.Range(0, cells.GetLength(1));
                        bool allowPosition = true;
                        for (int j = 0; j < columns; j++)
                        {
                            if (cells[randomRow, j] != TypePlace.None)
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
                        cells[randomRow, randomColumn] = TypePlace.SimpleObstacle;
                        for (int j = 0; j < columns; j++)
                        {
                            if (j != randomColumn)
                            {
                                cells[randomRow, j] = TypePlace.Busy;
                            }

                            if (randomRow > 0)
                            {
                                cells[randomRow - 1, j] = TypePlace.Busy;
                            }

                            if (randomRow < (rows - 1))
                            {
                                cells[randomRow + 1, j] = TypePlace.Busy;
                            }

                            if (randomRow > 1)
                            {
                                cells[randomRow - 2, j] = TypePlace.Busy;
                            }

                            if (randomRow < (rows - 2))
                            {
                                cells[randomRow + 2, j] = TypePlace.Busy;
                            }
                        }

                        AddObstacleSimple(new Vector3(-1 + randomColumn, 0.502f, randomRow * Config.RoadWidth - LengthSegment / 2.0f + 0.5f));
                    }
                }
                else
                {
                    var attempCount = 3;
                    var found = false;
                    int randomRow = 0;
                    while (attempCount > 0)
                    {
                        randomRow = UnityEngine.Random.Range(1, rows);
                        bool allowPosition = true;
                        for (int j = 0; j < columns; j++)
                        {
                            if (cells[randomRow, j] != TypePlace.None)
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
                        for (int j = 0; j < columns; j++)
                        {
                            cells[randomRow, j] = TypePlace.ComplexObstacle;
                            if (randomRow > 0)
                            {
                                cells[randomRow - 1, j] = TypePlace.Busy;
                            }

                            if (randomRow < (rows - 1))
                            {
                                cells[randomRow + 1, j] = TypePlace.Busy;
                            }

                            if (randomRow > 1)
                            {
                                cells[randomRow - 2, j] = TypePlace.Busy;
                            }

                            if (randomRow < (rows - 2))
                            {
                                cells[randomRow + 2, j] = TypePlace.Busy;
                            }
                        }

                        AddObstacleComplex(new Vector3(0, 0.502f, randomRow * Config.RoadWidth - LengthSegment / 2.0f + 0.2f));
                    }
                }
            }
        }

        private void GenerateCoins(TypePlace[,] cells)
        {
            int rows = cells.GetLength(0);
            int columns = cells.GetLength(1);
            int prevRow = 0;
            bool foundComplex = false;
            bool foundSimple = false;
            int columnSimple = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (cells[i, j] == TypePlace.ComplexObstacle)
                    {
                        foundComplex = true;
                    }
                    else if (cells[i, j] == TypePlace.SimpleObstacle)
                    {
                        foundSimple = true;
                        columnSimple = j;
                    }
                }

                if (foundComplex || foundSimple)
                {
                    for (int i1 = prevRow; i1 < i; i1++)
                    {
                        AddCoin(new Vector3(-1 + (foundSimple ? columnSimple : 0), 0.3f, 0.5f + i1 * Config.RoadWidth - LengthSegment / 2));
                    }

                    foundSimple = false;
                    foundComplex = false;
                    prevRow = i;
                }
            }
        }
    }
}