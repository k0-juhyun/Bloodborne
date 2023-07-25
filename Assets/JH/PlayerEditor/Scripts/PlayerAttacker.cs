using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bloodborne
{
    public class PlayerAttacker : MonoBehaviour
    {
        PlayerAnimatorManager playerAnimatorManager;
        InputHandler inputHandler;
        WeaponSlotManager weaponSlotManager;
        PlayerStats playerStats;

        public string lastAttack;

        private void Awake()
        {
            playerStats = GetComponent<PlayerStats>();
            playerAnimatorManager = GetComponentInChildren<PlayerAnimatorManager>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            inputHandler = GetComponent<InputHandler>();
        }

        public void HandleWeaponCombo(WeaponItem weapon)
        {
            if (playerStats.currentStamina <= 0)
                return;

            if (inputHandler.comboFlag)
            {
                playerAnimatorManager.anim.SetBool("canDoCombo", false);

                if (lastAttack == weapon.oneHandLightAttack1)
                {
                    playerAnimatorManager.PlayTargetAnimation(weapon.oneHandLightAttack2, true);
                }

                if (lastAttack == weapon.oneHandHeavyAttack1)
                {
                    playerAnimatorManager.PlayTargetAnimation(weapon.oneHandHeavyAttack2, true);
                }

            }
        }
        public void HandleLightAttack(WeaponItem weapon)
        {
            if (playerStats.currentStamina <= 0)
                return;

            weaponSlotManager.attackingWeapon = weapon;
            playerAnimatorManager.PlayTargetAnimation(weapon.oneHandLightAttack1, true);
            lastAttack = weapon.oneHandLightAttack1;
        }
        public void HandleHeavyAttack(WeaponItem weapon)
        {
            if (playerStats.currentStamina <= 0)
                return;

            weaponSlotManager.attackingWeapon = weapon;
            playerAnimatorManager.PlayTargetAnimation(weapon.oneHandHeavyAttack1, true);
            lastAttack = weapon.oneHandHeavyAttack1;
        }
    }
}