using TMPro;
using UniRx;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using Player;

namespace UI
{
    public class GameView : View
    {
        [SerializeField] private GameObject _dead;
        [SerializeField] private TMP_Text _score;
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _pauseGameButton;
        [SerializeField] private Button _resumeGameButton;

        private CompositeDisposable _scoreDisposable;
        private GameManager _gameManager;
        private IScoreController _scoreController;

        [Inject]
        private void Construct(GameManager gameManager, IScoreController scoreController)
        {
            _gameManager = gameManager;
            _scoreController = scoreController;
        }

        private void Awake()
        {
            _startGameButton.onClick.AddListener(OnStartGame);
            _pauseGameButton.onClick.AddListener(OnPauseGame);
            _resumeGameButton.onClick.AddListener(OnResumeGame);
        }

        private void OnDestroy()
        {
            _startGameButton.onClick.RemoveListener(OnStartGame);
            _pauseGameButton.onClick.RemoveListener(OnPauseGame);
            _resumeGameButton.onClick.RemoveListener(OnResumeGame);
        }

        public void ToggleDead(bool isShow)
        {
            _dead.SetActive(isShow);
        }

        public override void Show()
        {
            base.Show();
            ToggleDead(false);
        }

        private void OnStartGame()
        {
            _gameManager.StartGame();
        }

        private void OnPauseGame()
        {
           
        }

        private void OnResumeGame()
        { }

        private void OnEnable()
        {
            _scoreDisposable = new CompositeDisposable();
            _scoreController.CurrentScore.Subscribe(x => _score.text = x.ToString()).AddTo(_scoreDisposable);
        }

        private void OnDisable()
        {
            _scoreDisposable?.Dispose();
        }
    }
} 