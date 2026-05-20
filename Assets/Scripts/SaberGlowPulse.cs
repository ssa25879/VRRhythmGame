using UnityEngine;

public class SaberGlowPulse : MonoBehaviour
{
    [SerializeField] private Light glowLight;
    [SerializeField] private float baseIntensity = 1.8f;
    [SerializeField] private float pulseAmount = 0.45f;
    [SerializeField] private float pulseSpeed = 8f;

    private Renderer targetRenderer;
    private Color baseColor;

    private void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
        if (targetRenderer != null && targetRenderer.material.HasProperty("_EmissionColor"))
        {
            baseColor = targetRenderer.material.GetColor("_EmissionColor");
        }
    }

    private void Update()
    {
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;

        if (glowLight != null)
        {
            glowLight.intensity = baseIntensity * pulse;
        }

        if (targetRenderer != null && targetRenderer.material.HasProperty("_EmissionColor"))
        {
            targetRenderer.material.SetColor("_EmissionColor", baseColor * pulse);
        }
    }
}
