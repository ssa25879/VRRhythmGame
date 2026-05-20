using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ApplyNeonSaberVisuals
{
    private const string GameScenePath = "Assets/Scenes/Game.unity";
    private const string BlueMaterialPath = "Assets/Material/Saber_Neon_Blue.mat";
    private const string RedMaterialPath = "Assets/Material/Saber_Neon_Red.mat";
    private const string BlueHitPath = "Assets/Eric VFX Studio/Free Game VFX/Prefab/FX_LootDrop_Blue.prefab";
    private const string RedHitPath = "Assets/Eric VFX Studio/Free Game VFX/Prefab/FX_Purple_Hit_02.prefab";
    private const string WeaponEffectPath = "Assets/Eric VFX Studio/Free Game VFX/Prefab/FX_Weapon Effect.prefab";

    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene(GameScenePath);
        var blueMaterial = CreateNeonMaterial(BlueMaterialPath, new Color(0f, 0.85f, 1f, 1f), 3.2f);
        var redMaterial = CreateNeonMaterial(RedMaterialPath, new Color(1f, 0.08f, 0.12f, 1f), 3.2f);
        var blueHit = AssetDatabase.LoadAssetAtPath<GameObject>(BlueHitPath);
        var redHit = AssetDatabase.LoadAssetAtPath<GameObject>(RedHitPath);
        var weaponEffect = AssetDatabase.LoadAssetAtPath<GameObject>(WeaponEffectPath);

        int changed = 0;
        foreach (var saber in Object.FindObjectsByType<Saber>(FindObjectsSortMode.None))
        {
            bool isLeft = saber.transform.position.x < 0f || saber.name.ToLowerInvariant().Contains("left");
            var color = isLeft ? new Color(0f, 0.85f, 1f, 1f) : new Color(1f, 0.08f, 0.12f, 1f);
            var material = isLeft ? blueMaterial : redMaterial;
            saber.hitEffectPrefab = isLeft ? blueHit : redHit;
            saber.hitEffectLifetime = 1.2f;

            var blade = FindBlade(saber.transform);
            if (blade == null)
            {
                continue;
            }

            blade.name = isLeft ? "Blue Neon Blade" : "Red Neon Blade";
            blade.localPosition = new Vector3(0f, 0f, 0.55f);
            blade.localScale = new Vector3(0.035f, 0.035f, 1.25f);

            var renderer = blade.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }

            var trail = blade.GetComponent<TrailRenderer>();
            if (trail == null)
            {
                trail = blade.gameObject.AddComponent<TrailRenderer>();
            }
            trail.time = 0.16f;
            trail.minVertexDistance = 0.01f;
            trail.widthMultiplier = 0.18f;
            trail.alignment = LineAlignment.View;
            trail.textureMode = LineTextureMode.Stretch;
            trail.material = material;
            trail.colorGradient = BuildTrailGradient(color);
            trail.widthCurve = BuildTrailWidth();

            var glow = blade.GetComponentInChildren<Light>();
            if (glow == null)
            {
                var glowObject = new GameObject("Blade Glow");
                glowObject.transform.SetParent(blade, false);
                glowObject.transform.localPosition = new Vector3(0f, 0f, 0.4f);
                glow = glowObject.AddComponent<Light>();
            }
            glow.type = LightType.Point;
            glow.color = color;
            glow.range = 2.2f;
            glow.intensity = 1.8f;

            var pulse = blade.GetComponent<SaberGlowPulse>();
            if (pulse == null)
            {
                pulse = blade.gameObject.AddComponent<SaberGlowPulse>();
            }
            var serializedPulse = new SerializedObject(pulse);
            serializedPulse.FindProperty("glowLight").objectReferenceValue = glow;
            serializedPulse.FindProperty("baseIntensity").floatValue = 1.8f;
            serializedPulse.FindProperty("pulseAmount").floatValue = 0.25f;
            serializedPulse.FindProperty("pulseSpeed").floatValue = 9f;
            serializedPulse.ApplyModifiedPropertiesWithoutUndo();

            AddWeaponEffectPreview(blade, weaponEffect, isLeft);
            EditorUtility.SetDirty(saber);
            changed++;
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log($"Neon saber visuals applied. Changed sabers={changed}");
    }

    private static Transform FindBlade(Transform saber)
    {
        foreach (Transform child in saber.GetComponentsInChildren<Transform>(true))
        {
            if (child == saber)
            {
                continue;
            }

            if (child.GetComponent<Renderer>() != null && child.GetComponent<Collider>() != null)
            {
                return child;
            }
        }

        return saber.childCount > 0 ? saber.GetChild(0) : null;
    }

    private static Material CreateNeonMaterial(string path, Color color, float emission)
    {
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            AssetDatabase.CreateAsset(material, path);
        }

        material.SetColor("_BaseColor", color);
        material.SetColor("_EmissionColor", color * emission);
        material.EnableKeyword("_EMISSION");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        EditorUtility.SetDirty(material);
        return material;
    }

    private static Gradient BuildTrailGradient(Color color)
    {
        var gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(color, 0.25f),
                new GradientColorKey(color, 1f)
            },
            new[]
            {
                new GradientAlphaKey(0.9f, 0f),
                new GradientAlphaKey(0.55f, 0.35f),
                new GradientAlphaKey(0f, 1f)
            });
        return gradient;
    }

    private static AnimationCurve BuildTrailWidth()
    {
        return new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(0.5f, 0.55f),
            new Keyframe(1f, 0f));
    }

    private static void AddWeaponEffectPreview(Transform blade, GameObject prefab, bool isLeft)
    {
        if (prefab == null || blade.Find("Saber Weapon VFX") != null)
        {
            return;
        }

        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, blade);
        instance.name = "Saber Weapon VFX";
        instance.transform.localPosition = new Vector3(0f, 0f, 0.65f);
        instance.transform.localRotation = Quaternion.Euler(isLeft ? 0f : 180f, 0f, 0f);
        instance.transform.localScale = Vector3.one * 0.35f;
    }
}
