using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CaptureIntroSciFiUIPreview
{
    public static void Execute()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Intro.unity", OpenSceneMode.Single);

        var camera = Camera.main;
        if (camera == null)
        {
            Debug.LogError("[CaptureIntroSciFiUIPreview] Main Camera를 찾지 못했습니다.");
            return;
        }

        Directory.CreateDirectory("Assets/Screenshots");
        var path = "Assets/Screenshots/IntroSciFiUI_20260527.png";

        var previousTarget = camera.targetTexture;
        var previousClearFlags = camera.clearFlags;
        var previousBackground = camera.backgroundColor;

        var texture = new RenderTexture(1280, 720, 24);
        camera.targetTexture = texture;
        camera.clearFlags = CameraClearFlags.Skybox;
        camera.Render();

        RenderTexture.active = texture;
        var screenshot = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        screenshot.Apply();

        File.WriteAllBytes(path, screenshot.EncodeToPNG());

        RenderTexture.active = null;
        camera.targetTexture = previousTarget;
        camera.clearFlags = previousClearFlags;
        camera.backgroundColor = previousBackground;

        Object.DestroyImmediate(screenshot);
        texture.Release();
        Object.DestroyImmediate(texture);

        AssetDatabase.Refresh();
        Debug.Log($"[CaptureIntroSciFiUIPreview] Saved {path}");
    }
}
