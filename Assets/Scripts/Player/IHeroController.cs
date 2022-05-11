using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public interface IHeroController
    {
        void Run();

        void Stop();

        void Pause();

        void Resume();

        void Die();

        void Hit();
    }
}
