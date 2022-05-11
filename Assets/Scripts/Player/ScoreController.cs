using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace Player
{
	public class ScoreController : IScoreController, IDisposable
    {
        private readonly PlayerData _playerData;
        private readonly ReactiveProperty<int> _currentScore = new ReactiveProperty<int>();

        public ScoreController(PlayerData playerData)
        {
            _playerData = playerData;
        }

        #region IScoreController

        IReadOnlyReactiveProperty<int> IScoreController.CurrentScore => _currentScore;
        IReadOnlyReactiveProperty<int> IScoreController.TopScore => _playerData.TopScore;

		void IScoreController.AddScore(int score)
        {
            _currentScore.Value += score;
            if (_playerData.TopScore.Value < _currentScore.Value)
                _playerData.SetTopScore(_currentScore.Value);
        }

        void IScoreController.Reset()
        {
            _currentScore.Value = 0;
        }

        #endregion

        void IDisposable.Dispose()
        {
        }
    }
}
