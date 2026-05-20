using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SaveCurrentScene
{
    public static void Execute()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        bool ok = EditorSceneManager.SaveScene(scene, scene.path);
        Debug.Log(ok
            ? $"[SaveCurrentScene] 저장 완료: {scene.path}"
            : $"[SaveCurrentScene] 저장 실패: {scene.path}");
    }
}
