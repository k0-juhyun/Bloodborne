using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSChraracterController : MonoBehaviour
{
    [SerializeField]
    private Transform characterBody;
    [SerializeField]
    private Transform cameraArm;



   
    public Transform LockOnTransform;
    private CharacterController cc;
    Animator animator;

    public LayerMask enemyLayer;
   
    public float attackRadius = 1f;   // 공격 범위
    public float attackDamage = 10f;  // 공격력



    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        animator = characterBody.GetComponent<Animator>();
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
        if (isMove)
        {
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            characterBody.forward = moveDir;
            transform.position += moveDir * Time.deltaTime * 7f;
        }
        

        //Debug.DrawRay(cameraArm.position, new Vector3(cameraArm.forward.x, 0, cameraArm.forward.z).normalized,Color.red);
        
    }
    private void LockOn()
    {
        //마우스 오른쪽 버튼을 눌렀을 때 
        if (Input.GetButton("Fire2"))
        {
            //카메라와 player를 enemy에게 lookat 한다
            transform.LookAt(LockOnTransform);

            characterBody.LookAt(LockOnTransform);
        }
        
    }
    private void Attack()
    {
        animator.SetTrigger("isAttack");

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
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
    
        
}
