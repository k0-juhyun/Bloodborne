using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace bloodborne
{
    public class PlayerStats : MonoBehaviour
    {
        #region Stats
        public int healthLevel = 10;
        public int maxHealth;
        public int currentHealth;

        public int staminaLevel = 10;
        public float maxStamina;
        public float currentStamina;
        #endregion

        public HealthBar healthBar;
        public StaminaBar staminaBar;

        PlayerAnimatorManager animatorHandler;
        PlayerManager playerManager;
        PlayerLocomotion playerLocomotion;

        public int potionAmount = 10;
        public float staminaRegenerationAmount = 1;
        public float staminaRegenTimer = 0;

        private bool playOnce;

        private void Awake()
        {
            playerLocomotion = GetComponent<PlayerLocomotion>();    
            animatorHandler = GetComponentInChildren<PlayerAnimatorManager>();
            healthBar = FindObjectOfType<HealthBar>();
            staminaBar = FindObjectOfType<StaminaBar>();
            playerManager = GetComponent<PlayerManager>();
        }

        void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);

            maxStamina = SetMaxStaminaFromStaminaLevel();
            currentStamina = maxStamina;
            staminaBar.SetMaxStamina(maxStamina);
        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        private float SetMaxStaminaFromStaminaLevel()
        {
            maxStamina = staminaLevel * 10;
            return maxStamina;
        }

        public void TakeDamage(int damage)
        {
            if (playerManager.isInvulnerable)
                return;

            currentHealth = currentHealth - damage;
            AudioManager2.instance.PlaySFX("Player_Hit");
            healthBar.SetCurrentHealth(currentHealth);

            animatorHandler.PlayTargetAnimation("Damage", true);

            if (currentHealth <= 0 && playOnce == false)
            {
                playerLocomotion.isPlayerDie = true;
                currentHealth = 0;
                animatorHandler.PlayTargetAnimation("Dead", true);
                AudioManager2.instance.PlaySFX("Player_Die");
                playOnce = true;
            }
        }

        public void TakeStaminaDamage(int damage)
        {
            currentStamina = currentStamina - damage;

            staminaBar.SetCurrentStamina(currentStamina);
        }

        public void RegenerateHealth()
        {
            if (potionAmount > 0)
            {
                --potionAmount;
                currentHealth += 30;
                if(currentHealth >= maxHealth)
                {
                    currentHealth = maxHealth;
                }
                healthBar.SetCurrentHealth(currentHealth);
                AudioManager2.instance.PlaySFX("PlayerPotion");
            }
        }

        public void RegenerateStamina()
        {
            if (playerManager.isInteracting)
            {
                staminaRegenTimer = 0;
            }
            else
            {
                staminaRegenTimer += Time.deltaTime;
                if (currentStamina < maxStamina && staminaRegenTimer > 1f)
                {
                    currentStamina += staminaRegenerationAmount * Time.deltaTime;
                    staminaBar.SetCurrentStamina(Mathf.RoundToInt(currentStamina));
                }
            }
        }

        public int SetCurrentHpOne()
        {
            currentHealth = 1;
            healthBar.SetCurrentHealth(currentHealth);
            return currentHealth;
        }

        public void DeveloperMode()
        {
            currentHealth = maxHealth;
            currentStamina = maxStamina;
            healthBar.SetCurrentHealth(currentHealth);
            staminaBar.SetCurrentStamina(currentStamina);
        }
    }
}