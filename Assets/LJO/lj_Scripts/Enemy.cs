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
        // �ǰ� ó�� ���� ����
        currentHealth -= 10; // ���ط��� ���Ƿ� 10���� ����

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        // ��� ó�� ���� ����
        Debug.Log("DIE!!!!!!");
        Destroy(gameObject); // �� ������Ʈ ����
    }
}
