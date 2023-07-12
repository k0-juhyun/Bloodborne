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

    public float playerSpeed;



    public Transform LockOnTransform;
    private CharacterController cc;
    Animator animator;

    public LayerMask enemyLayer;

    public float attackRadius = 1f;   // ���� ����
    public float attackDamage = 10f;  // ���ݷ�


    public Transform Player;
    public Transform Boss;

    private float Dist;
    public float zoomSpeed = 10;
    public float minDist = 5;
    public float maxDist = 10;

    bool Move_B = false;
    bool Move_L = false;
    bool Move_R = false;
    bool isMove = false;

    private int attackCount = 0;
   



    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
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

        //if (Input.GetButtonDown("Fire1"))
        //{
        //    StartCoroutine(AttackCoroutine());                        //attack �ڷ�ƾ ��ŸƮ
        //}

    }

    public bool isAttack;

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
            transform.position += moveDir * Time.deltaTime * playerSpeed;
        }


        //Debug.DrawRay(cameraArm.position, new Vector3(cameraArm.forward.x, 0, cameraArm.forward.z).normalized,Color.red);

    }
    private void LockOn()
    {
        //���콺 ������ ��ư�� ������ �� 
        if (Input.GetButton("Fire2"))
        {
            //ī�޶�� player�� enemy���� lookat �Ѵ�
            transform.LookAt(LockOnTransform);

            characterBody.LookAt(LockOnTransform);

            //ī�޶� ���ξƿ�
            //float distance = Vector3.Distance(Player.position, Boss.position);
            //float zoomLevel = Mathf.InverseLerp(minDist, maxDist, distance);

            // Field of view View �ٲ�
            //float targetFOV = Mathf.Lerp(35f, 100f, zoomLevel);
            //Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);

            //transform.LookAt(enemy);

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
        animator.SetTrigger("isAttack");

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

    void AttackTrue()
    {
        isAttack = true;
    }

    void AttackFalse()
    {
        isAttack = false;
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
            print("aaaa");
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
        }
    }
}




