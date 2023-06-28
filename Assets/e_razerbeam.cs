using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class e_razerbeam : MonoBehaviour
{
    [Header("플레이어")]
    public Transform Player;
    [Header("레이저빔 쏘는시간")]
    public float razerBeamDuration;
    [Header("레이저빔 거리")]
    public float razerDistance;
    [Header("레이저 지속시간")]
    public float razerDuration;
    [Header("레이저 딜레이")]
    [Range(0f, 1f)]
    public float razerDelay;

    NavMeshAgent agent;
    public float distance;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Player.transform.position);
        razerBeaaam();
        agent.SetDestination(Player.transform.position);
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                Debug.Log("더 이상 갈 수 없음");
            }
        }
    }

    void razerBeaaam()
    {
        distance = Vector3.Distance(transform.position, Player.position);
        if (distance <= razerDistance && distance >= 6)
        {
            StartCoroutine(razerbeam(transform.position, razerBeamDuration, razerDelay));
        }
        else if (distance > razerDistance && distance < 6)
        {
            StopAllCoroutines();
        }
    }

    // playerPos : 1초전 플레이어 위치
    IEnumerator razerbeam(Vector3 startPos, float duration, float delayTime)
    {
        transform.position = startPos;
        duration = razerBeamDuration;
        delayTime = razerDelay;
        delayTime = 0.2f;
        for (float curTime = 0; curTime < duration; curTime += Time.deltaTime)
        {
            yield return new WaitForSeconds(delayTime);
            Debug.DrawLine(transform.position, transform.forward * razerDistance, Color.red, razerDuration);
        }
    }
}
