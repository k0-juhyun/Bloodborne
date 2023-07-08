using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    public Transform player;
    public Vector3 offset;
    public float damping = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LateUpdate()
    {
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position,
            desiredPosition, damping * Time.deltaTime);
        transform.position = smoothedPosition;
        //카메라가 플레이어를 바라보도록 설정
        transform.LookAt(player);
    }
}
