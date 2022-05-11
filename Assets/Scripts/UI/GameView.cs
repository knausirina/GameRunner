using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI
{
    public class GameView : View
    {
        public GameObject Dead;

        [SerializeField]
        private TMP_Text _score;

        [Inject]
        private GameViewModel _model;

        private CompositeDisposable _disposable;

        private void OnEnable()
        {
            _disposable = new CompositeDisposable();
            _model.CurrentScore
                .Subscribe(x => _score.text = x.ToString())
                .AddTo(_disposable);
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }

        public void ToggleDead(bool isShow)
        {
            Dead.SetActive(isShow);
        }
    }
} 