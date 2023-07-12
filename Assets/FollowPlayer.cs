using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player; // 플레이어 오브젝트의 Transform 컴포넌트
    public Transform boss; // 보스 오브젝트의 Transform 컴포넌트
    public float smoothSpeed = 10f; // 카메라 이동 속도
    public Vector3 offset; // 카메라의 상대적 위치 오프셋

    private bool isLockedOn; // 락온 상태 여부

    void Update()
    {
        // 락온 상태 토글
        if (Input.GetButtonDown("Fire2"))
        {
            isLockedOn = !isLockedOn;
        }

        if (Input.GetButtonUp("Fire2"))
        {
            isLockedOn = !isLockedOn;
        }
    }

    void LateUpdate()
    {
        if (isLockedOn)
        {
            // 플레이어를 기준으로 카메라의 위치 계산
            Vector3 desiredPosition = player.position + offset;

            // 카메라를 부드럽게 이동
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // 카메라가 보스를 바라보도록 회전
            transform.LookAt(boss);
        }
    }
}
