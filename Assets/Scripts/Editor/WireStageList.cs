using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class WireStageList
{
    /// Game 씬: GameBackgroundController → StageList 연결
    public static void Execute()
    {
        var stageList = AssetDatabase.LoadAssetAtPath<StageListSO>("Assets/Data/StageList.asset");
        if (stageList == null) { Debug.LogError("[WireStageList] Assets/Data/StageList.asset 없음"); return; }

        // GameBackgroundController 연결 (Game 씬)
        var ctrl = Object.FindFirstObjectByType<GameBackgroundController>();
        if (ctrl != null)
        {
            ctrl.stageList = stageList;
            Debug.Log("[WireStageList] GameBackgroundController.stageList 연결 완료");
        }

        // IntroManager 연결 (Intro 씬에 있을 경우)
        var mgr = Object.FindFirstObjectByType<IntroManager>();
        if (mgr != null)
        {
            mgr.stageList = stageList;
            Debug.Log("[WireStageList] IntroManager.stageList 연결 완료");
        }

        EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("[WireStageList] 연결 완료 — Ctrl+S 로 씬 저장 필요");
    }
}
