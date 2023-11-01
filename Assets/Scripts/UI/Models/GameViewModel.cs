using Player;
using UniRx;
using Zenject;

namespace UI
{
    public class GameViewModel
    {
        private IScoreController _scoreController;
        public IReadOnlyReactiveProperty<int> CurrentScore => _scoreController.CurrentScore;

        [Inject]
        private void Construct(IScoreController scoreController)
        {
            _scoreController = scoreController;
        }
    }
}