using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bloodborne
{
    public class particleSystemCollision : MonoBehaviour
    {
        public float checkRadius = 0.5f;

        private void Update()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, checkRadius);

            foreach (Collider collider in colliders)
            {
                Debug.Log(collider.gameObject.name);
            }
        }
    }
}