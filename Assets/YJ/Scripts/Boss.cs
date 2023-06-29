using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;


    enum BossPatternState               // 열거형, 보스 패턴 상태
    {
        Idle,
        Move,
        Avoid,
        SickelCombo1,
        SickelCombo2,
        SickelCombo3,
    }

    BossPatternState bossState;         // 보스 상태 = 0: 대기, 1:이동, 2:회피, 4:피격, 5:낫 공격(대시,점프 포함), 6: 칼 공격(대시 점프포함?), 7: 총 공격 8: 죽음


    public GameObject player;           // 플레이어

    Vector3 dir;                        // 이동 방향

    int phase = 1;                          // 페이즈 변환 변수

    float currDistance;                 // 현재 거리
    float attackDistance = 10f;         // 공격시작 거리

    float moveSpeed = 5f;               // 이동 속도
    float dashSpeed = 10f;              // 대시 속도

    float curTime;                      // 현재 시간
    float skTime_Sickel1_1 = 1f;        // 낫 스킬1 1번 공격 시작 시간
    float skTime_Sickel1_2 = 2f;        // 낫 스킬1 2번 공격 시작 시간
    float skTime_Sickel1_3 = 3f;        // 낫 스킬3 3번 공격 시작 시간

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 항상 플레이어를 바라보기
        // 플레이어와 나 사이의 방향을 구한다
        dir = player.transform.position - this.transform.position;
        dir.Normalize();
        // 그 방향을 나의 앞 방향으로 한다
        dir = transform.forward;

        transform.LookAt(player.transform);

        // 플레이어와 나 사이의 거리를 잰다
        currDistance = Vector3.Distance(transform.position, player.transform.position);

        print("currDistacne : " + currDistance);

        switch (bossState)
        {
            case BossPatternState.Idle:
                UpdateIdle();
                break;
            case BossPatternState.Move:
                UpdateIMove();
                break;
            case BossPatternState.Avoid:  // 만약 플레이어와의 거리가 피격 가능 거리이고, 플레이어가 공격 중이라면 실행
                UpdateIAvoid();
                break;
            case BossPatternState.SickelCombo1:
                UpdateSickelCombo1();
                break;
            case BossPatternState.SickelCombo2:
                UpdateSickelCombo2();
                break;
            case BossPatternState.SickelCombo3:
                UpdateSickelCombo3();
                break;
            default:
                break;
        }
    }


    private void UpdateIdle()           // 공격이 끝나면 idle 상태로 옴
    {
        print("Boss State : Idle");
        // 만약 현재 거리가 공격 가능 거리보다 크다면
        if (currDistance > attackDistance)
        {
            // 현재 상태를 Move 로 변화시킨다
            bossState = BossPatternState.Move;
            // 애니메이션 재생?
        }
        // 만약 현재 거리가 공격 가능 범위보다 작거나 같다면
        else if (currDistance <= attackDistance)
        {
            // 낫공격1으로 상태 변화시킨다 (랜덤 뽑기 나중에)
            bossState = BossPatternState.SickelCombo1;
        }
    }

    private void UpdateIMove()
    {
        print("Boss State : Move");
        // 플레이어 위치로 이동한다
        transform.position += dir * moveSpeed * Time.deltaTime;
        // 만약 현재 거리가 공격 가능 범위보다 작거나 같다면
        if (currDistance <= attackDistance)
        {
            // 낫공격1으로 상태 변화시킨다 (랜덤 뽑기 나중에)
            bossState = BossPatternState.SickelCombo1;
        }
    }

    private void UpdateIAvoid()
    {

    }

    enum SickelSubState                     // 낫 공격 상태
    {
        Phase1,
        Phase2,
        Phase3,
    }

    SickelSubState sickelSubState;

    private void UpdateSickelCombo1()
    {
        print("Boss State : SickelCombo1");

        // int로 값을 정해서 그 값에서만 실행되고, 다른 상태일때 다시 초기화되게 바꾸기

        switch (sickelSubState)
        {
            case SickelSubState.Phase1:
                if (phase == 1)
                {
                    print("SubState : Phase1");
                    // 애니메이션 재생
                    phase = 2; // 애니메이션이 끝나면 이벤트 함수로 넣기
                    sickelSubState = SickelSubState.Phase2;
                }
                break;
            case SickelSubState.Phase2:
                if (phase == 2)
                {
                    print("SubState : Phase2");
                    // 애니메이션 재생
                    phase = 3; // 애니메이션이 끝나면 이벤트 함수로 넣기
                    sickelSubState = SickelSubState.Phase3;
                }
                break;
            case SickelSubState.Phase3:
                if (phase == 3)
                {
                    print("SubState : Phase3");
                    // 애니메이션 재생
                    phase = 1; // 애니메이션이 끝나면 이벤트 함수로 넣기
                    bossState = BossPatternState.Idle;
                    sickelSubState = SickelSubState.Phase1;
                }
                break;
            default:
                break;
        }
    }

    private void UpdateSickelCombo2()
    {

    }

    private void UpdateSickelCombo3()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 from = transform.position;
        Vector3 to = from + transform.forward * currDistance;
        Gizmos.DrawLine(from, to);
    }
}
