using UnityEngine;
using UnityEngine.Video;
using UnityEditor;
using UnityEditor.SceneManagement;

public class GameSceneSetup
{
    public static void Execute()
    {
        // ── GameBackgroundController 오브젝트 생성 ─────────────────────────
        var existing = GameObject.Find("GameBackgroundController");
        if (existing != null) Object.DestroyImmediate(existing);

        var go  = new GameObject("GameBackgroundController");
        var ctrl = go.AddComponent<GameBackgroundController>();

        // BGM용 AudioSource 추가
        var bgmSrc = go.AddComponent<AudioSource>();
        bgmSrc.loop        = true;
        bgmSrc.volume      = 0.7f;
        bgmSrc.playOnAwake = false;
        ctrl.bgmSource = bgmSrc;

        // "Video" 오브젝트의 VideoPlayer 연결
        var videoGo = GameObject.Find("Video");
        if (videoGo != null)
        {
            var vp = videoGo.GetComponent<VideoPlayer>();
            ctrl.backgroundPlayer = vp;
            Debug.Log("[GameSceneSetup] backgroundPlayer 연결: Video");
        }
        else
        {
            Debug.LogWarning("[GameSceneSetup] 'Video' 오브젝트를 찾을 수 없습니다.");
        }

        // stageList는 에셋 생성 후 수동으로 드래그하거나
        // StageList.asset이 존재하면 자동 연결
        var stageList = AssetDatabase.LoadAssetAtPath<StageListSO>("Assets/Data/StageList.asset");
        if (stageList != null)
        {
            ctrl.stageList = stageList;
            Debug.Log("[GameSceneSetup] stageList 연결 완료");
        }
        else
        {
            Debug.Log("[GameSceneSetup] StageList.asset 없음 — 에셋 생성 후 드래그 필요");
        }

        EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("[GameSceneSetup] GameBackgroundController 설정 완료");
    }
}
