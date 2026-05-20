using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ApplyJudgementTuning
{
    private const string GameScenePath = "Assets/Scenes/Game.unity";
    private const string RedPrefabPath = "Assets/Prefab/RED.prefab";
    private const string BluePrefabPath = "Assets/Prefab/BLUE.prefab";

    public static void Execute()
    {
        TuneNotePrefab(RedPrefabPath);
        TuneNotePrefab(BluePrefabPath);

        var scene = EditorSceneManager.OpenScene(GameScenePath);
        int tunedSabers = 0;

        foreach (var saber in FindSceneSabers(scene))
        {
            var blade = FindBlade(saber.transform);
            if (blade == null)
            {
                continue;
            }

            var tip = blade.Find("Blade Tip");
            if (tip == null)
            {
                var tipObject = new GameObject("Blade Tip");
                tipObject.transform.SetParent(blade, false);
                tip = tipObject.transform;
            }

            tip.localPosition = new Vector3(0f, 0f, 0.72f);
            tip.localRotation = Quaternion.identity;
            tip.localScale = Vector3.one * 0.08f;

            saber.bladeRoot = blade;
            saber.bladeTip = tip;
            saber.hitRadius = 0.24f;
            saber.minSwingSpeed = 0.35f;
            saber.directionTolerance = 85f;
            EditorUtility.SetDirty(saber);
            EditorUtility.SetDirty(tip.gameObject);
            PrefabUtility.RecordPrefabInstancePropertyModifications(saber);
            tunedSabers++;
        }

        var spawner = Object.FindFirstObjectByType<Spawner>();
        if (spawner != null)
        {
            var serializedSpawner = new SerializedObject(spawner);
            serializedSpawner.FindProperty("noteScale").floatValue = 0.38f;
            serializedSpawner.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(spawner);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log($"Judgement tuning applied. Tuned sabers={tunedSabers}");
    }

    private static void TuneNotePrefab(string path)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogWarning($"Note prefab not found: {path}");
            return;
        }

        var root = prefab.transform;
        root.localScale = Vector3.one * 0.38f;

        var collider = prefab.GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.size = Vector3.one;
            collider.center = Vector3.zero;
        }

        EditorUtility.SetDirty(prefab);
    }

    private static Saber[] FindSceneSabers(Scene scene)
    {
        var sabers = new System.Collections.Generic.List<Saber>();
        foreach (var root in scene.GetRootGameObjects())
        {
            sabers.AddRange(root.GetComponentsInChildren<Saber>(true));
        }

        return sabers.ToArray();
    }

    private static Transform FindBlade(Transform saber)
    {
        foreach (Transform child in saber.GetComponentsInChildren<Transform>(true))
        {
            if (child != saber && child.GetComponent<Renderer>() != null && child.GetComponent<Collider>() != null)
            {
                return child;
            }
        }

        return null;
    }
}
