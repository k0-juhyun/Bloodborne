using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bloodborne
{
    public class collisionGround : MonoBehaviour
    {
        PlayerLocomotion playerLocomotion;
        bossAI bossAi;

        private GameObject dustEffect;
        private Transform playerPosition;
        public GameObject camShake;

        private float knockBackDistance = 4;
        private float collisionDistanceFromPlayer;

        private void Awake()
        {
            playerLocomotion = FindObjectOfType<PlayerLocomotion>();
            bossAi = FindObjectOfType<bossAI>();
            dustEffect = Resources.Load<GameObject>("DustSmoke");

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerPosition = player.GetComponent<Transform>();
        }
        private void OnCollisionEnter(Collision coll)
        {
            if (coll.collider.CompareTag("Hand") && bossAI.attackInProgress && bossAI.groundHit)
            {
                if (!camShake.activeSelf)
                {
                    camShake.SetActive(true);
                    StartCoroutine(camShakeOff());
                }

                Vector3 pos = coll.contacts[0].point;
                Vector3 _normal = coll.contacts[0].normal;

                collisionDistanceFromPlayer = Vector3.Distance(pos, playerPosition.position);

                if (collisionDistanceFromPlayer < knockBackDistance)
                {
                    playerLocomotion.playerExplosion = true;
                }

                Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);

                GameObject dust = Instantiate<GameObject>(dustEffect, pos, rot);

                Destroy(dust, 1.0f);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("playerFoot"))
            {
                AudioManager2.instance.PlaySFX("PlayerWalk");
            }

            if(other.CompareTag("bossFoot"))
            {
                AudioManager2.instance.PlaySFX("Moon_Walk");
            }
        }
        IEnumerator camShakeOff()
        {
            yield return new WaitForSeconds(1);
            camShake.SetActive(false);
        }
    }
}