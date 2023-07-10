using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    Rigidbody rb;
    BoxCollider bc;

     void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
    }
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(float damage) 
    {
        // 피격 처리 로직 구현
        currentHealth -= 10; // 피해량은 임의로 10으로 설정

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        // 사망 처리 로직 구현
        Debug.Log("DIE!!!!!!");
        Destroy(gameObject); // 적 오브젝트 제거
    }
}
