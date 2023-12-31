﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace bloodborne
{
    // 대체, 추가 : 나중에 합치면 교체해야 하는 것
    public class BossAlpha : MonoBehaviour
    {
        PlayerAnimatorManager animatorHandler;        // 플레이어 애니메이터 조정
        PlayerLocomotion playerLocomotion;          // 플레이어 날라가는 상태
        ObjectPool objectPool;                  // 이펙트용 오브젝트 풀
        FireAutoDeacitve fire;                  // 불 이펙트
        NavMeshAgent agent;                     // 길찾기
        Animator anim;                          // 애니메이터
        GehrmanSoundManager soundManager;              // 사운드 매니저
        FireAutoDeacitve fires;

        #region 사운드 목록
        [SerializeField]
        private string HitSound;                     // 1. 피격 피 터지는 소리 사운드
        [SerializeField]
        private string MoveSound;                   // 2. 이동 바람 사운드
        [SerializeField]
        private string GunSound;                    // 3. 총소리
        [SerializeField]
        private string QuickeningSound;             // 4. 폭발 공격 소리
        [SerializeField]
        private string ChangeWeaponSound;           // 5. 무기 교체 소리
        [SerializeField]
        private string SickleSound1;                // 6. 낫공격 1 소리
        [SerializeField]
        private string SickleSound2;                // 7. 낫공격 2 소리
        [SerializeField]
        private string SwordSound1;                 // 8. 칼공격 1 소리
        [SerializeField]
        private string SwordSound2;                 // 9. 낫공격 1 소리
        [SerializeField]
        private string AttackVoiceSound1;           // 10. 공격 목소리 1
        [SerializeField]
        private string AttackVoiceSound2;           // 11. 공격 목소리 2
        [SerializeField]
        private string AttackVoiceSound3;           // 12. 폭발 공격 목소리
        [SerializeField]
        private string DieSound;                    // 13. 죽음 소리
        [SerializeField]
        private string FootSoundRight1;              // 14. 오른쪽 발소리
        [SerializeField]
        private string FootSoundRight2;              // 14. 오른쪽 발소리
        [SerializeField]
        private string FootSoundLeft1;               // 15. 왼쪽 발소리
        [SerializeField]
        private string FootSoundLeft2;               // 15. 왼쪽 발소리
        #endregion


        BossHP bossHP;                          // 보스 hp

        public enum BossPatternState               // 열거형, 보스 패턴 상태
        {
            Idle,
            Move,
            Avoid,
            Hit,
            SickelCombo1,
            SickelCombo2,
            SickelCombo3,
            GunCombo,
            SwordCombo1,
            SwordCombo2,
            PhaseChange,
            Quickening,
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

        public enum AttackSubState                     // 낫 공격 하위 콤보 상태로 만들었지만, 같이 쓰자 > 이름바꿈
        {
            Attack1,
            Attack2,
            Attack3,
        }

        public AttackSubState attackSubState;


        public GameObject player;           // 플레이어
        Rigidbody rid;                      // 리지드바디
        RaycastHit hit;                     // 레이캐스트 히트
        public Transform rayPos;            // ray 쏘는 곳, 게르만 중심
        public Transform firePos;           // 총 쏘는 곳(이펙트
        public Transform effectPos;         // (이펙트
        public GameObject sickle;           // 무기
        public GameObject blade;            // 무기
        public GameObject gun;              // 무기
        private GameObject bloodEffect;     // 피 프리팹
        public GameObject dieEffect;       // 죽음 이펙트 프리팹
        public GameObject phase3Effect;    // 페이즈3 불꽃 이펙트 프리팹
        private GameObject quickeningEffect;// 폭발공격 이펙트 프리팹
        private GameObject gunEffect;       // 총공격 이펙트 프리팹
        public GameObject[] UI;             // 죽음 ui 


        #region 방향
        Vector3 dir;                        // 이동 방향
        Vector3 avoidDir;                   // 회피 방향
        Vector3 moveDir;                    // 움직임 방향
        Vector3 targetPos;                  // 회피 목적지
        Vector3 moveTargetPos;              // 공격시 이동 목적지
        Vector3 attackdir;
        Vector3 attackMovePos;
        Vector3 currPos;                    // 패턴1.2 공격 위치
        #endregion
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
        public bool isGehrmanDie = false;                   // 죽음 상태 확인(씬매니저용
        public bool ImDie = false;                          // 죽음 상태 확인(죽음 함수용
        public bool isGehrmanAttack = false;                // 공격 상태 확인 to 플레이어용
        public bool isPhase2 = false;                       // 페이즈 2 상태 확인
        public bool isPhase3 = false;                       // 페이즈 3 상태 확인
        public bool isQuickening = false;                   // Quickening 폭발 공격
        public bool gunAttack = false;                      // 총공격 상태 확인
        //public bool playerExplosion = false;                // 플레이어가 Quickening 폭발 공격 맞았음
        public bool isPhaseChangeQ = false;                 // 페이즈 전환 폭발
        public bool isSword1done = false;                   // 칼공격..패턴1
        public bool a = false;                              // sickle2 에서 플레이어 위치 한번만 계산
        public bool isGunHit = false;                       // 플레이어의 총에 맞은 상태
        public bool isMove = false;                         // 게르만 이동상태 phase 3 이펙트용


        // Start is called before the first frame update
        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();                       // agent
            //agent.isStopped = true;                                   // 이동 멈추기
            rid = GetComponent<Rigidbody>();                            // 리지드바디
            anim = GetComponent<Animator>();                            // 애니메이터 컴포턴트를 받아온다
            bossHP = GetComponent<BossHP>();                            // 보스 hp를 받아오자
            bossPhase = BossPhase.Phase1;                               // 시작시 보스 페이즈를 1로 설정한다
            bloodEffect = Resources.Load<GameObject>("BloodEffect_Gehrman");        // 블러드 이펙트 불러오기
            //dieEffect = Resources.Load<GameObject>("DieEffect_Gehrman");            // 죽음 이펙트 불러오기
            //phase3Effect = Resources.Load<GameObject>("Phase3 Fire Particle System");    // 페이즈3 이펙트 불러오기
            objectPool = FindObjectOfType<ObjectPool>();                                    // objpool 이펙트용
            fire = FindObjectOfType<FireAutoDeacitve>();                            // 불 이펙트용
            quickeningEffect = Resources.Load<GameObject>("Gehrman_Effect_Quickening");    // 폭발공격 이펙트 불러오기
            gunEffect = Resources.Load<GameObject>("GunFireEfferct_Gehrman");       // 총공격 이펙트 불러오기
            animatorHandler = FindObjectOfType<PlayerAnimatorManager>();      // 플레이어 피격 상태 애니메이션
            playerLocomotion = FindObjectOfType<PlayerLocomotion>();            // 플레이어 날라가는 상태
            soundManager = FindObjectOfType<GehrmanSoundManager>();                    // 사운드 매니저
            fires = FindObjectOfType<FireAutoDeacitve>();
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
                isHitted2 = false;
            }

            if (ImDie == false)
            {
                // 만약 현재 체력이 0이면
                if (bossHP.HP <= 0)
                {
                    print("Update : Die");
                    // 보스 상태를 죽음으로 한다
                    bossState = BossPatternState.Die;
                    Destroy(phase3Effect.gameObject);
                    //phase3Effect.SetActive(false);
                    // 죽음 애니메이션을 실행한다
                    anim.SetTrigger("Die");
                    // 사운드 재생
                    soundManager.PlaySound(DieSound);
                    // 파티클을 켠다
                    //GameObject dieEff = Instantiate<GameObject>(dieEffect, rayPos);
                    dieEffect.SetActive(true);
                    // 무기를 끈다 or 땅으로 떨어뜨려야 하나..?
                    gun.SetActive(false);
                    blade.SetActive(false);
                    ImDie = true;
                }
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
                case BossPatternState.GunCombo:
                    UpdateGunCombo();
                    break;
                case BossPatternState.PhaseChange:
                    UpdatePhaseChange();
                    break;
                case BossPatternState.Quickening:
                    UpdateQuickening();
                    break;
                case BossPatternState.SwordCombo1:
                    UpdateSwordCombo1();
                    break;
                case BossPatternState.SwordCombo2:
                    UpdateSwordCombo2();
                    break;
                case BossPatternState.Die:
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
                    // 페이즈 3 이펙트를 생성
                    UpdatePhase3();
                    break;
                default:
                    break;
            }
        }

        private void UpdatePhase3()
        {
            // 페이즈 3 일때 계속 할일
            // 하나 따로 만들어져 있는 불꽃 이펙트 오브젝트를 켠다
            if (bossState != BossPatternState.Die)
            {
                phase3Effect.SetActive(true);

            }
            // is Move 가 켜져있을때만
            //if (isMove == false)
            //{
            //    if(fire.gameObject != null)     // 오류계속남
            //    fire.objActiveFalse();
            //}
            // 오브젝트 풀로 관리한다
            GameObject fireEffect = objectPool.GetDeactiveObject();
            if (fireEffect != null)
            {
                fireEffect.transform.position = effectPos.transform.position;
                fireEffect.GetComponent<FireAutoDeacitve>().Play(1);
            }

        }

        private void UpdateSwordCombo2()
        {
            // 칼공격 콤보 2
            curTime += Time.deltaTime;
            // 이동하면서
            // 칼공격을 한다
            // 애니메이션을 키면
            // 애니메이션에 AnimMove라는 이벤트 함수에서 이동을 한다?
            switch (attackSubState)
            {
                case AttackSubState.Attack1:
                    if (curTime > skTime_Sickel1_1)
                    {
                        // 이동한다
                        moveTargetPos.y = transform.position.y;
                        transform.position = Vector3.Lerp(transform.position, moveTargetPos, dashSpeed * Time.deltaTime);

                        //print("moveTargetPos :" + moveTargetPos);
                        if (Vector3.Distance(transform.position, moveTargetPos) < 1f)
                        {
                            // 칼공격 3 애니메이션 켜기
                            anim.SetTrigger("Sword3");
                            attackSubState = AttackSubState.Attack2;
                        }
                        
                    }
                    break;
                case AttackSubState.Attack2:
                    if (curTime > skTime_Sickel1_2)
                    {

                        // 서브상태를 액션 3로 바꾼다
                        attackSubState = AttackSubState.Attack3;
                    }
                    break;
                case AttackSubState.Attack3:
                    if (curTime > skTime_Sickel1_3)
                    {
                        // 끝나면 총공격을 호출한다
                        bossState = BossPatternState.GunCombo;
                        isSword1done = false;
                    }
                    break;
            }
        }

        // 이벤트 함수 사운드
        void SwordAttackSound()
        {
            // 사운드 재생
            soundManager.PlaySound(SwordSound1);
            soundManager.PlaySound(AttackVoiceSound1);
        }

        private void UpdateSwordCombo1()
        {
            // 칼공격 콤보 1
            curTime += Time.deltaTime;
            // 한번 공격하고
            // x자로 공격한다
            // 끝나면 총공격을 호출한다
            switch (attackSubState)
            {
                case AttackSubState.Attack1:
                    if (curTime > skTime_Sickel1_1)
                    {
                        print("S1SubState : Attack1");
                        isMove = true;
                        // 앞으로 이동하고 싶다
                        moveTargetPos.y = transform.position.y;
                        transform.position = Vector3.Lerp(transform.position, moveTargetPos, dashSpeed * Time.deltaTime);

                        print("S1 moveTargetPos :" + moveTargetPos);
                        if (Vector3.Distance(transform.position, moveTargetPos) < 1f)
                        {
                            anim.SetBool("Leg", false);
                            // 한번 공격한다
                            anim.SetTrigger("Sword1");
                            // 사운드 재생
                            soundManager.PlaySound(SwordSound1);
                            soundManager.PlaySound(AttackVoiceSound2);
                            // 서브스테이트 상태를 Atttack2로 한다
                            attackSubState = AttackSubState.Attack2;
                            isMove = false;
                        }

                        //// 한번 공격한다
                        //anim.SetTrigger("Sword1");
                        //// 사운드 재생
                        //soundManager.PlaySound(SwordSound1);
                        //soundManager.PlaySound(AttackVoiceSound2);
                        //// 서브스테이트 상태를 Atttack2로 한다
                        //attackSubState = AttackSubState.Attack2;
                    }
                    break;
                case AttackSubState.Attack2:
                    if (curTime > skTime_Sickel1_2)
                    {
                        // x자로 공격한다
                        anim.SetTrigger("Sword2");
                        // 사운드 재생
                        soundManager.PlaySound(SwordSound2);
                        // 서브스테이트 상태를 Atttack3로 한다
                        attackSubState = AttackSubState.Attack3;
                    }
                    break;
                case AttackSubState.Attack3:
                    if (curTime > skTime_Sickel1_3)
                    {
                        // 끝나면 총공격을 호출한다
                        bossState = BossPatternState.GunCombo;
                        isSword1done = true;
                    }
                    break;

            }
        }

        private void UpdateGunCombo()
        {
            //print("총공격 콤보");
            // 2페이즈부터 등장한다
            // 칼콤보 끝나면 무조건 총으로 오기
            // substate필요 없을듯. 총만 쏘면됨
            // 칼 콤보1,2 가 끝나면 상태를 총 공격 상태로 바꾼다
            // 총 공격 부울값을 만들어서, false이면
            if (gunAttack == false)
            {
                print("총공격 콤보");
                // 왼손에 든 총을 발사하는 애니메이션을 실행한다
                anim.SetTrigger("Gun");
                gunAttack = true;
            }
            // 애니메이션의 총쏘는 타이밍에 이벤트 함수를 만든다 
        }

        // 총알 발사 이벤트 함수
        void GunFire()
        {
            print("c총");
            // 레이를 쏜다
            Ray ray = new Ray(rayPos.position, transform.forward);
            // 레이가 부딪힌 대상의 정보를 저장
            RaycastHit hitInfo = new RaycastHit();
            Debug.DrawRay(rayPos.position, transform.forward * currDistance, Color.blue);
            if (Physics.Raycast(ray, out hitInfo))
            {
                print("ccc");
                if (hitInfo.collider.CompareTag("Player"))
                {
                    // 플레이어에게 너 맞았다고 한다?
                    //playerExplosion = true;
                    playerLocomotion.playerExplosion = true; //합치고 바꾸기
                    print("총공격맞음");
                }
            }
            // 이벤트 함수로 이펙트 실행
            // 총공격이 끝나면 이벤트 ToIdle 상태로 한다
        }

        // 총알 이펙트 이벤트 함수
        void GunEffect()
        {
            // 총알 이펙트를 찾아서 이펙트를 쓴다
            // 사운드 재생
            soundManager.PlaySound(GunSound);
            // 불나오는 거
            // 총알이 퍼지는거 muzzle effect
            // 총알 이펙트를 firePos의 위치에 배치한다
            GameObject GEffect = Instantiate<GameObject>(gunEffect, firePos);
            Destroy(GEffect, 1.0f);
        }


        void ToIdle()
        {
            curTime = 0;
            gunAttack = false;
            bossState = BossPatternState.Idle;
            // 애니메이션 재생
            anim.SetTrigger("Idle");
            attackSubState = AttackSubState.Attack1;
        }


        // 페이즈 변경 상태
        private void UpdatePhaseChange()
        {
            // 1 > 2 페이즈 넘어갈 때 딱 한번만 할거임 (bool 로 만들어서 체크하기)
            // 만약 현재 bossPhase.Phase2 라면
            if (isPhase2 == false && bossPhase == BossPhase.Phase2)
            {
                // 무기교체 함수를 실행한다(무기 교체 함수 만들기> 이벤트함수로 애니메이션에서 호출함
                // 무기 교체 애니메이션 실행
                anim.SetTrigger("WeaponChange");
                // 사운드 재생
                soundManager.PlaySound(ChangeWeaponSound);
                isPhase2 = true;
                print("무기 바꿈 > 칼, 총");
                // 상태를 다시 idle로 한다(애니메이션 이벤트에서)
            }

            // 만약 현재 Phase3 이라면
            // Quickning 함수를 호출한다
            // 3페이즈에서 공격을 랜덤으로 돌릴때, Quikning 호출을 추가한다
            // 알파에서는 순서대로 실행하게 한다(3 되면 무조건 실행), 3 패턴이 어떻게 되는 건지 기획과 협의 필요.
            if (bossPhase == BossPhase.Phase3)
            {
                // 여기서 시간을 흐르게 한다(1초후에 사라지게 하려구..??

                // 한번만 호출해야함
                if (isPhaseChangeQ == false)
                {
                    bossState = BossPatternState.Quickening;
                    //애니메이션을 실행한다
                    anim.SetTrigger("Quickening");
                    // 사운드 재생
                    soundManager.PlaySound(QuickeningSound);
                    soundManager.PlaySound(AttackVoiceSound3);
                    //이펙트를 실행한다 = 이벤트 함수로 할까ㅇㅇ
                    // 한번만 호출하지만 Update로 작동해야함. 할거 다 끝나고 true로 하기
                    //Quickening();
                    isPhaseChangeQ = true;
                }
            }

            // 끝나면 다시 상태를 Idle로 만든다. 
        }

        // 폭발 공격 이펙트 실행하는 이벤트 함수
        void ActivateEffect()
        {
            // 여기서 만든 폭발 이펙트 실행하기(알파이후에..)
            print("Effect : quick");
            GameObject quick = Instantiate<GameObject>(quickeningEffect, rayPos);
            Destroy(quick, 2.0f);
        }

        // 무기 교체함수(이벤트 함수임)
        private void WeaponChange()
        {
            // 2페이즈때 한번만
            // 낫을 끈다
            sickle.SetActive(false);
            // 칼과 총을 킨다
            gun.SetActive(true);
            blade.SetActive(true);
        }

        // 폭발 공격 함수 > 상태로 만들어서 주기적으로 호출한다
        private void UpdateQuickening()
        {
            // 폭발 공격 상태
            if (isQuickening == false)
            {
                // 폭발 공격 애니메이션을 실행한다(한번만 호출
                anim.SetTrigger("Quickening");
                // 사운드 재생
                soundManager.PlaySound(QuickeningSound);
                // 폭발 공격 이펙트를 실행한다(애니메이션의 이벤트 함수에서(상태 아래에 만들어놓음 Activeeffect
                // 폭발 공격 함수를 호출한다
                //Quickening();
                isQuickening = true;
                // 폭발 공격 상태를 true로 한다

            }

            // 만약 폭발 공격 상태일때 플레이어가 폭발 공격 범위 안에 있다면
            // 플레이어에게 날아가라고 신호를 준다(플레이어의 함수를 호출해도 되나? 아니면 부울 값을 넘겨주면 되나?
        }

        private void Quickening()
        {
            print("폭발공격중");
            // 만약 isQuickening = true 일때, 플레이어가 충돌했다면(overlapSpher.layer로 비교)
            //int layer = 1 << LayerMask.NameToLayer("Player");
            ////게르만의 중심에서 overlapSpher를 만든다
            //Collider[] cols = Physics.OverlapSphere(rayPos.position, quickeningDis, layer);
            //print("들어옴" + cols);
            //if (cols.Length > 0)
            //{
            //    print("ON");
            //    // 플레이어에서 이동하기로 함
            //    // 여기서 부울 값이나 상태 함수를 호출할 것.
            //    playerExplosion = true;
            //    //player.instance.inputHandler.kn_input = true;
            //    // 플레이어의 피격시 움직임 못하게 하는 부울 상태를 트루로 한다(플레이어에서?

            //}
            if (currDistance <= 5)
            {
                print("ON");
                //layerExplosion = true;
                playerLocomotion.playerExplosion = true; //합치고 바꾸기
            }
            //1초 후에 사라진다
            //상태가 idle 로 바뀐다
            //isQuickening = true 로 한다

        }
        
        private void UpdateIdle()           // 공격이 끝나면 idle 상태로 옴
        {
            isAvoid = false;
            isGehrmanAttack = false;
            isMove = false;

            // 만약 현재 거리가 공격 가능 거리보다 크다면
            if (currDistance > attackDistance)
            {
                // 현재 상태를 Move 로 변화시킨다
                bossState = BossPatternState.Move;
                // 애니메이션 재생
                anim.SetTrigger("Move");
            }
            // 만약 현재 거리가 공격 가능 범위보다 작거나 같다면, 그 거리가 피격 거리보다 크다면
            else if (currDistance <= attackDistance && currDistance >= hitDistance && bossState != BossPatternState.Die)
            {
                if (bossPhase == BossPhase.Phase1)
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
                }

                else if (bossPhase == BossPhase.Phase2)
                {
                    //그냥 하나만들자..
                    if (isSword1done == false)
                    {
                        Sword01();
                    }

                    else if (isSword1done == true)
                    {
                        Sword02();
                    }
                }

                if (bossPhase == BossPhase.Phase3)
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
                }

                print("ldel > Attack");
            }
            // 만약 현재 거리가 피격거리이고, 플레이어가 공격중이라면(교체)
            else if (currDistance <= hitDistance && animatorHandler.isAttack)
            {
                // 회피 상태로 변화시킨다
                bossState = BossPatternState.Avoid;
                // 애니메이션 호출
                anim.SetTrigger("Avoid");
                // 사운드 재생
                soundManager.PlaySound(MoveSound);
                // 회피 방향
                avoidDir = -transform.forward;
                // 회피 목적지
                targetPos = transform.position + avoidDir * 5;

            }
        }

