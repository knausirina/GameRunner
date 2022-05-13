using UnityEngine;

namespace Player
{
    public class CoinEvent
    {
        public Transform Transform { get; }

        public CoinEvent(Transform transform)
        {
            Transform = transform;
        }
    }
}