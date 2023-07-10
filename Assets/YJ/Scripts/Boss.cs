using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// 대체, 추가 : 나중에 합치면 교체해야 하는 것
public class Boss : MonoBehaviour
{
    NavMeshAgent agent;                 // 길찾기
    Animator animator;                  // 애니메이터

    BossHP bossHP;                      // 보스 hp

    public enum BossPatternState               // 열거형, 보스 패턴 상태
    {
        Idle,
        Move,
        Avoid,
        Hit,
        SickelCombo1,
        SickelCombo2,
        SickelCombo3,
    }
    public BossPatternState bossState;         // 보스 상태 = 0: 대기, 1:이동, 2:회피, 4:피격, 5:낫 공격(대시,점프 포함), 6: 칼 공격(대시 점프포함?), 7: 총 공격 8: 죽음

    public enum BossPhase                      // 보스 페이즈 (체력에 따라 변함, 기본 = 1)
    {
        Phase1,
        Phase2,
        Phase3
    }
    public BossPhase bossPhase;                // 보스 페이즈 상태 = 1페이즈(시작), 2페이즈(체력75%이하), 3페이즈(체력 50%이하)


    public GameObject player;           // 플레이어
    PlayerMove playerscripts;           // 플레이어 스크립트
    Rigidbody rid;                      // 리지드바디
    RaycastHit hit;                     // 레이캐스트 히트
    public Transform rayPos;            // ray 쏘는 곳
    public GameObject sickle;           // 무기
    public GameObject hinge;            // 무기 힌지
    public GameObject sickleAttackPos;  // 무기 공격 위치 = 몸통 앞


    Vector3 dir;                        // 이동 방향
    Vector3 avoidDir;                   // 회피 방향
    Vector3 moveDir;                    // 움직임 방향
    Vector3 targetPos;                  // 회피 목적지
    Vector3 moveTargetPos;              // 공격시 이동 목적지
    Vector3 sidemoveTargetPos;          // 공격시 이동 목적지 : 왼쪽

    int damage = 2;                         // 데미지 (플레이어한테 어떤 공격인지 상태를 받아오기, 공격에 따라 다른 데미지를 구현)
    public int hitCount = 0;                   // 피격 횟수 (맞은 횟수)

    [Header("데미지")]
    public int attack1damage = 1;              // 플레이어 공격 1번 데미지
    public int attack2damage = 2;              // 플레이어 공격 2번 데미지
    public int attack3damage = 3;              // 플레이어 공격 3번 데미지

    float currDistance;                        // 현재 거리
    [Header("거리")]
    public float attackDistance = 10f;         // 공격시작 거리
    public float hitDistance = 5f;             // 피격 거리
    public float avoidDistance = 5f;           // 회피 거리
    public float moveDis = 5f;                 // 공격 이동거리

    [Header("속도")]
    public float moveSpeed = 1f;               // 이동 속도
    public float dashSpeed = 10f;              // 대시 속도


    [Header("시간")]
    public float curTime;                      // 현재 시간
    public float avoidcurTime;                 // 회피 시간
    public float hitcurrTime;                  // 현재 피격 시간
    public float hitTime = 2f;                 // 피격 2 재생되는 시간
    public float skTime_Sickel1_1 = 1f;        // 낫 스킬1 1번 공격 시작 시간
    public float skTime_Sickel1_2 = 2f;        // 낫 스킬1 2번 공격 시작 시간
    public float skTime_Sickel1_3 = 3f;        // 낫 스킬3 3번 공격 시작 시간

    [Header("상태 확인")]
    public bool isHitted = false;                     // 피격 상태 확인
    public bool isAvoid = false;                      // 회피 상태 확인
    public bool isCombo1done = false;                 // 프로토타입용 낫패턴 1
    public bool isCombo2done = false;                 // 프로토타입용 낫패턴 2
    public bool isCombo3done = false;                 // 프로토타입용 낫패턴 3
    public bool isMoveTargetPos = false;              // SC2_a2 이동 계산용 상태확인
    //bool isAttack = false;              // 플레이어 공격상태(대체, 플레이어 스크립트에서 받아오기, 추가하기)

    // Start is called before the first frame update
    void Start()
    {
        playerscripts = player.GetComponent<PlayerMove>();          // 플레이어의 스크립트를 받아오자
        rid = GetComponent<Rigidbody>();                            // 리지드바디
        bossHP = GetComponent<BossHP>();                            // 보스 hp를 받아오자
        bossPhase = BossPhase.Phase1;                               // 시작시 보스 페이즈를 1로 설정한다
        
    }

