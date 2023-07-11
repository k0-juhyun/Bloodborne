using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pattern6Rot : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // ���� ������Ʈ�� ȸ������ �����ɴϴ�.
        Vector3 rotation = animator.transform.rotation.eulerAngles;

        // X ���� 20����ŭ ����Դϴ�.
        rotation.x += 50f;

        // ȸ������ �����մϴ�.
        animator.transform.rotation = Quaternion.Euler(rotation);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
