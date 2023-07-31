using bloodborne;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class ResetAnimatorValues : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerAnimatorManager.instance.isAttack = false;
    }
}
