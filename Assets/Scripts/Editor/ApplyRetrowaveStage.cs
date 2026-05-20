using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ApplyRetrowaveStage
{
    private const string StageListPath = "Assets/Data/StageList.asset";
    private const string VaporSkyboxPath = "Assets/Suggo Creations/RETROWAVE SKIES Lite/Skybox Materials/Vapor_Skybox.mat";
    private const string VaporGridPath = "Assets/Suggo Creations/RETROWAVE SKIES Lite/Materials/M_Grid Vapor Lite.mat";
    private const string GameScenePath = "Assets/Scenes/Game.unity";

    public static void Execute()
    {
        var stageList = AssetDatabase.LoadAssetAtPath<StageListSO>(StageListPath);
        var skybox = AssetDatabase.LoadAssetAtPath<Material>(VaporSkyboxPath);
        var grid = AssetDatabase.LoadAssetAtPath<Material>(VaporGridPath);
        var bgm = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/360 Music/About That Oldie - Vibe Tracks.mp3");

        if (stageList == null || skybox == null || grid == null)
        {
            Debug.LogError($"Missing retrowave stage asset. stageList={stageList != null}, skybox={skybox != null}, grid={grid != null}");
            return;
        }

        AddOrUpdateStage(stageList, skybox, grid, bgm);
        EditorUtility.SetDirty(stageList);
        AssetDatabase.SaveAssets();

        var scene = EditorSceneManager.OpenScene(GameScenePath);
        var gridRenderer = EnsureGridFloor(grid);
        var controller = Object.FindFirstObjectByType<GameBackgroundController>();
        if (controller != null)
        {
            controller.gridRenderer = gridRenderer;
            EditorUtility.SetDirty(controller);
        }

        RenderSettings.skybox = skybox;
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Retrowave stage applied.");
    }

    private static void AddOrUpdateStage(StageListSO stageList, Material skybox, Material grid, AudioClip bgm)
    {
        var stages = stageList.stages ?? new StageEntry[0];
        for (int i = 0; i < stages.Length; i++)
        {
            if (stages[i].stageName == "Neon Grid")
            {
                stages[i].thumbnail = null;
                stages[i].backgroundVideo = null;
                stages[i].skyboxMaterial = skybox;
                stages[i].gridMaterial = grid;
                stages[i].bgm = bgm;
                stages[i].bpm = 128f;
                stageList.stages = stages;
                return;
            }
        }

        var updated = new StageEntry[stages.Length + 1];
        for (int i = 0; i < stages.Length; i++)
        {
            updated[i] = stages[i];
        }

        updated[updated.Length - 1] = new StageEntry
        {
            stageName = "Neon Grid",
            thumbnail = null,
            backgroundVideo = null,
            skyboxMaterial = skybox,
            gridMaterial = grid,
            bgm = bgm,
            bpm = 128f
        };
        stageList.stages = updated;
    }

    private static Renderer EnsureGridFloor(Material grid)
    {
        var existing = GameObject.Find("Retrowave Grid Floor");
        if (existing == null)
        {
            existing = GameObject.CreatePrimitive(PrimitiveType.Plane);
            existing.name = "Retrowave Grid Floor";
        }

        existing.transform.position = new Vector3(0f, -1.28f, 7f);
        existing.transform.rotation = Quaternion.identity;
        existing.transform.localScale = new Vector3(8f, 1f, 18f);

        var collider = existing.GetComponent<Collider>();
        if (collider != null)
        {
            Object.DestroyImmediate(collider);
        }

        var renderer = existing.GetComponent<Renderer>();
        renderer.sharedMaterial = grid;
        renderer.enabled = false;
        return renderer;
    }
}
