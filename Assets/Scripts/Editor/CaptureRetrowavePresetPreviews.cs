using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CaptureRetrowavePresetPreviews
{
    private const int Width = 1280;
    private const int Height = 720;

    private static readonly (string scenePath, string fileName)[] Presets =
    {
        ("Assets/Suggo Creations/RETROWAVE SKIES Lite/Demo Scenes/Vapor_PresetScene Lite.unity", "retrowave_vapor.png"),
        ("Assets/Suggo Creations/RETROWAVE SKIES Lite/Demo Scenes/Orange_PresetScene Lite.unity", "retrowave_orange.png"),
        ("Assets/Suggo Creations/RETROWAVE SKIES Lite/Demo Scenes/VHS_PresetScene Lite.unity", "retrowave_vhs.png")
    };

    public static void Execute()
    {
        Directory.CreateDirectory("Assets/Screenshots/RetrowavePresets");

        foreach (var preset in Presets)
        {
            EditorSceneManager.OpenScene(preset.scenePath);
            var camera = Object.FindFirstObjectByType<Camera>();
            if (camera == null)
            {
                var cameraObject = new GameObject("Preview Camera");
                camera = cameraObject.AddComponent<Camera>();
                camera.transform.position = new Vector3(0f, 2.2f, -7f);
                camera.transform.rotation = Quaternion.Euler(12f, 0f, 0f);
            }

            camera.clearFlags = CameraClearFlags.Skybox;
            camera.fieldOfView = 72f;
            Render(camera, Path.Combine("Assets/Screenshots/RetrowavePresets", preset.fileName));
        }

        EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");
        Debug.Log("[RetrowavePreview] Preset screenshots captured.");
    }

    private static void Render(Camera camera, string path)
    {
        var renderTexture = new RenderTexture(Width, Height, 24);
        var texture = new Texture2D(Width, Height, TextureFormat.RGB24, false);
        var previousTarget = camera.targetTexture;
        var previousActive = RenderTexture.active;

        camera.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;
        camera.Render();
        texture.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
        texture.Apply();

        File.WriteAllBytes(path, texture.EncodeToPNG());

        camera.targetTexture = previousTarget;
        RenderTexture.active = previousActive;
        Object.DestroyImmediate(texture);
        Object.DestroyImmediate(renderTexture);
    }
}
