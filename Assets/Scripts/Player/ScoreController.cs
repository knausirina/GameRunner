using UniRx;
using System;

namespace Player
{
	public class ScoreController : IScoreController
    {
        private readonly PlayerData _playerData;
        private readonly ReactiveProperty<int> _currentScore = new ReactiveProperty<int>();

        public IReadOnlyReactiveProperty<int> CurrentScore => _currentScore;
        public IReadOnlyReactiveProperty<int> TopScore => _playerData.TopScore;

        public ScoreController()
        {
            _playerData = new PlayerData();
        }

		public void AddScore(int score)
        {
            _currentScore.Value += score;
            if (_playerData.TopScore.Value < _currentScore.Value)
                _playerData.SetTopScore(_currentScore.Value);
        }

        public void Reset()
        {
            _currentScore.Value = 0;
        }
    }
}
