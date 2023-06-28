using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class p_move : MonoBehaviour
{
    [Header("속도")]
    public float speed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        Vector3 dir = new Vector3(-h, 0, -v);
        dir.Normalize();
        transform.position += dir * speed * Time.deltaTime;
    }
}
