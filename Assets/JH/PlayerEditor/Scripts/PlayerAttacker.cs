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
        bossAI bossAi;
        BossAlpha bossAlpha;

        public string lastAttack;
        private GameObject muzzleEffect;

        private void Awake()
        {
            playerStats = GetComponent<PlayerStats>();
            playerAnimatorManager = GetComponentInChildren<PlayerAnimatorManager>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            inputHandler = GetComponent<InputHandler>();
            bossAi = FindObjectOfType<bossAI>();
            bossAlpha = FindObjectOfType<BossAlpha>();

            muzzleEffect = Resources.Load<GameObject>("GunFireEfferct_Player");
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
                    AudioManager2.instance.PlaySFX("Playerwhip");
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
            AudioManager2.instance.PlaySFX("Playerwhip");
        }
        public void HandleHeavyAttack(WeaponItem weapon)
        {
            if (playerStats.currentStamina <= 0)
                return;

            weaponSlotManager.attackingWeapon = weapon;
            playerAnimatorManager.PlayTargetAnimation(weapon.oneHandHeavyAttack1, true);
            AudioManager2.instance.PlaySFX("Playerwhip");
            lastAttack = weapon.oneHandHeavyAttack1;
        }

        public void HandlePistolAttack(GameObject pistol)
        {
            if (pistol == null)
                return;

            float raycastDistance = 10f;

            Ray ray = new Ray(pistol.transform.position, pistol.transform.forward);
            RaycastHit hitInfo;
            AudioManager2.instance.PlaySFX("PlayerShooting");

            Transform MuzzlePoint = GameObject.Find("MuzzlePoint").GetComponent<Transform>();
            GameObject MuzzleEffect = Instantiate<GameObject>(muzzleEffect, MuzzlePoint);
            Destroy(MuzzleEffect, 0.5f);

            if (Physics.Raycast(ray, out hitInfo, raycastDistance))
            {
                Debug.Log(hitInfo.collider.name);
                if (hitInfo.collider.CompareTag("Boss") || hitInfo.collider.CompareTag("Hand"))
                {
                    if (bossAi != null)
                    {
                        bossAi.isBulletHit = true;
                    }
                    else if (bossAlpha != null)
                    {
                        bossAlpha.isGunHit = true;
                    }
                }
            }

            Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red, 1f);
        }
        public GameObject HandleShootPoint()
        {
            return GameObject.Find("MuzzlePoint");
        }

    }
}