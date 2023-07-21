using cam;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TPSChraracterController : CharacterManager
{
    public static TPSChraracterController instance;
    private void Awake()
    {
        instance = this;
    }
    public bool playerLock = false;
    [SerializeField]
    private Transform characterBody;
    [SerializeField]
    private Transform cameraArm;

    // 사운드
    //public AudioClip[] Audioclip;
    //AudioSource soundSource;


    // public Transform LockOnTransform;
    private CharacterController cc;

    Animator animator;

    public LayerMask enemyLayer;

    public float attackRadius = 1f;   // 공격 범위
    public float attackDamage = 10f;  // 공격력

    private float Dist;
    [SerializeField]
    [Header("Lock-On Zoom")]
    public float zoomSpeed = 10;
    public float minDist = 5;
    public float maxDist = 10;

    bool Move_B = false;
    bool Move_L = false;
    bool Move_R = false;
    bool isMove = false;
    bool Dash_atk = false;

    public bool isLockON;
    public string playerTag = "Player";
    public string bossTag = "Boss";
    private Transform Player;
    private Transform Boss;

    private int attackCount = 0;

    Vector3 dir;
    [Header("PlayerSpeed")]
    public float moveSpeed = 3f;


    // player
    public Transform player;

    // boss
    public Transform boss;

    // Camera Speed
    public float cameraRotSpeed = 20f;

    // Camera position Offset
    public Vector3 offset;

    // Camera LockOn
    public bool isLockedOn;

    // Ground Layer
    public LayerMask groundLayer;

    // collisionOffset
    public float collisionOffset = 2.2f;

    CameraHandler cameraHandler;
    // Start is called before the first frame update
    void Start()
    {
        cameraHandler = CameraHandler.instance;

        playerLock = false;
        isAttack = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();

       // soundSource = gameObject.GetComponent<AudioSource>();

        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            Player = playerObject.transform;
        }

        // Boss 오브젝트 찾기
        GameObject bossObject = GameObject.FindGameObjectWithTag(bossTag);
        if (bossObject != null)
        {
            Boss = bossObject.transform;
        }
    }
    private void FixedUpdate()
    {
        float delta = Time.deltaTime;
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        cameraHandler.FollowTarget(delta);
        cameraHandler.HandleCameraRotation(delta, mouseDelta);
    }

    float curTime = 0;
   public  float lockOnTime = 0.5f;
    void Update()
    {
        //LockCheck();
        Move();
        print("isAttack: " + isAttack);
       // LookAround();
        LockOn();
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
        B_Step();
        R_Step();
        L_Step();
        Dash_Atk();
    }



    public bool isAttack = false;

    void isAttackTrue()
    {
        isAttack = true;
    }
    void isAttackFalse()
    {
        isAttack = false;
    }

    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraArm.rotation.eulerAngles;

        float x = camAngle.x - mouseDelta.y;
        if (x < 180f)
        {
            x = Mathf.Clamp(x, 0f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }

        cameraArm.rotation = Quaternion.Euler(camAngle.x - mouseDelta.y, camAngle.y + mouseDelta.x, camAngle.z);

    }

    private void Move()
    {
        if (playerLock)
        {
            curTime += Time.deltaTime;
            if (curTime > lockOnTime)
            {
                playerLock = false;
                curTime = 0;
            }
            return;
        }
        else if (!playerLock) 
        {
            // 플레이어의 움직임을 입력받는 부분
            Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            bool isMove = moveInput.magnitude != 0;
            animator.SetBool("isMove", isMove);
            animator.SetBool("Lock", playerLock);
            //animator.SetFloat("MoveSpeed", moveInput.magnitude);
            if (isMove && !isAttack)
            {
                Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
                Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
                Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

                characterBody.forward = moveDir;
                transform.position += moveDir * Time.deltaTime * 1.5f;
                cc.Move(moveDir * Time.deltaTime * moveSpeed + Physics.gravity * Time.deltaTime);
            }
        }
        else
        {
            // 플레이어가 멈춘 상태라면 움직임을 0으로 초기화
            characterBody.forward = Vector3.zero;
        }
    }
    public float playerRotationSpeed = 10f;

    private void LockOn()
    {
        GameObject lockOnObject = GameObject.FindGameObjectWithTag("Boss");  // LockOnTarget 태그로 오브젝트 찾기

        if (Boss != null)
        {
            Transform lockOnTransform = lockOnObject.transform;

            //마우스 오른쪽 버튼을 눌렀을 때 
            if (Input.GetButton("Fire2"))
            {
                isLockON = true;
                //카메라와 player를 enemy에게 lookat 한다

                cameraArm.transform.LookAt(Boss);

                //거리조절
                float distance = Vector3.Distance(Player.position, Boss.position);

                // 카메라와 Boss 사이의 거리가 최소 거리보다 작을 경우
                if (distance < minDist)
                {
                    //// 카메라 위치를 최소 거리만큼 뒤로 조정합니다.
                    //Vector3 offset = transform.position - Boss.position;
                    //offset = offset.normalized * minDist;
                    //transform.position = Boss.position + offset;
                    // 카메라 위치를 최소 거리만큼 뒤로 조정합니다.
                    Vector3 direction = transform.position - Boss.position;
                    direction = direction.normalized * minDist;
                    transform.position = Boss.position + direction;
                }
                else if (distance > maxDist)
                {
                    // 카메라 위치를 최대 거리만큼 앞으로 조정합니다.
                    Vector3 direction = transform.position - Boss.position;
                    direction = direction.normalized * maxDist;
                    transform.position = Boss.position + direction;
                }
                characterBody.LookAt(Boss);
                //카메라 줌인아웃
                //float distance = Vector3.Distance(Player.position, Boss.position);

                float zoomLevel = Mathf.InverseLerp(minDist, maxDist, distance);
                // Field of view View 바꿈
                float targetFOV = Mathf.Lerp(75f, 60f, zoomLevel);
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);



                //transform.LookAt(Boss);

            }
            else
            {
                isLockON = false;
            }

        }
    }


    //private Vector3 GetCenterPoint()
    //{
    //    // 두 오브젝트의 중심점 계산
    //    Vector3 centerPoint = (Player.position + Boss.position) / 2f;

    //    return centerPoint;
    //}
    private bool isAttacking = false;

    private void Attack()
    {
        if (isAttacking)
            return;

        isAttacking = true;

        animator.SetTrigger("isAttack");
        Debug.Log("dfd");


        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
        bool hitBoss = false;
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Boss"))
            {
                // 적에게 데미지를 입히는 코드를 작성하세요.
                Enemy enemyHealth = hitCollider.GetComponent<Enemy>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(attackDamage);
                    //soundSource.clip = Audioclip[5];
                    //soundSource.PlayOneShot(Audioclip[5]);
                }
            }
        }
        if (!hitBoss)
        {
            //soundSource.clip = Audioclip[1];
            //soundSource.PlayOneShot(Audioclip[1]);
        }
        isAttacking = false;
    }




    private bool isSKeyPressed = false; // 's' 키가 눌렸는지 여부를 저장하는 변수

    private void B_Step()
    {
        if (Input.GetKey(KeyCode.S))
        {
            isSKeyPressed = true;

        }
        else
        {
            isSKeyPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isSKeyPressed && !Move_B)
        {
            // Move_B = true;
            animator.SetTrigger("Move_B");
            //soundSource.clip = Audioclip[2];
            //soundSource.PlayOneShot(Audioclip[2]);
        }

    }

    private void L_Step()
    {
        if (Input.GetKey(KeyCode.A))
        {
            isSKeyPressed = true;
        }
        else
        {
            isSKeyPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isSKeyPressed && !Move_L)
        {
            //Move_L = true;
            animator.SetTrigger("Move_L");
            //soundSource.clip = Audioclip[2];
            //soundSource.PlayOneShot(Audioclip[2]);
        }
    }

    private void R_Step()
    {
        if (Input.GetKey(KeyCode.D))
        {
            isSKeyPressed = true;
        }
        else
        {
            isSKeyPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isSKeyPressed && !Move_R)
        {
            //Move_R = true;
            animator.SetTrigger("Move_R");
            //soundSource.clip = Audioclip[2];
            //soundSource.PlayOneShot(Audioclip[2]);

        }

    }


    float dashAtkDuration = 1.0f;
    private void Dash_Atk()
    {
        if (Input.GetMouseButton(1) && Input.GetKey(KeyCode.W) && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ExecuteDashAtk());
        }
    }

    private IEnumerator ExecuteDashAtk()
    {
        // Dash_atk 애니메이션 실행
        animator.SetTrigger("Dash_atk");
        Debug.Log("Dash_atk");

        // Dash_atk 애니메이션의 재생 시간만큼 대기
        yield return new WaitForSeconds(dashAtkDuration);

        // Dash_atk 이후 처리 작성
        animator.ResetTrigger("isAttack");
    }

    float playerHeight = 1.0f;    // 플레이어의 높이값 변수
    float cameraHeight = 2.0f;    // 카메라의 높이값 변수

    float smoothSpeed = 10f;

    void LockFalse()
    {
        playerLock = false;
    }
}