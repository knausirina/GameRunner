using System;
using UniRx;
using UnityEngine;

namespace Player
{
    public class PlayerData : IDisposable
    {
        private const string TopScorePlayerPrefs = "topScore";

        private readonly ReactiveProperty<int> _topScore;

        public IReadOnlyReactiveProperty<int> TopScore => _topScore;

        public PlayerData()
        {
            _topScore = new ReactiveProperty<int>(PlayerPrefs.GetInt(TopScorePlayerPrefs, 0));
        }

        public void SetTopScore(int score)
        {
            _topScore.Value = score;
        }

        public void Dispose()
        {
            PlayerPrefs.SetInt(TopScorePlayerPrefs, _topScore.Value);
        }
    }
}