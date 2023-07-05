using Retro.ThirdPersonCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossDamage : MonoBehaviour
{
    private const string weaponTag = "p_Weapon";
    private float hp = 100.0f;

    private Transform playerTr;
    private Animator animator;
    private GameObject bloodEffect;
    public bossAI bossai;

    public bool isHitted;
    public float playerDamage = 20f;
    string currentDamageAnimation;

    // Start is called before the first frame update
    void Awake()
    {
        bloodEffect = Resources.Load<GameObject>("DAX_Blood_Spray_00(Fade_2s)");
        animator = GetComponent<Animator>();
        bossai = GetComponent<bossAI>();

        var player = GameObject.FindGameObjectWithTag("Player");
        // 인스턴스 체크
        if (player != null)
            playerTr = player.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    float GetAngle(Vector3 from, Vector3 to)
    {
        return Quaternion.FromToRotation(transform.forward, to - from).eulerAngles.y;
    }

    protected virtual void WhichDirectionDamageCameFrom(float direction)
    {
        // hit count 1일때 
        //forward
        if (direction >= 0 && direction < 45)
        {
            currentDamageAnimation = "GetHitFront";
        }
        if (direction >= 315 && direction < 360)
        {
            currentDamageAnimation = "GetHitFront";
        }
        //Right
        else if (direction >= 45 && direction < 135)
        {
            currentDamageAnimation = "GetHitRight";
        }
        //Back
        else if (direction >= 135 && direction < 225)
        {
            currentDamageAnimation = "GetHitBack";
        }
        //Left
        else if (direction >= 225 && direction <= 315)
        {
            currentDamageAnimation = "GetHitLeft";
        }
        return;
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag == ("p_Weapon") && !isHitted && Combat.P_Attack)
        {
            bossai.curHp -= playerDamage;
            // 피격확인
            isHitted = true;
            Collider[] childColliders = GetComponentsInChildren<Collider>();
            foreach (Collider collider in childColliders)
            {
                collider.isTrigger = true;
            }
            // 피격초기화
            StartCoroutine(ResetHitted());

            // 피격 위치 확인
            float directionHitFrom = (GetAngle(transform.position, playerTr.transform.position));
            WhichDirectionDamageCameFrom(directionHitFrom);

            // 피격 애니메이션 실행
            animator.Play(currentDamageAnimation);

            // bloodEffect 실행
            ShowBloodEffect(coll);
            print("hit");
        }
    }

    private void ShowBloodEffect(Collision coll)
    {
        Vector3 pos = coll.contacts[0].point;
        Vector3 _normal = coll.contacts[0].normal;

        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);
        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot);
        Destroy(blood, 1.0f);
    }

    private IEnumerator ResetHitted()
    {
        yield return new WaitForSeconds(0.5f);
        Collider[] childColliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in childColliders)
        {
            collider.isTrigger = false;
        }
        isHitted = false; // 충돌 상태 초기화
    }
}
