using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TMPro;

public static class WireGameScoreController
{
    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Game.unity", OpenSceneMode.Single);
        var background = Object.FindFirstObjectByType<GameBackgroundController>();
        var spawner = Object.FindFirstObjectByType<Spawner>();

        if (background == null)
        {
            Debug.LogError("[WireGameScore] GameBackgroundController not found.");
            return;
        }

        var controller = Object.FindFirstObjectByType<GameScoreController>();
        if (controller == null)
        {
            controller = background.gameObject.AddComponent<GameScoreController>();
        }

        var serializedObject = new SerializedObject(controller);
        serializedObject.FindProperty("backgroundController").objectReferenceValue = background;
        serializedObject.FindProperty("spawner").objectReferenceValue = spawner;
        serializedObject.FindProperty("maxScore").intValue = 100000;
        serializedObject.FindProperty("baseScoreRatio").floatValue = 0.7f;
        serializedObject.FindProperty("maxHp").floatValue = 100f;
        serializedObject.FindProperty("missHpDamage").floatValue = 12f;
        serializedObject.FindProperty("badHpDamageRatio").floatValue = 0.333f;
        serializedObject.FindProperty("panelSprite").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VRTemplateAssets/Sprites/UI/Round Radius 10.png");
        serializedObject.FindProperty("panelOutlineSprite").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VRTemplateAssets/Sprites/UI/Round Radius 10 Outline.png");
        serializedObject.FindProperty("hpFrameSprite").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VRTemplateAssets/Sprites/UI/Circle_60x60_Vertical.png");
        serializedObject.FindProperty("hudFont").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/VRTemplateAssets/Fonts/Inter/Inter-Regular SDF.asset");
        serializedObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(controller);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        Debug.Log("[WireGameScore] GameScoreController wired and Game scene saved.");
    }
}
