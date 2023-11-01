using UnityEngine;

namespace Player.Events
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