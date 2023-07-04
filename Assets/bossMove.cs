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

    // Start is called before the first frame update
    void Awake()
    {
        originTr = transform.position;
        bossTr = GetComponent<Transform>();
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        // 자동회전 끄기
        //agent.updatePosition = false;
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
        agent.isStopped = false;
        damping = 1.0f;
        agent.SetDestination(playerTr.transform.position);
    }

    public void stopTracing()
    {
        print("stopTracing");
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
    }

    public void moveToOrigin()
    {
        agent.isStopped = false;
        print("moveToOrigin");
        print(originTr);
        agent.SetDestination(originTr);
    }
}
