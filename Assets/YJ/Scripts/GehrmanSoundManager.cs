using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace bloodborne
{
    [System.Serializable]
    public class Sound
    {
        public string name;         // 음악이름
        public AudioClip clip;      // 음악
    }

    public class GehrmanSoundManager : MonoBehaviour
    {
        // 효과음 배열
        public AudioSource[] audioSourcesEffects;
        // BGM
        public AudioSource audioSourceBgm;

        // 재생중인 사운드 목록
        public string[] playSoundName;

        public Sound[] effectSounds;
        public Sound bgmSounds;



        // 효과음 사운드 재생 함수
        public void PlaySound(string _name)
        {
            for (int i = 0; i < effectSounds.Length; i++)
            {
                if (_name == effectSounds[i].name)
                {
                    for (int j = 0; j < audioSourcesEffects.Length; j++)
                    {
                        if (!audioSourcesEffects[j].isPlaying)
                        {
                            // 재생중인 사운드 목록에 이름을 추가한다
                            playSoundName[j] = effectSounds[i].name;
                            // 같은 이름의 클립으로 교체한다
                            audioSourcesEffects[j].clip = effectSounds[i].clip;
                            // 사운드를 재생한다
                            audioSourcesEffects[j].Play();
                            return;
                        }
                    }
                    Debug.Log("모든 AudioSource 재생중");
                    return;
                }
            }
            Debug.Log(_name + "사운드가 SoundManager에 없음");
        }

        // 모든 사운드 멈춤
        public void StopAllSound()
        {
            for (int i = 0; i < audioSourcesEffects.Length; i++)
            {
                // 사운드를 멈춘다
                audioSourcesEffects[i].Stop();
            }
        }

        //효과음 사운드 하나만 멈춤
        public void StopSound(string _name)
        {
            for (int i = 0; i < audioSourcesEffects.Length; i++)
            {
                if (playSoundName[i] == _name)
                {
                    audioSourcesEffects[i].Stop();
                    return;
                }
            }
            Debug.Log("재생중인" + _name + "사운드가 없음");
        }
    }
}
