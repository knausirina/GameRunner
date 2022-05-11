using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

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
