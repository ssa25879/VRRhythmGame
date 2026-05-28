using System.IO;
using UnityEditor;
using UnityEngine;

public static class ApplyRuntimeNoteVisualToPrefabs
{
    const string RedPrefabPath = "Assets/Prefab/RED.prefab";
    const string BluePrefabPath = "Assets/Prefab/BLUE.prefab";
    const string MaterialFolder = "Assets/Material/CombatVisuals";

    public static void Execute()
    {
        Directory.CreateDirectory(MaterialFolder);

        Apply(
            RedPrefabPath,
            CreateUnlitMaterial("Note_Red_Core.mat", new Color(0.18f, 0.02f, 0.035f, 1f), new Color(1f, 0.02f, 0.04f, 1f), 1.2f),
            CreateUnlitMaterial("Note_Red_Accent.mat", new Color(1f, 0.06f, 0.08f, 1f), new Color(1f, 0.06f, 0.08f, 1f), 3.5f),
            CreateUnlitMaterial("Note_White_Guide.mat", Color.white, Color.white, 2.2f));

        Apply(
            BluePrefabPath,
            CreateUnlitMaterial("Note_Blue_Core.mat", new Color(0.015f, 0.055f, 0.14f, 1f), new Color(0f, 0.82f, 1f, 1f), 1.2f),
            CreateUnlitMaterial("Note_Blue_Accent.mat", new Color(0f, 0.82f, 1f, 1f), new Color(0f, 0.82f, 1f, 1f), 3.5f),
            CreateUnlitMaterial("Note_White_Guide.mat", Color.white, Color.white, 2.2f));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[ApplyRuntimeNoteVisualToPrefabs] RED/BLUE 프리팹을 런타임 노트 비주얼과 일치하도록 수정했습니다.");
    }

    static void Apply(string prefabPath, Material coreMaterial, Material accentMaterial, Material guideMaterial)
    {
        var root = PrefabUtility.LoadPrefabContents(prefabPath);
        if (root == null)
        {
            Debug.LogWarning($"[ApplyRuntimeNoteVisualToPrefabs] Prefab not found: {prefabPath}");
            return;
        }

        root.transform.localScale = Vector3.one * 0.38f;

        var renderer = root.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = coreMaterial;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }

        var sphere = root.transform.Find("Sphere");
        if (sphere != null && sphere.TryGetComponent<Renderer>(out var sphereRenderer))
        {
            sphereRenderer.enabled = false;
            sphereRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            sphereRenderer.receiveShadows = false;
        }

        RemoveChild(root.transform, "Front Neon Frame");
        RemoveChild(root.transform, "Cut Arrow");
        RemoveChild(root.transform, "Back Glow");
        RemoveChild(root.transform, "Frame Top");
        RemoveChild(root.transform, "Frame Bottom");
        RemoveChild(root.transform, "Frame Left");
        RemoveChild(root.transform, "Frame Right");
        RemoveChild(root.transform, "Cut Arrow Stem");
        RemoveChild(root.transform, "Cut Arrow Left");
        RemoveChild(root.transform, "Cut Arrow Right");
        RemoveChild(root.transform, "Cut Arrow Stem Back");
        RemoveChild(root.transform, "Cut Arrow Left Back");
        RemoveChild(root.transform, "Cut Arrow Right Back");
        RemoveChild(root.transform, "Direction Guide");
        RemoveChild(root.transform, "Direction Chevron Left");
        RemoveChild(root.transform, "Direction Chevron Right");
        RemoveChild(root.transform, "Direction Chevron Left Back");
        RemoveChild(root.transform, "Direction Chevron Right Back");
        RemoveChild(root.transform, "Energy Glow");

        AddBar(root.transform, "Frame Top", new Vector3(0f, 0.51f, -0.515f), new Vector3(1.08f, 0.055f, 0.055f), Quaternion.identity, accentMaterial);
        AddBar(root.transform, "Frame Bottom", new Vector3(0f, -0.51f, -0.515f), new Vector3(1.08f, 0.055f, 0.055f), Quaternion.identity, accentMaterial);
        AddBar(root.transform, "Frame Left", new Vector3(-0.51f, 0f, -0.515f), new Vector3(0.055f, 1.08f, 0.055f), Quaternion.identity, accentMaterial);
        AddBar(root.transform, "Frame Right", new Vector3(0.51f, 0f, -0.515f), new Vector3(0.055f, 1.08f, 0.055f), Quaternion.identity, accentMaterial);

        AddChevron(root.transform, string.Empty, 0.66f, guideMaterial);
        AddChevron(root.transform, " Back", -0.66f, guideMaterial);

        var glow = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        glow.name = "Energy Glow";
        glow.transform.SetParent(root.transform, false);
        glow.transform.localPosition = new Vector3(0f, 0f, 0.18f);
        glow.transform.localScale = Vector3.one * 0.34f;
        Object.DestroyImmediate(glow.GetComponent<Collider>());
        ConfigureRenderer(glow.GetComponent<Renderer>(), accentMaterial);

        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        PrefabUtility.UnloadPrefabContents(root);
    }

    static void AddBar(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Quaternion localRotation, Material material)
    {
        var bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bar.name = name;
        bar.transform.SetParent(parent, false);
        bar.transform.localPosition = localPosition;
        bar.transform.localRotation = localRotation;
        bar.transform.localScale = localScale;
        Object.DestroyImmediate(bar.GetComponent<Collider>());
        ConfigureRenderer(bar.GetComponent<Renderer>(), material);
    }

    static void AddChevron(Transform parent, string suffix, float zPosition, Material material)
    {
        AddBar(parent, "Direction Chevron Left" + suffix, new Vector3(-0.18f, 0.06f, zPosition), new Vector3(0.18f, 0.72f, 0.12f), Quaternion.Euler(0f, 0f, -42f), material);
        AddBar(parent, "Direction Chevron Right" + suffix, new Vector3(0.18f, 0.06f, zPosition), new Vector3(0.18f, 0.72f, 0.12f), Quaternion.Euler(0f, 0f, 42f), material);
    }

    static void ConfigureRenderer(Renderer renderer, Material material)
    {
        if (renderer == null)
        {
            return;
        }

        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    static void RemoveChild(Transform parent, string name)
    {
        var child = parent.Find(name);
        if (child != null)
        {
            Object.DestroyImmediate(child.gameObject);
        }
    }

    static Material CreateUnlitMaterial(string filename, Color baseColor, Color emissionColor, float emission)
    {
        var path = $"{MaterialFolder}/{filename}";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            AssetDatabase.CreateAsset(material, path);
        }

        material.SetColor("_BaseColor", baseColor);
        material.SetColor("_EmissionColor", emissionColor * emission);
        material.EnableKeyword("_EMISSION");
        EditorUtility.SetDirty(material);
        return material;
    }
}
