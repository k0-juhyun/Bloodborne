using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class b_Controller : MonoBehaviour
{
    string currentDamageAnimation;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("p_Weapon"))
        {

        }
    }

    protected virtual void WhichDirectionDamageCameFrom(float direction)
    {
        //forward
        if (direction >= 225 && direction <= 315)
        {

        }
        //right
        else if (direction >= 315 && direction <= 45)
        {

        }
        //back
        else if(direction >= 45 && direction <= 135)
        {

        }
        //left
        else if(direction >= 135 && direction <= 225)
        {

        }
        return;
    }
}
