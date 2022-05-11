using System;
using UniRx;
using UnityEngine;

namespace Player
{
    public class PlayerData : IDisposable
    {
        private readonly ReactiveProperty<int> _topScore;

        public IReadOnlyReactiveProperty<int> TopScore => _topScore;

        public PlayerData()
        {
            _topScore = new ReactiveProperty<int>(PlayerPrefs.GetInt("topScore", 0));
        }

        public void SetTopScore(int score)
        {
            _topScore.Value = score;
        }

        #region IDisposable

        void IDisposable.Dispose()
        {
            PlayerPrefs.SetInt("topScore", _topScore.Value);
        }

        #endregion
    }
}