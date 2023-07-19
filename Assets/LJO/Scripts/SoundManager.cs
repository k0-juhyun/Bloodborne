using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] AudioClip;
    AudioSource soundSource;
    // Start is called before the first frame update
    

    // Update is called once per frame
    
    void B_Step_Sound()
    {
        soundSource.PlayOneShot(AudioClip[3]);
    }
    void L_Step_sound()
    {
        soundSource.PlayOneShot(AudioClip[3]);
    }
    void R_Step_sound()
    {
        soundSource.PlayOneShot(AudioClip[3]);
    }
    void Start()
    {
        soundSource = gameObject.GetComponent<AudioSource>();
    }

}
