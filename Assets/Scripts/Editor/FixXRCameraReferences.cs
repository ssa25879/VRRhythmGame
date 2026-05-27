using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class FixXRCameraReferences
{
    private static readonly string[] ScenePaths =
    {
        "Assets/Scenes/Intro.unity",
        "Assets/Scenes/Game.unity",
    };

    public static void Execute()
    {
        foreach (var scenePath in ScenePaths)
        {
            var scene = EditorSceneManager.OpenScene(scenePath);
            var xrCamera = FindXRCamera();

            if (xrCamera == null)
            {
                Debug.LogWarning($"[XR Camera Fix] {scenePath}: XR Origin camera not found.");
                continue;
            }

            FixCameraTagAndListeners(xrCamera);
            FixCanvasCameraReferences(xrCamera);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[XR Camera Fix] {scenePath}: camera={GetPath(xrCamera.transform)}");
        }
    }

    private static Camera FindXRCamera()
    {
        var cameras = UnityEngine.Object.FindObjectsByType<Camera>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        return cameras
            .OrderByDescending(camera => GetPath(camera.transform).Contains("XR Origin", StringComparison.OrdinalIgnoreCase))
            .ThenByDescending(camera => camera.name.Equals("Main Camera", StringComparison.OrdinalIgnoreCase))
            .ThenByDescending(camera => GetPath(camera.transform).Contains("Camera Offset", StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault(camera => GetPath(camera.transform).Contains("XR", StringComparison.OrdinalIgnoreCase)
                || GetPath(camera.transform).Contains("Camera Offset", StringComparison.OrdinalIgnoreCase));
    }

    private static void FixCameraTagAndListeners(Camera xrCamera)
    {
        xrCamera.gameObject.SetActive(true);
        xrCamera.enabled = true;
        xrCamera.tag = "MainCamera";

        var allCameras = UnityEngine.Object.FindObjectsByType<Camera>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        foreach (var camera in allCameras)
        {
            if (camera == xrCamera)
            {
                continue;
            }

            if (camera.CompareTag("MainCamera"))
            {
                camera.tag = "Untagged";
            }
        }

        var xrListener = xrCamera.GetComponent<AudioListener>();
        if (xrListener == null)
        {
            xrListener = xrCamera.gameObject.AddComponent<AudioListener>();
        }

        xrListener.enabled = true;

        var listeners = UnityEngine.Object.FindObjectsByType<AudioListener>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        foreach (var listener in listeners)
        {
            if (listener != xrListener)
            {
                listener.enabled = false;
            }
        }
    }

    private static void FixCanvasCameraReferences(Camera xrCamera)
    {
        var canvases = UnityEngine.Object.FindObjectsByType<Canvas>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        foreach (var canvas in canvases)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                continue;
            }

            canvas.worldCamera = xrCamera;
        }
    }

    private static string GetPath(Transform transform)
    {
        var path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }

        return path;
    }
}
