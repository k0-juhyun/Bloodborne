using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    Animator animator;
    public static bool P_Attack = false;
    public float attackRadius = 1f;   // ���� ����
    public float attackDamage = 10f;  // ���ݷ�

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }

    private void Attack()
    {
        animator.SetTrigger("isAttack");

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Boss"))
            {
                // ������ �������� ������ �ڵ带 �ۼ��ϼ���.
                Enemy enemyHealth = hitCollider.GetComponent<Enemy>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(attackDamage);
                }
            }
        }
    }

    void CallAttackTrueEvent()
    {
        P_Attack = true;
    }

    void CallAttackFalseEvent()
    {
        P_Attack = false;
    }
}
