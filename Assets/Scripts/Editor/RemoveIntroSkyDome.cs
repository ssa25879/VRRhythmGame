using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class RemoveIntroSkyDome
{
    [MenuItem("Tools/Remove Intro SkyDome360")]
    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Intro.unity", OpenSceneMode.Single);

        var skyDome = GameObject.Find("SkyDome360");
        if (skyDome != null)
        {
            Object.DestroyImmediate(skyDome);
            Debug.Log("[RemoveIntroSkyDome] SkyDome360 removed.");
        }
        else
        {
            Debug.LogWarning("[RemoveIntroSkyDome] SkyDome360 not found.");
        }

        var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        bool ok = EditorSceneManager.SaveScene(activeScene, activeScene.path);
        Debug.Log($"[RemoveIntroSkyDome] Scene saved: {ok}");
    }
}
