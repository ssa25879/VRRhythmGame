using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class FixXRUIInteraction
{
    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Intro.unity");
        var changedCount = 0;

        foreach (var behaviour in Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            var typeName = behaviour.GetType().FullName ?? string.Empty;
            if (!typeName.Contains("XRRayInteractor"))
            {
                continue;
            }

            var serializedObject = new SerializedObject(behaviour);
            var enableUi = serializedObject.FindProperty("m_EnableUIInteraction");
            if (enableUi == null)
            {
                continue;
            }

            if (!enableUi.boolValue)
            {
                enableUi.boolValue = true;
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(behaviour);
                changedCount++;
                Debug.Log($"Enabled UI Interaction on {GetPath(behaviour.transform)}");
            }
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log($"XR UI Interaction fix complete. Changed XRRayInteractor count: {changedCount}");
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
