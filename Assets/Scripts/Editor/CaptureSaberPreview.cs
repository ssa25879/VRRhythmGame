using System.IO;
using UnityEditor;
using UnityEngine;

public static class CaptureSaberPreview
{
    private const string ScreenshotPath = "Assets/Screenshots/saber_preview.png";

    public static void Capture()
    {
        var sabers = Resources.FindObjectsOfTypeAll<Saber>();
        if (sabers.Length == 0)
        {
            Debug.LogError("[SaberPreview] No sabers found.");
            return;
        }

        if (!TryGetBounds(sabers, out var bounds))
        {
            Debug.LogError("[SaberPreview] No saber renderers found.");
            return;
        }

        var cameraObject = new GameObject("Saber Preview Camera");
        var camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.015f, 0.015f, 0.025f, 1f);
        camera.fieldOfView = 42f;
        camera.nearClipPlane = 0.01f;
        camera.farClipPlane = 20f;

        var lightObject = new GameObject("Saber Preview Light");
        var light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.25f;
        light.transform.rotation = Quaternion.Euler(45f, -35f, 0f);

        Vector3 center = bounds.center;
        float distance = Mathf.Max(1.2f, bounds.size.magnitude * 1.8f);
        camera.transform.position = center + new Vector3(0f, 0.25f, -distance);
        camera.transform.LookAt(center + Vector3.up * 0.05f);

        var renderTexture = new RenderTexture(1280, 720, 24);
        camera.targetTexture = renderTexture;
        camera.Render();

        RenderTexture.active = renderTexture;
        var texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        Directory.CreateDirectory("Assets/Screenshots");
        File.WriteAllBytes(ScreenshotPath, texture.EncodeToPNG());

        RenderTexture.active = null;
        camera.targetTexture = null;
        Object.DestroyImmediate(texture);
        Object.DestroyImmediate(renderTexture);
        Object.DestroyImmediate(cameraObject);
        Object.DestroyImmediate(lightObject);

        AssetDatabase.Refresh();
        Debug.Log($"[SaberPreview] Saved {ScreenshotPath}");
    }

    public static void CaptureStandalone()
    {
        Directory.CreateDirectory("Assets/Screenshots");

        var root = new GameObject("Standalone Saber Preview Root");
        BuildPreviewSaber(root.transform, new Vector3(-0.35f, 0f, 0f), new Color(0f, 0.86f, 1f, 1f), 18f);
        BuildPreviewSaber(root.transform, new Vector3(0.35f, 0f, 0f), new Color(1f, 0.06f, 0.08f, 1f), -18f);

        var cameraObject = new GameObject("Standalone Saber Preview Camera");
        var camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.012f, 0.012f, 0.018f, 1f);
        camera.fieldOfView = 38f;
        camera.nearClipPlane = 0.01f;
        camera.farClipPlane = 20f;
        camera.transform.position = new Vector3(0f, 0.6f, -3.2f);
        camera.transform.LookAt(new Vector3(0f, 0.32f, 0.55f));

        var lightObject = new GameObject("Standalone Saber Preview Light");
        var light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.4f;
        light.transform.rotation = Quaternion.Euler(45f, -25f, 0f);

        var renderTexture = new RenderTexture(1280, 720, 24);
        camera.targetTexture = renderTexture;
        camera.Render();

        RenderTexture.active = renderTexture;
        var texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        File.WriteAllBytes(ScreenshotPath, texture.EncodeToPNG());

        RenderTexture.active = null;
        camera.targetTexture = null;
        Object.DestroyImmediate(texture);
        Object.DestroyImmediate(renderTexture);
        Object.DestroyImmediate(cameraObject);
        Object.DestroyImmediate(lightObject);
        Object.DestroyImmediate(root);

        AssetDatabase.Refresh();
        Debug.Log($"[SaberPreview] Saved standalone {ScreenshotPath}");
    }

    private static void BuildPreviewSaber(Transform parent, Vector3 position, Color color, float zRotation)
    {
        var saber = new GameObject(color.b > color.r ? "Blue Saber Preview" : "Red Saber Preview");
        saber.transform.SetParent(parent, false);
        saber.transform.localPosition = position;
        saber.transform.localRotation = Quaternion.Euler(0f, 0f, zRotation);

        var bladeMaterial = CreateMaterial(color, color, 4.4f);
        var coreMaterial = CreateMaterial(Color.white, Color.white, 2.4f);
        var gripMaterial = CreateMaterial(new Color(0.025f, 0.025f, 0.03f, 1f), new Color(0.08f, 0.08f, 0.1f, 1f), 0.4f);

        var blade = GameObject.CreatePrimitive(PrimitiveType.Cube);
        blade.name = "Energy Blade";
        blade.transform.SetParent(saber.transform, false);
        blade.transform.localPosition = new Vector3(0f, 0.65f, 0f);
        blade.transform.localScale = new Vector3(0.055f, 1.65f, 0.055f);
        Object.DestroyImmediate(blade.GetComponent<Collider>());
        blade.GetComponent<Renderer>().material = bladeMaterial;

        var core = GameObject.CreatePrimitive(PrimitiveType.Cube);
        core.name = "White Core";
        core.transform.SetParent(blade.transform, false);
        core.transform.localPosition = Vector3.zero;
        core.transform.localScale = new Vector3(0.42f, 1.02f, 0.42f);
        Object.DestroyImmediate(core.GetComponent<Collider>());
        core.GetComponent<Renderer>().material = coreMaterial;

        AddPart(saber.transform, "Handle", PrimitiveType.Cylinder, new Vector3(0f, -0.35f, 0f), new Vector3(0.12f, 0.36f, 0.12f), Quaternion.identity, gripMaterial);
        AddPart(saber.transform, "Emitter Ring", PrimitiveType.Cylinder, new Vector3(0f, 0.04f, 0f), new Vector3(0.18f, 0.06f, 0.18f), Quaternion.identity, bladeMaterial);

        var pointLight = new GameObject("Glow Light");
        pointLight.transform.SetParent(saber.transform, false);
        pointLight.transform.localPosition = new Vector3(0f, 0.65f, 0f);
        var light = pointLight.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.range = 2f;
        light.intensity = 2f;
    }

    private static void AddPart(Transform parent, string name, PrimitiveType primitive, Vector3 position, Vector3 scale, Quaternion rotation, Material material)
    {
        var part = GameObject.CreatePrimitive(primitive);
        part.name = name;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = position;
        part.transform.localScale = scale;
        part.transform.localRotation = rotation;
        Object.DestroyImmediate(part.GetComponent<Collider>());
        part.GetComponent<Renderer>().material = material;
    }

    private static Material CreateMaterial(Color baseColor, Color emissionColor, float emission)
    {
        var material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        material.SetColor("_BaseColor", baseColor);
        material.SetColor("_EmissionColor", emissionColor * emission);
        material.EnableKeyword("_EMISSION");
        return material;
    }

    private static bool TryGetBounds(Saber[] sabers, out Bounds bounds)
    {
        bounds = default;
        var initialized = false;
        foreach (var saber in sabers)
        {
            foreach (var renderer in saber.GetComponentsInChildren<Renderer>(true))
            {
                if (!initialized)
                {
                    bounds = renderer.bounds;
                    initialized = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }
        }

        return initialized;
    }
}
