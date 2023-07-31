using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace bloodborne
{
    public class bossAI : MonoBehaviour
    {
        PlayerAnimatorManager playerAnimatorManager;
        PlayerLocomotion playerLocomotion;
        PlayerStats playerStats;

        #region Boolean Values
        public bool moonpresenceAttack;
        public bool isDie;
        public bool isFinished;
        public bool isBulletHit;
        private bool isReact;
        #endregion

        #region TransformComponent
        private Transform playerPos;
        private Transform thisPos;
        #endregion

        private Animator animator;
        private NavMeshAgent agent;
        private AudioSource audioSource;
        [Header("DieAudioClips")]
        public AudioClip[] moonDieAudioClips;
        RFX4_CameraShake cameraShake;

        #region GameObject Values
        private GameObject target;
        private GameObject bloodEffect;
        private GameObject dieEffect;
        #endregion

        private string reactAnimation;
        private int hitCount = 0;

        [Header("SpecialPattern")]
        public GameObject[] SpecialPattern1;
        public GameObject[] SpecialPattern2;
        public GameObject camShake;
        public float distanceToMove = 10f;

        [Header("Die")]
        public GameObject bloodRain;
        public GameObject[] DieCanvas;
        // post Processing Values
        [Header("Post Processing Volumes")]
        public PostProcessVolume postProcessVolume;
        private LensDistortion lensDistortion;

        [Header("SoundFX")]
        public string[] attackSound;
        public string[] damageSound;

        #region AttackPatternBoolValues
        public bool isSpecialPattern1Active = false;
        private bool isSpecialPattern1InProgress = false;
        public bool playerHP1;
        private bool isSpecialPattern2Active = false;
        private bool isSpecialPattern2InProgress = false;
        private bool pattern1;
        static public bool groundHit;
        #endregion

        #region StateMachine
        // StateMachine
        public enum StateMachine
        {
            IdleState,
            MoveState,
            AttackState,
            AttackDelayState,
            ReactState,
            DieState
        }

        // Set IdleState
        public StateMachine stateMachine = StateMachine.IdleState;

        // Distance playr & moonpresence
        private float MoveDistance;

        // Delay for Updating StateMachine
        private WaitForSeconds UpdateStateMachineDelay;


        // Check Attack Inprogress
        static public bool attackInProgress;

        // AttackSubStateMachine
        public enum AttackSubStateMachine
        {
            AttackDelay,
            Pattern1,
            Pattern2,
            Pattern3,
            Pattern4,
            Pattern5,
            Pattern6,
            SpecialPattern1
        }
        public AttackSubStateMachine attackSubStateMachine = AttackSubStateMachine.AttackDelay;
        #endregion

        #region Public Values
        // Attack Values
        [Header("Attack Values")]
        public float AttackRange;
        public float AttackDamage;

        [Header("Hp Values")]
        public float maxHp;
        public float curHp;
        private float curHpPercentage = 1f;
        public Slider sliderHp;
        #endregion

        private void Start()
        {
            playerStats = FindObjectOfType<PlayerStats>();
            playerLocomotion = FindObjectOfType<PlayerLocomotion>();
            playerAnimatorManager = FindObjectOfType<PlayerAnimatorManager>();
            audioSource = GetComponent<AudioSource>();
        }

        private void Awake()
        {
            sliderHp.maxValue = maxHp;

            cameraShake = GetComponent<RFX4_CameraShake>();

            // Get volume From Post Processing
            postProcessVolume.profile.TryGetSettings(out lensDistortion);

            if (lensDistortion == null)
            {
                // LensDistortion
                lensDistortion = postProcessVolume.profile.AddSettings<LensDistortion>();
            }

            // Set Hp
            curHp = maxHp;

            // Get Animation Component
            animator = GetComponent<Animator>();

            // Get NavmeshAgent Component
            agent = GetComponent<NavMeshAgent>();

            // Get Player Transform Component
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerPos = player.GetComponent<Transform>();

            // Get this obj Transform Component
            thisPos = GetComponent<Transform>();

            // Set UpdateStateMachineDelay
            UpdateStateMachineDelay = new WaitForSeconds(0.2f);

            // Load Effects
            bloodEffect = Resources.Load<GameObject>("DAX_Blood_Spray_00(Fade_2s)");
            dieEffect = Resources.Load<GameObject>("DieEffect");
        }

        private void OnEnable()
        {
            // Call Couroutines
            StartCoroutine(UpdateStateMachine());
            StartCoroutine(ChangeStateMachine());
        }

        // Updating StateMachine Before DieState
        private IEnumerator UpdateStateMachine()
        {
            while (!isDie)
            {
                // Update Distance between player & thisobj
                MoveDistance = Vector3.Distance(playerPos.position, thisPos.position);

                // Die state -> Stop Coroutine
                if (stateMachine == StateMachine.DieState)
                    yield break;

                #region special pattern 1
                // special pattern1 Active Condition
                if (curHpPercentage <= 0.6f && !isSpecialPattern1Active)
                {
                    animator.StopPlayback();

                    isSpecialPattern1Active = true;

                    attackInProgress = true;

                    // Move Object away from player
                    StartCoroutine(MoveObjectAwayFromPlayer());

                    AnimatorTrigger("SpecialPattern1Start");
                }

                if (isSpecialPattern1InProgress)
                {
                    transform.LookAt(playerPos.position);
                }
                #endregion

                #region special pattern2

                // special pattern1 Active Condition
                if (curHpPercentage <= 0.4f && !isSpecialPattern2Active)
                {
                    animator.StopPlayback();

                    isSpecialPattern2Active = true;

                    attackInProgress = true;

                    AnimatorTrigger("SpecialPattern2Start");
                }
                #endregion

                yield return UpdateStateMachineDelay;
            }
        }

        // per Action for StateMachine
        private IEnumerator ChangeStateMachine()
        {
            while (!isDie)
            {
                // Update per UpdateStateMachineDelay
                yield return UpdateStateMachineDelay;

                switch (stateMachine)
                {
                    case StateMachine.IdleState:
                        UpdateIdleState();
                        break;

                    case StateMachine.MoveState:
                        UpdateMoveState();
                        break;

                    case StateMachine.AttackState:
                        UpdateAttackState();
                        break;

                    case StateMachine.ReactState:
                        break;

                    case StateMachine.DieState:
                        break;
                }
            }
        }

        #region UpdateStateFunctions
        //Update Idle
        private void UpdateIdleState()
        {
            // Find Player
            target = GameObject.Find("Player");

            if (target != null)
            {
                // Switch State
                stateMachine = StateMachine.MoveState;

                // Move Animation Trigger
                // animator.SetTrigger("Move");
                animator.SetTrigger("Move");

                // Agent Move
                agent.isStopped = false;
            }
        }

        // Update Move
        private void UpdateMoveState()
        {
            // Set Destination -> Target
            agent.SetDestination(target.transform.position);

            // Move Close State Change to Attack
            if (MoveDistance < AttackRange)
            {
                stateMachine = StateMachine.AttackState;

                //animator.SetTrigger("AttackState");
                AnimatorTrigger("AttackState");
                // Agent Stop
                agent.isStopped = true;
            }
        }

        // Update Attack
        private void UpdateAttackState()
        {
            // Not in Attack Progress
            if (!attackInProgress && !isSpecialPattern1InProgress)
            {
                // Trigger On
                AnimatorTrigger("AttackState");

                // Random Attack Index 
                int randomIndex = Random.Range((int)AttackSubStateMachine.Pattern1, (int)AttackSubStateMachine.Pattern6 + 1);

                attackSubStateMachine = (AttackSubStateMachine)randomIndex;

                switch (attackSubStateMachine)
                {
                    case AttackSubStateMachine.Pattern1:
                        AnimatorTrigger("Pattern1");
                        attackInProgress = true;
                        break;

                    case AttackSubStateMachine.Pattern2:
                        AnimatorTrigger("Pattern2");
                        attackInProgress = true;
                        break;

                    case AttackSubStateMachine.Pattern3:
                        AnimatorTrigger("Pattern3");
                        attackInProgress = true;
                        break;

                    case AttackSubStateMachine.Pattern4:
                        AnimatorTrigger("Pattern4");
                        attackInProgress = true;
                        break;

                    case AttackSubStateMachine.Pattern5:
                        AnimatorTrigger("Pattern5");
                        attackInProgress = true;
                        break;

                    case AttackSubStateMachine.Pattern6:
                        AnimatorTrigger("Pattern6");
                        playerLocomotion.playerExplosion = true;
                        attackInProgress = true;
                        break;
                }
            }
        }
        #endregion

        private void Update()
        {
            curHpPercentage = curHp / maxHp;

            sliderHp.value = curHp;
        }

        #region AnimEventFun
        // Ground Hit
        private void GroundHitTrue()
        {
            groundHit = true;
            moonpresenceAttack = true;
        }

        private void GroundHitFalse()
        {
            groundHit = false;
            moonpresenceAttack = false;
        }

        // Special Pattern1
        private IEnumerator SpecialPattern1AnimStart()
        {
            AudioManager2.instance.PlaySFX("Moon_Special2");

            moonpresenceAttack = false;

            // Set Animator Speed 50%
            animator.speed = 0.5f;

            // Stop Agent
            agent.isStopped = true;

            // Set true value -> Dont React
            isSpecialPattern1InProgress = true;

            yield return new WaitForSeconds(2f);

            // Set player hp -> 1
            playerHP1 = true;

            // lensDistortion on
            lensDistortion.active = true;

            // Load Prefabs
            SpecialPattern1[1].SetActive(true);
            SpecialPattern1[2].SetActive(true);

            playerStats.SetCurrentHpOne();

            yield return new WaitForSeconds(2f);

            // lensDistortion off
            lensDistortion.active = false;

            SpecialPattern1[1].SetActive(false);
            SpecialPattern1[2].SetActive(false);

            // Aniamtion Trigger
            AnimatorTrigger("SpecialPattern1Finish");

            // Move Agent
            agent.isStopped = false;

            // React true 
            isSpecialPattern1InProgress = false;

            // EyeOff
            SpecialPattern1[0].SetActive(false);

            // Reset Animator Speed 100%
            animator.speed = 1f;
        }

        private IEnumerator MoveObjectAwayFromPlayer()
        {

            Vector3 originalPosition = thisPos.position;
            Vector3 targetPosition = playerPos.position - (playerPos.forward * distanceToMove);

            float elapsedTime = 0f;
            float moveTime = 1f; // 1초 동안 이동

            while (elapsedTime < moveTime)
            {
                thisPos.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / moveTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 움직인 후의 위치가 targetPosition이 아닐 수 있으므로 마지막으로 목표 위치로 정확하게 설정
            thisPos.position = targetPosition;
        }
        // Special Pattern2

        IEnumerator SpecialPattern2AnimStart()
        {
            moonpresenceAttack = false;

            animator.speed = 0.5f;

            // Stop Agent
            agent.isStopped = true;

            // Set true value -> Dont React
            isSpecialPattern2InProgress = true;

            yield return new WaitForSeconds(2f);

            SpecialPattern2[0].SetActive(true);

            if (MoveDistance < 10)
            {
                playerLocomotion.canDrinkPotion = false;
            }

            yield return new WaitForSeconds(3f);

            animator.speed = 1;

            isSpecialPattern2InProgress = false;

            agent.isStopped = false;

            yield return new WaitForSeconds(2f);

            SpecialPattern2[0].SetActive(false);
        }

        private void SpecialPattern2AnimEffectOn()
        {
            AudioManager2.instance.PlaySFX("Moon_special");
            SpecialPattern2[0].SetActive(true);
        }

        private void SpecialPattern1AnimEyeOn()
        {
            SpecialPattern1[0].SetActive(true);
            StartCoroutine(SoundDelay());
        }
        IEnumerator SoundDelay()
        {
            yield return new WaitForSeconds(1);
            AudioManager2.instance.PlaySFX("Moon_Whip2");
        }

        // Animation Trigger Function
        private void AnimatorTrigger(string TriggerName)
        {
            animator.SetTrigger(TriggerName);

            int randomValue = Random.Range(0, 2);
            AudioManager2.instance.PlaySFX("Moon_Whip");

            if (randomValue == 0)
            {
                AudioManager2.instance.PlaySFX("Moon_Attack1");
            }
            else
            {
                AudioManager2.instance.PlaySFX("Moon_Attack2");
            }
        }

        // Attack Finish Animation Event
        private void StateAnimFinishFunction()
        {
            // Reset
            attackInProgress = false;

            // Out of Attack Range
            if (MoveDistance > AttackRange)
            {
                stateMachine = StateMachine.MoveState;

                // Move Animation Trigger
                animator.SetTrigger("Move");

                // Agent Move
                agent.isStopped = false;
            }
            // In Attack Range
            else
            {
                // Agent Move
                agent.isStopped = true;

                stateMachine = StateMachine.AttackState;
            }
        }

        // DieAnimFinish
        void DieStateFisnish()
        {
            DieCanvas[0].SetActive(true);
            DieCanvas[1].SetActive(true);
            bloodRain.SetActive(true);

            Destroy(gameObject);
            isFinished = true;
        }

        // AttackAnimFinish
        public void AttackStateFinish()
        {
            StateAnimFinishFunction();
        }

        // AttackDelayAnimFinish
        public void AttackDelayStateFinished()
        {
            StateAnimFinishFunction();
        }

        // ReactStateAnimFinish
        internal void ReactStateFinished()
        {
            StateAnimFinishFunction();
        }
        #endregion

        #region ReactProcess

        // Calculate Angle From Other with Pos
        private float GetAngle(Vector3 from, Vector3 to)
        {
            return Quaternion.FromToRotation(transform.forward, to - from).eulerAngles.y;
        }

        void attackTrue()
        {
            moonpresenceAttack = true;
        }

        void attackFalse()
        {
            moonpresenceAttack = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "p_Weapon" && playerAnimatorManager.isAttack && !isReact
                && !isSpecialPattern1InProgress && !isSpecialPattern2InProgress && !isDie
                || isBulletHit)
            {

                if (curHp > 0)
                {
                    curHp -= AttackDamage;

                    int randomValue = Random.Range(0, 2);

                    if (randomValue == 0)
                    {
                        AudioManager2.instance.PlaySFX("Moon_Hit");    //사운드 추가
                    }
                    else
                    {
                        AudioManager2.instance.PlaySFX("Moon_Hit2");     //사운드 추가
                    }

                    if (!camShake.activeSelf)
                    {
                        camShake.SetActive(true);
                        StartCoroutine(camShakeOff());
                    }

                    // Increase Hit Count
                    if (hitCount < 3)
                    {
                        hitCount++;
                    }

                    // Set React True
                    isReact = true;

                    // Make Array Child Collider
                    Collider[] childColliders = GetComponentsInChildren<Collider>();

                    // Collider -> is trigger true
                    foreach (Collider collider in childColliders)
                    {
                        collider.isTrigger = true;
                    }

                    // Reset React
                    StartCoroutine(ReactDelayCoroutine());

                    // Get Angle from Distance
                    float directionHitFrom = (GetAngle(transform.position, playerPos.transform.position));

                    // Calculate Angle -> Call Animation
                    WhichDirectionDamageCameFrom(directionHitFrom);

                    // Set Trigger React Animation
                    AnimatorTrigger(reactAnimation);

                    // Load Blood Effect Particle System
                    LoadBloodEffect(other);

                    isBulletHit = false;
                }

                // ReactState Process
                // Hp, Die Animation
                else
                {

                    isDie = true;

                    print("test");

                    // Load Die Effect
                    LoadDieEffect();

                    // Agent Stop
                    agent.isStopped = true;

                    // Die State
                    stateMachine = StateMachine.DieState;

                    // Die Anim
                    AnimatorTrigger("Die");

                    return;
                }
            }
        }

        private void LoadBloodEffect(Collider collider)
        {
            Vector3 pos = collider.ClosestPointOnBounds(transform.position);
            Vector3 normal = transform.position - collider.transform.position;

            Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, normal);
            GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot);
            Destroy(blood, 1.0f);
        }


        protected virtual void WhichDirectionDamageCameFrom(float direction)
        {
            // Front
            switch (hitCount)
            {
                case 0:
                    ReactPerAngle(direction);
                    break;
                case 1:
                    ReactPerAngle(direction);
                    break;
                case 2:
                    ReactPerAngle(direction);
                    break;
            }
            return;
        }

        // Load Blood Effect From Resources

        private void LoadDieEffect()
        {
            GameObject DieEffect = Instantiate<GameObject>(dieEffect, thisPos);
        }

        // Delay Coroutine -> React Delay
        private IEnumerator ReactDelayCoroutine()
        {
            // Prevent Double Checking Collider
            yield return new WaitForSeconds(0.3f);

            // Reset Hit Count
            if (hitCount == 2)
                hitCount = 0;

            Collider[] childColliders = GetComponentsInChildren<Collider>();

            foreach (Collider collider in childColliders)
            {
                collider.isTrigger = false;
            }

            // Reset isReact
            isReact = false;
        }

        void ReactPerAngle(float direction)
        {
            if (direction >= 0 && direction < 90)
            {
                reactAnimation = "ReactFront" + hitCount;
            }
            else if (direction >= 270 && direction < 360)
            {
                reactAnimation = "ReactFront" + hitCount;
            }
            // Back
            else if (direction >= 90 && direction < 270)
            {
                reactAnimation = "ReactBack" + hitCount;
            }
        }

        IEnumerator camShakeOff()
        {
            yield return new WaitForSeconds(1);
            camShake.SetActive(false);
        }

        void moonFirstDieSound()
        {
            audioSource.clip = moonDieAudioClips[0];
            audioSource.Play();
        }

        void moonSecondDieSound()
        {
            audioSource.clip = moonDieAudioClips[1];
            audioSource.Play();
        }

        void moonThirdDieSound()
        {
            audioSource.clip = moonDieAudioClips[2];
            audioSource.Play();
        }

        #endregion
    }
}