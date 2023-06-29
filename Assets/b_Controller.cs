using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class b_Controller : MonoBehaviour
    {
        [Header("플레이어")]
        public GameObject Player;

        string currentDamageAnimation;

        // 컴포넌트들
        Animator animator;
        void Start()
        {
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            WhichPositionLookAt(Player.transform.position);
        }


        float GetAngle(Vector3 from, Vector3 to)
        {
            return Quaternion.FromToRotation(transform.forward, to - from).eulerAngles.y;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("p_Weapon"))
            {
                float directionHitFrom = (GetAngle(transform.position, Player.transform.position));
                WhichDirectionDamageCameFrom(directionHitFrom);
                animator.Play(currentDamageAnimation);
            }
        }

        protected virtual void WhichDirectionDamageCameFrom(float direction)
        {
            //forward
            if (direction >= 0 && direction < 45)
            {
                currentDamageAnimation = "GetHitFront";
                print("Forward");
            }
            if (direction >= 315 && direction < 360)
            {
                currentDamageAnimation = "GetHitFront";
                print("Forward");
            }
            //Right
            else if (direction >= 45 && direction < 135)
            {
                currentDamageAnimation = "GetHitRight";
                print("Right");
            }
            //Back
            else if (direction >= 135 && direction < 225)
            {
                currentDamageAnimation = "GetHitBack";
                print("Back");
            }
            //Left
            else if (direction >= 225 && direction <= 315)
            {
                currentDamageAnimation = "GetHitLeft";
                print("Left");
            }
            return;
        }

        Vector3 WhichPositionLookAt(Vector3 playerPos)
        {
            float lookDirection = GetAngle(transform.position, Player.transform.position);
            playerPos = Player.transform.position.normalized;
            if (lookDirection >= 0 && lookDirection < 45)
            {
                gameObject.transform.LookAt(playerPos);
                print("look");
            }
            else if (lookDirection >= 315 && lookDirection < 360)
            {
                gameObject.transform.LookAt(playerPos);
                print("look");
            }
            return playerPos;
        }
    }
