using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AddNearFarUIInteractors
{
    private const string LeftPrefabPath = "Assets/Samples/XR Interaction Toolkit/3.3.0/Starter Assets/Prefabs/Interactors/Left_NearFarInteractor.prefab";
    private const string RightPrefabPath = "Assets/Samples/XR Interaction Toolkit/3.3.0/Starter Assets/Prefabs/Interactors/Right_NearFarInteractor.prefab";

    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Intro.unity");

        var changed = false;
        changed |= EnsureInteractor("Left Controller", "Left_NearFarInteractor", LeftPrefabPath);
        changed |= EnsureInteractor("Right Controller", "Right_NearFarInteractor", RightPrefabPath);

        if (changed)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
        }

        Debug.Log($"Near-Far UI Interactor setup complete. Changed={changed}");
    }

    private static bool EnsureInteractor(string controllerName, string interactorName, string prefabPath)
    {
        var controller = GameObject.Find(controllerName);
        if (controller == null)
        {
            Debug.LogError($"Controller not found: {controllerName}");
            return false;
        }

        var existing = controller.transform.Find(interactorName);
        if (existing != null)
        {
            existing.gameObject.SetActive(true);
            EnableUIInteraction(existing.gameObject);
            Debug.Log($"Existing Near-Far interactor enabled: {GetPath(existing)}");
            return true;
        }

        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"Near-Far interactor prefab not found: {prefabPath}");
            return false;
        }

        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.name = interactorName;
        instance.transform.SetParent(controller.transform, false);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;
        instance.SetActive(true);
        EnableUIInteraction(instance);

        Debug.Log($"Near-Far interactor added: {GetPath(instance.transform)}");
        return true;
    }

    private static void EnableUIInteraction(GameObject root)
    {
        foreach (var behaviour in root.GetComponentsInChildren<MonoBehaviour>(true))
        {
            var typeName = behaviour.GetType().FullName ?? string.Empty;
            if (!typeName.Contains("XRRayInteractor"))
            {
                continue;
            }

            var serializedObject = new SerializedObject(behaviour);
            var enableUi = serializedObject.FindProperty("m_EnableUIInteraction");
            if (enableUi != null)
            {
                enableUi.boolValue = true;
            }

            var blockUi = serializedObject.FindProperty("m_BlockUIOnInteractableSelection");
            if (blockUi != null)
            {
                blockUi.boolValue = false;
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(behaviour);
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
