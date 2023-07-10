using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    Animator animator;
    public static bool P_Attack = false;
    public float attackRadius = 1f;   // 공격 범위
    public float attackDamage = 10f;  // 공격력

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
                // 적에게 데미지를 입히는 코드를 작성하세요.
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
