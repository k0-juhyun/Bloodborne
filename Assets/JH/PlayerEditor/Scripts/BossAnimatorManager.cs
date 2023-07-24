using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bloodborne
{
    public class BossAnimatorManager : AnimatorManager
    {
        BossLocomotionManager bossLocomotionManager;
        private void Start()
        {
            anim = GetComponent<Animator>();
            bossLocomotionManager = GetComponent<BossLocomotionManager>();  
        }

        private void OnAnimatorMove()
        {
            float delta = Time.deltaTime;
            bossLocomotionManager.bossRigidBody.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;

            Vector3 velocity = deltaPosition / delta;
            bossLocomotionManager.bossRigidBody.velocity = velocity;
        }
    }
}