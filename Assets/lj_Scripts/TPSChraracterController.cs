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

    // ����
    public AudioClip[] Audioclip;
    AudioSource soundSource;


    // public Transform LockOnTransform;
    private CharacterController cc;
  
    Animator animator;

    public LayerMask enemyLayer;

    public float attackRadius = 1f;   // ���� ����
    public float attackDamage = 10f;  // ���ݷ�


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

    //private float camera_dist = 0f; //���׷κ��� ī�޶������ �Ÿ�
    //public float camera_width = -10f; //���ΰŸ�
    //public float camera_height = 4f; //���ΰŸ�

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

        //ī�޶󸮱׿��� ī�޶������ ����
        //camera_dist = Mathf.Sqrt(camera_width * camera_width + camera_height * camera_height);

        //ī�޶󸮱׿��� ī�޶���ġ������ ���⺤��
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


        //����ĳ��Ʈ�� ���Ͱ�
        //Vector3 ray_target = transform.up * camera_height + transform.forward * camera_width;

        //RaycastHit hitinfo;
        //Physics.Raycast(transform.position, ray_target, out hitinfo, camera_dist);

        //if (hitinfo.point != Vector3.zero)//�����ɽ�Ʈ ������
        //{
        //    //point�� �ű��.
        //    MainCamera.transform.position = hitinfo.point;
        //}
        //else
        //{
        //    //������ǥ�� 0���� �����. (ī�޶󸮱׷� �ű��.)
        //    MainCamera.transform.localPosition = Vector3.zero;
        //    //ī�޶���ġ������ ���⺤�� * ī�޶� �ִ�Ÿ� �� �ű��.
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
        GameObject lockOnObject = GameObject.FindGameObjectWithTag("Boss");  // LockOnTarget �±׷� ������Ʈ ã��
        if (lockOnObject != null)
        {
            Transform lockOnTransform = lockOnObject.transform;
            
            //���콺 ������ ��ư�� ������ �� 
            if (Input.GetButton("Fire2"))
            {
                //ī�޶�� player�� enemy���� lookat �Ѵ�
                
                transform.LookAt(Boss);

               

                //�Ÿ�����
                float distance = Vector3.Distance(Player.position, Boss.position);

                // ī�޶�� Boss ������ �Ÿ��� �ּ� �Ÿ����� ���� ���
                if (distance < minDist)
                {
                    // ī�޶� ��ġ�� �ּ� �Ÿ���ŭ �ڷ� �����մϴ�.
                    Vector3 offset = transform.position - Boss.position;
                    offset = offset.normalized * minDist;
                    transform.position = Boss.position + offset;
                }

                //ī�޶� ���ξƿ�
                //float distance = Vector3.Distance(Player.position, Boss.position);

                float zoomLevel = Mathf.InverseLerp(minDist, maxDist, distance);
                // Field of view View �ٲ�
                float targetFOV = Mathf.Lerp(35f, 60f, zoomLevel);
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);


                characterBody.LookAt(Boss);
                //transform.LookAt(Boss);

            }

        }
    }
    private Vector3 GetCenterPoint()
    {
        // �� ������Ʈ�� �߽��� ���
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
                // ������ �������� ������ �ڵ带 �ۼ��ϼ���.
                Enemy enemyHealth = hitCollider.GetComponent<Enemy>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(attackDamage);
                    
                }
            }
        }
    }



   
    private bool isSKeyPressed = false; // 's' Ű�� ���ȴ��� ���θ� �����ϴ� ����
    
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

    float playerHeight = 1.0f;    // �÷��̾��� ���̰� ����
     float cameraHeight = 2.0f;    // ī�޶��� ���̰� ����

    float smoothSpeed = 10f;

   // public LayerMask groundLayer;
    //private void LateUpdate()
    //{
    //    // �÷��̾�� ī�޶��� ��ġ�� ������Ʈ�ϱ� ���� ����ĳ��Ʈ�� ����Ͽ� ������� �浹�� ����
    //    RaycastHit hit;
    //    Vector3 origin = transform.position + Vector3.up * 0.1f;
    //    Vector3 dir = -Vector3.up;

    //    //if (Physics.Raycast(origin, dir, out hit, 1000, groundLayer))
    //    {
    //        //// �浹 ������ ���̰��� �������� �÷��̾�� ī�޶��� ��ġ�� ����
    //        //vector3 targetposition = hit.point + vector3.up * playerheight;
    //        //transform.position = targetposition;
    //        //cameraarm.position = targetposition + vector3.up * cameraheight;
    //        ////
    //        //vector3 cameraarmposition = cameraarm.position;
    //        //cameraarmposition.y = targetposition.y + cameraheight;
    //        //cameraarm.position = cameraarmposition;

    //        // �浹 ������ ���̰��� �������� �÷��̾�� ī�޶��� ��ġ�� ����
    //        Vector3 targetPosition = hit.point + Vector3.up * playerHeight;
    //        //transform.position = targetPosition;

    //        // ī�޶��� ���̸� ������Ű�� �ʰ�, ī�޶��� ��ġ�� ������Ʈ�Ͽ� �÷��̾ ���󰡵��� ��
    //        //Vector3 cameraTargetPosition = targetPosition + Vector3.up * cameraHeight;
    //        //cameraArm.position = Vector3.Lerp(cameraArm.position, cameraTargetPosition, Time.deltaTime * smoothSpeed);


    //    }
    //    else
    //    {
    //        // ����ĳ��Ʈ �浹�� ���� ��쿡�� ���� ��ġ�� ����
    //    }

    //    // ������ ������ �ڵ���� ����
    //    LookAround();
    //    Move();
    //    LockOn();
       
    }





