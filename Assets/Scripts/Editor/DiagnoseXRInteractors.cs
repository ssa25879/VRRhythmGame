using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class DiagnoseXRInteractors
{
    public static void Execute()
    {
        DiagnoseScene("Assets/Scenes/Intro.unity");
        DiagnoseScene("Assets/Scenes/Game.unity");
    }

    private static void DiagnoseScene(string scenePath)
    {
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        Debug.Log($"[XR Interactor Diagnose] Scene={scenePath}");

        var behaviours = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        foreach (var behaviour in behaviours.OrderBy(b => GetPath(b.transform)))
        {
            if (behaviour == null)
                continue;

            var type = behaviour.GetType();
            var fullName = type.FullName ?? type.Name;
            if (!fullName.Contains("XRRayInteractor", StringComparison.Ordinal) &&
                !fullName.Contains("NearFarInteractor", StringComparison.Ordinal) &&
                !fullName.Contains("XRPokeInteractor", StringComparison.Ordinal) &&
                !fullName.Contains("TeleportInteractor", StringComparison.Ordinal) &&
                !fullName.Contains("ControllerInputActionManager", StringComparison.Ordinal) &&
                !fullName.Contains("TrackedDeviceGraphicRaycaster", StringComparison.Ordinal) &&
                !fullName.Contains("GraphicRaycaster", StringComparison.Ordinal) &&
                !fullName.Contains("XRUIInputModule", StringComparison.Ordinal) &&
                !fullName.Contains("InputSystemUIInputModule", StringComparison.Ordinal))
            {
                continue;
            }

            Debug.Log(
                $"[XR Interactor Diagnose] path={GetPath(behaviour.transform)} type={fullName} " +
                $"goActive={behaviour.gameObject.activeInHierarchy} enabled={behaviour.enabled}");
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
