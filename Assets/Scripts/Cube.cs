using UnityEngine;

public class Cube : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float hitShrinkDuration = 0.08f;

    private bool wasHit;
    private Material coreMaterial;
    private Material accentMaterial;

    void Start()
    {
        BuildNoteVisual();
    }

    void Update()
    {
        if (!wasHit)
        {
            transform.position += Time.deltaTime * transform.forward * moveSpeed;
        }
    }

    public void Hit()
    {
        if (wasHit)
        {
            return;
        }

        wasHit = true;
        StartCoroutine(HitRoutine());
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

        AddBar("Frame Top", new Vector3(0f, 0.51f, -0.515f), new Vector3(1.08f, 0.055f, 0.055f), Quaternion.identity);
        AddBar("Frame Bottom", new Vector3(0f, -0.51f, -0.515f), new Vector3(1.08f, 0.055f, 0.055f), Quaternion.identity);
        AddBar("Frame Left", new Vector3(-0.51f, 0f, -0.515f), new Vector3(0.055f, 1.08f, 0.055f), Quaternion.identity);
        AddBar("Frame Right", new Vector3(0.51f, 0f, -0.515f), new Vector3(0.055f, 1.08f, 0.055f), Quaternion.identity);

        AddBar("Cut Arrow Stem", new Vector3(0f, -0.08f, -0.59f), new Vector3(0.1f, 0.58f, 0.06f), Quaternion.identity);
        AddBar("Cut Arrow Left", new Vector3(-0.16f, 0.17f, -0.59f), new Vector3(0.09f, 0.34f, 0.06f), Quaternion.Euler(0f, 0f, 45f));
        AddBar("Cut Arrow Right", new Vector3(0.16f, 0.17f, -0.59f), new Vector3(0.09f, 0.34f, 0.06f), Quaternion.Euler(0f, 0f, -45f));

        var glow = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        glow.name = "Energy Glow";
        glow.transform.SetParent(transform, false);
        glow.transform.localPosition = new Vector3(0f, 0f, 0.24f);
        glow.transform.localScale = Vector3.one * 0.72f;
        Destroy(glow.GetComponent<Collider>());
        var glowRenderer = glow.GetComponent<Renderer>();
        glowRenderer.material = accentMaterial;
        glowRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        glowRenderer.receiveShadows = false;
    }

    private void AddBar(string name, Vector3 localPosition, Vector3 localScale, Quaternion localRotation)
    {
        var bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bar.name = name;
        bar.transform.SetParent(transform, false);
        bar.transform.localPosition = localPosition;
        bar.transform.localRotation = localRotation;
        bar.transform.localScale = localScale;
        Destroy(bar.GetComponent<Collider>());

        var renderer = bar.GetComponent<Renderer>();
        renderer.material = accentMaterial;
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
