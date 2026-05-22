using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class DiagnoseIntroStageRuntime
{
    private const string IntroScenePath = "Assets/Scenes/Intro.unity";

    public static void Execute()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorSceneManager.OpenScene(IntroScenePath);
        }

        IntroManager manager = Object.FindFirstObjectByType<IntroManager>();
        if (manager == null)
        {
            Debug.LogError("[IntroStageDiag] IntroManager missing.");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine($"[IntroStageDiag] scene={SceneManager.GetActiveScene().path}, playMode={EditorApplication.isPlaying}");
        sb.AppendLine($"[IntroStageDiag] manager={manager.name}, active={manager.gameObject.activeInHierarchy}, enabled={manager.enabled}");
        sb.AppendLine($"[IntroStageDiag] stageList={(manager.stageList != null ? manager.stageList.name : "null")}");

        if (manager.stageList != null && manager.stageList.stages != null)
        {
            sb.AppendLine($"[IntroStageDiag] stageCount={manager.stageList.stages.Length}");
            for (int i = 0; i < manager.stageList.stages.Length; i++)
            {
                StageEntry stage = manager.stageList.stages[i];
                sb.AppendLine(
                    $"[IntroStageDiag] stage[{i}] name={stage.stageName}, " +
                    $"video={(stage.backgroundVideo != null ? stage.backgroundVideo.name : "null")}, " +
                    $"bgm={(stage.bgm != null ? stage.bgm.name : "null")}, " +
                    $"thumbnail={(stage.thumbnail != null ? stage.thumbnail.name : "null")}");
            }
        }

        AudioSource source = manager.bgmSource;
        sb.AppendLine(
            $"[IntroStageDiag] bgmSource={(source != null ? source.name : "null")}, " +
            $"clip={(source != null && source.clip != null ? source.clip.name : "null")}, " +
            $"isPlaying={(source != null && source.isPlaying)}, loop={(source != null && source.loop)}, " +
            $"volume={(source != null ? source.volume : -1f)}, mute={(source != null && source.mute)}, " +
            $"enabled={(source != null && source.enabled)}, spatialBlend={(source != null ? source.spatialBlend : -1f)}");

        foreach (AudioListener listener in Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None))
        {
            sb.AppendLine(
                $"[IntroStageDiag] audioListener={listener.name}, active={listener.gameObject.activeInHierarchy}, " +
                $"enabled={listener.enabled}, pos={listener.transform.position}");
        }

        sb.AppendLine($"[IntroStageDiag] stageNameText={(manager.stageNameText != null ? manager.stageNameText.text : "null")}");
        sb.AppendLine($"[IntroStageDiag] thumbnailImage={(manager.thumbnailImage != null ? manager.thumbnailImage.name : "null")}");
        sb.AppendLine($"[IntroStageDiag] eventSystem={(EventSystem.current != null ? EventSystem.current.name : "null")}");

        foreach (Button button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
        {
            sb.AppendLine(
                $"[IntroStageDiag] button={button.name}, active={button.gameObject.activeInHierarchy}, " +
                $"interactable={button.interactable}, listeners={button.onClick.GetPersistentEventCount()}");
            for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
            {
                Object target = button.onClick.GetPersistentTarget(i);
                string method = button.onClick.GetPersistentMethodName(i);
                sb.AppendLine($"[IntroStageDiag] buttonListener {button.name}[{i}] target={(target != null ? target.name : "null")}, method={method}");
            }
        }

        Debug.Log(sb.ToString());
    }

    public static void SimulateNext()
    {
        IntroManager manager = Object.FindFirstObjectByType<IntroManager>();
        if (manager == null)
        {
            Debug.LogError("[IntroStageDiag] IntroManager missing.");
            return;
        }

        manager.NextStage();
        Debug.Log("[IntroStageDiag] Invoked NextStage.");
        Execute();
    }

    public static void WireIntroGridRenderer()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorSceneManager.OpenScene(IntroScenePath);
        }

        IntroManager manager = Object.FindFirstObjectByType<IntroManager>();
        if (manager == null)
        {
            Debug.LogError("[IntroStageDiag] IntroManager missing.");
            return;
        }

        Renderer selected = null;
        foreach (Renderer renderer in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
        {
            string objectName = renderer.name.ToLowerInvariant();
            string materialName = renderer.sharedMaterial != null ? renderer.sharedMaterial.name.ToLowerInvariant() : string.Empty;
            Debug.Log($"[IntroStageDiag] renderer={renderer.name}, material={materialName}, active={renderer.gameObject.activeInHierarchy}");

            if (selected == null && (objectName.Contains("grid") || materialName.Contains("grid") || objectName.Contains("floor")))
            {
                selected = renderer;
            }
        }

        if (selected == null)
        {
            Debug.LogError("[IntroStageDiag] No grid/floor renderer found.");
            return;
        }

        manager.gridRenderer = selected;
        EditorUtility.SetDirty(manager);
        EditorSceneManager.MarkSceneDirty(manager.gameObject.scene);
        EditorSceneManager.SaveScene(manager.gameObject.scene);
        Debug.Log($"[IntroStageDiag] Wired IntroManager.gridRenderer={selected.name}");
    }

    public static void SelectAboutThatOldie()
    {
        PlayerPrefs.SetInt("SelectedStage", 0);
        PlayerPrefs.Save();
        Debug.Log("[IntroStageDiag] SelectedStage=0 (About That Oldie)");
    }
}
