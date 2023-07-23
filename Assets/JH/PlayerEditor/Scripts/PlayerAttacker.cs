using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bloodborne
{
    public class PlayerAttacker : MonoBehaviour
    {
        AnimatorHandler animatorHandler;
        InputHandler inputHandler;
        WeaponSlotManager weaponSlotManager;

        public string lastAttack;

        private void Awake()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            inputHandler = GetComponent<InputHandler>();
        }

        public void HandleWeaponCombo(WeaponItem weapon)
        {
            if (inputHandler.comboFlag)
            {
                animatorHandler.anim.SetBool("canDoCombo", false);

                if (lastAttack == weapon.oneHandLightAttack1)
                {
                    animatorHandler.PlayTargetAnimation(weapon.oneHandLightAttack2, true);
                }

                if (lastAttack == weapon.oneHandHeavyAttack1)
                {
                    animatorHandler.PlayTargetAnimation(weapon.oneHandHeavyAttack2, true);
                }

            }
        }
        public void HandleLightAttack(WeaponItem weapon)
        {
            weaponSlotManager.attackingWeapon = weapon;
            animatorHandler.PlayTargetAnimation(weapon.oneHandLightAttack1, true);
            lastAttack = weapon.oneHandLightAttack1;
        }
        public void HandleHeavyAttack(WeaponItem weapon)
        {
            weaponSlotManager.attackingWeapon = weapon;
            animatorHandler.PlayTargetAnimation(weapon.oneHandHeavyAttack1, true);
            lastAttack = weapon.oneHandHeavyAttack1;
        }
    }
}