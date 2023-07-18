using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyProps : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag == ("Weapon") && TPSChraracterController.instance.isAttack)
        {
            Destroy(gameObject);
        }

        else if (coll.collider.tag == ("Weapon") && BossAlpha.instance.isGehrmanAttack == true)
        {
            Destroy(gameObject);
        }
    }
}
