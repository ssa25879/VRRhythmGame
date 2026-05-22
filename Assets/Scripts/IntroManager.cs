using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

// ─────────────────────────────────────────────────────────────────────────────
// IntroManager — VR Beat Saber 인트로 씬 메인 컨트롤러
//
// ★ 스테이지(배경+BGM) 추가 방법:
//   1. Project 창 우클릭 → Create → VRBeatSaber → Stage List
//      (또는 기존 StageList.asset 선택)
//   2. Stage List Inspector → Stages → Size 를 늘린다
//   3. 새 항목에 Stage Name / Background Video / BGM / Thumbnail 입력
//   4. IntroManager와 GameBackgroundController 모두 동일한 StageList.asset 참조
// ─────────────────────────────────────────────────────────────────────────────
public class IntroManager : MonoBehaviour
{
    [Header("스테이지 목록 (StageList.asset 드래그)")]
    public StageListSO stageList;

    [Header("배경 플레이어 (Video 오브젝트의 VideoPlayer 드래그)")]
    public VideoPlayer backgroundPlayer;

    [Header("BGM (AudioSource 드래그)")]
    public AudioSource bgmSource;

    [Header("Skybox 스테이지")]
    public Renderer gridRenderer;

    [Header("UI 레퍼런스")]
    public Image        thumbnailImage;
    public TextMeshProUGUI stageNameText;
    public CanvasGroup  fadeOverlay;

    [Header("로딩 VFX")]
    public GameObject loadingVfxPrefab;
    public Transform loadingVfxAnchor;
    public float loadingVfxLifetime = 1.5f;

    [Header("씬 전환 — Build Settings의 씬 이름과 일치해야 합니다")]
    public string gameSceneName = "Game";

    int _selectedIndex;

    static readonly Color colorSelected = new Color(0.20f, 0.50f, 1.00f, 0.95f);
    static readonly Color colorNormal   = new Color(0.08f, 0.08f, 0.18f, 0.85f);

    void Start()
    {
        if (bgmSource != null) bgmSource.loop = true;
        if (fadeOverlay != null) StartCoroutine(FadeIn(1.2f));
        SelectStage(0);
        if (bgmSource != null && !bgmSource.isPlaying) bgmSource.Play();
    }

    // ── 스테이지 선택 ─────────────────────────────────────────────────────────
    public void SelectStage(int index)
    {
        if (!HasStages()) return;
        _selectedIndex = Mathf.Clamp(index, 0, stageList.stages.Length - 1);

        var s = stageList.stages[_selectedIndex];

        if (stageNameText != null) stageNameText.text = s.stageName;
        if (thumbnailImage != null && s.thumbnail != null)
        {
            thumbnailImage.sprite = s.thumbnail;
            thumbnailImage.color = Color.white;
        }
        else if (thumbnailImage != null)
        {
            thumbnailImage.sprite = null;
            thumbnailImage.color = GetFallbackStageColor(s.stageName);
        }

        ApplyVideo(s.backgroundVideo);
        ApplySkyboxStage(s);

        if (bgmSource != null && s.bgm != null && bgmSource.clip != s.bgm)
        {
            bgmSource.clip = s.bgm;
            bgmSource.Play();
        }

        Debug.Log($"[IntroManager] Selected stage {_selectedIndex}: {s.stageName}, bgm={(s.bgm != null ? s.bgm.name : "null")}, playing={(bgmSource != null && bgmSource.isPlaying)}");
    }

    // ── 이전/다음 버튼 ────────────────────────────────────────────────────────
    public void NextStage()
    {
        if (!HasStages()) return;
        SelectStage((_selectedIndex + 1) % stageList.stages.Length);
    }

    public void PrevStage()
    {
        if (!HasStages()) return;
        int n = stageList.stages.Length;
        SelectStage((_selectedIndex - 1 + n) % n);
    }

    // ── 게임 시작 버튼 ────────────────────────────────────────────────────────
    public void OnStartGame()
    {
        PlayerPrefs.SetInt("SelectedStage", _selectedIndex);
        PlayerPrefs.Save();
        StartCoroutine(FadeOutAndLoad());
    }

    // ── 배경 영상 교체 ────────────────────────────────────────────────────────
    void ApplyVideo(VideoClip clip)
    {
        if (backgroundPlayer == null) return;
        if (clip == null)
        {
            backgroundPlayer.Stop();
            backgroundPlayer.gameObject.SetActive(false);
            return;
        }

        backgroundPlayer.gameObject.SetActive(true);
        if (backgroundPlayer.clip == clip) return;
        backgroundPlayer.Stop();
        backgroundPlayer.clip = clip;
        backgroundPlayer.Play();
    }

    void ApplySkyboxStage(StageEntry stage)
    {
        if (stage.skyboxMaterial != null)
        {
            RenderSettings.skybox = stage.skyboxMaterial;
            DynamicGI.UpdateEnvironment();
        }

        if (gridRenderer == null) return;

        gridRenderer.enabled = stage.gridMaterial != null;
        if (stage.gridMaterial != null)
        {
            gridRenderer.sharedMaterial = stage.gridMaterial;
        }
    }

    bool HasStages() =>
        stageList != null && stageList.stages != null && stageList.stages.Length > 0;

    Color GetFallbackStageColor(string stageName)
    {
        if (stageName.Contains("Orange")) return new Color(1f, 0.35f, 0.08f, 0.95f);
        if (stageName.Contains("VHS")) return new Color(0.25f, 0.1f, 0.75f, 0.95f);
        if (stageName.Contains("Vapor")) return new Color(0.1f, 0.85f, 1f, 0.95f);
        return new Color(0.18f, 0.18f, 0.30f, 0.95f);
    }

    // ── 페이드 ────────────────────────────────────────────────────────────────
    IEnumerator FadeIn(float duration)
    {
        fadeOverlay.alpha = 1f;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            fadeOverlay.alpha = 1f - Mathf.Clamp01(t / duration);
            yield return null;
        }
        fadeOverlay.alpha = 0f;
    }

    IEnumerator FadeOutAndLoad()
    {
        SpawnLoadingVfx();

        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            fadeOverlay.alpha = Mathf.Clamp01(t);
            yield return null;
        }
        SceneManager.LoadScene(gameSceneName);
    }

    void SpawnLoadingVfx()
    {
        if (loadingVfxPrefab == null) return;

        Transform anchor = loadingVfxAnchor != null ? loadingVfxAnchor : transform;
        GameObject vfx = Instantiate(loadingVfxPrefab, anchor.position, anchor.rotation);
        vfx.transform.localScale = anchor.lossyScale;
        Destroy(vfx, loadingVfxLifetime);
    }
}
