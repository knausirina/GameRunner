using Player;
using UniRx;
using Zenject;

namespace UI
{
    public class GameViewModel
    {
        [Inject]
        private IScoreController _scoreController;

        public IReadOnlyReactiveProperty<int> CurrentScore => _scoreController.CurrentScore;
    }
}