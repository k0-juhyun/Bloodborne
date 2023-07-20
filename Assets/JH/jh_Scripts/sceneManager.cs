using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class sceneManager : MonoBehaviour
{
    public string nextScene;
    public VideoPlayer player;

    private void Awake()
    {
        Scene curScene = SceneManager.GetActiveScene();
        // 영상씬 넘어가기
        player.loopPointReached += OnVideoFinished;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SceneManager.LoadScene(nextScene);
        }

        // 스타트 씬에서 넘어가기
        if(SceneManager.GetActiveScene().name == "jh_GermanStartScene" && ClickToShowText.nextScene)
        {
            SceneManager.LoadScene(nextScene);
            ClickToShowText.nextScene = false;
        }

        // 게르만 죽으면
        if (SceneManager.GetActiveScene().name == "YJ_HuntersDream" && BossAlpha.instance.isGehrmanDie)
        {
            StartCoroutine(delayLoadScene(3f));
        }

        // 달의존재 죽으면
        if (SceneManager.GetActiveScene().name == "jh_HuntersDream" && bossAI.instance.isDie)
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

    public void OnGameStart()
    {
        SceneManager.LoadScene(nextScene);
    }

    public void OnGameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료때
#endif
    }
}
