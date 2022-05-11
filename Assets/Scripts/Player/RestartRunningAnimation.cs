using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Player;
using UniRx;

public class RestartRunningAnimation : StateMachineBehaviour
{
    private static readonly int DEAD = Animator.StringToHash("Dead");

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool(DEAD))
        {
            return;
        }

        MessageBroker.Default.Publish(new ResumeEvent());
    }
}
