using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public interface IPlayerController
    {
        void Start();

        void Stop();

        void Pause();

        void Resume();

        void Die();

        void Hit();
    }
}
