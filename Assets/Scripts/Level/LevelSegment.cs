using System;
using System.Collections.Generic;
using Player;
using UniRx;
using UnityEngine;

namespace Level
{
    public class LevelSegment : IDisposable
    {
        private readonly LevelPools _levelPools;
        private readonly Transform _transform;

        private readonly List<Transform> _coins;
        private readonly List<Transform> _obstaclesSimple;
        private readonly List<Transform> _obstaclesComplex;

        private readonly CompositeDisposable _disposable;

        public readonly float LengthSegment;
        public Vector3 Position => _transform.position;

        public LevelSegment(LevelPools levelPools, Vector3 position)
        {
            _levelPools = levelPools;
            _coins = new List<Transform>();
            _obstaclesSimple = new List<Transform>();
            _obstaclesComplex = new List<Transform>();

            _disposable = new CompositeDisposable();

            _transform = _levelPools.Segments.Rent();
            LengthSegment = Math.Abs(_transform.localScale.z);
            _transform.position = position;

            MessageBroker.Default
                .Receive<CoinEvent>()
                .Subscribe(x => OnPickCoin(x.Transform))
                .AddTo(_disposable);
        }

        public void Generate(bool empty)
        {
            if (empty) return;
            var cells = new TypePlace[(int)Math.Ceiling(LengthSegment / LevelController.CELL), LevelController.COLUMNS];
            GenerateObstacles(cells);
            GenerateCoins(cells);
        }

        public void Clear()
        {
            foreach (var coin in _coins)
            {
                _levelPools.Coins.Return(coin);
            }

            _coins.Clear();

            foreach (var obstacle in _obstaclesSimple)
            {
                _levelPools.ObstaclesSimple.Return(obstacle);
            }

            _obstaclesSimple.Clear();

            foreach (var obstacle in _obstaclesComplex)
            {
                _levelPools.ObstaclesComplex.Return(obstacle);
            }

            _obstaclesComplex.Clear();

            _levelPools.Segments.Return(_transform);
        }

        private void AddObstacleSimple(Vector3 position)
        {
            var obstacle = _levelPools.ObstaclesSimple.Rent();
            _obstaclesSimple.Add(obstacle);

            obstacle.position = Position + position;

            var animation = obstacle.GetComponentInChildren<Animation>();
            animation.clip.SampleAnimation(obstacle.gameObject, 0);
        }

        private void AddObstacleComplex(Vector3 position)
        {
            var obstacle = _levelPools.ObstaclesComplex.Rent();
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
            var coin = _levelPools.Coins.Rent();
            coin.transform.position = _transform.position + position;
            _coins.Add(coin);
        }

        private void RemoveCoin(Transform transform)
        {
            _coins.Remove(transform);
            _levelPools.Coins.Return(transform);
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
                if (typeObstacle == (int)TypePlace.obstacleSimple)
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
                            if (cells[randomRow, j] != TypePlace.none)
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
                        cells[randomRow, randomColumn] = TypePlace.obstacleSimple;
                        for (int j = 0; j < columns; j++)
                        {
                            if (j != randomColumn)
                            {
                                cells[randomRow, j] = TypePlace.busy;
                            }

                            if (randomRow > 0)
                            {
                                cells[randomRow - 1, j] = TypePlace.busy;
                            }

                            if (randomRow < (rows - 1))
                            {
                                cells[randomRow + 1, j] = TypePlace.busy;
                            }

                            if (randomRow > 1)
                            {
                                cells[randomRow - 2, j] = TypePlace.busy;
                            }

                            if (randomRow < (rows - 2))
                            {
                                cells[randomRow + 2, j] = TypePlace.busy;
                            }
                        }

                        AddObstacleSimple(new Vector3(-1 + randomColumn, 0.502f, randomRow * LevelController.CELL - LengthSegment / 2.0f + 0.5f));
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
                            if (cells[randomRow, j] != TypePlace.none)
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
                            cells[randomRow, j] = TypePlace.obstacleComplex;
                            if (randomRow > 0)
                            {
                                cells[randomRow - 1, j] = TypePlace.busy;
                            }

                            if (randomRow < (rows - 1))
                            {
                                cells[randomRow + 1, j] = TypePlace.busy;
                            }

                            if (randomRow > 1)
                            {
                                cells[randomRow - 2, j] = TypePlace.busy;
                            }

                            if (randomRow < (rows - 2))
                            {
                                cells[randomRow + 2, j] = TypePlace.busy;
                            }
                        }

                        AddObstacleComplex(new Vector3(0, 0.502f, randomRow * LevelController.CELL - LengthSegment / 2.0f + 0.2f));
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
                    if (cells[i, j] == TypePlace.obstacleComplex)
                    {
                        foundComplex = true;
                    }
                    else if (cells[i, j] == TypePlace.obstacleSimple)
                    {
                        foundSimple = true;
                        columnSimple = j;
                    }
                }

                if (foundComplex || foundSimple)
                {
                    for (int i1 = prevRow; i1 < i; i1++)
                    {
                        AddCoin(new Vector3(-1 + (foundSimple ? columnSimple : 0), 0.3f, 0.5f + i1 * LevelController.CELL - LengthSegment / 2));
                    }

                    foundSimple = false;
                    foundComplex = false;
                    prevRow = i;
                }
            }
        }
    }
}