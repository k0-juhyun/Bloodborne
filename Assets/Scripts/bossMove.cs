using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class bossMove : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform playerTr;
    private Vector3 originTr;
    private float damping = 1.0f;
    private Transform bossTr;
    private bool moveToOriginInProgress = false; // moveToOrigin이 진행 중인지 체크하는 변수
    public bool laserbeaaam = false;
    // Start is called before the first frame update
    void Awake()
    {
        originTr = transform.position;
        bossTr = GetComponent<Transform>();
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        // 자동회전 끄기
    }
    public float speed
    {
        get { return agent.velocity.magnitude; }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void moveToPlayer()
    {
        print("moveToPlayer");
        agent.speed = 3f;
        agent.isStopped = false;
        damping = 1.0f;
        agent.SetDestination(playerTr.transform.position);

        //if (moveToOriginInProgress) // moveToOrigin이 진행 중인 경우
        //{
        //    // 한 바퀴 회전
        //    Quaternion rotation = Quaternion.Euler(0f, 360f, 0f);
        //    bossTr.rotation *= rotation;

        //    moveToOriginInProgress = false; // moveToOrigin 진행 중 해제
        //}
    }

    public void stopTracing()
    {
        print("stopTracing");
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
    }

    public void moveToOrigin()
    {
        moveToOriginInProgress = true; // moveToOrigin 진행 중으로 설정

        agent.speed = 7f;
        agent.isStopped = false;
        print("moveToOrigin");
        agent.SetDestination(originTr);
    }

    public void laserLookPlayer()
    {
        agent.SetDestination(playerTr.transform.position);
        agent.updatePosition = false;
    }
}
