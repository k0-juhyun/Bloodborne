using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class bossMove : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform playerTr;
    private float damping = 1.0f;
    private Transform bossTr;
    // Start is called before the first frame update
    void Start()
    {
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

    public void isRotate()
    {
        //Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
        //bossTr.rotation = Quaternion.Slerp(bossTr.rotation, rot, Time.deltaTime * damping);
    }
    public void traceTransform()
    {
        agent.isStopped = false;
        damping = 1.0f;
        agent.SetDestination(playerTr.transform.position);
    }

    public void stopTracing()
    {
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
    }
}
