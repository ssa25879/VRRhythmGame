using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ApplyCombatVisualRedesign
{
    private const string GameScenePath = "Assets/Scenes/Game.unity";
    private const string RedPrefabPath = "Assets/Prefab/RED.prefab";
    private const string BluePrefabPath = "Assets/Prefab/BLUE.prefab";
    private const string DesignMaterialFolder = "Assets/Material/CombatVisuals";

    public static void Execute()
    {
        Directory.CreateDirectory(DesignMaterialFolder);

        var redCore = CreateUnlitMaterial("Note_Red_Core.mat", new Color(0.18f, 0.02f, 0.035f, 1f), new Color(1f, 0.02f, 0.04f, 1f), 1.5f);
        var blueCore = CreateUnlitMaterial("Note_Blue_Core.mat", new Color(0.015f, 0.055f, 0.14f, 1f), new Color(0f, 0.68f, 1f, 1f), 1.5f);
        var redAccent = CreateUnlitMaterial("Note_Red_Accent.mat", new Color(1f, 0.06f, 0.05f, 1f), new Color(1f, 0.05f, 0.03f, 1f), 3.5f);
        var blueAccent = CreateUnlitMaterial("Note_Blue_Accent.mat", new Color(0f, 0.78f, 1f, 1f), new Color(0f, 0.85f, 1f, 1f), 3.5f);
        var whiteCore = CreateUnlitMaterial("Saber_White_Core.mat", Color.white, Color.white, 2.5f);
        var grip = CreateUnlitMaterial("Saber_Dark_Grip.mat", new Color(0.025f, 0.025f, 0.03f, 1f), new Color(0.06f, 0.06f, 0.08f, 1f), 0.5f);

        RedesignNotePrefab(RedPrefabPath, redCore, redAccent);
        RedesignNotePrefab(BluePrefabPath, blueCore, blueAccent);

        var scene = EditorSceneManager.OpenScene(GameScenePath);
        RedesignSabers(scene, whiteCore, grip);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("Combat visual redesign applied.");
    }

    private static void RedesignNotePrefab(string prefabPath, Material core, Material accent)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogWarning($"Note prefab not found: {prefabPath}");
            return;
        }

        var root = prefab.transform;
        root.localScale = Vector3.one * 0.38f;

        var renderer = prefab.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = core;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }

        RemoveChild(root, "Sphere");
        RemoveChild(root, "Front Neon Frame");
        RemoveChild(root, "Cut Arrow");
        RemoveChild(root, "Back Glow");

        var frame = new GameObject("Front Neon Frame");
        frame.transform.SetParent(root, false);
        AddFrameBar(frame.transform, "Top", new Vector3(0f, 0.51f, -0.505f), new Vector3(1.08f, 0.055f, 0.055f), accent);
        AddFrameBar(frame.transform, "Bottom", new Vector3(0f, -0.51f, -0.505f), new Vector3(1.08f, 0.055f, 0.055f), accent);
        AddFrameBar(frame.transform, "Left", new Vector3(-0.51f, 0f, -0.505f), new Vector3(0.055f, 1.08f, 0.055f), accent);
        AddFrameBar(frame.transform, "Right", new Vector3(0.51f, 0f, -0.505f), new Vector3(0.055f, 1.08f, 0.055f), accent);

        var arrow = new GameObject("Cut Arrow");
        arrow.transform.SetParent(root, false);
        AddFrameBar(arrow.transform, "Stem", new Vector3(0f, -0.08f, -0.56f), new Vector3(0.105f, 0.58f, 0.06f), accent);
        AddArrowWing(arrow.transform, "Left Wing", new Vector3(-0.16f, 0.17f, -0.56f), 45f, accent);
        AddArrowWing(arrow.transform, "Right Wing", new Vector3(0.16f, 0.17f, -0.56f), -45f, accent);

        var glow = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        glow.name = "Back Glow";
        glow.transform.SetParent(root, false);
        glow.transform.localPosition = new Vector3(0f, 0f, 0.24f);
        glow.transform.localScale = Vector3.one * 0.72f;
        Object.DestroyImmediate(glow.GetComponent<Collider>());
        var glowRenderer = glow.GetComponent<MeshRenderer>();
        if (glowRenderer != null)
        {
            glowRenderer.sharedMaterial = accent;
            glowRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            glowRenderer.receiveShadows = false;
        }

        EditorUtility.SetDirty(prefab);
    }

    private static void RedesignSabers(UnityEngine.SceneManagement.Scene scene, Material whiteCore, Material grip)
    {
        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (var saber in root.GetComponentsInChildren<Saber>(true))
            {
                bool left = saber.transform.name.ToLowerInvariant().Contains("left") || saber.transform.position.x < 0f;
                var color = left ? new Color(0f, 0.85f, 1f, 1f) : new Color(1f, 0.06f, 0.08f, 1f);
                var glowMaterial = CreateUnlitMaterial(left ? "Saber_Blue_Glow.mat" : "Saber_Red_Glow.mat", color, color, 4.5f);

                var blade = FindBlade(saber.transform);
                if (blade != null)
                {
                    blade.name = left ? "Blue Energy Blade" : "Red Energy Blade";
                    blade.localPosition = new Vector3(0f, 0f, 0.62f);
                    blade.localScale = new Vector3(0.026f, 0.026f, 1.42f);
                    var renderer = blade.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        renderer.sharedMaterial = glowMaterial;
                        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        renderer.receiveShadows = false;
                    }

                    AddBladeCore(blade, whiteCore);
                    ConfigureTrail(blade, glowMaterial, color);
                }

                AddHilt(saber.transform, grip, glowMaterial, left);
                EditorUtility.SetDirty(saber);
            }
        }
    }

    private static void AddBladeCore(Transform blade, Material material)
    {
        RemoveChild(blade, "White Blade Core");
        var core = GameObject.CreatePrimitive(PrimitiveType.Cube);
        core.name = "White Blade Core";
        core.transform.SetParent(blade, false);
        core.transform.localPosition = Vector3.zero;
        core.transform.localScale = new Vector3(0.45f, 0.45f, 1.04f);
        Object.DestroyImmediate(core.GetComponent<Collider>());

        var renderer = core.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }
    }

    private static void AddHilt(Transform saber, Material grip, Material accent, bool left)
    {
        RemoveChild(saber, "Neon Saber Hilt");

        var hiltRoot = new GameObject("Neon Saber Hilt");
        hiltRoot.transform.SetParent(saber, false);
        hiltRoot.transform.localPosition = new Vector3(0f, 0f, -0.03f);
        hiltRoot.transform.localRotation = Quaternion.identity;

        var handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        handle.name = "Handle";
        handle.transform.SetParent(hiltRoot.transform, false);
        handle.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        handle.transform.localPosition = new Vector3(0f, 0f, -0.12f);
        handle.transform.localScale = new Vector3(0.06f, 0.22f, 0.06f);
        Object.DestroyImmediate(handle.GetComponent<Collider>());
        handle.GetComponent<MeshRenderer>().sharedMaterial = grip;

        var emitter = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        emitter.name = "Emitter Ring";
        emitter.transform.SetParent(hiltRoot.transform, false);
        emitter.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        emitter.transform.localPosition = new Vector3(0f, 0f, 0.1f);
        emitter.transform.localScale = new Vector3(0.095f, 0.035f, 0.095f);
        Object.DestroyImmediate(emitter.GetComponent<Collider>());
        emitter.GetComponent<MeshRenderer>().sharedMaterial = accent;

        var guard = GameObject.CreatePrimitive(PrimitiveType.Cube);
        guard.name = left ? "Left Guard" : "Right Guard";
        guard.transform.SetParent(hiltRoot.transform, false);
        guard.transform.localPosition = Vector3.zero;
        guard.transform.localScale = new Vector3(0.32f, 0.035f, 0.045f);
        Object.DestroyImmediate(guard.GetComponent<Collider>());
        guard.GetComponent<MeshRenderer>().sharedMaterial = accent;
    }

    private static void ConfigureTrail(Transform blade, Material material, Color color)
    {
        var trail = blade.GetComponent<TrailRenderer>();
        if (trail == null)
        {
            trail = blade.gameObject.AddComponent<TrailRenderer>();
        }

        trail.time = 0.22f;
        trail.minVertexDistance = 0.008f;
        trail.widthMultiplier = 0.23f;
        trail.material = material;
        var gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(color, 0.24f),
                new GradientColorKey(color, 1f)
            },
            new[]
            {
                new GradientAlphaKey(0.85f, 0f),
                new GradientAlphaKey(0.52f, 0.35f),
                new GradientAlphaKey(0f, 1f)
            });
        trail.colorGradient = gradient;
        trail.widthCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.55f, 0.55f), new Keyframe(1f, 0f));
    }

    private static void AddFrameBar(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
    {
        var bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bar.name = name;
        bar.transform.SetParent(parent, false);
        bar.transform.localPosition = position;
        bar.transform.localScale = scale;
        Object.DestroyImmediate(bar.GetComponent<Collider>());
        var renderer = bar.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    private static void AddArrowWing(Transform parent, string name, Vector3 position, float zAngle, Material material)
    {
        var wing = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wing.name = name;
        wing.transform.SetParent(parent, false);
        wing.transform.localPosition = position;
        wing.transform.localRotation = Quaternion.Euler(0f, 0f, zAngle);
        wing.transform.localScale = new Vector3(0.095f, 0.36f, 0.06f);
        Object.DestroyImmediate(wing.GetComponent<Collider>());
        var renderer = wing.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    private static Transform FindBlade(Transform saber)
    {
        foreach (Transform child in saber.GetComponentsInChildren<Transform>(true))
        {
            if (child != saber && child.GetComponent<Renderer>() != null && child.GetComponent<Collider>() != null)
            {
                return child;
            }
        }

        return null;
    }

    private static void RemoveChild(Transform parent, string name)
    {
        var child = parent.Find(name);
        if (child != null)
        {
            Object.DestroyImmediate(child.gameObject);
        }
    }

    private static Material CreateUnlitMaterial(string filename, Color baseColor, Color emissionColor, float emission)
    {
        var path = $"{DesignMaterialFolder}/{filename}";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            AssetDatabase.CreateAsset(material, path);
        }

        material.SetColor("_BaseColor", baseColor);
        material.SetColor("_EmissionColor", emissionColor * emission);
        material.EnableKeyword("_EMISSION");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        EditorUtility.SetDirty(material);
        return material;
    }
}
