using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotationFix : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 현재 위치를 가져옴
        Vector3 currentPosition = transform.position;

        // X 좌표를 0으로 설정
        currentPosition.x = 0f;

        // 새로운 위치를 적용
        transform.position = currentPosition;
    }
}
