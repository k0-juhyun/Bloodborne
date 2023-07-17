using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    CharacterController cc;
    bool Step_b = false;
    private Animator ani;
   
    
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        ani = GetComponent<Animator>();
    }
   
    // Update is called once per frame
    void Update()
    {
        Step();
    }
    private void Step()
    {
        if (Input.GetKeyDown(KeyCode.Space)&&!Step_b)
        {
            Step_b = true;
            ani.SetTrigger("Step_b");
        }
    }

    private void OnRollAnimationComplete()
    {
        Step_b = false;
    }
         
}
