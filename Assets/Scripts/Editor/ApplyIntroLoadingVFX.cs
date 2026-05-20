using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ApplyIntroLoadingVFX
{
    private const string IntroScenePath = "Assets/Scenes/Intro.unity";
    private const string LoadingPrefabPath = "Assets/VisualX_Studio/Sci-Fi_Loading_Screen_Effects_FREE/VFX/Prefab/Loading_free_blue.prefab";

    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene(IntroScenePath);
        var manager = Object.FindFirstObjectByType<IntroManager>();
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(LoadingPrefabPath);

        if (manager == null)
        {
            Debug.LogError("IntroManager not found.");
            return;
        }

        if (prefab == null)
        {
            Debug.LogError($"Loading VFX prefab not found: {LoadingPrefabPath}");
            return;
        }

        var anchor = GameObject.Find("LoadingVFXAnchor");
        if (anchor == null)
        {
            anchor = new GameObject("LoadingVFXAnchor");
        }

        anchor.transform.position = new Vector3(0f, 0.2f, 2.15f);
        anchor.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        anchor.transform.localScale = Vector3.one * 0.38f;

        var serializedManager = new SerializedObject(manager);
        serializedManager.FindProperty("loadingVfxPrefab").objectReferenceValue = prefab;
        serializedManager.FindProperty("loadingVfxAnchor").objectReferenceValue = anchor.transform;
        serializedManager.FindProperty("loadingVfxLifetime").floatValue = 1.5f;
        serializedManager.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(manager);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Intro loading VFX applied.");
    }
}
