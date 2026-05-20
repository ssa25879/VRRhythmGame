using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class VerifyEditorScopeTasks
{
    private const string StageListPath = "Assets/Data/StageList.asset";
    private const string IntroScenePath = "Assets/Scenes/Intro.unity";
    private const string GameScenePath = "Assets/Scenes/Game.unity";

    public static void Execute()
    {
        VerifyStageList();
        VerifyIntroText();
        VerifyGameSceneReferences();
        EditorSceneManager.OpenScene(GameScenePath);
        Debug.Log("[EditorScopeVerify] Completed editor-scope verification.");
    }

    private static void VerifyStageList()
    {
        var stageList = AssetDatabase.LoadAssetAtPath<StageListSO>(StageListPath);
        if (stageList == null || stageList.stages == null)
        {
            Debug.LogError("[EditorScopeVerify] StageList is missing.");
            return;
        }

        Debug.Log($"[EditorScopeVerify] Stage count: {stageList.stages.Length}");
        VerifyStage(stageList, "Retrowave Vapor", 124f);
        VerifyStage(stageList, "Retrowave Orange", 110f);
        VerifyStage(stageList, "Retrowave VHS", 132f);
    }

    private static void VerifyStage(StageListSO stageList, string stageName, float expectedBpm)
    {
        var stage = stageList.stages.FirstOrDefault(item => item.stageName == stageName);
        if (stage == null)
        {
            Debug.LogError($"[EditorScopeVerify] Missing stage: {stageName}");
            return;
        }

        if (stage.skyboxMaterial == null || stage.gridMaterial == null || stage.bgm == null)
        {
            Debug.LogError($"[EditorScopeVerify] {stageName} has missing references. skybox={stage.skyboxMaterial != null}, grid={stage.gridMaterial != null}, bgm={stage.bgm != null}");
            return;
        }

        if (!Mathf.Approximately(stage.bpm, expectedBpm))
        {
            Debug.LogError($"[EditorScopeVerify] {stageName} BPM mismatch. expected={expectedBpm}, actual={stage.bpm}");
            return;
        }

        Debug.Log($"[EditorScopeVerify] {stageName}: skybox={stage.skyboxMaterial.name}, grid={stage.gridMaterial.name}, bgm={stage.bgm.name}, bpm={stage.bpm}");
    }

    private static void VerifyIntroText()
    {
        EditorSceneManager.OpenScene(IntroScenePath);
        var texts = Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var text in texts)
        {
            if (text.text.Contains("▶") || text.text.Contains("◀"))
            {
                Debug.LogError($"[EditorScopeVerify] Intro text still contains unsupported arrow symbol: {text.name} => {text.text}");
            }
        }

        Debug.Log($"[EditorScopeVerify] Intro TMP text count checked: {texts.Length}");
    }

    private static void VerifyGameSceneReferences()
    {
        EditorSceneManager.OpenScene(GameScenePath);
        var controller = Object.FindFirstObjectByType<GameBackgroundController>();
        if (controller == null)
        {
            Debug.LogError("[EditorScopeVerify] GameBackgroundController is missing.");
            return;
        }

        if (controller.stageList == null || controller.bgmSource == null || controller.gridRenderer == null)
        {
            Debug.LogError($"[EditorScopeVerify] GameBackgroundController references missing. stageList={controller.stageList != null}, bgmSource={controller.bgmSource != null}, gridRenderer={controller.gridRenderer != null}");
            return;
        }

        Debug.Log($"[EditorScopeVerify] GameBackgroundController references OK. grid={controller.gridRenderer.name}, bgmLoop={controller.bgmSource.loop}");
    }
}
