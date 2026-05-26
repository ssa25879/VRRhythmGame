using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSongEndController : MonoBehaviour
{
    [SerializeField] private GameBackgroundController backgroundController;
    [SerializeField] private string introSceneName = "Intro";
    [SerializeField] private float returnDelay = 1.5f;
    [SerializeField] private float minimumPlayTime = 1f;

    private AudioSource bgmSource;
    private bool hasStarted;
    private bool isReturning;
    private float startedAt;

    void Awake()
    {
        if (backgroundController == null)
        {
            backgroundController = FindFirstObjectByType<GameBackgroundController>();
        }
    }

    void Start()
    {
        bgmSource = backgroundController != null ? backgroundController.bgmSource : null;
        startedAt = Time.time;

        if (bgmSource == null)
        {
            Debug.LogWarning("[GameSongEnd] BGM AudioSource is missing. Song end return is disabled.");
        }
    }

    void Update()
    {
        if (isReturning || bgmSource == null || bgmSource.clip == null)
        {
            return;
        }

        if (bgmSource.isPlaying)
        {
            hasStarted = true;
            return;
        }

        bool playedLongEnough = Time.time - startedAt >= minimumPlayTime;
        bool reachedEnd = bgmSource.timeSamples >= bgmSource.clip.samples - 1 ||
                          bgmSource.time >= bgmSource.clip.length - 0.05f;

        if (hasStarted && playedLongEnough && reachedEnd)
        {
            StartCoroutine(ReturnToIntro());
        }
    }

    private System.Collections.IEnumerator ReturnToIntro()
    {
        isReturning = true;
        Debug.Log("[GameSongEnd] Song ended. Showing result screen.");
        yield return new WaitForSeconds(returnDelay);

        if (GameScoreController.Instance != null)
        {
            GameScoreController.Instance.ShowResultScreen(introSceneName);
        }
        else
        {
            SceneManager.LoadScene(introSceneName);
        }
    }
}
