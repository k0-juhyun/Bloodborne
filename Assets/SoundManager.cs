using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

namespace bloodborne
{
    public class SoundManager : MonoBehaviour
    {
        public AudioSource[] audioSourceEffects;
        public AudioSource audioSourceBgm;

        public string[] playSoundName;

        public Sound[] effectSounds;
        public Sound[] bgmSounds;

        public void PlaySE(string _name)
        {
            for (int i = 0; i < effectSounds.Length; i++)
            {
                if(_name == effectSounds[i].name)
                {
                    for(int j = 0; j < audioSourceEffects.Length; j++)
                    {
                        if (!audioSourceEffects[j].isPlaying)
                        {
                            playSoundName[j] = effectSounds[i].name;
                            audioSourceEffects[j].clip = effectSounds[i].clip;
                            audioSourceEffects[j].Play();
                            return;
                        }
                    }
                    Debug.Log("FUll");
                    return;
                }
            }
            Debug.Log(_name + "사운드 등록x");
        }

        public void StopAllSE() 
        {
            for(int i =0;i < effectSounds.Length; i++)
            {
                audioSourceEffects[i].Stop();
            }
        }

        public void StopSE(string _name)
        {
            for (int i = 0; i < audioSourceEffects.Length; i++)
            {
                if (playSoundName[i] == _name)
                {
                    audioSourceEffects[i].Stop();
                    break;
                }
            }

            Debug.Log("재생중" +  _name + "X");
        }

        void Start()
        {
            PlaySE("BGM");       
        }
    }
}