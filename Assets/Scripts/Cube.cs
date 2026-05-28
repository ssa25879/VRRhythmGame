using UnityEngine;

public class Cube : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float hitShrinkDuration = 0.08f;
    [SerializeField] private float missAfterSeconds = 8f;
    [SerializeField] private float maxDistanceFromOrigin = 24f;

    private bool wasHit;
    private bool wasScored;
    private float spawnedAt;
    private Material coreMaterial;
    private Material accentMaterial;
    private Material guideMaterial;

    void Start()
    {
        spawnedAt = Time.time;
        BuildNoteVisual();
    }

    void Update()
    {
        if (!wasHit)
        {
            transform.position += Time.deltaTime * transform.forward * moveSpeed;
            CheckMiss();
        }
    }

    public void Hit()
    {
        if (wasHit || wasScored)
        {
            return;
        }

        wasHit = true;
        wasScored = true;
        GameScoreController.Instance?.RegisterHit();
        StartCoroutine(HitRoutine());
    }

    public void BadCut()
    {
        if (wasHit || wasScored)
        {
            return;
        }

        wasHit = true;
        wasScored = true;
        GameScoreController.Instance?.RegisterBad();
        StartCoroutine(HitRoutine());
    }

    private void CheckMiss()
    {
        if (wasScored)
        {
            return;
        }

        bool timedOut = Time.time - spawnedAt >= missAfterSeconds;
        bool tooFar = transform.position.sqrMagnitude >= maxDistanceFromOrigin * maxDistanceFromOrigin;
        if (!timedOut && !tooFar)
        {
            return;
        }

        wasScored = true;
        GameScoreController.Instance?.RegisterMiss("MISS");
        Destroy(gameObject);
    }

    private System.Collections.IEnumerator HitRoutine()
    {
        Vector3 startScale = transform.localScale;
        for (float t = 0f; t < hitShrinkDuration; t += Time.deltaTime)
        {
            float amount = 1f - Mathf.Clamp01(t / hitShrinkDuration);
            transform.localScale = startScale * amount;
            yield return null;
        }

        Destroy(gameObject);
    }

    private void BuildNoteVisual()
    {
        Color accent = gameObject.layer == LayerMask.NameToLayer("Blue")
            ? new Color(0f, 0.82f, 1f, 1f)
            : new Color(1f, 0.06f, 0.08f, 1f);
        Color core = Color.Lerp(Color.black, accent, 0.22f);

        coreMaterial = CreateMaterial(core, accent, 1.2f);
        accentMaterial = CreateMaterial(accent, accent, 3.5f);
        guideMaterial = CreateMaterial(Color.white, Color.white, 2.2f);

        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = coreMaterial;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }

        var oldSphere = transform.Find("Sphere");
        if (oldSphere != null && oldSphere.TryGetComponent<Renderer>(out var sphereRenderer))
        {
            sphereRenderer.enabled = false;
        }

        if (HasPrefabNoteVisual())
        {
            ApplyMaterialToExistingVisual("Frame Top");
            ApplyMaterialToExistingVisual("Frame Bottom");
            ApplyMaterialToExistingVisual("Frame Left");
            ApplyMaterialToExistingVisual("Frame Right");
            ApplyMaterialToExistingVisual("Direction Chevron Left");
            ApplyMaterialToExistingVisual("Direction Chevron Right");
            ApplyMaterialToExistingVisual("Direction Chevron Left Back");
            ApplyMaterialToExistingVisual("Direction Chevron Right Back");
            ApplyMaterialToExistingVisual("Energy Glow");
            return;
        }

        AddBar("Frame Top", new Vector3(0f, 0.51f, -0.515f), new Vector3(1.08f, 0.055f, 0.055f), Quaternion.identity);
        AddBar("Frame Bottom", new Vector3(0f, -0.51f, -0.515f), new Vector3(1.08f, 0.055f, 0.055f), Quaternion.identity);
        AddBar("Frame Left", new Vector3(-0.51f, 0f, -0.515f), new Vector3(0.055f, 1.08f, 0.055f), Quaternion.identity);
        AddBar("Frame Right", new Vector3(0.51f, 0f, -0.515f), new Vector3(0.055f, 1.08f, 0.055f), Quaternion.identity);

        AddChevron(0.66f);
        AddChevron(-0.66f);

        var glow = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        glow.name = "Energy Glow";
        glow.transform.SetParent(transform, false);
        glow.transform.localPosition = new Vector3(0f, 0f, 0.18f);
        glow.transform.localScale = Vector3.one * 0.34f;
        Destroy(glow.GetComponent<Collider>());
        var glowRenderer = glow.GetComponent<Renderer>();
        glowRenderer.material = accentMaterial;
        glowRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        glowRenderer.receiveShadows = false;
    }

    private void AddChevron(float zPosition)
    {
        string suffix = zPosition < 0f ? " Back" : string.Empty;
        AddBar("Direction Chevron Left" + suffix, new Vector3(-0.18f, 0.06f, zPosition), new Vector3(0.18f, 0.72f, 0.12f), Quaternion.Euler(0f, 0f, -42f), guideMaterial);
        AddBar("Direction Chevron Right" + suffix, new Vector3(0.18f, 0.06f, zPosition), new Vector3(0.18f, 0.72f, 0.12f), Quaternion.Euler(0f, 0f, 42f), guideMaterial);
    }

    private void AddBar(string name, Vector3 localPosition, Vector3 localScale, Quaternion localRotation)
    {
        AddBar(name, localPosition, localScale, localRotation, accentMaterial);
    }

    private void AddBar(string name, Vector3 localPosition, Vector3 localScale, Quaternion localRotation, Material material)
    {
        var bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bar.name = name;
        bar.transform.SetParent(transform, false);
        bar.transform.localPosition = localPosition;
        bar.transform.localRotation = localRotation;
        bar.transform.localScale = localScale;
        Destroy(bar.GetComponent<Collider>());

        var renderer = bar.GetComponent<Renderer>();
        renderer.material = material;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    private bool HasPrefabNoteVisual()
    {
        return transform.Find("Frame Top") != null
            && transform.Find("Frame Bottom") != null
            && transform.Find("Frame Left") != null
            && transform.Find("Frame Right") != null
            && transform.Find("Direction Chevron Left") != null
            && transform.Find("Direction Chevron Right") != null
            && transform.Find("Energy Glow") != null;
    }

    private void ApplyMaterialToExistingVisual(string childName)
    {
        var child = transform.Find(childName);
        if (child == null || !child.TryGetComponent<Renderer>(out var renderer))
        {
            return;
        }

        renderer.material = childName.StartsWith("Direction Chevron") ? guideMaterial : accentMaterial;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    private Material CreateMaterial(Color baseColor, Color emissionColor, float emission)
    {
        var material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        material.SetColor("_BaseColor", baseColor);
        material.SetColor("_EmissionColor", emissionColor * emission);
        material.EnableKeyword("_EMISSION");
        return material;
    }
}
