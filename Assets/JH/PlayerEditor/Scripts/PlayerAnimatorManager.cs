using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bloodborne
{
    public class PlayerAnimatorManager : MonoBehaviour
    {
        public Animator anim;
        InputHandler inputHandler;
        PlayerLocomotion playerLocomotion;
        PlayerManager playerManager;
        PlayerAttacker playerAttacker;

        int vertical;
        int horizontal;

        public bool canRotate;
        public bool isAttack;
        public void Initialize()
        {
            playerAttacker = GetComponentInParent<PlayerAttacker>();
            playerManager = GetComponentInParent<PlayerManager>();
            anim = GetComponent<Animator>();
            inputHandler = GetComponentInParent<InputHandler>();
            playerLocomotion = GetComponentInParent<PlayerLocomotion>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting)
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

            else if (horizontalMovement > 0.55f)
            {
                h = 1;
            }

            else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
            {
                h = -0.5f;
            }

            else if (horizontalMovement < -0.55f)
            {
                h = -1;
            }

            else
            {
                h = 0;
            }
            #endregion

            if (isSprinting)
            {
                v = 2;
                h = horizontalMovement;
            }

            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }

        public void PlayTargetAnimation(string targetAnim, bool isInteracting)
        {
            anim.applyRootMotion = isInteracting;
            anim.SetBool("canRotate", false);
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(targetAnim, 0.2f);
        }

        public void CanRotate()
        {
            anim.SetBool("canRotate", true);
        }

        public void StopRotation()
        {
            anim.SetBool("canRotate", false);
        }

        public void EnableCombo()
        {
            anim.SetBool("canDoCombo", true);
            isAttack = true;
        }

        public void DisableCombo()
        {
            anim.SetBool("canDoCombo", false);
            isAttack = false;
        }

        private void OnAnimatorMove()
        {
            if (playerManager.isInteracting == false)
                return;
            float delta = Time.deltaTime;
            playerLocomotion.rigidbody.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            playerLocomotion.rigidbody.velocity = velocity;
        }

        public void EnableIsInvulnerable()
        {
            anim.SetBool("isInvulnerable", true);
        }

        public void DisableIsInvulnerable()
        {
            anim.SetBool("isInvulnerable", false);
        }

        public void EnableFireBullet()
        {
            playerAttacker.HandlePistolAttack(playerAttacker.HandleShootPoint());
        }
    }
}
