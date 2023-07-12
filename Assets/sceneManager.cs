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
        // 씬이 변경될 때마다 호출되는 콜백
        // 필요한 초기화 또는 로직을 수행할 수 있습니다.
        // 예시: 비디오 플레이어 리스너 등록, 변수 초기화 등
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
