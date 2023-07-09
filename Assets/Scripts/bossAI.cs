using Retro.ThirdPersonCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

public class bossAI : MonoBehaviour
{
    // TransformConponents
    #region TransformComponent
    private Transform playerPos;
    private Transform thisPos;
    #endregion

    // Components
    private Animator animator;
    private NavMeshAgent agent;

    // target obj -> Player
    private GameObject target;
    private string reactAnimation;
    private GameObject bloodEffect;
    private bool isReact;
    private int hitCount = 0;
    private bool isSpecialPattern1Active = false;
    private bool isSpecialPattern1InProgress = false;
    private float curHpPercentage = 1f;
    [Header("SpecialPattern1 Eye")]
    public GameObject[] EyeLights;

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

    // Check Die
    private bool isDie;

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
        Pattern7
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
    #endregion

    private void Awake()
    {
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
            // Update curHp
            curHpPercentage = curHp / maxHp;

            // Update Distance between player & thisobj
            MoveDistance = Vector3.Distance(playerPos.position, thisPos.position);

            // Die state -> Stop Coroutine
            if (stateMachine == StateMachine.DieState)
                yield break;

            // Pattern 6 Active Condition
            if (curHpPercentage <= 0.4f && !isSpecialPattern1Active)
            {
                // Stop Animation
                animator.StopPlayback();

                isSpecialPattern1Active = true;

                attackInProgress = true;
                
                // Pattern 6
                AnimatorTrigger("SpecialPattern1Start");
            }

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
                    attackInProgress = true;
                    break;
            }
        }
    }
    #endregion


    #region AnimEventFun

    // Special Pattern1
    private IEnumerator SpecialPattern1AnimStart()
    {
        // Set Animator Speed 50%
        animator.speed = 0.5f;

        // Stop Agent
        agent.isStopped = true;

        // Set true value -> Dont React
        isSpecialPattern1InProgress = true;
        
        yield return new WaitForSeconds(0.5f);

        // Load Prefabs
        EyeLights[1].SetActive(true);

        yield return new WaitForSeconds(2.5f);

        EyeLights[1].SetActive(false);

        // Aniamtion Trigger
        AnimatorTrigger("SpecialPattern1Finish");

        // Move Agent
        agent.isStopped = false;

        // React true 
        isSpecialPattern1InProgress = false;

        // EyeOff
        EyeLights[0].SetActive(false);

        // Reset Animator Speed 100%
        animator.speed = 1f;
    }

    private void SpecialPattern1AnimEyeOn()
    {
        EyeLights[0].SetActive(true);
    }

    // Animation Trigger Function
    private void AnimatorTrigger(string TriggerName)
    {
        animator.SetTrigger(TriggerName);
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
            stateMachine = StateMachine.AttackState;
        }
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


    private void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag == ("p_Weapon") && Combat.P_Attack 
            && !isReact && !isSpecialPattern1InProgress)
        {
            // Reduction
            curHp -= AttackDamage;

            // ReactState Process
            // Hp, Die Animation
            if (curHp <= 0)
            {
                // Agent Stop
                agent.isStopped = true;

                // Die State
                stateMachine = StateMachine.DieState;

                // Die Anim
                AnimatorTrigger("Die");
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
            LoadBloodEffect(coll);
        }
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
    private void LoadBloodEffect(Collision coll)
    {
        Vector3 pos = coll.contacts[0].point;
        Vector3 _normal = coll.contacts[0].normal;

        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);
        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot);
        Destroy(blood, 1.0f);
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
    #endregion
}
