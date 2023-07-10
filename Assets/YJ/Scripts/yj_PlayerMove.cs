using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class yj_PlayerMove : MonoBehaviour
{
    // 이동 속도
    [Header("이동 속도")]
    public float speed = 5f;
    public bool isAttack = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0, v);
        dir.Normalize();
        transform.position += dir * speed * Time.deltaTime;

        // 공격상태
        if (Input.GetMouseButtonDown(0))
        {
            isAttack = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isAttack = false;
        }
    }
}