    // Update is called once per frame
    void Update()
    {
        // 밀림방지
        rid.velocity = Vector3.zero;
        // 항상 플레이어를 바라보기
        // 플레이어와 나 사이의 방향을 구한다
        dir = player.transform.position - this.transform.position;
        dir.Normalize();
        // 그 방향을 나의 앞 방향으로 한다
        dir = transform.forward;

        // 이동 맵 navemesh 로 바꾸기(알파)
        Vector3 ad = player.transform.position;
        ad.y = 0;
        transform.LookAt(ad);


        // 플레이어와 나 사이의 거리를 잰다
        currDistance = Vector3.Distance(transform.position, player.transform.position);

        //print("currDistacne : " + currDistance);

        // 플레이어가 없어서 플레이어 공격 상태를 마우스 우클릭으로 설정한다(대체-플레이어에서)

        // 피격 시간을 흐르게 한다
        hitcurrTime += Time.deltaTime;
        // 만약 피격 시간이 2차 피격시간보다 크다면,
        if (hitcurrTime >= hitTime)
        {
            // 피격 횟수를 초기화
            hitCount = 0;
        }

        switch (bossState)
        {
            case BossPatternState.Idle:
                UpdateIdle();
                break;
            case BossPatternState.Move:
                UpdateMove();
                break;
            case BossPatternState.Avoid:  // 만약 플레이어와의 거리가 피격 가능 거리이고, 플레이어가 공격 중이라면, 한번 실행
                UpdateAvoid();
                break;
            case BossPatternState.Hit:
                UpdateHit();
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
                // 페이즈 1에서 실행할 공격 콤보들 호출?
                // 낫공격 1,2,3
                break;
            case BossPhase.Phase2:
                // 페이즈 2에서 실행할 공격 콤보
                // 칼 공격 콤보 1,2,3
                break;
            case BossPhase.Phase3:
                // 페이즈 3에서 실행할 공격 콤보
                break;
            default:
                break;
        }
    }


    private void UpdateIdle()           // 공격이 끝나면 idle 상태로 옴
    {
        isAvoid = false;
        
        // 만약 현재 거리가 공격 가능 거리보다 크다면
        if (currDistance > attackDistance)
        {
            // 현재 상태를 Move 로 변화시킨다
            bossState = BossPatternState.Move;
            // 애니메이션 재생?
        }
        // 만약 현재 거리가 공격 가능 범위보다 작거나 같다면, 그 거리가 피격 거리보다 크다면
        else if (currDistance <= attackDistance && currDistance >= hitDistance)
        {
            if (isCombo1done == false)
            {
                SickelC1();
            }

            else if (isCombo1done == true && isCombo2done == false)
            {
                SickelC2();
            }

            else if (isCombo2done == true && isCombo3done == false)
            {
                SickelC3();
            }

            print("ldel > Attack");
        }
        // 만약 현재 거리가 피격거리이고, 플레이어가 공격중이라면(교체)
        else if (currDistance <= hitDistance && PlayerAttack.P_Attack == true)
        {
            // 회피 상태로 변화시킨다
            bossState = BossPatternState.Avoid;
            // 회피 방향
            avoidDir = -transform.forward;
            // 회피 목적지
            targetPos = transform.position + avoidDir * 5;

        }
    }

    private void SickelC3()
    {
        // 낫공격3으로 상태 변화시킨다 (랜덤 뽑기 나중에)
        bossState = BossPatternState.SickelCombo3;
        print("SickelCombo3");
    }

    private void SickelC2()
    {
        // 낫공격2으로 상태 변화시킨다 (랜덤 뽑기 나중에)
        bossState = BossPatternState.SickelCombo2;
        print("SickelCombo2");
        moveDir = transform.forward;
        // 거리는 플레이어 움직임 속도 보면서 결정하기
        moveDis = 5f;
        // 이동 목적지
        moveTargetPos = transform.position + moveDir * moveDis;
    }

    private void SickelC1()
    {
        // 낫공격1으로 상태 변화시킨다 (랜덤 뽑기 나중에)
        bossState = BossPatternState.SickelCombo1;
        print("SickelCombo1");
        moveDir = transform.forward;
        moveDis = 5f;
        // 이동 목적지
        moveTargetPos = transform.position + moveDir * moveDis;
    }

    

