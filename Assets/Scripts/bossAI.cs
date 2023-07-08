using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    private float curTime;

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
    private bool attackInProgress;

    // AttackSubStateMachine
    public enum AttackSubStateMachine
    {
        AttackDelay,
        Pattern1,
        Pattern2,
        Pattern3,
        Pattern4,
        Pattern5
    }
    public AttackSubStateMachine attackSubStateMachine = AttackSubStateMachine.AttackDelay;
    #endregion

    // Attack Values
    [Header("Attack Values")]
    public float AttackRange;
    public float AttackDamage;

    [Header("Hp Values")]
    public float maxHp;
    private float _curHp;

    public float curHp
    {
        get { return _curHp; }
        set { _curHp = value; }
    }

    private void Awake()
    {
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

            animator.SetTrigger("AttackState");
            // Agent Stop
            agent.isStopped = true;
        }
    }

    private void UpdateAttackState()
    {
        // Not in Attack Progress
        if (!attackInProgress)
        {
            // Trigger On
            AnimatorTrigger("AttackState");

            // Random Attack Index 
            int randomIndex = Random.Range((int)AttackSubStateMachine.Pattern1, (int)AttackSubStateMachine.Pattern5 + 1);

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
            }
        }
    }
    #endregion

    private void AnimatorTrigger(string TriggerName)
    {
        animator.SetTrigger(TriggerName);
    }

    #region AnimEventFun
    private void StateAnimFinishFunc()
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
        StateAnimFinishFunc();
    }

    // AttackDelayAnimFinish
    public void AttackDelayStateFinished()
    {
        StateAnimFinishFunc();
    }

    // ReactStateAnimFinish
    internal void ReactStateFinished()
    {
        StateAnimFinishFunc();
    }
    #endregion

    internal void ReactStateControll()
    {
        if (stateMachine == StateMachine.DieState)
        {
            return;
        }
        agent.isStopped = true;

        // Reduction
        curHp -= AttackDamage;

        // Hp = 0
        if (curHp <= 0)
        {
            // Die State
            stateMachine = StateMachine.DieState;

            // Die Anim

        }

        else
        {
            // React State
            stateMachine = StateMachine.ReactState;

            // React Anim

        }
    }
}
