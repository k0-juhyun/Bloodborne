using Retro.ThirdPersonCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class bossAI : MonoBehaviour
{
    private Transform playerTr;
    private Transform bossTr;
    private bool attackInProgress;
    private bool isRetreat;
    private bool isLaser;

    public GameObject[] Laser;

    [Header("공격사거리")]
    public float attackDis;

    [Header("추적사거리")]
    public float traceDis;

    [Header("죽었는지 확인")]
    public bool isDie = false;

    [Header("보스 이동속도")]
    public float traceSpeed;

    [Header("보스 체력")]
    public float maxHp;
    public float curHp;


    // 컴포넌트들
    private Animator animator;
    private bossMove bossmove;
    private bossDamage bossdamage;
    private Rigidbody rb;
    // 보스 상태
    public enum State
    {
        Idle, // 기본
        Retreat, // 후퇴
        Trace, // 추적
        Phase1_Attack, // 공격
        Phase2_Attack, // 공격
        Phase3_Attack, // 공격
        Damage, // 피격
        Die // 죽음
    }
    // 처음에는 기본상태
    [Header("현재 보스 상태")]
    public State state = State.Idle;

    // 보스 공격 패턴
    public enum Phase1_AttackPattern
    {
        OverHeadWheel,
        JumpingSlam,
        ChargingCombo,
        RightPunch,
        LeftPunch
    }

    [Header("공격 패턴")]
    public Phase1_AttackPattern attackPattern = Phase1_AttackPattern.JumpingSlam;

    // 보스 페이즈
    public enum Phase
    {
        Phase1,
        Phase2,
        Phase3
    }
    
    [Header("페이즈")]
    public Phase phase = Phase.Phase1;

    private WaitForSeconds stateCheckDelay;

    [Header("공격 딜레이")]
    public float attackDelay;

    private readonly int hashMove = Animator.StringToHash("isMove");
    private readonly int hashRetreat = Animator.StringToHash("Retreating");
    private readonly int hashSpeed = Animator.StringToHash("speed");

    void Awake()
    {
        curHp = maxHp;
        animator = GetComponent<Animator>();
        bossmove = GetComponent<bossMove>();
        bossdamage = GetComponent<bossDamage>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        var player = GameObject.FindGameObjectWithTag("Player");

        // 인스턴스 체크
        if (player != null)
            playerTr = player.GetComponent<Transform>();

        bossTr = GetComponent<Transform>();
        //  transform.position = originPos.transform.position;
        // 0.2초 주기로 상태체크
        stateCheckDelay = new WaitForSeconds(0.2f);
    }
    private void OnEnable()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());
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

            // 체력이 40퍼센트 이하면 2페이즈로 돌입
            // + 레이저 안쐈으면
            if (curHp / maxHp <= 0.4)
            {
                phase = Phase.Phase2;
                state = State.Phase2_Attack;
            }

            if (dis <= attackDis            // 공격 사거리 내에 들어와 있고
                && !bossdamage.isHitted     // 피격 당하지 않고
                && !isRetreat               // 후퇴하지 않는다면 공격패턴
                && phase == Phase.Phase1)         
            {
                state = State.Phase1_Attack;
            }


            else if (dis <= traceDis            // 추적 사거리 내에 있고
                    && !bossdamage.isHitted     // 피격 당하지 않고
                    && !isRetreat
                    && phase != Phase.Phase2)              // 후퇴중이아니라면    
            {
                state = State.Trace;
            }

            // 피격 당했다면 피격상태
            else if (bossdamage.isHitted)
            {
                state = State.Damage;
            }

            else if (isRetreat)
            {
                state = State.Retreat;
            }
            yield return stateCheckDelay;
        }
    }

    // 보스 상태 제어 코루틴
    IEnumerator Action()
    {
        while (!isDie)
        {
            yield return stateCheckDelay;
            switch (state)
            {
                case State.Idle:
                    break;
                case State.Trace:
                    bossmove.moveToPlayer();
                    animator.SetBool(hashMove, true);
                    break;
                case State.Phase1_Attack:
                    bossmove.stopTracing();
                    StartCoroutine(ChcekPhase1AttackPattern());
                    break;
                case State.Phase2_Attack:
                    animator.SetBool(hashMove, false);
                    bossmove.laserLookPlayer();
                    animator.SetBool("laserSetup", true);
                    break;
                case State.Damage:
                    break;
                case State.Retreat:
                    if(phase!=Phase.Phase2)
                    {
                        StartCoroutine(RetreatOff(0.8f));
                        bossmove.moveToOrigin();
                        animator.SetBool(hashMove, false);
                        animator.Play("Retreating");
                    }
                    break;
                case State.Die:
                    break;
            }
        }
    }

    IEnumerator ChcekPhase1AttackPattern()
    {
        // 1페이즈
        if (!attackInProgress && phase == Phase.Phase1)
        {
            StartCoroutine(RetreatOn(5f));
            int randomIndex = Random.Range((int)Phase1_AttackPattern.OverHeadWheel, (int)Phase1_AttackPattern.LeftPunch + 1);
            attackPattern = (Phase1_AttackPattern)randomIndex;

            // 선택된 attackPattern에 따라 행동 수행
            switch (attackPattern)
            {
                // 전방 오버헤드 채찍 공격 뒤로 1~2회 빠르게 이동
                case Phase1_AttackPattern.OverHeadWheel:
                    attackInProgress = true;
                    animator.Play("OverHeadWheel");
                    break;

                // 장거리 점프 공격 앞으로 퀵스텝 1~2회
                case Phase1_AttackPattern.JumpingSlam:
                    attackInProgress = true;
                    animator.Play("JumpingSlam");
                    break;

                // 전방 돌진 콤보 퀵스텝 옆 or 대각선 뒤로
                case Phase1_AttackPattern.ChargingCombo:
                    attackInProgress = true;
                    animator.Play("ChargingCombo");
                    break;

                // 오른쪽 펀치
                case Phase1_AttackPattern.RightPunch:
                    attackInProgress = true;
                    animator.Play("RightPunch");
                    break;

                // 왼쪽 펀치
                case Phase1_AttackPattern.LeftPunch:
                    attackInProgress = true;
                    animator.Play("LeftPunch");
                    break;
            }
            yield return new WaitForSeconds(attackDelay);
            attackInProgress = false;
        }

    }

    void Update()
    {
        animator.SetFloat(hashSpeed, bossmove.speed);
    }

    // 상대의 위치와 내 위치를 통해 각도를 계산하는 함수
    float GetAngle(Vector3 from, Vector3 to)
    {
        return Quaternion.FromToRotation(transform.forward, to - from).eulerAngles.y;
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

    IEnumerator RetreatOff(float time)
    {
        yield return new WaitForSeconds(time);
        isRetreat = false;
    }

    IEnumerator RetreatOn(float time)
    {
        yield return new WaitForSeconds(time);
        isRetreat = true;
    }

    void LaserCoroutine()
    {
        StartCoroutine(LaserBeam());
    }
    IEnumerator LaserBeam()
    {
        if(!isLaser)
        {
            Laser[0].SetActive(true);
            yield return new WaitForSeconds(0.3f);
            Laser[1].SetActive(true);
            yield return new WaitForSeconds(5f);
            Laser[0].SetActive(false);
            Laser[1].SetActive(false);
            animator.SetBool("laserSetup", false);
            isLaser = true;
            phase = Phase.Phase3;
            state = State.Phase3_Attack;
        }
    }
}