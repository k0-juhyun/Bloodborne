using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    float rotX = 0;
    float rotY = 0;
    public float rotSpeed = 200f;

    // 회전 최대값
    float maxRotY = 90f;
    // 회전 최솟값
    float minRotY = 20f;

    // Start is called before the first frame update
    void Start()
    {
        rotX = transform.localEulerAngles.y;
        rotY = -transform.localEulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        // 사용자의 마우스 움직임 값을 받는다
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        // 마우스의 움직임값을 누적시키자
        rotX += mx * Time.deltaTime * rotSpeed;
        rotY += my * Time.deltaTime * rotSpeed;

        //if (rotY < -90) rotY = -90;
        //if (rotY > 0) rotY = 0;

        // X회전 최대값과 최솟값을 정한다
        rotY = Mathf.Clamp(rotY, -maxRotY, minRotY);
        transform.localEulerAngles = new Vector3(-rotY, rotX, 0);
    }
}
