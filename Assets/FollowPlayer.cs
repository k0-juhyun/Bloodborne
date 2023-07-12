using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player; // �÷��̾� ������Ʈ�� Transform ������Ʈ
    public Transform boss; // ���� ������Ʈ�� Transform ������Ʈ
    public float smoothSpeed = 10f; // ī�޶� �̵� �ӵ�
    public Vector3 offset; // ī�޶��� ����� ��ġ ������

    private bool isLockedOn; // ���� ���� ����

    void Update()
    {
        // ���� ���� ���
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
            // �÷��̾ �������� ī�޶��� ��ġ ���
            Vector3 desiredPosition = player.position + offset;

            // ī�޶� �ε巴�� �̵�
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // ī�޶� ������ �ٶ󺸵��� ȸ��
            transform.LookAt(boss);
        }
    }
}
