using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace bloodborne
{
    public class sceneManager : MonoBehaviour
    {
        public string thisScene;
        public string nextScene;
        public string playerDieNextScene = "PlayerDie";
        private string playerDieCurScene;

        public VideoPlayer player;

        BossAlpha bossAlpha;
        bossAI bossAi;
        PlayerLocomotion playerLocomotion;
        PlayerAnimatorManager playerAnimatorManager;
        AudioSource audioSource;

        private void Awake()
        {
            bossAi = FindObjectOfType<bossAI>();
            bossAlpha = FindObjectOfType<BossAlpha>();
            playerLocomotion = FindObjectOfType<PlayerLocomotion>();
            playerAnimatorManager = FindObjectOfType<PlayerAnimatorManager>();
            audioSource = GetComponent<AudioSource>();

            Scene curScene = SceneManager.GetActiveScene();
            // ����� �Ѿ��
            player.loopPointReached += OnVideoFinished;

            if(SceneManager.GetActiveScene().name == "PlayerDie")
            {
                nextScene = thisScene;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K) /*&& SceneManager.GetActiveScene().name != "jh_MoonPresenceEndingScene"*/)
            {
                SceneManager.LoadScene(nextScene);
            }

            if (playerAnimatorManager != null)
            {
                if (playerAnimatorManager.playerDieScene)
                {
                    thisScene = playerDieCurScene;
                    SceneManager.LoadScene(playerDieNextScene);
                }
            }

            // ��ŸƮ ������ �Ѿ��
            if (SceneManager.GetActiveScene().name == "jh_GermanStartScene" && ClickToShowText.nextScene)
            {
                SceneManager.LoadScene(nextScene);
            }

            // �Ը��� ������
            if (SceneManager.GetActiveScene().name == "YJ_HuntersDream" && bossAlpha.isGehrmanDie)
            {
                StartCoroutine(delayLoadScene(3f));
            }

            // �������� ������
            if (SceneManager.GetActiveScene().name == "jh_HuntersDream" && bossAi.isFinished)
            {
                StartCoroutine(delayLoadScene(8f));
            }

        }
        private void OnVideoFinished(VideoPlayer vp)
        {
            SceneManager.LoadScene(nextScene);
        }

        IEnumerator delayLoadScene(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            SceneManager.LoadScene(nextScene);
        }

        public void ReTry()
        {
            SceneManager.LoadScene("StartScene");
        }
        public void OnGameStart()
        {
            SceneManager.LoadScene(nextScene);
            audioSource.enabled = true;
        }

        public void OnGameQuit()
        {
            audioSource.enabled = true;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // ���ø����̼� ���ᶧ
#endif
        }
    }
}