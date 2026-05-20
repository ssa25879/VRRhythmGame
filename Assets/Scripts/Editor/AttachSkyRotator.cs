using UnityEditor;
using UnityEngine;

public class AttachSkyRotator
{
    public static void Execute()
    {
        var skyDome = GameObject.Find("SkyDome360");
        if (skyDome == null) { Debug.LogError("[SkyRotator] SkyDome360 없음"); return; }

        if (skyDome.GetComponent<SkyRotator>() == null)
            skyDome.AddComponent<SkyRotator>();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("[SkyRotator] SkyDome360에 SkyRotator 추가 완료");
    }
}
