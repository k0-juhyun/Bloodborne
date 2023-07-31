using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bloodborne
{
    public class destroyProps : MonoBehaviour
    {
        PlayerAnimatorManager playerAnimatorManager;
        BossAlpha bossAlpha;

        AudioSource audioSource;
        Rigidbody rb;
        MeshRenderer[] mr;
        public float knockbackForce = 5f;
        // Start is called before the first frame update
        void Start()
        {
            playerAnimatorManager = FindObjectOfType<PlayerAnimatorManager>();
            audioSource = GetComponent<AudioSource>();
            rb = GetComponent<Rigidbody>();
            mr = GetComponentsInChildren<MeshRenderer>();
            bossAlpha = GetComponent<BossAlpha>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("p_Weapon") && playerAnimatorManager.isAttack)
            {
                audioSource.enabled = true;
                foreach (MeshRenderer renderer in mr)
                {
                    renderer.enabled = false;
                }
            }
            else if (other.gameObject.CompareTag("Weapon") && bossAlpha.isGehrmanAttack)
            {
                audioSource.enabled = true;
                foreach (MeshRenderer renderer in mr)
                {
                    renderer.enabled = false;
                }
            }
        }
    }
}