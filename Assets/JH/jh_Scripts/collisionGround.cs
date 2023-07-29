using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bloodborne
{
    public class collisionGround : MonoBehaviour
    {
        private GameObject dustEffect;
        public GameObject camShake;

        private void Awake()
        {
            dustEffect = Resources.Load<GameObject>("DustSmoke");

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

                Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);

                GameObject dust = Instantiate<GameObject>(dustEffect, pos, rot);

                Destroy(dust, 1.0f);
            }
           
        }

        IEnumerator camShakeOff()
        {
            yield return new WaitForSeconds(1);
            camShake.SetActive(false);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PlayerFoot"))
            {
                AudioManager2.instance.PlaySFX("PlayerWalk");
                Debug.Log("PlayerFoot과 충돌 발생!");
            }
        }
    }
}