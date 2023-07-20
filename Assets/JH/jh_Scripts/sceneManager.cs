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
        // ����� �Ѿ��
        player.loopPointReached += OnVideoFinished;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SceneManager.LoadScene(nextScene);
        }

        // ��ŸƮ ������ �Ѿ��
        if(SceneManager.GetActiveScene().name == "jh_GermanStartScene" && ClickToShowText.nextScene)
        {
            SceneManager.LoadScene(nextScene);
            ClickToShowText.nextScene = false;
        }

        // �Ը��� ������
        if (SceneManager.GetActiveScene().name == "YJ_HuntersDream" && BossAlpha.instance.isGehrmanDie)
        {
            StartCoroutine(delayLoadScene(3f));
        }

        // �������� ������
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
        Application.Quit(); // ���ø����̼� ���ᶧ
#endif
    }
}
