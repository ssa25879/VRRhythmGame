using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class TestPlayScreenshot
{
    private const string GameScenePath = "Assets/Scenes/Game.unity";
    private const string ScreenshotPath = "Assets/Screenshots/test_play_game.png";

    public static void OpenGameScene()
    {
        EditorSceneManager.OpenScene(GameScenePath);
        Debug.Log("[TestPlay] Game scene opened.");
    }

    public static void Capture()
    {
        Directory.CreateDirectory("Assets/Screenshots");
        ScreenCapture.CaptureScreenshot(ScreenshotPath);
        Debug.Log($"[TestPlay] Screenshot requested: {ScreenshotPath}");
    }
}
