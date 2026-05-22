using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

public static class DiagnoseIntroPcView
{
    private const string IntroScenePath = "Assets/Scenes/Intro.unity";
    private static readonly Vector3 PcVisibleCanvasPosition = new Vector3(0f, 1.2f, 2.5f);

    public static void Execute()
    {
        EditorSceneManager.OpenScene(IntroScenePath);

        foreach (Camera camera in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
        {
            Debug.Log(
                $"[IntroPCView] Camera name={camera.name}, active={camera.gameObject.activeInHierarchy}, " +
                $"enabled={camera.enabled}, tag={camera.tag}, pos={camera.transform.position}, " +
                $"rot={camera.transform.eulerAngles}, depth={camera.depth}, targetDisplay={camera.targetDisplay}, " +
                $"cullingMask={camera.cullingMask}");
        }

        foreach (Canvas canvas in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            Debug.Log(
                $"[IntroPCView] Canvas name={canvas.name}, active={canvas.gameObject.activeInHierarchy}, " +
                $"enabled={canvas.enabled}, mode={canvas.renderMode}, worldCamera={(canvas.worldCamera != null ? canvas.worldCamera.name : "null")}, " +
                $"pos={canvas.transform.position}, rot={canvas.transform.eulerAngles}, scale={canvas.transform.lossyScale}, " +
                $"planeDistance={canvas.planeDistance}, sortingOrder={canvas.sortingOrder}");
        }
    }

    public static void FixPcClickVisibility()
    {
        EditorSceneManager.OpenScene(IntroScenePath);

        Camera mainCamera = Camera.main;
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (mainCamera == null)
        {
            Debug.LogError("[IntroPCView] Main Camera not found.");
            return;
        }

        if (canvas == null)
        {
            Debug.LogError("[IntroPCView] Canvas not found.");
            return;
        }

        Undo.RecordObject(canvas.transform, "Move Intro Canvas For PC View");
        Undo.RecordObject(canvas, "Assign Intro Canvas Camera");

        RectTransform rectTransform = canvas.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Undo.RecordObject(rectTransform, "Move Intro Canvas For PC View");
            rectTransform.localPosition = PcVisibleCanvasPosition;
            rectTransform.anchoredPosition3D = PcVisibleCanvasPosition;
        }

        canvas.transform.position = PcVisibleCanvasPosition;
        canvas.transform.rotation = Quaternion.identity;
        canvas.worldCamera = mainCamera;

        EditorUtility.SetDirty(canvas);
        EditorUtility.SetDirty(canvas.transform);
        EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
        EditorSceneManager.SaveScene(canvas.gameObject.scene);

        Debug.Log(
            $"[IntroPCView] Fixed Canvas for PC testing. camera={mainCamera.name}, " +
            $"canvasPos={canvas.transform.position}, canvasRot={canvas.transform.eulerAngles}, " +
            $"worldCamera={canvas.worldCamera.name}");
    }

    public static void CaptureScreen()
    {
        Directory.CreateDirectory("Assets/Screenshots");
        ScreenCapture.CaptureScreenshot("Assets/Screenshots/intro_pc_click_visibility.png");
        Debug.Log("[IntroPCView] Screenshot requested: Assets/Screenshots/intro_pc_click_visibility.png");
    }

    public static void CaptureStageState()
    {
        Directory.CreateDirectory("Assets/Screenshots/IntroStageStates");
        string stageName = "unknown";
        IntroManager manager = Object.FindFirstObjectByType<IntroManager>();
        if (manager != null && manager.stageNameText != null)
        {
            stageName = manager.stageNameText.text.Replace(" ", "_").ToLowerInvariant();
        }

        string path = $"Assets/Screenshots/IntroStageStates/intro_{stageName}.png";
        ScreenCapture.CaptureScreenshot(path);
        Debug.Log($"[IntroPCView] Stage screenshot requested: {path}");
    }
}
