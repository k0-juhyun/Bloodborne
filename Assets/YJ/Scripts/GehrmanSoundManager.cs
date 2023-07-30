using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace bloodborne
{
    [System.Serializable]
    public class Sound
    {
        public string name;         // �����̸�
        public AudioClip clip;      // ����
    }

    public class GehrmanSoundManager : MonoBehaviour
    {
        // ȿ���� �迭
        public AudioSource[] audioSourcesEffects;
        // BGM
        public AudioSource audioSourceBgm;

        // ������� ���� ���
        public string[] playSoundName;

        public Sound[] effectSounds;
        public Sound bgmSounds;



        // ȿ���� ���� ��� �Լ�
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
                            // ������� ���� ��Ͽ� �̸��� �߰��Ѵ�
                            playSoundName[j] = effectSounds[i].name;
                            // ���� �̸��� Ŭ������ ��ü�Ѵ�
                            audioSourcesEffects[j].clip = effectSounds[i].clip;
                            // ���带 ����Ѵ�
                            audioSourcesEffects[j].Play();
                            return;
                        }
                    }
                    Debug.Log("��� AudioSource �����");
                    return;
                }
            }
            Debug.Log(_name + "���尡 SoundManager�� ����");
        }

        // ��� ���� ����
        public void StopAllSound()
        {
            for (int i = 0; i < audioSourcesEffects.Length; i++)
            {
                // ���带 �����
                audioSourcesEffects[i].Stop();
            }
        }

        //ȿ���� ���� �ϳ��� ����
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
            Debug.Log("�������" + _name + "���尡 ����");
        }
    }
}
