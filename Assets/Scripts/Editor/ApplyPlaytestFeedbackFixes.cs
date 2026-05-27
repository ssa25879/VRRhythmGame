using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TMPro;

public static class ApplyPlaytestFeedbackFixes
{
    public static void Execute()
    {
        FixIntroCanvas();
        FixGameScene();
        AssetDatabase.SaveAssets();
        Debug.Log("[PlaytestFeedbackFix] Intro UI, saber layers, hit VFX, and HUD adjusted.");
    }

    private static void FixIntroCanvas()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Intro.unity", OpenSceneMode.Single);
        var canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[PlaytestFeedbackFix] Intro Canvas not found.");
            return;
        }

        var transform = canvas.transform;
        transform.localPosition = new Vector3(0f, -0.22f, 2.5f);

        if (transform is RectTransform rect)
        {
            rect.anchoredPosition3D = new Vector3(0f, -0.22f, 2.5f);
        }

        EditorUtility.SetDirty(transform);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void FixGameScene()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Game.unity", OpenSceneMode.Single);

        int blueLayer = LayerMask.NameToLayer("Blue");
        int redLayer = LayerMask.NameToLayer("Red");
        if (blueLayer < 0 || redLayer < 0)
        {
            Debug.LogError($"[PlaytestFeedbackFix] Missing layers. Blue={blueLayer}, Red={redLayer}");
            return;
        }

        foreach (var saber in Object.FindObjectsByType<Saber>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            string lowerName = saber.gameObject.name.ToLowerInvariant();
            var serializedObject = new SerializedObject(saber);
            if (lowerName.Contains("blue"))
            {
                serializedObject.FindProperty("layer").intValue = 1 << blueLayer;
            }
            else if (lowerName.Contains("red"))
            {
                serializedObject.FindProperty("layer").intValue = 1 << redLayer;
            }

            serializedObject.FindProperty("hitEffectScale").floatValue = 0.18f;
            serializedObject.FindProperty("hitEffectLifetime").floatValue = 0.55f;
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(saber);
        }

        var score = Object.FindFirstObjectByType<GameScoreController>();
        if (score != null)
        {
            var serializedObject = new SerializedObject(score);
            serializedObject.FindProperty("panelSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VRTemplateAssets/Sprites/UI/Round Radius 10.png");
            serializedObject.FindProperty("panelOutlineSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VRTemplateAssets/Sprites/UI/Round Radius 10 Outline.png");
            serializedObject.FindProperty("hpFrameSprite").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VRTemplateAssets/Sprites/UI/Circle_60x60_Vertical.png");
            serializedObject.FindProperty("hudFont").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/VRTemplateAssets/Fonts/Inter/Inter-Regular SDF.asset");
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(score);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }
}
