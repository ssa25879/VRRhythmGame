using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Video;

public static class ApplyPrototypeBackground
{
    public static void Execute()
    {
        var skyboxMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/360 Music/Material360.mat");
        var renderTexture = AssetDatabase.LoadAssetAtPath<RenderTexture>("Assets/360 Music/RenderTexture360.renderTexture");
        var blackMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Material/MaterialBlack.mat");

        if (skyboxMaterial == null || renderTexture == null || blackMaterial == null)
        {
            Debug.LogError("Required background assets are missing.");
            return;
        }

        MatchPrototypeSkyboxMaterial(skyboxMaterial, renderTexture);
        ApplyToScene("Assets/Scenes/Intro.unity", skyboxMaterial, renderTexture, blackMaterial, includeSpawner: false);
        ApplyToScene("Assets/Scenes/Game.unity", skyboxMaterial, renderTexture, blackMaterial, includeSpawner: true);

        AssetDatabase.SaveAssets();
        Debug.Log("Prototype-style background applied to Intro and Game scenes.");
    }

    private static void MatchPrototypeSkyboxMaterial(Material material, RenderTexture renderTexture)
    {
        material.shader = Shader.Find("Skybox/Panoramic");
        material.SetTexture("_MainTex", renderTexture);
        material.SetFloat("_ImageType", 1f);
        material.SetFloat("_Mapping", 1f);
        material.SetFloat("_Layout", 0f);
        material.SetFloat("_MirrorOnBack", 1f);
        material.SetFloat("_Rotation", 0f);
        EditorUtility.SetDirty(material);
    }

    private static void ApplyToScene(string scenePath, Material skyboxMaterial, RenderTexture renderTexture, Material blackMaterial, bool includeSpawner)
    {
        var scene = EditorSceneManager.OpenScene(scenePath);

        RenderSettings.skybox = skyboxMaterial;
        DynamicGI.UpdateEnvironment();

        foreach (var camera in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
        {
            camera.clearFlags = CameraClearFlags.Skybox;
        }

        var videoObject = GameObject.Find("Video") ?? GameObject.Find("video");
        if (videoObject != null)
        {
            var player = videoObject.GetComponent<VideoPlayer>();
            if (player != null)
            {
                player.renderMode = VideoRenderMode.RenderTexture;
                player.targetTexture = renderTexture;
                player.isLooping = true;
                player.playOnAwake = true;
                EditorUtility.SetDirty(player);
            }
        }

        RemoveIfExists("DebugCube");
        RemoveIfExists("SkyDome360");

        var background = GameObject.Find("Background");
        if (background == null)
        {
            background = new GameObject("Background");
        }

        background.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        background.transform.localScale = Vector3.one;

        ConfigureBackgroundCube(background.transform, "Cube", new Vector3(0f, -1.3f, 0f), new Vector3(5f, 1f, 5f), blackMaterial);
        ConfigureBackgroundCube(background.transform, "Cube (1)", new Vector3(0f, -1.3f, 12f), new Vector3(5f, 1f, 13f), blackMaterial);

        if (includeSpawner)
        {
            var spawner = GameObject.Find("Spawner");
            if (spawner != null)
            {
                spawner.transform.SetParent(background.transform, worldPositionStays: true);
            }
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void ConfigureBackgroundCube(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Material material)
    {
        var child = parent.Find(name);
        var cube = child != null ? child.gameObject : GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent, false);
        cube.transform.localPosition = localPosition;
        cube.transform.localRotation = Quaternion.identity;
        cube.transform.localScale = localScale;

        var renderer = cube.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = material;
        }
    }

    private static void RemoveIfExists(string name)
    {
        var target = GameObject.Find(name);
        if (target != null)
        {
            Object.DestroyImmediate(target);
        }
    }
}
