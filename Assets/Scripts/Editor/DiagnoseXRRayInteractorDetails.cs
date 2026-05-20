using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class DiagnoseXRRayInteractorDetails
{
    public static void Execute()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Intro.unity");

        foreach (var behaviour in UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            var typeName = behaviour.GetType().FullName ?? string.Empty;
            if (!typeName.Contains("XRRayInteractor", StringComparison.OrdinalIgnoreCase)
                && !typeName.Contains("InteractorLineVisual", StringComparison.OrdinalIgnoreCase)
                && !typeName.Contains("ControllerInputActionManager", StringComparison.OrdinalIgnoreCase)
                && !typeName.Contains("XRUIInputModule", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            Debug.Log($"[XR Ray Details] {GetPath(behaviour.transform)} :: {typeName} enabled={behaviour.enabled}");
            var serialized = new SerializedObject(behaviour);
            var iterator = serialized.GetIterator();
            var enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                var propertyName = iterator.name;
                var lower = propertyName.ToLowerInvariant();
                if (lower.Contains("ui")
                    || lower.Contains("select")
                    || lower.Contains("activate")
                    || lower.Contains("input")
                    || lower.Contains("raycast")
                    || lower.Contains("maxraycast")
                    || lower.Contains("interaction"))
                {
                    Debug.Log($"[XR Ray Details]   {iterator.propertyPath} ({iterator.propertyType}) = {PropertyValue(iterator)}");
                }
            }
        }
    }

    private static string PropertyValue(SerializedProperty property)
    {
        return property.propertyType switch
        {
            SerializedPropertyType.Boolean => property.boolValue.ToString(),
            SerializedPropertyType.Integer => property.intValue.ToString(),
            SerializedPropertyType.Float => property.floatValue.ToString("0.###"),
            SerializedPropertyType.Enum => property.enumDisplayNames.Length > property.enumValueIndex ? property.enumDisplayNames[property.enumValueIndex] : property.enumValueIndex.ToString(),
            SerializedPropertyType.ObjectReference => property.objectReferenceValue != null ? property.objectReferenceValue.name : "null",
            SerializedPropertyType.String => property.stringValue,
            _ => property.propertyType.ToString()
        };
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
