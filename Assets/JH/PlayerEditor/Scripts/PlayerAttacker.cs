using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bloodborne
{
    public class PlayerAttacker : MonoBehaviour
    {
        AnimatorHandler animatorHandler;

        private void Awake()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
        }
        public void HandleLightAttack(WeaponItem weapon)
        {
            animatorHandler.PlayTargetAnimation(weapon.oneHandLightAttack, true);
        }

        public void HandleHeavyAttack(WeaponItem weapon)
        {
            animatorHandler.PlayTargetAnimation(weapon.oneHandHeavyAttack, true);
        }
    }
}