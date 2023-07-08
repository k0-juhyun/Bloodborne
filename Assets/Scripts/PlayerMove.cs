using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    CharacterController cc;
    bool fDown;
    float fireDelay;
    bool isFireReady;
    GameObject equipWeapon;
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }
    void GetInput()
    {
        fDown = Input.GetButtonDown("Fire1");
    }
    // Update is called once per frame
    void Update()
    {
        Dash();
    }
    private void Dash()
    {

    }
         void Attack()
    {
        
    }
}
