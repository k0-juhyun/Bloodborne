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
        AudioManager2 theAudioManager;

        public string lastAttack;

        private void Awake()
        {
            playerStats = GetComponent<PlayerStats>();
            playerAnimatorManager = GetComponentInChildren<PlayerAnimatorManager>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            inputHandler = GetComponent<InputHandler>();
            theAudioManager = AudioManager2.instance;
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
                    AudioManager2.instance.PlaySFX("Playerwhip");
                    
                }

                if (lastAttack == weapon.oneHandHeavyAttack1)
                {
                    playerAnimatorManager.PlayTargetAnimation(weapon.oneHandHeavyAttack2, true);
                    AudioManager2.instance.PlaySFX("PlayerShooting");
                }

            }
        }
        public void HandleLightAttack(WeaponItem weapon) 
        {
            if (playerStats.currentStamina <= 0)
                return;

            weaponSlotManager.attackingWeapon = weapon;
            playerAnimatorManager.PlayTargetAnimation(weapon.oneHandLightAttack1, true);
            AudioManager2.instance.PlaySFX("Playerwhip");
            lastAttack = weapon.oneHandLightAttack1;
            AudioManager2.instance.PlaySFX("Playerwhip");




        }
        public void HandleHeavyAttack(WeaponItem weapon)
        {
            if (playerStats.currentStamina <= 0)
                return;
            Debug.Log("fdffd");
            weaponSlotManager.attackingWeapon = weapon;
            playerAnimatorManager.PlayTargetAnimation(weapon.oneHandHeavyAttack1, true); 
            AudioManager2.instance.PlaySFX("PlayerShooting");
            lastAttack = weapon.oneHandHeavyAttack1;
           
        }
    }
}