    private void UpdateMove()
    {
        
        // 플레이어 위치로 이동한다
        transform.position += dir * moveSpeed * Time.deltaTime;
        // 만약 현재 거리가 공격 가능 범위보다 작거나 같다면
        if (currDistance <= attackDistance)
        {
            if (isCombo1done == false)
            {
                SickelC1();
            }

            else if (isCombo1done == true && isCombo2done == false)
            {
                SickelC2();
            }

            else if (isCombo1done == true && isCombo2done == true)
            {
                SickelC3();
            }

            print("Move > Attack");
        }  
    }
    // float a = 0;
    int avoidCount = 0;         // 프로토타입용
    private void UpdateAvoid()
    {
        // 시간을 잰다
        avoidcurTime += Time.deltaTime;
        // 회피 상태임
        isAvoid = true;
        print("Avoid");
        avoidDir = -transform.forward;

        // 뒤로 회피
        if (isAvoid == true)
        {
            if (avoidCount == 0)
            {
                // 뒤로회피
                transform.position = Vector3.Lerp(transform.position, targetPos, dashSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, targetPos) < 0.1f)
                {
                    bossState = BossPatternState.Idle;
                    print("avoid back");
                    avoidcurTime = 0;
                    avoidCount++;
                    print("avoidcount :" + avoidCount);
                }
            }
            else        // 오른쪽으로 회피
            {
                if (isMoveTargetPos == false)
                {
                    moveDir = transform.right;
                    // 거리는 플레이어 움직임 속도 보면서 결정하기
                    moveDis = 8f;
                    // 이동 목적지
                    moveTargetPos = transform.position + moveDir * moveDis;
                    isMoveTargetPos = true;
                }

                transform.position = Vector3.Lerp(transform.position, moveTargetPos, dashSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, moveTargetPos) < 0.1f)
                {
                    bossState = BossPatternState.Idle;
                    print("avoid right");
                    avoidcurTime = 0;
                    avoidCount = 0;
                    isMoveTargetPos = false;
                }
            }
            // 뒤로 회피                     
            //transform.position += avoidDir * 50 * Time.deltaTime;
            //a += 50 * Time.deltaTime;

