using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bloodborne
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance = null; //Singleton instance
        public AudioSource musicSource; //배경 음악 재생
        public List<AudioSource> sfxSources; // 

        public AudioClip[] backgroundMusics; //여러 배경 음악
        public AudioClip[] playerSFXs; //플레이어 효과
        public AudioClip[] boss1SFXs; // 보스1 효과
        public AudioClip[] boss2SFXs; // 보스2 효과

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);

            sfxSources = new List<AudioSource>();
            for (int i = 0; i < 5; i++)
            {
                AudioSource newSFXSource = gameObject.AddComponent<AudioSource>();
                sfxSources.Add(newSFXSource);
            } //오디오 소스 할당
        }
        // Start is called before the first frame update
        void Start()
        {

            musicSource.loop = true;
            musicSource.Play();
            PlayBackgroundMusic(0);
        }
        public void PlaySFX(SoundEffectType effectType, int clipIndex)
        {
            AudioClip[] sfxArray = GetSFXArray(effectType);
            AudioSource sfxSource = FindAvailableSFXSource();
            if (sfxArray != null && clipIndex >= 0 && clipIndex < sfxArray.Length)
            {
                sfxSource.PlayOneShot(sfxArray[clipIndex]);
            }
            else
            {
                Debug.LogWarning("Invalid SoundEffectType or clipIndex: " + effectType + ", " + clipIndex);
            }
        }
        private AudioClip[] GetSFXArray(SoundEffectType effectType)
        {
            switch (effectType)
            {
                case SoundEffectType.Player:
                    return playerSFXs;
                case SoundEffectType.Boss1:
                    return boss1SFXs;
                case SoundEffectType.Boss2:
                    return boss2SFXs;
                default:
                    Debug.LogWarning("Unknown SoundEffectType: " + effectType);
                    return null;
            }
        }
        public enum SoundEffectType
        {
            Player,
            Boss1,
            Boss2
        }
        public void PlayBackgroundMusic(int musicIndex)
        {
            if (musicIndex >= 0 && musicIndex < backgroundMusics.Length)
            {
                musicSource.clip = backgroundMusics[musicIndex];
                musicSource.loop = true;
                musicSource.Play();
            }
        }
        private AudioSource FindAvailableSFXSource()
        {
            foreach (var sfxSource in sfxSources)
            {
                if (!sfxSource.isPlaying)
                {
                    return sfxSource;
                }
            }
            return null;
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}