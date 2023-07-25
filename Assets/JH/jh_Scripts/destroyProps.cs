using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bloodborne
{
    public class destroyProps : MonoBehaviour
    {
        PlayerAnimatorManager playerAnimatorManager;
        BossAlpha bossAlpha;
        // Start is called before the first frame update
        void Start()
        {
            playerAnimatorManager = FindObjectOfType<PlayerAnimatorManager>();
            bossAlpha = GetComponent<BossAlpha>();
        }


        private void OnCollisionEnter(Collision coll)
        {
            if (coll.collider.tag == ("p_Weapon") && playerAnimatorManager.isAttack)
            {
                Destroy(gameObject);
            }

            else if (coll.collider.tag == ("Weapon") && bossAlpha.isGehrmanAttack == true)
            {
                Destroy(gameObject);
            }
        }
    }
}
