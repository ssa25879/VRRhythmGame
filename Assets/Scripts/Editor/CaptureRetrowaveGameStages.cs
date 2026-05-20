using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CaptureRetrowaveGameStages
{
    private const string GameScenePath = "Assets/Scenes/Game.unity";
    private const string ScreenshotFolder = "Assets/Screenshots/RetrowaveGameStages";

    public static void SetVapor()
    {
        SetStage("Retrowave Vapor");
    }

    public static void SetOrange()
    {
        SetStage("Retrowave Orange");
    }

    public static void SetVhs()
    {
        SetStage("Retrowave VHS");
    }

    public static void CaptureVapor()
    {
        Capture("retrowave_game_vapor.png");
    }

    public static void CaptureOrange()
    {
        Capture("retrowave_game_orange.png");
    }

    public static void CaptureVhs()
    {
        Capture("retrowave_game_vhs.png");
    }

    private static void SetStage(string stageName)
    {
        EditorSceneManager.OpenScene(GameScenePath);
        var stageList = AssetDatabase.LoadAssetAtPath<StageListSO>("Assets/Data/StageList.asset");
        if (stageList == null || stageList.stages == null)
        {
            Debug.LogError("[RetrowaveGameCapture] StageList is missing.");
            return;
        }

        for (int i = 0; i < stageList.stages.Length; i++)
        {
            if (stageList.stages[i].stageName == stageName)
            {
                PlayerPrefs.SetInt("SelectedStage", i);
                PlayerPrefs.Save();
                Debug.Log($"[RetrowaveGameCapture] SelectedStage set to {i} ({stageName}).");
                return;
            }
        }

        Debug.LogError($"[RetrowaveGameCapture] Stage not found: {stageName}");
    }

    private static void Capture(string fileName)
    {
        Directory.CreateDirectory(ScreenshotFolder);
        var path = Path.Combine(ScreenshotFolder, fileName).Replace("\\", "/");
        ScreenCapture.CaptureScreenshot(path);
        Debug.Log($"[RetrowaveGameCapture] Screenshot requested: {path}");
    }
}
