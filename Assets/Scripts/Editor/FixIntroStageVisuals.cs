using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class FixIntroStageVisuals
{
    private const string IntroScenePath = "Assets/Scenes/Intro.unity";
    private const string GridObjectName = "StageGridPreview";

    public static void Execute()
    {
        EditorSceneManager.OpenScene(IntroScenePath);

        IntroManager manager = Object.FindFirstObjectByType<IntroManager>();
        if (manager == null)
        {
            Debug.LogError("[IntroStageVisuals] IntroManager missing.");
            return;
        }

        Renderer gridRenderer = FindOrCreateGridRenderer();

        Undo.RecordObject(manager, "Wire Intro Stage Visuals");
        manager.gridRenderer = gridRenderer;
        EditorUtility.SetDirty(manager);
        EditorSceneManager.MarkSceneDirty(manager.gameObject.scene);
        EditorSceneManager.SaveScene(manager.gameObject.scene);

        Debug.Log($"[IntroStageVisuals] Wired gridRenderer={gridRenderer.name}");
    }

    private static Renderer FindOrCreateGridRenderer()
    {
        GameObject existing = GameObject.Find(GridObjectName);
        if (existing != null && existing.TryGetComponent(out Renderer existingRenderer))
        {
            return existingRenderer;
        }

        Material vaporGrid = AssetDatabase.LoadAssetAtPath<Material>(
            "Assets/Suggo Creations/RETROWAVE SKIES Lite/Materials/M_Grid Vapor Lite.mat");

        GameObject grid = GameObject.CreatePrimitive(PrimitiveType.Plane);
        grid.name = GridObjectName;
        grid.transform.position = new Vector3(0f, -1.28f, 7f);
        grid.transform.rotation = Quaternion.identity;
        grid.transform.localScale = new Vector3(1.8f, 1f, 6f);

        Renderer renderer = grid.GetComponent<Renderer>();
        if (renderer != null && vaporGrid != null)
        {
            renderer.sharedMaterial = vaporGrid;
            renderer.enabled = false;
        }

        return renderer;
    }

    private static Renderer FindBestGridRenderer()
    {
        foreach (Renderer renderer in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
        {
            if (!renderer.gameObject.activeInHierarchy)
            {
                continue;
            }

            string objectName = renderer.name.ToLowerInvariant();
            string materialName = renderer.sharedMaterial != null ? renderer.sharedMaterial.name.ToLowerInvariant() : string.Empty;

            if (objectName.Contains("grid") || materialName.Contains("grid"))
            {
                return renderer;
            }
        }

        return null;
    }
}
