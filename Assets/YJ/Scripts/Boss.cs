using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    NavMeshAgent agent;                 // 길찾기
    Animator animator;                  // 애니메이터

    BossHP bossHP;                      // 보스 hp

    enum BossPatternState               // 열거형, 보스 패턴 상태
    {
        Idle,
        Move,
        Avoid,
        Hit,
        SickelCombo1,
        SickelCombo2,
        SickelCombo3,
    }
    BossPatternState bossState;         // 보스 상태 = 0: 대기, 1:이동, 2:회피, 4:피격, 5:낫 공격(대시,점프 포함), 6: 칼 공격(대시 점프포함?), 7: 총 공격 8: 죽음

    enum BossPhase                      // 보스 페이즈 (체력에 따라 변함, 기본 = 1)
    {
        Phase1,
        Phase2,
        Phase3
    }
    BossPhase bossPhase;


    public GameObject player;           // 플레이어
    PlayerMove playerscripts;           // 플레이어 스크립트

    Vector3 dir;                        // 이동 방향

    int phase = 1;                      // 페이즈 변환 변수
    int damage;                         // 데미지 (플레이어한테 어떤 공격인지 상태를 받아오기, 공격에 따라 다른 데미지를 구현)
    int hitCount = 0;                   // 피격 횟수 (맞은 횟수)
    int attack1damage = 1;              // 플레이어 공격 1번 데미지
    int attack2damage = 2;              // 플레이어 공격 2번 데미지
    int attack3damage = 3;              // 플레이어 공격 3번 데미지

    float currDistance;                 // 현재 거리
    float attackDistance = 10f;         // 공격시작 거리
    float hitDistance = 5f;             // 피격 거리
    float avoidDistance = 5f;           // 회피 거리

    float moveSpeed = 5f;               // 이동 속도
    float dashSpeed = 10f;              // 대시 속도


    float curTime;                      // 현재 시간
    float hitcurrTime;                  // 현재 피격 시간
    float hitTime = 2f;                      // 피격 2 재생되는 시간
    float skTime_Sickel1_1 = 1f;        // 낫 스킬1 1번 공격 시작 시간
    float skTime_Sickel1_2 = 2f;        // 낫 스킬1 2번 공격 시작 시간
    float skTime_Sickel1_3 = 3f;        // 낫 스킬3 3번 공격 시작 시간

    // Start is called before the first frame update
    void Start()
    {
        playerscripts = player.GetComponent<PlayerMove>();          // 플레이어의 스크립트를 받아오자

        bossPhase = BossPhase.Phase1;                               // 시작시 보스 페이즈를 1로 설정한다
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

        // 플레이어가 없어서


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
            case BossPatternState.Hit:
                UpdateIHit();
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

        switch (bossPhase)
        {
            case BossPhase.Phase1:
                break;
            case BossPhase.Phase2:
                break;
            case BossPhase.Phase3:
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

    private void UpdateIHit()
    {
        // 맞았을때 하는일
        // 만약 플레이어가 공격 상태이고, 플레이어의 무기에 맞았을 때(onCollisionEnter) 호출됨
        // 피격 시간을 흐르게 한다
        hitcurrTime += Time.deltaTime;
        // 히트 카운드를 1더한다
        hitCount++;
        // 어떤 공격인지에 따라 그 데미지만큼을 현재 체력에서 뺀다
        bossHP.HP -= damage;
        // 피격 1 애니메이션을 실행한다
        // 만약 히트 카운트가 1일때
        if (hitCount == 1)
        {
            // 시간을 한번 초기화
            curTime = 0;
            // 만약 시간이 
        }
        // 만약 히트 카운트가 2라면
        if (hitCount >= 2)
        {
            // 피격 2 애니메이션을 실행한다
            // 히트 카운트를 0으로 한다
            hitCount = 0;
        }
        // 
        else
        {
            // 히트 카운트를 0으로 초기화한다
            hitCount = 0;
        }
        // 애니메이션이 끝나면 idle 상태로 돌린다(이벤트 함수로)
    }

    enum SickelSubState                     // 낫 공격 상태
    {
        Attack1,
        Attack2,
        Attack3,
    }

    SickelSubState sickelSubState;

    private void UpdateSickelCombo1()
    {
        print("Boss State : SickelCombo1");

        // int로 값을 정해서 그 값에서만 실행되고, 다른 상태일때 다시 초기화되게 바꾸기

        curTime += Time.deltaTime;

        switch (sickelSubState)
        {
            case SickelSubState.Attack1:
                if (curTime > skTime_Sickel1_1)
                {
                    print("SubState : Attack1");
                    sickelSubState = SickelSubState.Attack2;
                    // 애니메이션 재생
                }
                break;
            case SickelSubState.Attack2:
                if (curTime > skTime_Sickel1_2)
                {
                    print("SubState : Attack2");
                    sickelSubState = SickelSubState.Attack3;
                    // 애니메이션 재생
                }
                break;
            case SickelSubState.Attack3:
                if (curTime > skTime_Sickel1_3)
                {
                    print("SubState : Attack3");
                    curTime = 0;
                    bossState = BossPatternState.Idle;
                    // 애니메이션 재생
                    sickelSubState = SickelSubState.Attack1;
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

    private void OnCollisionEnter(Collision collision)
    {
        // 만약 플레이어가 공격 상태이고, 플레이어의 무기와 충돌했을때
        //if()
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 from = transform.position;
        Vector3 to = from + transform.forward * currDistance;
        Gizmos.DrawLine(from, to);
    }
}
