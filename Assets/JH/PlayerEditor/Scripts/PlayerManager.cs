using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bloodborne
{
    public class PlayerManager : CharacterManager
    {
        InputHandler inputHandler;
        Animator anim;
        CameraHandler cameraHandler;
        PlayerLocomotion playerLocomotion;
        PlayerStats playerStats;
        PlayerAnimatorManager playerAnimatorManager;

        [Header("Player Flags")]
        public bool isInteracting;
        public bool isSprinting;
        public bool isInAir;
        public bool isGrounded;
        public bool canDoCombo;
        public bool isInvulnerable;

        private void Start()
        {
            playerStats = GetComponent<PlayerStats>();
            cameraHandler = FindObjectOfType<CameraHandler>();
            inputHandler = GetComponent<InputHandler>();
            anim = GetComponentInChildren<Animator>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            playerAnimatorManager = GetComponentInChildren<PlayerAnimatorManager>();
        }

        private void Update()
        {
            Cursor.visible = false;
            if (this.gameObject != null)
            {
                float delta = Time.deltaTime;
                isInteracting = anim.GetBool("isInteracting");
                canDoCombo = anim.GetBool("canDoCombo");
                isInvulnerable = anim.GetBool("isInvulnerable");


                inputHandler.TickInput(delta);
                playerLocomotion.HandleRollingAndSprinting(delta);
                playerLocomotion.HandleDrinkPotion();
                playerLocomotion.HandleKnockBack();
                playerStats.RegenerateStamina();
                playerAnimatorManager.canRotate = anim.GetBool("canRotate");
            }

            if(Input.GetKey(KeyCode.Alpha1))
            {
                playerStats.DeveloperMode();
            }
            if(Input.GetKeyDown(KeyCode.Alpha2)) 
            {
                cameraHandler.ResetCameraPosition();
                print("reset");
            }
        }

        private void FixedUpdate()
        {
            if (this.gameObject != null)
            {
                float delta = Time.fixedDeltaTime;
                playerLocomotion.HandleMovement(delta);
                playerLocomotion.HandleRotation(delta);
                playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
            }
        }

        private void LateUpdate()
        {
            if (this.gameObject != null)
            {
                inputHandler.rollFlag = false;
                inputHandler.rb_Input = false;
                inputHandler.rt_Input = false;
                inputHandler.lf_Input = false;
                inputHandler.lg_Input = false;
                inputHandler.potion_Input = false;

                float delta = Time.deltaTime;

                if (cameraHandler != null)
                {
                    cameraHandler.FollowTarget(delta);
                    cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
                }

                if (isInAir)
                {
                    playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
                }
            }
        }
    }
}
