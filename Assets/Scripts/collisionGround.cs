using Retro.ThirdPersonCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionGround : MonoBehaviour
{
    private GameObject dustEffect;

    private void Awake()
    {
        dustEffect = Resources.Load<GameObject>("DustSmoke");
    }
    private void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("Hand") && bossAI.attackInProgress)
        {
            Vector3 pos = coll.contacts[0].point;
            Vector3 _normal = coll.contacts[0].normal;

            Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);
            GameObject dust = Instantiate<GameObject>(dustEffect, pos, rot);
            Destroy(dust, 1.0f);
        }
    }
}
