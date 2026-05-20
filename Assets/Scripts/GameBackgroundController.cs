using UnityEngine;
using UnityEngine.Video;

// ─────────────────────────────────────────────────────────────────────────────
// GameBackgroundController — Game 씬 배경 + BGM 적용
//
// Intro 씬에서 선택한 스테이지 인덱스를 PlayerPrefs로 수신해
// 해당 배경 영상과 BGM을 재생합니다.
//
// ★ 세팅 방법:
//   1. Game 씬의 아무 GameObject에 이 컴포넌트를 추가
//   2. Stage List → IntroManager와 동일한 StageList.asset 드래그
//   3. Background Player → "Video" 오브젝트의 VideoPlayer 드래그
//   4. Bgm Source → BGM용 AudioSource 드래그 (없으면 자동 생성됨)
// ─────────────────────────────────────────────────────────────────────────────
public class GameBackgroundController : MonoBehaviour
{
    [Header("스테이지 목록 (IntroManager와 동일한 StageList.asset 드래그)")]
    public StageListSO stageList;

    [Header("배경 VideoPlayer")]
    public VideoPlayer backgroundPlayer;

    [Header("BGM AudioSource")]
    public AudioSource bgmSource;

    [Header("Skybox 스테이지")]
    public Renderer gridRenderer;

    void Awake()
    {
        // bgmSource가 없으면 자동 생성
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.volume = 0.7f;
        }
        bgmSource.loop = true;

        int index = PlayerPrefs.GetInt("SelectedStage", 0);
        ApplyStage(index);
    }

    void ApplyStage(int index)
    {
        if (stageList == null || stageList.stages == null || stageList.stages.Length == 0)
        {
            Debug.LogWarning("[GameBackgroundController] StageList가 비어 있습니다.");
            return;
        }

        index = Mathf.Clamp(index, 0, stageList.stages.Length - 1);
        var stage = stageList.stages[index];

        if (backgroundPlayer != null && stage.backgroundVideo != null)
        {
            backgroundPlayer.gameObject.SetActive(true);
            backgroundPlayer.clip = stage.backgroundVideo;
            backgroundPlayer.Play();
        }
        else if (backgroundPlayer != null)
        {
            backgroundPlayer.Stop();
            backgroundPlayer.gameObject.SetActive(false);
        }

        if (stage.skyboxMaterial != null)
        {
            RenderSettings.skybox = stage.skyboxMaterial;
        }

        if (gridRenderer != null)
        {
            gridRenderer.enabled = stage.gridMaterial != null;
            if (stage.gridMaterial != null)
            {
                gridRenderer.sharedMaterial = stage.gridMaterial;
            }
        }

        if (bgmSource != null && stage.bgm != null)
        {
            bgmSource.clip = stage.bgm;
            bgmSource.Play();
        }
    }
}
