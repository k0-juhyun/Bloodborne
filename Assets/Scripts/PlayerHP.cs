using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public Slider hpSlider;
    float currHP = 100;
    float maxHP = 100;
    Animator ani;
    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Hit();
    }
    private void Hit()
    {
        
        currHP -= 10;
        float ratio = currHP / 100;
        hpSlider.value = ratio;
    }
}
