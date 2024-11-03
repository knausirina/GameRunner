using UnityEngine;
using Player;
using Player.Events;
using UI;
using UniRx;
using Zenject;
using Assets.Scripts;
using Level;
using Pool;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameManager : IDisposable
{
    [Inject] private IPlayerController _playerController;
    [Inject] private IScoreController _scoreController;
	[Inject] private LevelController _levelController;

    private const int MaxCountLife = 3;
	private int _hits;
	private int _score;
	private GameStateEnum _gameState;
	private GameContext _gameContext;
	private CompositeDisposable _compositeDisposable = new CompositeDisposable();

    public GameManager(GameContext gameContext)
	{
		_gameContext = gameContext;
	}

    private void Awake()
	{
		MessageBroker.Default.Receive<CoinEvent>()
			  .Subscribe(x => OnPickCoin(x.Transform)).AddTo(_compositeDisposable);

		MessageBroker.Default.Receive<ObstacleEvent>()
			  .Subscribe(x => OnHitObstacle()).AddTo(_compositeDisposable);

		MessageBroker.Default.Receive<ResumeEvent>()
			  .Subscribe(x => OnResume()).AddTo(_compositeDisposable);
	}

	public void StartGame()
	{
		_hits = 0;

		_scoreController.Reset();
		_playerController.Start();
        _levelController.SetPlayer((_playerController as MonoBehaviour).transform);
		_levelController.Build();

        _gameContext.Views.ShowGame();
	}

	public void StopGame()
	{
		_playerController.Stop();
        _levelController.Stop();

        _gameContext.Views.ShowStart();
	}

    public void OnResume()
    {
        _playerController.Resume();
        _levelController.Resume();

    }

    private void OnPickCoin(Transform transform)
	{
		_scoreController.AddScore(1);
	}

	private void OnHitObstacle()
	{
		_hits++;

		if (_hits <= MaxCountLife)
		{
			_playerController.Hit();
            _levelController.Stop();
        }
		else
		{
			_playerController.Die();
            _levelController.Stop();

            _gameContext.Views.ShowDead();
		}
	}

	public void Dispose()
	{
        _compositeDisposable.Dispose();

    }
}
