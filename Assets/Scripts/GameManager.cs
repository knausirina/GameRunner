using UnityEngine;
using Player;
using UI;
using UniRx;
using Zenject;

public class GameManager : MonoBehaviour
{
	private const int MAX_COUNT_LIFE = 3;

	[Inject]
	public IHeroController _heroController;
	[Inject]
	private IScoreController _scoreController;

	private int _hits;

	public Views Views;
	public GameObject Player;

	private void Awake()
	{
		Player.SetActive(false);

		MessageBroker.Default.Receive<CoinEvent>()
			  .Subscribe(x => OnPickCoin(x.Transform)).AddTo(this);

		MessageBroker.Default.Receive<ObstacleEvent>()
			  .Subscribe(x => OnHitObstacle()).AddTo(this);

		MessageBroker.Default.Receive<ResumeEvent>()
			  .Subscribe(x => OnResume()).AddTo(this);
	}

	public void StartGame()
	{
		_hits = 0;

		_scoreController.Reset();

		Player.SetActive(true);

		_heroController.Run();

		Views.ShowGame();
	}

	public void StopGame()
	{
		Player.SetActive(false);

		_heroController.Stop();

		Views.ShowStart();
	}

	private void OnPickCoin(Transform transform)
	{
		_scoreController.AddScore(1);
	}

	private void OnHitObstacle()
	{
		_hits++;

		if (_hits <= MAX_COUNT_LIFE)
		{
			_heroController.Hit();
		}
		else
		{
			_heroController.Die();

			Views.ShowDead();
		}
	}

	private void OnResume()
	{
		_heroController.Resume();
	}
}
