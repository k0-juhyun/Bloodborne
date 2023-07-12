using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int damagePerCollision = 10;

    private int collisionCount = 0;
    private Animator animator;
    public Slider healthSlider; // HP �����̴�
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        healthSlider.maxValue = maxHealth; // �����̴��� �ִ밪 ����
        healthSlider.value = currentHealth; // �����̴��� ���簪 ����
    }

    // Update is called once per frame
    void Update()
    {
        
    }
  
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collisionCount++;

            if (collisionCount <= 3) //�ִ� 3����� �¾��� ��
            {
                currentHealth -= damagePerCollision;
                Debug.Log("Player hit! Current health: " + currentHealth);

                if (currentHealth <= 0)
                {
                    GameOver();
                }

                // �浹 Ƚ���� ���� �ִϸ��̼� ���
                switch (collisionCount)
                {
                    case 1:
                        animator.SetTrigger("Hit1");
                        break;
                    case 2:
                        animator.SetTrigger("Hit2");
                        break;
                    case 3:
                        animator.SetTrigger("Hit3");
                        break;
                }

                
                healthSlider.value = currentHealth; // HP �����̴� ����
            }
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over");
    }
}


