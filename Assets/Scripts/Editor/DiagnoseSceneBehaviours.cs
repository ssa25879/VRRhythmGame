using UnityEditor.SceneManagement;
using UnityEngine;

public static class DiagnoseSceneBehaviours
{
    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");
        int roots = scene.GetRootGameObjects().Length;
        int behaviours = 0;
        int sabers = 0;

        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (var behaviour in root.GetComponentsInChildren<MonoBehaviour>(true))
            {
                behaviours++;
                Debug.Log($"[BehaviourDiag] {behaviour.GetType().FullName} on {GetPath(behaviour.transform)}");
                if (behaviour.GetType().Name == "Saber")
                {
                    sabers++;
                }
            }
        }

        Debug.Log($"[BehaviourDiag] roots={roots}, behaviours={behaviours}, sabersByName={sabers}");
    }

    private static string GetPath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }

        return path;
    }
}
