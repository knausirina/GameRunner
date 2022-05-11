using UniRx;

namespace Player
{
    public interface IScoreController
    {
        IReadOnlyReactiveProperty<int> CurrentScore { get; }
        IReadOnlyReactiveProperty<int> TopScore { get; }

        void AddScore(int score);
        void Reset();
    }
}