using UnityEditor;
using UnityEngine;

public static class SetRetrowavePreviewStage
{
    public static void Execute()
    {
        var stageList = AssetDatabase.LoadAssetAtPath<StageListSO>("Assets/Data/StageList.asset");
        if (stageList == null || stageList.stages == null)
        {
            Debug.LogError("[RetrowavePreview] StageList.asset is missing or empty.");
            return;
        }

        for (int i = 0; i < stageList.stages.Length; i++)
        {
            if (stageList.stages[i].stageName == "Retrowave Vapor")
            {
                PlayerPrefs.SetInt("SelectedStage", i);
                PlayerPrefs.Save();
                Debug.Log($"[RetrowavePreview] SelectedStage set to {i} ({stageList.stages[i].stageName}).");
                return;
            }
        }

        Debug.LogError("[RetrowavePreview] Retrowave Vapor stage was not found.");
    }
}
