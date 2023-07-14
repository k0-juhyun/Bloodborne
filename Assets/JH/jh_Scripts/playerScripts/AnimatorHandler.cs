using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class AnimatorHandler : MonoBehaviour
    {
        PlayerManager playerManager;
        public Animator anim;
        public InputHandler inputHandler;
        public PlayerLocomotion locomotion;
        int vertical;
        int horizontal;
        public bool canRotate;

        [HideInInspector]
        public Transform myTransform;

        public void Initialize()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            anim = GetComponent<Animator>();
            inputHandler = GetComponentInParent<InputHandler>();
            locomotion = GetComponentInParent<PlayerLocomotion>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement)
        {
            #region Vertical
            float v = 0;

            if (verticalMovement > 0 && verticalMovement < 0.55f)
            {
                v = 0.5f;
            }
            else if (verticalMovement > 0.55f)
            {
                v = 1;
            }
            else if (verticalMovement < 0 && verticalMovement > -0.55f)
            {
                v = -0.5f;
            }
            else if (verticalMovement < -0.55f)
            {
                v = -1;
            }
            else
            {
                v = 0;
            }
            #endregion

            #region Horizontal
            float h = 0;

            if (horizontalMovement > 0 && horizontalMovement < 0.55f)
            {
                h = 0.5f;
            }
            if (horizontalMovement > 0.55f)
            {
                h = 1;
            }
            if (horizontalMovement < 0 && horizontalMovement > -0.55f)
            {
                h = -0.5f;
            }
            if (horizontalMovement < -0.55f)
            {
                h = -1;
            }
            else
            {
                h = 0;
            }
            #endregion

            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }

        public void CanRotate()
        {
            canRotate = true;
        }

        public void StopRotate()
        {
            canRotate = false;
        }

        public void PlayTargetAnimation(string targetAnim, bool isInteracting)
        {
            anim.applyRootMotion = isInteracting;
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(targetAnim, 0.2f);
        }

        private void OnAnimatorMove()
        {
            if (playerManager.isInteracting == false)
                return;

            float delta = Time.deltaTime;
            locomotion.rigidbody.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            locomotion.rigidbody.velocity = velocity;
        }
    }
}
