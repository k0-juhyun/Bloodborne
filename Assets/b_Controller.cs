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
        print(collision.gameObject.name);
    }

    protected virtual void WhichDirectionDamageCameFrom(float direction)
    {
        if (direction >= 145 && direction <= 180)
        {

        }
        return;
    }
}
