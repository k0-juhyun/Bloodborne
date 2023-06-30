using Retro.ThirdPersonCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class bossAI : MonoBehaviour
{
    private Transform playerTr;
    private Transform bossTr;
    string currentDamageAnimation;

    [Header("공격사거리")]
    public float attackDis;

    [Header("추적사거리")]
    public float traceDis;

    [Header("죽었는지 확인")]
    public bool isDie = false;

    [Header("보스 이동속도")]
    public float traceSpeed;

    // 컴포넌트들
    private Animator animator;
    private bossMove bossmove;

    // 보스 상태
    public enum State
    {
        Idle,
        Trace, 
        Attack,
        Die
    }
    // 처음에는 기본상태
    [Header("현재 보스 상태")]
    public State state = State.Idle;

    private WaitForSeconds waitforsecs;
    private readonly int hashMove = Animator.StringToHash("isMove");
    private readonly int hashSpeed = Animator.StringToHash("speed");
  
    void Awake()
    {
        animator = GetComponent<Animator>();
        bossmove = GetComponent<bossMove>();

        var player = GameObject.FindGameObjectWithTag("Player");

        // 인스턴스 체크
        if (player != null)
            playerTr = player.GetComponent<Transform>();

        bossTr = GetComponent<Transform>();

        // 0.2초 주기로 상태체크
        waitforsecs = new WaitForSeconds(0.2f);
    }
    private void OnEnable()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());
    }

    IEnumerator Action()
    {
        while(!isDie)
        {
            yield return waitforsecs;
            switch(state) 
            {
                case State.Idle:
                    break;
                case State.Trace:
                    bossmove.traceTransform();
                    animator.SetBool(hashMove, true);
                    break;
                case State.Attack:
                    bossmove.stopTracing();
                    animator.SetBool(hashMove, false);
                    break;
                case State.Die:
                    break;
            }
        }
    }

    // 보스 상태 체크 코루틴
    IEnumerator CheckState()
    {
        // 죽지 않았다면
        while (!isDie)
        {
            if (state == State.Die)
                yield break;

            float dis = Vector3.Distance(playerTr.position, bossTr.position);

            if (dis <= attackDis)
            {
                state = State.Attack;
            }
            else if (dis <= traceDis)
            {
                state = State.Trace;
            }

            yield return waitforsecs;
        }
    }
    void Update()
    {
        animator.SetFloat(hashSpeed, bossmove.speed);
        // 시야각 내에 있으면 오브젝트를 바라봄
        WhichPositionLookAt(playerTr.transform.position);
    }

    // 상대의 위치와 내 위치를 통해 각도를 계산하는 함수
    float GetAngle(Vector3 from, Vector3 to)
    {
        return Quaternion.FromToRotation(transform.forward, to - from).eulerAngles.y;
    }

    // 피격함수
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("p_Weapon") && Combat.P_Attack)
        {
            float directionHitFrom = (GetAngle(transform.position, playerTr.transform.position));
            WhichDirectionDamageCameFrom(directionHitFrom);
            animator.Play(currentDamageAnimation);
        }
    }

    // 피격시 어느 위치에 맞았는지 확인하는 함수
    protected virtual void WhichDirectionDamageCameFrom(float direction)
    {
        //forward
        if (direction >= 0 && direction < 45)
        {
            currentDamageAnimation = "GetHitFront";
            print("Forward");
        }
        if (direction >= 315 && direction < 360)
        {
            currentDamageAnimation = "GetHitFront";
            print("Forward");
        }
        //Right
        else if (direction >= 45 && direction < 135)
        {
            currentDamageAnimation = "GetHitRight";
            print("Right");
        }
        //Back
        else if (direction >= 135 && direction < 225)
        {
            currentDamageAnimation = "GetHitBack";
            print("Back");
        }
        //Left
        else if (direction >= 225 && direction <= 315)
        {
            currentDamageAnimation = "GetHitLeft";
            print("Left");
        }
        return;
    }

    // 시야 각 내에 있으면 플레이어를 바라보는 함수
    Vector3 WhichPositionLookAt(Vector3 playerPos)
    {
        float lookDirection = GetAngle(transform.position, playerTr.transform.position);
        playerPos = playerTr.transform.position;
        if (lookDirection >= 0 && lookDirection < 45)
        {
            gameObject.transform.LookAt(playerTr.transform.position);
            print("look");
        }
        else if (lookDirection >= 315 && lookDirection < 360)
        {
            gameObject.transform.LookAt(playerTr.transform.position);
            print("look");
        }
        return playerPos;
    }
}
