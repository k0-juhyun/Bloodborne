using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// 대체, 추가 : 나중에 합치면 교체해야 하는 것
public class BossAlpha : MonoBehaviour
{
    //싱글톤
    public static BossAlpha instance;

    NavMeshAgent agent;                 // 길찾기
    Animator anim;                  // 애니메이터

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
        PhaseChange,
        Die
    }
    public BossPatternState bossState;         // 보스 상태 = 0: 대기, 1:이동, 2:회피, 4:피격, 5:낫 공격(대시,점프 포함), 6: 칼 공격(대시 점프포함?), 7: 총 공격 8: 죽음

    public enum BossPhase                      // 보스 페이즈 (체력에 따라 변함, 기본 = 1)
    {
        Phase1,
        Phase2,
        Phase3
    }
    public BossPhase bossPhase;                // 보스 페이즈 상태 = 1페이즈(시작), 2페이즈(체력75%이하), 3페이즈(체력 50%이하)

    public enum SickelSubState                     // 낫 공격 상태
    {
        Attack1,
        Attack2,
        Attack3,
    }

    public SickelSubState sickelSubState;

    public GameObject player;           // 플레이어
    Rigidbody rid;                      // 리지드바디
    RaycastHit hit;                     // 레이캐스트 히트
    public Transform rayPos;            // ray 쏘는 곳, 게르만 중심
    public GameObject sickle;           // 무기
    public GameObject blade;            // 무기
    public GameObject gun;              // 무기
    private GameObject bloodEffect;     // 피 프리팹



    Vector3 dir;                        // 이동 방향
    Vector3 avoidDir;                   // 회피 방향
    Vector3 moveDir;                    // 움직임 방향
    Vector3 quickeningDir;              // 폭발시 플레이어 밀어낼 방향
    Vector3 targetPos;                  // 회피 목적지
    Vector3 moveTargetPos;              // 공격시 이동 목적지
    Vector3 attackdir;
    Vector3 attackMovePos;
    Vector3 currPos;                    // 패턴1.2 공격 위치

    int damage = 2;                         // 데미지 (플레이어한테 어떤 공격인지 상태를 받아오기, 공격에 따라 다른 데미지를 구현)
    public int hitCount = 0;                   // 피격 횟수 (맞은 횟수)

    [Header("데미지")]
    public int attack1damage = 1;              // 플레이어 공격 1번 데미지
    public int attack2damage = 2;              // 플레이어 공격 2번 데미지
    public int attack3damage = 3;              // 플레이어 공격 3번 데미지

    float currDistance;                        // 현재 거리
    [Header("거리")]
    public float attackDistance = 8f;          // 공격시작 거리
    public float hitDistance = 5f;             // 피격 거리
    public float avoidDistance = 5f;           // 회피 거리
    public float moveDis = 5f;                 // 공격 이동거리
    public float quickeningDis = 2f;           // 폭발 범위 (거리, 구의 반지름

    [Header("속도")]
    public float moveSpeed = 1f;               // 이동 속도
    public float dashSpeed = 10f;              // 대시 속도
    public float attackMoveSpeed = 5f;         // 공격 이동 속도
    public float quickeningForce = 5f;         // 폭발 공격시 힘의 크기


    [Header("시간")]
    public float curTime;                      // 현재 시간
    public float avoidcurTime;                 // 회피 시간
    public float hitcurrTime;                  // 현재 피격 시간
    public float hitTime = 3f;                 // 피격 2 재생되는 시간 2 > 3
    public float skTime_Sickel1_1 = 1f;        // 낫 스킬1 1번 공격 시작 시간
    public float skTime_Sickel1_2 = 2f;        // 낫 스킬1 2번 공격 시작 시간
    public float skTime_Sickel1_3 = 3f;        // 낫 스킬3 3번 공격 시작 시간

    [Header("상태 확인")]
    public bool isHitted = false;                       // 피격 상태 확인
    public bool isHitted2 = false;                      // 2회 피격 상태 확인
    public bool isAvoid = false;                        // 회피 상태 확인
    public bool isCombo1done = false;                   // 프로토타입용 낫패턴 1
    public bool isCombo2done = false;                   // 프로토타입용 낫패턴 2
    public bool isCombo3done = false;                   // 프로토타입용 낫패턴 3
    public bool isMoveTargetPos = false;                // SC2_a2 이동 계산용 상태확인
    public bool isGehrmanDie = false;                   // 죽음 상태 확인
    public bool isGehrmanAttack = false;                // 공격 상태 확인 to 플레이어용
    public bool isPhase2 = false;                       // 페이즈 2 상태 확인
    public bool isQuickening = false;                   // Quickening 폭발 공격
    public bool a = false;                              // sickle2 에서 플레이어 위치 한번만 계산


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        agent = GetComponent<NavMeshAgent>();                       // agent
        //agent.isStopped = true;                                   // 이동 멈추기
        rid = GetComponent<Rigidbody>();                            // 리지드바디
        anim = GetComponent<Animator>();                            // 애니메이터 컴포턴트를 받아온다
        bossHP = GetComponent<BossHP>();                            // 보스 hp를 받아오자
        bossPhase = BossPhase.Phase1;                               // 시작시 보스 페이즈를 1로 설정한다
        bloodEffect = Resources.Load<GameObject>("DAX_Blood_Spray_00(Fade_2s)");        // 블러드 이펙트 불러오기
    }

    // Update is called once per frame
    void Update()
    {
        // 밀림방지
        //rid.velocity = Vector3.zero;
        // 항상 플레이어를 바라보기
        // 플레이어와 나 사이의 방향을 구한다
        dir = player.transform.position - this.transform.position;
        dir.Normalize();
        // 그 방향을 나의 앞 방향으로 한다
        dir = transform.forward;

        // 이동 맵 navemesh 로 바꾸기(알파)
        Vector3 ad = player.transform.position;
        ad.y = transform.position.y;

        transform.LookAt(ad);


        // 플레이어와 나 사이의 거리를 잰다
        currDistance = Vector3.Distance(transform.position, player.transform.position);

        //print("currDistacne : " + currDistance);

        // 피격 시간을 흐르게 한다 // 옮길지 말지 생각해보기 **수정, 안바꿔도 잘됨 아마도
        hitcurrTime += Time.deltaTime;
        // 만약 피격 시간이 2차 피격시간보다 크다면,
        if (hitcurrTime >= hitTime)
        {
            // 피격 횟수를 초기화
            //hitCount = 2;
            hitCount = 0;
            isHitted = false;
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
            case BossPatternState.Die:
                UpdateDie();
                break;
            case BossPatternState.PhaseChange:
                UpdatePhaseChange();
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

    // 페이즈 변경 상태
    private void UpdatePhaseChange()
    {
        // 1 > 2 페이즈 넘어갈 때 딱 한번만 할거임 (bool 로 만들어서 체크하기)
        // 만약 현재 bossPhase.Phase2 라면
        if (isPhase2 == false && bossPhase == BossPhase.Phase2)
        {
            // 무기교체 함수를 실행한다(무기 교체 함수 만들기
            // 무기 교체 함수에서 애니메이션도 같이 실행한다
        }

        // 만약 현재 Phase3 이라면
        // Quickning 함수를 호출한다
        // 3페이즈에서 공격을 랜덤으로 돌릴때, Quikning 호출을 추가한다
        // 알파에서는 순서대로 실행하게 한다(3 되면 무조건 실행), 3 패턴이 어떻게 되는 건지 기획과 협의 필요.
        if (bossPhase == BossPhase.Phase3)
        {
            // 여기서 시간을 흐르게 한다(1초후에 사라지게 하려구..??

            // 한번만 호출해야함
            if (isQuickening == false)
            {
                //애니메이션을 실행한다
                anim.SetTrigger("Quickening");
                //이펙트를 실행한다
                // 한번만 호출하지만 Update로 작동해야함. 할거 다 끝나고 true로 하기
                Quickening();
            }
        }

        // 끝나면 다시 상태를 Idle로 만든다. 
    }

    

    private void Quickening()
    {
        // 만약 isQuickening = true 일때, 플레이어가 충돌했다면(overlapSpher.layer로 비교)
        int layer = 1 << LayerMask.NameToLayer("Player");
        //게르만의 중심에서 overlapSpher를 만든다
        Collider[] cols = Physics.OverlapSphere(rayPos.position, quickeningDis, layer);
        if (cols.Length > 0)
        {
            // 플레이어의 리지드 바디를 받아온다
            Rigidbody playerRid = player.GetComponent<Rigidbody>();
            // 나에서 플레이어로 가는 방향의 벡터를 구한다 = 폭발시 플레이어에게 가할 힘의 방향을 구한다
            quickeningDir = transform.forward + transform.up;
            quickeningDir.Normalize();

            // 플레이어의 피격시 움직임 못하게 하는 부울 상태를 트루로 한다

            // 그 방향으로 Addforce를 한다
            playerRid.AddForce(quickeningDir * quickeningForce, ForceMode.Impulse);

        }
        //1초 후에 사라진다
        //상태가 idle 로 바뀐다
        //isQuickening = true 로 한다
    }

    private void UpdateIdle()           // 공격이 끝나면 idle 상태로 옴
    {
        isAvoid = false;

        // 만약 현재 거리가 공격 가능 거리보다 크다면
        if (currDistance > attackDistance)
        {
            // 현재 상태를 Move 로 변화시킨다
            bossState = BossPatternState.Move;
            // 애니메이션 재생
            anim.SetTrigger("Move");
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
        else if (currDistance <= hitDistance && TPSChraracterController.instance.isAttack == true)
        {
            // 회피 상태로 변화시킨다
            bossState = BossPatternState.Avoid;
            // 애니메이션 호출
            anim.SetTrigger("Avoid");
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
        FindMoveTargetPos();
        anim.SetBool("Leg", true);
    }

    private void SickelC2()
    {
        // 낫공격2으로 상태 변화시킨다 (랜덤 뽑기 나중에)
        bossState = BossPatternState.SickelCombo2;
        print("SickelCombo2");
        FindMoveTargetPos();
        anim.SetBool("Leg", true);
    }

    private void SickelC1()
    {
        // 낫공격1으로 상태 변화시킨다 (랜덤 뽑기 나중에)
        bossState = BossPatternState.SickelCombo1;
        print("SickelCombo1");
        FindMoveTargetPos();
        anim.SetBool("Leg", true);
    }

    // 이동 목적지 찾기 함수
    private void FindMoveTargetPos()
    {
        moveDir = transform.forward;
        // 거리는 플레이어 움직임 속도 보면서 결정하기
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
            // 랜덤으로 뽑는다
            // 일단 무조건 SickelCombo1으로 가게해서 완성한다
            SickelC1();

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

        // 회피상태가 true가 되면
        // 뒤로 Ray를 쏜다
        // 맞은 정보를 확인해서, 만약 맞은 곳이 땅 또는 벽이고, 거리가 회피가능 거리보다 크거나 같으면
        // 뒤로 회피한다
        // 만약 맞은 거리가 더 작다면
        // 오른쪽으로 Ray를 쏜다
        // 만약 맞은 거리가 회피 가능 거리보다 크면,
        // 오른쪽으로 회피한다
        // 

        // 뒤로 회피
        if (isAvoid == true)
        {
            if (avoidCount == 0)
            {
                // 뒤로회피
                targetPos.y = transform.position.y;
                transform.position = Vector3.Lerp(transform.position, targetPos, dashSpeed * Time.deltaTime);
                // 애니메이션 호출
                //anim.SetTrigger("Avoid");
                if (Vector3.Distance(transform.position, targetPos) < 0.1f)
                {
                    bossState = BossPatternState.Idle;
                    //Idle 애니메이션 실행(상태바꿀때 한번만 호출)
                    anim.SetTrigger("Idle");
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
                    moveDis = 5f;
                    // 이동 목적지
                    moveTargetPos = transform.position + moveDir * moveDis;
                    isMoveTargetPos = true;
                }

                moveTargetPos.y = transform.position.y;
                transform.position = Vector3.Lerp(transform.position, moveTargetPos, dashSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, moveTargetPos) < 0.1f)
                {
                    bossState = BossPatternState.Idle;
                    //Idle 애니메이션 실행(상태바꿀때 한번만 호출)
                    anim.SetTrigger("Idle");
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
            // 현재 보스 상태를 페이즈 변경 상태로 한다
            bossState = BossPatternState.PhaseChange;
            // 현재 페이즈를 2페이즈로 한다
            bossPhase = BossPhase.Phase2;
            // 보스 공격 상태를 ?로 바꾼다
            //return;     // 바로가기?? 아마 아래에 있는 피깎이를 위로 올려야할듯? 무적상태가 따로 있나? 없나? 체크필요

            // 만약 현재 체력이 50보다 작다면
            if (bossHP.HP <= 5)
            {
                // 현재 보스 상태를 페이즈 변경 상태로 한다
                bossState = BossPatternState.PhaseChange;
                // 현재 페이즈를 3페이즈로 한다
                bossPhase = BossPhase.Phase3;
                
                // 보스 공격 상태를 ?로 바꾼다
                //return;
            }

            // 만약 현재 체력이 0이면
            if (bossHP.HP == 0)
            {
                // 보스 상태를 죽음으로 한다
                bossState = BossPatternState.Die;
                // 죽음 애니메이션을 실행한다
                anim.SetTrigger("Die");
                // 파티클을 켠다
            }
        }
        // 만약 isHitted 가 false일때,
        // 왜 false여야하지?
        if (isHitted == false)
        {
            hitcurrTime = 0;
            // 히트 카운드를 1더한다
            hitCount++;
            print("hitCount : " + hitCount);
            // 어떤 공격인지에 따라 그 데미지만큼을 현재 체력에서 뺀다
            bossHP.HP -= damage;
            print(bossHP.HP);
            if (hitCount == 1)
            {
                // 피격 1 애니메이션을 실행한다
                anim.SetTrigger("Hit1");

            }
            // 피격 상태를 true로 만든다(한번만 되게)
            isHitted = true;
        }


        // 만약 현재 피격시간이 피격2 시간보다 작을때, 히트 카운트가 2라면
        else if (hitcurrTime <= hitTime && hitCount >= 2)
        {
            print("hitcount : " + hitCount);
            isHitted2 = true;

            if (isHitted2 == true)
            {
                // 피격 2 애니메이션을 실행한다
                anim.SetTrigger("Hit2");
                print("피격2");
                // 히트 카운트를 0으로 한다
                hitCount = 0;
            }
            // 부울값을 넣어서 
            // if문안에 트루일때를 만들어서
            // 위를 실행
            //isHitted = false;
        }


        if (hitcurrTime > 1)
        {
            isHitted = false;
            bossState = BossPatternState.Idle;
            //Idle 애니메이션 실행(상태바꿀때 한번만 호출)
            anim.SetTrigger("Idle");

        }

        // 밖에서 부울값이 false이면 시간 증가
        // 일정 시간이 증가하면
        // 부울 값 모두 초기화

        // hit2 up 애니메이션이 끝나면 idle 상태로 돌린다(이벤트 함수로)
    }

    // 이벤트 함수
    void AnimIdle()
    {
        // Idle상태로 한다
        bossState = BossPatternState.Idle;
    }

    void AnimHitUp()
    {
        // Idle상태로 한다
        //bossState = BossPatternState.Idle;
        // hit up 애니메이션 켠다
        anim.SetTrigger("HitUp");
        isHitted2 = false;
        isHitted = false;
    }


    void AnimDie()
    {
        Destroy(gameObject);
    }


    private void UpdateSickelCombo1()
    {
        isGehrmanAttack = true;

        // int로 값을 정해서 그 값에서만 실행되고, 다른 상태일때 다시 초기화되게 바꾸기

        curTime += Time.deltaTime;

        switch (sickelSubState)
        {
            case SickelSubState.Attack1:
                if (curTime > skTime_Sickel1_1)
                {
                    //anim.SetBool("Leg", true);
                    print("SubState : Attack1");
                    // 앞으로 이동하고 싶다
                    moveTargetPos.y = transform.position.y;
                    transform.position = Vector3.Lerp(transform.position, moveTargetPos, dashSpeed * Time.deltaTime);

                    //print("moveTargetPos :" + moveTargetPos);
                    if (Vector3.Distance(transform.position, moveTargetPos) < 1f)
                    {
                        anim.SetBool("Leg", false);
                        // 서브상태를 액션 2로 바꾼다
                        sickelSubState = SickelSubState.Attack2;
                        // 애니메이션 재생
                        anim.SetTrigger("Attack2");
                        print("com1_attack_move");
                        // 현재 위치 기억하기
                        currPos = transform.position;
                    }
                }
                break;
            case SickelSubState.Attack2:
                if (curTime > skTime_Sickel1_2)
                {
                    print("SubState : Attack2");
                    // 플레이어의 오른쪽으로 이동한다
                    // 항상 플레이어의 오른쪽 인지 아닌지를 모르겠어...~!~~!!!그자리에서 움직이는 건가????
                    // 이동방향
                    //Vector3 attackdir = transform.forward + player.transform.right;
                    //attackdir.Normalize();
                    //Vector3 attackMovePos = player.transform.position + player.transform.right * 2;
                    // 한번만 계산하기
                    if (a == false)
                    {
                        //print("현재위치 :" + transform.position);

                        // 이동방향
                        attackdir = transform.forward + player.transform.right;
                        attackdir.Normalize();
                        // 한번만 계산하기
                        attackMovePos = player.transform.position + player.transform.right * 2;
                        a = true;
                        //print("목적지 : " + attackMovePos);
                        //print("playerPos : " + player.transform.position);

                    }

                    //print("목적지2 : " + attackMovePos);
                    //anim.SetBool("Leg", true);

                    attackMovePos.y = transform.position.y;
                    transform.position = Vector3.Lerp(currPos, attackMovePos, 0.5f);

                    //print("현재위치2 :" + transform.position);
                    print("위치차 : " + Vector3.Distance(transform.position, attackMovePos));
                    if (Vector3.Distance(transform.position, attackMovePos) < 5f)
                    {
                        anim.SetBool("Leg", false);
                        sickelSubState = SickelSubState.Attack3;
                        // 애니메이션 재생
                        anim.SetTrigger("Attack1");
                    }


                    //transform.position += attackdir * 50 * Time.deltaTime;
                }
                break;
            case SickelSubState.Attack3:
                if (curTime > skTime_Sickel1_3)
                {
                    print("SubState : Attack3");
                    curTime = 0;
                    // 현재 거리가 공격시 이동 거리보다 작으면 그만큼 뒤로 이동하게 해야하나??? **교체,수정
                    // 프로토타입용, 콤보 2로 넘어가는 조건, 수정용, 순서대로 호출
                    isCombo1done = true;
                    //a = false;
                    bossState = BossPatternState.Idle;
                    // 애니메이션 재생
                    anim.SetTrigger("Idle");
                    sickelSubState = SickelSubState.Attack1;
                    isGehrmanAttack = false;
                }
                break;
        }
    }


    private void UpdateSickelCombo2()
    {
        isGehrmanAttack = true;
        curTime += Time.deltaTime;
        switch (sickelSubState)
        {
            case SickelSubState.Attack1:
                if (curTime > skTime_Sickel1_1)
                {
                    //print("C2A1");
                    //anim.SetBool("Leg", true);
                    // 앞으로 이동하고 싶다
                    moveTargetPos.y = transform.position.y;
                    transform.position = Vector3.Lerp(transform.position, moveTargetPos, attackMoveSpeed * Time.deltaTime);
                    if (Vector3.Distance(transform.position, moveTargetPos) < 0.1f)
                    {

                        // 서브상태를 액션 2로 바꾼다
                        sickelSubState = SickelSubState.Attack2;
                        // 애니메이션 재생
                        anim.SetTrigger("Attack3");
                        anim.SetBool("Leg", false);
                        print("com2_attack_move");
                    }

                }
                break;
            case SickelSubState.Attack2:
                if (curTime > skTime_Sickel1_2)
                {

                    anim.SetBool("Leg", true);
                    // 한번 더 앞으로 이동
                    // 정해진 거리가 아니라 플레이어 위치까지인거 같은데...
                    if (isMoveTargetPos == false)
                    {
                        moveDir = player.transform.forward;
                        // 거리는 플레이어 움직임 속도 보면서 결정하기
                        //moveDis = 5f;
                        // 이동 목적지
                        //moveTargetPos = transform.position + moveDir * moveDis;
                        moveTargetPos = player.transform.position + moveDir * 1;
                        isMoveTargetPos = true;
                    }
                    //print("C2A2");
                    // 앞으로 이동하고 싶다
                    moveTargetPos.y = transform.position.y;
                    transform.position = Vector3.Lerp(transform.position, moveTargetPos, attackMoveSpeed * Time.deltaTime);
                    if (Vector3.Distance(transform.position, moveTargetPos) < 0.1f)
                    {

                        // 서브상태를 액션 3로 바꾼다
                        sickelSubState = SickelSubState.Attack3;
                        // 애니메이션 재생
                        anim.SetTrigger("Attack4");
                        anim.SetBool("Leg", false);
                        print("com2_attack_move");
                    }

                }
                break;
            case SickelSubState.Attack3:
                if (curTime > skTime_Sickel1_3)
                {
                    print("C2A3");

                    // 애니메이션 재생
                    anim.SetTrigger("Attack5");
                    // 프로토타입용, 콤보 3로 넘어가는 조건, 수정용, 순서대로 호출
                    isCombo2done = true;
                    isMoveTargetPos = false;
                    curTime = 0;
                    bossState = BossPatternState.Idle;
                    anim.SetTrigger("Idle");
                    sickelSubState = SickelSubState.Attack1;
                    isGehrmanAttack = false;
                }
                break;
        }
    }

    private void UpdateSickelCombo3()
    {
        isGehrmanAttack = true;
        curTime += Time.deltaTime;
        switch (sickelSubState)
        {
            case SickelSubState.Attack1:
                if (curTime > skTime_Sickel1_1)
                {
                    // 앞으로 이동하면서
                    // 이동할때 서서오니까 이상한데...
                    // 부울을 만들어서 켜고 끄자 굿

                    //anim.SetBool("Leg", true);
                    // 360 낫을 휘두르는 애니메이션을 한다
                    moveTargetPos.y = transform.position.y;
                    transform.position = Vector3.Lerp(transform.position, moveTargetPos, attackMoveSpeed * Time.deltaTime);
                    if (Vector3.Distance(transform.position, moveTargetPos) < 0.1f)
                    {
                        // 서브상태를 액션 2로 바꾼다
                        sickelSubState = SickelSubState.Attack2;
                        // 애니메이션 재생
                        anim.SetTrigger("Attack6");
                        anim.SetBool("Leg", false);
                        print("com3_attack_move");
                    }
                }
                break;
            case SickelSubState.Attack2:
                if (curTime > skTime_Sickel1_2)
                {
                    // 서브상태를 액션 3로 바꾼다
                    sickelSubState = SickelSubState.Attack3;
                    // 애니메이션 재생
                    // 낫이 360 도 돌아가는 게 있어야하는데...없네
                    print("com3_attack_Rdu");
                }
                break;
            case SickelSubState.Attack3:
                if (curTime > skTime_Sickel1_3)
                {
                    print("C3A3");

                    // 프로토타입용, 콤보 3로 넘어가는 조건, 수정용, 순서대로 호출
                    curTime = 0;
                    isCombo1done = false;
                    isCombo2done = false;
                    isMoveTargetPos = false;
                    bossState = BossPatternState.Idle;
                    // 애니메이션 재생
                    anim.SetTrigger("Idle");
                    sickelSubState = SickelSubState.Attack1;
                    print("3 > idle");
                    isGehrmanAttack = false;
                }
                break;
        }
    }

    private void UpdateDie()
    {
        // 만약 hp가 0이되면 Die 함수가 호출된다(hit에서)
        // 죽음 상태라고 알려주기(SceneManager에서 받아가기)
        isGehrmanDie = true;

    }

    

    // 충돌 체크
    private void OnTriggerEnter(Collider other)
    {
        // 만약 플레이어가 공격 상태이고, 플레이어의 무기와 충돌했을때
        if (isHitted == false && isHitted2 == false && TPSChraracterController.instance.isAttack == true && other.gameObject.CompareTag("Weapon"))
        {
            // 보스 피격 상태로 전환
            bossState = BossPatternState.Hit;
            print("hittt");
            LoadBloodEffect(other);

        }

        // 충돌 체크를 여기서 하는게 맞는지 확인 해보기(저 위에서 함.Quickening.)
        
    }

    // 피격 출혈 이펙트
    private void LoadBloodEffect(Collider other)
    {
        Vector3 pos = other.ClosestPointOnBounds(transform.position);
        Vector3 normal = transform.position - other.transform.position;

        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, normal);
        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot);
        Destroy(blood, 1.0f);
    }

    // 게르만 방향 확인용. 필요 없어지면 지우기
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 from = transform.position;
        Vector3 to = from + transform.forward * currDistance;
        Gizmos.DrawLine(from, to);
    }
}

