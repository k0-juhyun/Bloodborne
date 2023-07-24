using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bloodborne
{
    public class DamageCollider : MonoBehaviour
    {
        Collider damageCollider;

        public static bool isAttack;
        public int currentWeaponDamage = 10;

        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

        public void EnableDamageCollider()
        {
            damageCollider.enabled = true;
            isAttack = true;
        }

        public void DisableDamageCollider()
        {
            damageCollider.enabled = false;
            isAttack = false;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if(collision.tag == "Player")
            {
                PlayerStats playerStats = collision.GetComponent<PlayerStats>();

                if(playerStats != null)
                {
                    playerStats.TakeDamage(currentWeaponDamage);
                }
            }

            if(collision.tag == "Boss")
            {
                BossStats bossStats = collision.GetComponent<BossStats>();

                if(bossStats != null)
                {
                    bossStats.TakeDamage(currentWeaponDamage);
                }
            }
        }
    }
}