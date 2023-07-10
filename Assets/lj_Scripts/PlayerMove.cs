using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    CharacterController cc;
    bool isRolling = false;
    private Animator ani;
   
    
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        ani = GetComponent<Animator>();
    }
   
    // Update is called once per frame
    //void Update()
    //{
    //    Roll();
    //}
    //private void Roll()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space)&&!isRolling)
    //    {
    //        isRolling = true;
    //        ani.SetTrigger("isRolling");
    //    }
    //}

    private void OnRollAnimationComplete()
    {
        isRolling = false;
    }
         
}
