using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class BossStats : MonoBehaviour
    {
        public int healthLevel = 10;
        public int maxHealth;
        public int currentHealth;

        Animator animator;

        public HealthBar healthBar;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public void TakeDamage(int dagmage)
        {
            currentHealth = currentHealth - dagmage;

            healthBar.SetCurretnHealth(currentHealth);

            if(currentHealth <= 0)
            {
                currentHealth = 0;      
            }
        }
    }
}