            //if (a >= 1)
            //{
            //    // 정확하게 하려면 보정값을 빼주는 작업이 한 줄 있어야한다
            //    transform.position -= avoidDir * (a - 1);
            //    // 1초 후 상태를 idle로
            //    bossState = BossPatternState.Idle;
            //    print("Idle");
            //    avoidcurTime = 0;
            //}
        }
    }

    private void UpdateHit()
    {
        // 맞았을때 하는일
        // 만약 플레이어가 공격 상태이고, 플레이어의 무기에 맞았을 때(onCollisionEnter) 호출됨
        // 보스 페이즈 체력 체크
        // 만약 현재 체력이 75보다 작다면
        if (bossHP.HP <= 7.5)
        {
            
            // 현재 페이즈를 2페이즈로 한다
            bossPhase = BossPhase.Phase2;
            // 보스 공격 상태를 ?로 바꾼다
            //return;     // 바로가기?? 아마 아래에 있는 피깎이를 위로 올려야할듯? 무적상태가 따로 있나? 없나? 체크필요

            // 만약 현재 체력이 50보다 작다면
            if (bossHP.HP <= 5)
            {
                // 현재 페이즈를 3페이즈로 한다
                bossPhase = BossPhase.Phase3;
                // 보스 공격 상태를 ?로 바꾼다
                //return;
            }
        }

        if (isHitted == false)
        {
            hitcurrTime = 0;
            // 히트 카운드를 1더한다
            hitCount++;
            print("hitCount : " + hitCount);
            // 어떤 공격인지에 따라 그 데미지만큼을 현재 체력에서 뺀다
            bossHP.HP -= damage;
            print(bossHP.HP);
            // 피격 1 애니메이션을 실행한다
            isHitted = true;
            
            
        }

        if (hitcurrTime > 1)
        {
            isHitted = false;
            bossState = BossPatternState.Idle;
        }

        // 만약 현재 피격시간이 피격2 시간보다 작을때, 히트 카운트가 2라면
        else if (hitcurrTime <= hitTime && hitCount >= 2)
        {
            print("hitcount : " + hitCount);
            // 피격 2 애니메이션을 실행한다
            print("피격2");
            // 히트 카운트를 0으로 한다
            hitCount = 0;
        }
        // 
        
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
        

        // int로 값을 정해서 그 값에서만 실행되고, 다른 상태일때 다시 초기화되게 바꾸기

        curTime += Time.deltaTime;

        switch (sickelSubState)
        {
            case SickelSubState.Attack1:
                if (curTime > skTime_Sickel1_1)
                {
                    print("SubState : Attack1");
                    // 앞으로 이동하고 싶다
                    transform.position = Vector3.Lerp(transform.position, moveTargetPos, dashSpeed * Time.deltaTime);
                    if (Vector3.Distance(transform.position, moveTargetPos) < 0.1f)
                    {
                        // 낫이 회전하고 싶다 (위 > 아래)
                        StartCoroutine(IEUpdown(0.5f));
                        // 서브상태를 액션 2로 바꾼다
                        sickelSubState = SickelSubState.Attack2;
                        // 애니메이션 재생
                        print("com1_attack_move");
                    }
                }
                break;
            case SickelSubState.Attack2:
                if (curTime > skTime_Sickel1_2)
                {
                    print("SubState : Attack2");
                    // 낫 회전 (오른쪽 위 > 왼쪽 가운데쯤으로)
                    StartCoroutine(IERightupdown(0.5f));
                    sickelSubState = SickelSubState.Attack3;
                    // 애니메이션 재생
                }
                break;
            case SickelSubState.Attack3:
                if (curTime > skTime_Sickel1_3)
                {
                    print("SubState : Attack3");
                    curTime = 0;
                    // 현재 거리가 공격시 이동 거리보다 작으면 그만큼 뒤로 이동하게 해야하나??? **교체,수정
                    // 프로토타입용, 콤보 2로 넘어가는 조건
                    isCombo1done = true;
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
        curTime += Time.deltaTime;
        switch (sickelSubState)
        {
            case SickelSubState.Attack1:
                if (curTime > skTime_Sickel1_1)
                {
                    //print("C2A1");
                    // 앞으로 이동하고 싶다
                    transform.position = Vector3.Lerp(transform.position, moveTargetPos, dashSpeed * Time.deltaTime);
                    if (Vector3.Distance(transform.position, moveTargetPos) < 0.1f)
                    {
                        // 낫위치를 몸 앞으로(sickleAttackPos) 바꾼다
                        sickle.transform.position = sickleAttackPos.transform.position;
                        // 낫 회전 왼쪽위 > 오른쪽 중간쯤으로
                        StartCoroutine(IELeftupdown(0.2f));
                        // 서브상태를 액션 2로 바꾼다
                        sickelSubState = SickelSubState.Attack2;
                        // 애니메이션 재생
                        print("com2_attack_move");
                    }

                }
                    break;
            case SickelSubState.Attack2:
                if (curTime > skTime_Sickel1_2)
                {

                    // 한번 더 앞으로 이동하고 싶은데,,
                    // moveTargetPos은 상태가 Attack으로 바뀔때만, 밖에서 한번만 계산되어 저장됨.
                    // 그러면 moveTargetPos를 한번더 계산해서 넣어줘야하는데,
                    // 여기서 계산하면 Update라 계속 호출이 되어서 이상해짐
                    // 코루틴을 써도 똑같고
                    // 다른 방법이 없을까?
                    // 한번만 계산해서 여기에 값을 넣어주는 방법
                    // 한번만 호출되는거 뭐가있지
                    // 부울로 체크해서 값을 구해줘볼까???
                    if (isMoveTargetPos == false)
                    {
                        moveDir = transform.forward;
                        // 거리는 플레이어 움직임 속도 보면서 결정하기
                        moveDis = 5f;
                        // 이동 목적지
                        moveTargetPos = transform.position + moveDir * moveDis;
                        isMoveTargetPos = true;
                    }
                    //print("C2A2");
                    // 앞으로 이동하고 싶다
                    transform.position = Vector3.Lerp(transform.position, moveTargetPos, dashSpeed * Time.deltaTime);
                    if (Vector3.Distance(transform.position, moveTargetPos) < 0.1f)
                    {
                        
                        // 낫위치를 몸 앞으로(sickleAttackPos) 바꾼다
                        sickle.transform.position = sickleAttackPos.transform.position;
                        // 낫 회전 오른쪽위 > 왼쪽 중간쯤으로
                        StartCoroutine(IERightupdown(0.2f));
                        // 서브상태를 액션 3로 바꾼다
                        sickelSubState = SickelSubState.Attack3;
                        // 애니메이션 재생
                        print("com2_attack_move"); 
                    }

                }
                break;
            case SickelSubState.Attack3:
                if (curTime > skTime_Sickel1_3)
                {
                    print("C2A3");
                    // 낫이 회전하고 싶다
                    StartCoroutine(IEUpdown(0.5f));
                    // 프로토타입용, 콤보 3로 넘어가는 조건
                    isCombo2done = true;
                    isMoveTargetPos = false;
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

    private void UpdateSickelCombo3()
    {
        //print("Combo3");
        curTime += Time.deltaTime;
        switch (sickelSubState)
        {
            case SickelSubState.Attack1:
                if (curTime > skTime_Sickel1_1)
                {
                    
                    // 점프 방향을 정한다
                    Vector3 jumpDir = transform.up;
                    // 점프한다
                    //rid.AddForce(jumpDir * 5, ForceMode.Impulse);
                    if (isMoveTargetPos == false)
                    {
                        // 낫을 오른쪽 위로 치켜든다
                        hinge.transform.localEulerAngles = new Vector3(0, 0, -45);
                        moveDir = transform.forward;
                        // 거리는 플레이어 움직임 속도 보면서 결정하기
                        moveDis = 10f;
                        // 이동 목적지
                        moveTargetPos = transform.position + moveDir * moveDis;
                        isMoveTargetPos = true;
                        print("111" + moveTargetPos);
                    }
                    print("ddd");
                    transform.position = Vector3.Lerp(transform.position, moveTargetPos, dashSpeed * Time.deltaTime);
                    if (Vector3.Distance(transform.position, moveTargetPos) < 0.1f)
                    {
                        // 서브상태를 액션 2로 바꾼다
                        sickelSubState = SickelSubState.Attack2;
                        // 애니메이션 재생
                        print("com3_attack_jump");

                    }
                }
                break;
            case SickelSubState.Attack2:
                if (curTime > skTime_Sickel1_2)
                {
                    // 오른쪽 아래에서 왼쪽 위로 휘두른다
                    StartCoroutine(IERightdownup(0.5f));
                    // 서브상태를 액션 3로 바꾼다
                    sickelSubState = SickelSubState.Attack3;
                    // 애니메이션 재생
                    print("com3_attack_Rdu");
                }
                break;
            case SickelSubState.Attack3:
                if (curTime > skTime_Sickel1_3)
                {
                    // 착지 후 왼쪽 위에서 오른쪽 아래로 휘두른다()
                    StartCoroutine(IELeftupdown(0.5f));
                    curTime = 0;
                    isCombo1done = false;
                    isCombo2done = false;
                    isMoveTargetPos = false;
                    bossState = BossPatternState.Idle;
                    // 애니메이션 재생
                    sickelSubState = SickelSubState.Attack1;
                    print("3 > idle");
                }
                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 만약 플레이어가 공격 상태이고, 플레이어의 무기와 충돌했을때
        if (PlayerAttack.P_Attack == true && collision.gameObject.CompareTag("Weapone"))
        {
            // 보스 피격 상태로 전환
            bossState = BossPatternState.Hit;
            
        }
    }


    IEnumerator IEUpdown(float time)      // 위에서 아래로 내려찍기
    {
        
        hinge.transform.localEulerAngles = new Vector3(60, 0, 0);
        yield return new WaitForSeconds(time);
        hinge.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    IEnumerator IERightupdown(float time)      // 오른쪽 위 > 왼쪽 가운데
    {
        //sickle.transform.localEulerAngles = new Vector3(10, 10, -10);
        hinge.transform.localEulerAngles = new Vector3(0, 0, -45);
        yield return new WaitForSeconds(time);
        sickle.transform.localEulerAngles = new Vector3(0, -120, 0);
        hinge.transform.localEulerAngles = new Vector3(0, 0, -90);
        yield return new WaitForSeconds(time);
        sickle.transform.localEulerAngles = new Vector3(0, 0, 0);
        hinge.transform.localEulerAngles = new Vector3(0, 0, 0);

    }

    IEnumerator IELeftupdown(float time)        // 왼쪽 위 > 오른쪽 가운데
    {
        hinge.transform.localEulerAngles = new Vector3(0, 0, 45);
        yield return new WaitForSeconds(time);
        sickle.transform.localEulerAngles = new Vector3(0, 120, 0);
        hinge.transform.localEulerAngles = new Vector3(0, 0, 90);
        yield return new WaitForSeconds(time);
        sickle.transform.localEulerAngles = new Vector3(0, 0, 0);
        hinge.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    IEnumerator IERightdownup(float time)
    {
        hinge.transform.localEulerAngles = new Vector3(0, 0, -90);
        yield return new WaitForSeconds(time);
        sickle.transform.localEulerAngles = new Vector3(0, -120, 0);
        hinge.transform.localEulerAngles = new Vector3(0, 0, -30);
    }

    IEnumerator IELeftupdown2(float time)
    {
        // 왼쪽 위에서 다시 아래
        sickle.transform.localEulerAngles = new Vector3(0, 0, 60);
        yield return new WaitForSeconds(time);
        hinge.transform.localEulerAngles = new Vector3(0, 0, 10);
        sickle.transform.localEulerAngles = new Vector3(0, 160, 60);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 from = transform.position;
        Vector3 to = from + transform.forward * currDistance;
        Gizmos.DrawLine(from, to);
    }
}
