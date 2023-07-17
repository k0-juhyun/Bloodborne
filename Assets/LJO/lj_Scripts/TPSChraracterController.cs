using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSChraracterController : MonoBehaviour
{
    public static TPSChraracterController instance;
    private void Awake()
    {
        instance = this;
    }
    [SerializeField]
    private Transform characterBody;
    [SerializeField]
    private Transform cameraArm;

    // 사운드
    public AudioClip[] Audioclip;
    AudioSource soundSource;


    // public Transform LockOnTransform;
    private CharacterController cc;

    Animator animator;

    public LayerMask enemyLayer;

    public float attackRadius = 1f;   // 공격 범위
    public float attackDamage = 10f;  // 공격력


    public Transform Player;
    public Transform Boss;

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

    private int attackCount = 0;


    //public GameObject Player1;
    //public GameObject MainCamera;

    //private float camera_dist = 0f; //리그로부터 카메라까지의 거리
    //public float camera_width = -10f; //가로거리
    //public float camera_height = 4f; //세로거리

    Vector3 dir;
    [Header("PlayerSpeed")]
    public float moveSpeed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        isAttack = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();

        soundSource = gameObject.GetComponent<AudioSource>();

        //카메라리그에서 카메라까지의 길이
        //camera_dist = Mathf.Sqrt(camera_width * camera_width + camera_height * camera_height);

        //카메라리그에서 카메라위치까지의 방향벡터
        //dir = new Vector3(0, camera_height, camera_width).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        print("isAttack: "+isAttack);
        LookAround();
        Move();
        LockOn();
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
        B_Step();
        R_Step();
        L_Step();
        Dash_Atk();

        if (soundSource == null)
        {
            soundSource = gameObject.AddComponent<AudioSource>();
        }

        soundSource.enabled = true;

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
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMove = moveInput.magnitude != 0;
        animator.SetBool("isMove", isMove);
        //animator.SetFloat("MoveSpeed", moveInput.magnitude);
        if (isMove)
        {
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            characterBody.forward = moveDir;
            transform.position += moveDir * Time.deltaTime * 1.5f;
            cc.Move(moveDir * Time.deltaTime * moveSpeed + Physics.gravity * Time.deltaTime);

        }



        //Debug.DrawRay(cameraArm.position, new Vector3(cameraArm.forward.x, 0, cameraArm.forward.z).normalized,Color.red);

    }

    private void LockOn()
    {
        GameObject lockOnObject = GameObject.FindGameObjectWithTag("Boss");  // LockOnTarget 태그로 오브젝트 찾기

        if (lockOnObject != null)
        {
            Transform lockOnTransform = lockOnObject.transform;

            //마우스 오른쪽 버튼을 눌렀을 때 
            if (Input.GetButton("Fire2"))
            {
                //카메라와 player를 enemy에게 lookat 한다

                transform.LookAt(Boss);



                //거리조절
                float distance = Vector3.Distance(Player.position, Boss.position);

                // 카메라와 Boss 사이의 거리가 최소 거리보다 작을 경우
                if (distance < minDist)
                {
                    // 카메라 위치를 최소 거리만큼 뒤로 조정합니다.
                    Vector3 offset = transform.position - Boss.position;
                    offset = offset.normalized * minDist;
                    transform.position = Boss.position + offset;
                }

                //카메라 줌인아웃
                //float distance = Vector3.Distance(Player.position, Boss.position);

                float zoomLevel = Mathf.InverseLerp(minDist, maxDist, distance);
                // Field of view View 바꿈
                float targetFOV = Mathf.Lerp(50f, 60f, zoomLevel);
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);


                characterBody.LookAt(Boss);
                //transform.LookAt(Boss);

            }

        }
    }
    private Vector3 GetCenterPoint()
    {
        // 두 오브젝트의 중심점 계산
        Vector3 centerPoint = (Player.position + Boss.position) / 2f;

        return centerPoint;
    }

    private void Attack()
    {
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
                    soundSource.clip = Audioclip[5];
                    soundSource.PlayOneShot(Audioclip[5]);
                }
            }
        }
        if (!hitBoss)
        {
            soundSource.clip = Audioclip[1];
            soundSource.PlayOneShot(Audioclip[1]);
        }
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
            soundSource.clip = Audioclip[2];
            soundSource.PlayOneShot(Audioclip[2]);
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
            soundSource.clip = Audioclip[2];
            soundSource.PlayOneShot(Audioclip[2]);
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
            soundSource.clip = Audioclip[2];
            soundSource.PlayOneShot(Audioclip[2]);

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
}





