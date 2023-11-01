using UnityEngine;
using Player;
using UniRx;

public class RestartRunningAnimation : StateMachineBehaviour
{
    private static readonly int Dead = Animator.StringToHash("Dead");

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool(Dead))
        {
            return;
        }

        MessageBroker.Default.Publish(new ResumeEvent());
    }
}
