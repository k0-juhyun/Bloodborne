using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float rot_speed = 100.0f;

    public GameObject Player;
    public GameObject MainCamera;

    private float camera_dist = 0f; //리그로부터 카메라까지의 거리
    public float camera_width = -10f; //가로거리
    public float camera_height = 4f; //세로거리
    public float camera_fix = 3f;//레이케스트 후 리그쪽으로 올 거리

    Vector3 dir;
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        //카메라리그에서 카메라까지의 길이
        camera_dist = Mathf.Sqrt(camera_width * camera_width + camera_height * camera_height);

        //카메라리그에서 카메라위치까지의 방향벡터
        dir = new Vector3(0, camera_height, camera_width).normalized;

    }


    void Update()
    {
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * rot_speed, Space.World);

        transform.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * Time.deltaTime * rot_speed, Space.Self);

        transform.position = Player.transform.position;


        //레이캐스트할 벡터값
        Vector3 ray_target = transform.up * camera_height + transform.forward * camera_width;
        Debug.Log("ray_target : " + ray_target);

        RaycastHit hitinfo;
        Physics.Raycast(transform.position, ray_target, out hitinfo, camera_dist);

        if (hitinfo.point != Vector3.zero)//레이케스트 성공시
        {
            //point로 옮긴다.
            MainCamera.transform.position = hitinfo.point;
            //카메라 보정
            MainCamera.transform.Translate(dir * -1 * camera_fix);
        }
        else
        {
            //로컬좌표를 0으로 맞춘다. (카메라리그로 옮긴다.)
            MainCamera.transform.localPosition = Vector3.zero;
            //카메라위치까지의 방향벡터 * 카메라 최대거리 로 옮긴다.
            MainCamera.transform.Translate(dir * camera_dist);
            //카메라 보정
            MainCamera.transform.Translate(dir * -1 * camera_fix);

        }
    }
}
