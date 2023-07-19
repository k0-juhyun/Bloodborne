using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Test2 : MonoBehaviour
{
    public static Test2 instance;
    private int hp;
    public int maxHP = 100;
    public Slider HPslider;
    Animator ani;
    private int hitcount = 0;

    Rigidbody rb;
    // ����
    public AudioClip[] Audioclip;
    AudioSource soundSource;
    private bool isLiedown = false;
    public bool nutback = false;
    // Start is called before the first frame update
    void Start()
    {
        HPslider.maxValue = maxHP;
        SetHP(maxHP);
        ani = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        soundSource = gameObject.GetComponent<AudioSource>();

        if (soundSource == null)
        {
            soundSource = gameObject.AddComponent<AudioSource>();
        }

        soundSource.enabled = true;
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            IncreaseHP(20);
            ani.SetTrigger("Potion");
        }

    }
    void SetHP(int value)
    {
        hp = value;
        HPslider.value = hp;
    }
    void IncreaseHP(int amount)
    {
        hp += amount;

        // �ִ� ü���� �ʰ����� �ʵ��� Ŭ����
        hp = Mathf.Clamp(hp, 0, maxHP);

        // HP �����̴� ����
        HPslider.value = hp;
    }
    public void TakeDamage(int damage)
    {
        hp -= damage;

        // ü���� 0 ���Ϸ� �������� �ʵ��� Ŭ����
        hp = Mathf.Clamp(hp, 0, maxHP);

        // HP �����̴� ����
        HPslider.value = hp;

        // ü���� 0�� �Ǿ��� �� ó���� ����.
        if (hp <= 0)
        {
            Die();
        }
        else
        {

            hitcount++;

            if (isLiedown)
            {
                ani.SetTrigger("Liedown");
            }
            else
            {
                if (hitcount == 1)
                {

                    ani.SetTrigger("Hit1");
                }
                else if (hitcount == 2)
                {
                    Debug.Log("dsds");
                    ani.SetTrigger("Hit2");
                }
                else if (hitcount == 3)
                {
                    ani.SetTrigger("Hit3");
                    hitcount = 0;
                }
            }
        }
    }
    private void Die()
    {
        // �÷��̾ ���� �� ����
        ani.SetTrigger("Die");


    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand") || other.CompareTag("Weapon"))
        {
            if (BossAlpha.instance != null) 
            {
                if (BossAlpha.instance.isGehrmanAttack)
                {
                    TPSChraracterController.instance.playerLock = true;
                    print("hit");
                    TakeDamage(5);
                    soundSource.clip = Audioclip[0];
                    soundSource.PlayOneShot(Audioclip[0]);

                }
                if (BossAlpha.instance.bossState == BossAlpha.BossPatternState.SickelCombo1 && BossAlpha.instance.sickelSubState == BossAlpha.SickelSubState.Attack1)
                {
                    TPSChraracterController.instance.playerLock = true;
                    Debug.Log("dsds");
                    TakeDamage(5);
                    ani.SetTrigger("LieDown");
                }
            }

            if(bossAI.instance != null) 
            {
                if (bossAI.instance.moonpresenceAttack)
                {
                    print("hit");
                    TakeDamage(5);
                    soundSource.clip = Audioclip[0];
                    soundSource.PlayOneShot(Audioclip[0]);

                }
            }
        }
     
    }




}
