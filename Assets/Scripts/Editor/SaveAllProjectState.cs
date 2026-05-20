using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SaveAllProjectState
{
    public static void Execute()
    {
        AssetDatabase.SaveAssets();
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.Refresh();
        Debug.Log("[SaveAllProjectState] Saved assets and open scenes.");
    }
}
