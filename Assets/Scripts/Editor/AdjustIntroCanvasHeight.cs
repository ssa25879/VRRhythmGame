using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AdjustIntroCanvasHeight
{
    public static void Execute()
    {
        const string scenePath = "Assets/Scenes/Intro.unity";
        var scene = EditorSceneManager.OpenScene(scenePath);

        var canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("Canvas not found in Intro scene.");
            return;
        }

        var rectTransform = canvas.GetComponent<RectTransform>();
        var before = canvas.transform.localPosition;

        if (rectTransform != null)
        {
            rectTransform.localPosition = new Vector3(0f, -0.35f, 2.5f);
            rectTransform.anchoredPosition3D = new Vector3(0f, -0.35f, 2.5f);
            rectTransform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        }
        else
        {
            canvas.transform.localPosition = new Vector3(0f, -0.35f, 2.5f);
            canvas.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        Debug.Log($"Adjusted Intro Canvas local height: {before} -> {canvas.transform.localPosition}");
    }
}
