using TMPro;
using UniRx;
using UnityEngine;
using Zenject;
using UnityEngine.UI;

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
        private GameViewModel _gameViewModel;
        private GameManager _gameManager;

        [Inject]
        private void Construct(GameViewModel gameViewModel, GameManager gameManager)
        {
            _gameViewModel = gameViewModel;
            _gameManager = gameManager;
        }

        private void Awake()
        {
            _startGameButton.onClick.AddListener(OnStartGame);
            _pauseGameButton.onClick.AddListener(OnPauseGame);
            _resumeGameButton.onClick.AddListener(OnResumeGame);
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
            _gameViewModel.CurrentScore.Subscribe(x => _score.text = x.ToString()).AddTo(_scoreDisposable);
        }

        private void OnDisable()
        {
            _scoreDisposable?.Dispose();
        }
    }
} 