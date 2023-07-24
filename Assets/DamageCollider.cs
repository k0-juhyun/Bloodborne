using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bloodborne
{
    public class DamageCollider : MonoBehaviour
    {
        public static Collider damageCollider;

        public bool isAttack;
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
            isAttack = true;
            damageCollider.enabled = true;
        }

        public void DisableDamageCollider()
        {
            isAttack = false;
            damageCollider.enabled = false;
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