using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class FixXRPointerRegistrationErrors
{
    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Intro.unity", OpenSceneMode.Single);
        var changed = false;

        foreach (var behaviour in UnityEngine.Object.FindObjectsByType<MonoBehaviour>(
                     FindObjectsInactive.Include,
                     FindObjectsSortMode.None))
        {
            if (behaviour == null)
                continue;

            var typeName = behaviour.GetType().FullName ?? behaviour.GetType().Name;
            var objectName = behaviour.gameObject.name;
            var path = GetPath(behaviour.transform);

            if (typeName.Contains("XRPokeInteractor", StringComparison.Ordinal))
            {
                changed |= SetEnabled(behaviour, false, path, "Poke interactor is not needed for controller UI ray testing.");
                continue;
            }

            if (typeName.Contains("NearFarInteractor", StringComparison.Ordinal) &&
                objectName != "Left_NearFarInteractor" &&
                objectName != "Right_NearFarInteractor")
            {
                changed |= SetEnabled(behaviour, false, path, "Duplicate Near-Far interactor consumes XR UI pointer indices.");
                continue;
            }

            if (typeName.Contains("XRRayInteractor", StringComparison.Ordinal) &&
                objectName == "Teleport Interactor")
            {
                changed |= DisableUiInteraction(behaviour, path);
            }
        }

        if (changed)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
        }

        Debug.Log($"[XR Pointer Fix] Complete. Changed={changed}");
    }

    private static bool SetEnabled(MonoBehaviour behaviour, bool enabled, string path, string reason)
    {
        if (behaviour.enabled == enabled)
            return false;

        behaviour.enabled = enabled;
        EditorUtility.SetDirty(behaviour);
        Debug.Log($"[XR Pointer Fix] {(enabled ? "Enabled" : "Disabled")} {behaviour.GetType().Name} at {path}. {reason}");
        return true;
    }

    private static bool DisableUiInteraction(MonoBehaviour behaviour, string path)
    {
        var serializedObject = new SerializedObject(behaviour);
        var enableUi = serializedObject.FindProperty("m_EnableUIInteraction");
        if (enableUi == null || !enableUi.boolValue)
            return false;

        enableUi.boolValue = false;
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(behaviour);
        Debug.Log($"[XR Pointer Fix] Disabled UI interaction on teleport ray at {path}.");
        return true;
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
