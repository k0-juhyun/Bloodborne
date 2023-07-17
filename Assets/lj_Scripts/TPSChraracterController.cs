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


        //레이캐스트할 벡터값
        //Vector3 ray_target = transform.up * camera_height + transform.forward * camera_width;

        //RaycastHit hitinfo;
        //Physics.Raycast(transform.position, ray_target, out hitinfo, camera_dist);

        //if (hitinfo.point != Vector3.zero)//레이케스트 성공시
        //{
        //    //point로 옮긴다.
        //    MainCamera.transform.position = hitinfo.point;
        //}
        //else
        //{
        //    //로컬좌표를 0으로 맞춘다. (카메라리그로 옮긴다.)
        //    MainCamera.transform.localPosition = Vector3.zero;
        //    //카메라위치까지의 방향벡터 * 카메라 최대거리 로 옮긴다.
        //    MainCamera.transform.Translate(dir * camera_dist);

        //}

    }

   

    public  bool isAttack;

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
                float targetFOV = Mathf.Lerp(35f, 60f, zoomLevel);
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
        isAttack = true;
        animator.SetTrigger("isAttack");
        Debug.Log("dfd");
        soundSource.clip = Audioclip[4];
        soundSource.PlayOneShot(Audioclip[4]);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Boss"))
            {
                // 적에게 데미지를 입히는 코드를 작성하세요.
                Enemy enemyHealth = hitCollider.GetComponent<Enemy>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(attackDamage);
                    
                }
            }
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
    
    private bool isWKeyPressed = false;
    private void Dash_Atk()
    {
        if (Input.GetKey(KeyCode.W))
        {
            isWKeyPressed = true;

        }
        else
        {
            isWKeyPressed = false;
        }

        if (Input.GetMouseButtonDown(0) && isWKeyPressed && !Dash_atk)
        {
        
            animator.SetTrigger("Dash_atk");

            //soundSource.clip = Audioclip[2];
            //soundSource.PlayOneShot(Audioclip[2]);
        }
    }

    float playerHeight = 1.0f;    // 플레이어의 높이값 변수
     float cameraHeight = 2.0f;    // 카메라의 높이값 변수

    float smoothSpeed = 10f;

   // public LayerMask groundLayer;
    //private void LateUpdate()
    //{
    //    // 플레이어와 카메라의 위치를 업데이트하기 전에 레이캐스트를 사용하여 지면과의 충돌을 감지
    //    RaycastHit hit;
    //    Vector3 origin = transform.position + Vector3.up * 0.1f;
    //    Vector3 dir = -Vector3.up;

    //    //if (Physics.Raycast(origin, dir, out hit, 1000, groundLayer))
    //    {
    //        //// 충돌 지점의 높이값을 기준으로 플레이어와 카메라의 위치를 설정
    //        //vector3 targetposition = hit.point + vector3.up * playerheight;
    //        //transform.position = targetposition;
    //        //cameraarm.position = targetposition + vector3.up * cameraheight;
    //        ////
    //        //vector3 cameraarmposition = cameraarm.position;
    //        //cameraarmposition.y = targetposition.y + cameraheight;
    //        //cameraarm.position = cameraarmposition;

    //        // 충돌 지점의 높이값을 기준으로 플레이어와 카메라의 위치를 설정
    //        Vector3 targetPosition = hit.point + Vector3.up * playerHeight;
    //        //transform.position = targetPosition;

    //        // 카메라의 높이를 고정시키지 않고, 카메라의 위치를 업데이트하여 플레이어를 따라가도록 함
    //        //Vector3 cameraTargetPosition = targetPosition + Vector3.up * cameraHeight;
    //        //cameraArm.position = Vector3.Lerp(cameraArm.position, cameraTargetPosition, Time.deltaTime * smoothSpeed);


    //    }
    //    else
    //    {
    //        // 레이캐스트 충돌이 없을 경우에는 기존 위치를 유지
    //    }

    //    // 기존의 나머지 코드들을 실행
    //    LookAround();
    //    Move();
    //    LockOn();
       
    }





