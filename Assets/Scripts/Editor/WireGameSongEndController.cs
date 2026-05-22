using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class WireGameSongEndController
{
    private const string GameScenePath = "Assets/Scenes/Game.unity";

    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene(GameScenePath);
        GameBackgroundController backgroundController = Object.FindFirstObjectByType<GameBackgroundController>();
        if (backgroundController == null)
        {
            Debug.LogError("[WireGameSongEnd] GameBackgroundController not found.");
            return;
        }

        GameSongEndController endController = Object.FindFirstObjectByType<GameSongEndController>();
        if (endController == null)
        {
            endController = backgroundController.gameObject.AddComponent<GameSongEndController>();
        }

        SerializedObject serialized = new SerializedObject(endController);
        serialized.FindProperty("backgroundController").objectReferenceValue = backgroundController;
        serialized.FindProperty("introSceneName").stringValue = "Intro";
        serialized.FindProperty("returnDelay").floatValue = 1.5f;
        serialized.FindProperty("minimumPlayTime").floatValue = 1f;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(endController);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("[WireGameSongEnd] GameSongEndController wired.");
    }
}
