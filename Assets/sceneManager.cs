using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class sceneManager : MonoBehaviour
{
    public static sceneManager Instance { get; private set; }

    public bool isVideoScene = true;
    public VideoPlayer videoPlayer;
    public bool bossDie = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (isVideoScene)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ���� ����� ������ ȣ��Ǵ� �ݹ�
        // �ʿ��� �ʱ�ȭ �Ǵ� ������ ������ �� �ֽ��ϴ�.
        // ����: ���� �÷��̾� ������ ���, ���� �ʱ�ȭ ��
        if (isVideoScene)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    private void Update()
    {
        if (!isVideoScene && bossDie)
        {
            LoadNextScene();
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}
