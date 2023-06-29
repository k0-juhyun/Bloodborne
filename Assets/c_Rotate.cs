using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class c_Rotate : MonoBehaviour
{
    float rx;
    float ry;
    public float rotSpeed = 300;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 1. 사용자의 입력에따라
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        // 2. X와 Y축의 값을 누적하고
        rx += my * rotSpeed * Time.deltaTime;
        ry += mx * rotSpeed * Time.deltaTime;
        // 3. rx에대해 각도를 제한하고싶다.
        rx = Mathf.Clamp(rx, -50, 75);
        ry = Mathf.Clamp(ry, -180, 180);
        // 4. 그 누적값으로 회전을 하고싶다.
        transform.eulerAngles = new Vector3(-rx, ry, 0);
    }
}
