using TMPro;
using UniRx;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using TMPro.Examples;

namespace UI
{
    public class StartView : View
    {
        [SerializeField] Button _startGameButton;

        private GameManager _gameManager;

        [Inject]
        private void Construct(GameManager gameManager)
        {
            Debug.Log("xxxxx StartView construct");
            _gameManager = gameManager;
        }

        private void Awake()
        {
            _startGameButton.onClick.AddListener(OnStartGameButton);
        }

        private void OnStartGameButton()
        {
            _gameManager.StartGame();
        }

        private void OnDestroy()
        {
            _startGameButton.onClick.RemoveListener(OnStartGameButton);
        }
    }
}
