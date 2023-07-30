using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSounds
{
    public string name;
    public AudioClip clip;
}
public class AudioManager2 : MonoBehaviour
{
    public static AudioManager2 instance;

    [SerializeField] PlayerSounds[] sfx = null;
    [SerializeField] PlayerSounds[] bgm = null;

    [SerializeField] AudioSource bgmPlayer = null;
    [SerializeField] AudioSource[] sfxPlayer = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 이 부분을 추가하여 게임 오브젝트를 영구적으로 유지합니다.
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 중복된 AudioManager2 게임 오브젝트를 파괴합니다.
        }
    }

    public void PlayBGM(string p_bgmName)
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            if (p_bgmName == bgm[i].name)
            {
                bgmPlayer.clip = bgm[i].clip;
                bgmPlayer.Play();
            }
        }
    }
    public void StopBGM()
    {
        bgmPlayer.Stop();
    }
    public void PlaySFX(string p_sfxName)
    {
        for (int i = 0; i < sfx.Length; i++)
        {
            if (p_sfxName == sfx[i].name)
            {
                for (int x = 0; x < sfxPlayer.Length; x++)
                {
                    if (!sfxPlayer[x].isPlaying)
                    {
                        sfxPlayer[x].clip = sfx[i].clip;
                        sfxPlayer[x].Play();
                        return;
                    }
                }
                Debug.Log("모든 오디오 플레이어를 재생");
                return;
            }
        }
        Debug.Log(p_sfxName + "이름의 효과음이 없다");
    }
}
