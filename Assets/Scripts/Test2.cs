using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test2 : MonoBehaviour
{
    private int hp;
    public int maxHP = 100;
    public Slider HPslider;
    Animator ani;
    private int hitcount = 0;

    Rigidbody rb;
    // 사운드
    public AudioClip[] Audioclip;
    AudioSource soundSource;
    // Start is called before the first frame update
    void Start()
    {
        HPslider.maxValue = maxHP;
        SetHP(maxHP);
        ani = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        soundSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (soundSource == null)
        {
            soundSource = gameObject.AddComponent<AudioSource>();
        }

        soundSource.enabled = true;

    }
    void SetHP(int value)
    {
        hp = value;
        HPslider.value = hp;
    }
    public void TakeDamage(int damage)
    {
        hp -= damage;

        // 체력이 0 이하로 내려가지 않도록 클램핑
        hp = Mathf.Clamp(hp, 0, maxHP);

        // HP 슬라이더 갱신
        HPslider.value = hp;

        // 체력이 0이 되었을 때 처리를 실행.
        if (hp <= 0)
        {
            Die();
        }
        //else
        //{
           
        //    hitcount++;
        //    if (hitcount == 1)
        //    {
        //        ani.SetTrigger("Hit1");
        //    }
        //    else if(hitcount == 2)
        //    {
        //        ani.SetTrigger("Hit2");
        //    }
        //    else if(hitcount == 3)
        //    {
        //        ani.SetTrigger("Hit3");
        //    }
        //}
    }
    private void Die()
    {
        // 플레이어가 죽을 때 실행
        ani.SetTrigger("Die");

       
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss"))
        {
            ani.SetTrigger("Hit1");
            TakeDamage(5);
            soundSource.clip = Audioclip[1];
            soundSource.PlayOneShot(Audioclip[1]);
        }
        
    }
}