# region 공격상태변환
        private void Sword01()
        {
            // 칼공격1으로 상태 변화시킨다
            bossState = BossPatternState.SwordCombo1;
            print("SwordCombo1");
            anim.SetBool("Leg", true);
        }

        private void Sword02()
        {
            // 칼공격2으로 상태 변화시킨다
            bossState = BossPatternState.SwordCombo2;
            print("SwordCombo2");
            FindMoveTargetPos();
            anim.SetBool("Leg", true);
        }

        private void SickelC3()
        {
            // 낫공격3으로 상태 변화시킨다
            bossState = BossPatternState.SickelCombo3;
            print("SickelCombo3");
            FindMoveTargetPos();
            anim.SetBool("Leg", true);
        }

        private void SickelC2()
        {
            // 낫공격2으로 상태 변화시킨다
            bossState = BossPatternState.SickelCombo2;
            print("SickelCombo2");
            FindMoveTargetPos();
            anim.SetBool("Leg", true);
        }

        private void SickelC1()
        {
            // 낫공격1으로 상태 변화시킨다
            bossState = BossPatternState.SickelCombo1;
            print("SickelCombo1");
            FindMoveTargetPos();
            anim.SetBool("Leg", true);
        }

        #endregion 


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
                // 일단 무조건 SickelCombo1으로 가게해서 완성한다
                SickelC1();

                print("Move > Attack");
            }

        }

        int avoidCount = 0;         // 순서대로 호출용

        private void UpdateAvoid()
        {
            // 시간을 잰다
            avoidcurTime += Time.deltaTime;
            // 회피 상태임
            isAvoid = true;
            //print("Avoid");
            avoidDir = -transform.forward;

            // 뒤로 회피
            if (isAvoid == true)
            {
                if (avoidCount == 0)
                {
                    isMove = true;
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
                        isMove = false;
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

                    isMove = true;

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
                        isMove = false;
                    }
                }

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
                if (isPhase2 == false)
                {
                    // 현재 보스 상태를 페이즈 변경 상태로 한다
                    bossState = BossPatternState.PhaseChange;
                    print("PhaseChange2로 상태 바꿈");
                    // 현재 페이즈를 2페이즈로 한다
                    bossPhase = BossPhase.Phase2;
                    print("Phase2로 상태 바꿈");
                }

                // 보스 공격 상태를 ?로 바꾼다
                //return;     // 바로가기?? 아마 아래에 있는 피깎이를 위로 올려야할듯? 무적상태가 따로 있나? 없나? 체크필요

                // 만약 현재 체력이 50보다 작다면 계속 들어옴
                if (bossHP.HP <= 5)
                {
                    // 부울 값을 만들어서 한번만 되게 하자
                    if (isPhase3 == false)
                    {
                        // 현재 보스 상태를 페이즈 변경 상태로 한다
                        bossState = BossPatternState.PhaseChange;
                        print("PhaseChange3로 상태 바꿈");
                        // 현재 페이즈를 3페이즈로 한다
                        bossPhase = BossPhase.Phase3;
                        print("Phase3로 상태 바꿈");
                        isPhase3 = true;
                    }

                    // 보스 공격 상태를 ?로 바꾼다
                    //return;
                }

                // 죽음상태로 만들기
                // 업데이트에서 그냥 부울만들어서 확인하고 있어서 여기 없어도 될거같음
                // 만약 현재 체력이 0이면
                //if (bossHP.HP <= 0)
                //{
                //    print("Hit : Die");
                //    // 보스 상태를 죽음으로 한다
                //    bossState = BossPatternState.Die;
                //    // 죽음 애니메이션을 실행한다
                //    anim.SetTrigger("Die");
                //    // 파티클을 켠다
                //    phase3Effect.SetActive(false);
                //}
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
            isGunHit = false;
            // hit2 up 애니메이션이 끝나면 idle 상태로 돌린다(이벤트 함수로)
        }

        // 애니메이션 이벤트 함수
        void AnimIdle()
        {
            curTime = 0;
            isQuickening = false;
            // Idle상태로 한다
            bossState = BossPatternState.Idle;
            attackSubState = AttackSubState.Attack1;
            print("AnimIdle 호출됨");
        }

        void AnimHitUp()
        {
            // Idle상태로 한다
            //bossState = BossPatternState.Idle;
            // hit up 애니메이션 켠다
            anim.SetTrigger("HitUp");
            isHitted2 = false;
            isHitted = false;
            print("AnimHitUP 호출됨");
        }

        void effectDestroy()
        {
            fires.FireDestroy();
        }

        void AnimDie()
        {
            // 죽음 애니메이션이 끝나면 나를 삭제해라
            // 삭제하기 말고 setactive false할까
            Destroy(gameObject);
            // 씬 넘어가는 // 죽음 상태라고 알려주기(SceneManager에서 받아가기)
            // UI 넣고 몇 초 후에 불리는 것으로 수정하기
            UI[0].SetActive(true);
            UI[1].SetActive(true);
            isGehrmanDie = true;
        }

        // 공격 상태 켜고 끄기(한번에 여러번 공격 되지 않도록 모든 공격 애니메이션에서 켜고 끄기
        void AttackTrue()
        {
            isGehrmanAttack = true;
        }

        void AttackFalse()
        {
            isGehrmanAttack = false;
        }
        

        // 발소리 이벤트 함수
        void PlayFootSoundRight1()
        {
            soundManager.PlaySound(FootSoundRight1);
        }

        void PlayFootSoundRight2()
        {
            soundManager.PlaySound(FootSoundRight2);
        }

        void PlayFootSoundLeft1()
        {
            soundManager.PlaySound(FootSoundLeft1);
        }

        void PlayFootSoundLeft2()
        {
            soundManager.PlaySound(FootSoundLeft2);
        }

        private void UpdateSickelCombo1()
        {

            // int로 값을 정해서 그 값에서만 실행되고, 다른 상태일때 다시 초기화되게 바꾸기

            curTime += Time.deltaTime;

            switch (attackSubState)
            {
                case AttackSubState.Attack1:
                    if (curTime > skTime_Sickel1_1)
                    {
                        //anim.SetBool("Leg", true);
                        print("SubState : Attack1");
                        isMove = true;
                        // 앞으로 이동하고 싶다
                        moveTargetPos.y = transform.position.y;
                        transform.position = Vector3.Lerp(transform.position, moveTargetPos, dashSpeed * Time.deltaTime);
                        

                        //print("moveTargetPos :" + moveTargetPos);
                        if (Vector3.Distance(transform.position, moveTargetPos) < 1f)
                        {
                            anim.SetBool("Leg", false);
                            // 서브상태를 액션 2로 바꾼다
                            attackSubState = AttackSubState.Attack2;
                            // 애니메이션 재생
                            anim.SetTrigger("Attack2");
                            // 사운드 재생
                            soundManager.PlaySound(SickleSound1);
                            soundManager.PlaySound(AttackVoiceSound1);
                            print("com1_attack_move");
                            // 현재 위치 기억하기
                            currPos = transform.position;
                            isMove = false;
                        }
                    }
                    break;
                case AttackSubState.Attack2:
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

                        print("목적지2 : " + attackMovePos);
                        anim.SetBool("Leg", true);
                        isMove = true;

                        attackMovePos.y = transform.position.y;
                        print("목적지3 : " + attackMovePos);
                        print("CurrPos:" + currPos);
                        transform.position = Vector3.Lerp(currPos, attackMovePos, 0.5f);

                        print("현재위치2 :" + transform.position);
                        print("위치차 : " + Vector3.Distance(transform.position, attackMovePos));
                        if (Vector3.Distance(transform.position, attackMovePos) < 5f)
                        {
                            anim.SetBool("Leg", false);
                            attackSubState = AttackSubState.Attack3;
                            // 애니메이션 재생
                            anim.SetTrigger("Attack1");
                            // 사운드 재생
                            soundManager.PlaySound(SickleSound2);
                            isMove = false;
                        }


                        //transform.position += attackdir * 50 * Time.deltaTime;
                    }
                    break;
                case AttackSubState.Attack3:
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
                        attackSubState = AttackSubState.Attack1;
                    }
                    break;
            }
        }

        private void UpdateSickelCombo2()
        {
            curTime += Time.deltaTime;
            switch (attackSubState)
            {
                case AttackSubState.Attack1:
                    if (curTime > skTime_Sickel1_1)
                    {
                        //print("C2A1");
                        //anim.SetBool("Leg", true);
                        isMove = true;
                        // 앞으로 이동하고 싶다
                        moveTargetPos.y = transform.position.y;
                        transform.position = Vector3.Lerp(transform.position, moveTargetPos, attackMoveSpeed * Time.deltaTime);
                        if (Vector3.Distance(transform.position, moveTargetPos) < 0.1f)
                        {

                            // 서브상태를 액션 2로 바꾼다
                            attackSubState = AttackSubState.Attack2;
                            // 애니메이션 재생
                            anim.SetTrigger("Attack3");
                            // 사운드 재생
                            soundManager.PlaySound(SickleSound1);
                            soundManager.PlaySound(AttackVoiceSound2);
                            anim.SetBool("Leg", false);
                            print("com2_attack_move");
                            isMove = false;
                        }

                    }
                    break;
                case AttackSubState.Attack2:
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
                        isMove = true;
                        // 앞으로 이동하고 싶다
                        moveTargetPos.y = transform.position.y;
                        transform.position = Vector3.Lerp(transform.position, moveTargetPos, attackMoveSpeed * Time.deltaTime);
                        if (Vector3.Distance(transform.position, moveTargetPos) < 0.1f)
                        {

                            // 서브상태를 액션 3로 바꾼다
                            attackSubState = AttackSubState.Attack3;
                            // 애니메이션 재생
                            anim.SetTrigger("Attack4");
                            // 사운드 재생
                            soundManager.PlaySound(SickleSound2);
                            anim.SetBool("Leg", false);
                            print("com2_attack_move");
                            isMove = false;
                        }

                    }
                    break;
                case AttackSubState.Attack3:
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
                        attackSubState = AttackSubState.Attack1;

                    }
                    break;
            }
        }

        private void UpdateSickelCombo3()
        {

            curTime += Time.deltaTime;
            switch (attackSubState)
            {
                case AttackSubState.Attack1:
                    if (curTime > skTime_Sickel1_1)
                    {
                        // 앞으로 이동하면서
                        // 이동할때 서서오니까 이상한데...
                        // 부울을 만들어서 켜고 끄자 굿
                        isMove = true;
                        //anim.SetBool("Leg", true);
                        // 360 낫을 휘두르는 애니메이션을 한다
                        moveTargetPos.y = transform.position.y;
                        transform.position = Vector3.Lerp(transform.position, moveTargetPos, attackMoveSpeed * Time.deltaTime);
                        if (Vector3.Distance(transform.position, moveTargetPos) < 0.1f)
                        {
                            // 서브상태를 액션 2로 바꾼다
                            attackSubState = AttackSubState.Attack2;
                            // 애니메이션 재생
                            anim.SetTrigger("Attack6");
                            // 사운드 재생
                            soundManager.PlaySound(SickleSound1);
                            soundManager.PlaySound(AttackVoiceSound1);
                            anim.SetBool("Leg", false);
                            print("com3_attack_move");
                            isMove = false;
                        }
                    }
                    break;
                case AttackSubState.Attack2:
                    if (curTime > skTime_Sickel1_2)
                    {
                        // 서브상태를 액션 3로 바꾼다
                        attackSubState = AttackSubState.Attack3;
                        // 애니메이션 재생
                        // 낫이 360 도 돌아가는 게 있어야하는데...없네
                        print("com3_attack_Rdu");
                    }
                    break;
                case AttackSubState.Attack3:
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
                        attackSubState = AttackSubState.Attack1;
                        print("3 > idle");
                    }
                    break;
            }
        }

        // 가짜임. 이벤트 함수로 다이 애니메이션이 끝나면 호출
        private void UpdateDie()
        {
            // 만약 hp가 0이되면 Die 함수가 호출된다(hit에서)
            // 죽음 상태라고 알려주기(SceneManager에서 받아가기)
            //isGehrmanDie = true;
            phase3Effect.SetActive(false);

        }



        // 충돌 체크
        private void OnTriggerEnter(Collider other)
        {
            // 만약 플레이어가 공격 상태이고, 플레이어의 무기와 충돌했을때
            if (isHitted == false && isHitted2 == false && animatorHandler.isAttack && other.gameObject.CompareTag("p_Weapon") || isGunHit)
            {
                // 보스 피격 상태로 전환
                bossState = BossPatternState.Hit;
                print("hittt");
                LoadBloodEffect(other);
                isGunHit = false;
                soundManager.PlaySound(HitSound); //사운드

            }

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
}