using UnityEngine;

// 세이버의 이동 방향과 블록의 방향을 비교해 블록을 파괴하는 스크립트
public class Saber : MonoBehaviour
{
    public LayerMask layer;  // 감지할 블록 레이어
    public GameObject hitEffectPrefab;
    public float hitEffectLifetime = 1.5f;
    public float hitEffectScale = 0.18f;
    public Transform bladeRoot;
    public Transform bladeTip;
    public float hitRadius = 0.24f;
    public float minSwingSpeed = 0.35f;
    public float directionTolerance = 85f;
    public float bladeTipOffset = 0.72f;
    public float hapticAmplitude = 0.35f;
    public float hapticDuration = 0.08f;

    Vector3 prevTipPos;
    bool hasPrevTip;
    Component hapticPlayer;
    Material bladeMaterial;
    Material whiteCoreMaterial;
    Material gripMaterial;

    void Start()
    {
        AutoBindBladePoints();
        BuildSaberVisual();
    }

    void Update()
    {
        if (bladeRoot == null || bladeTip == null)
        {
            AutoBindBladePoints();
        }

        Vector3 tipPos = GetTipPosition();
        if (!hasPrevTip)
        {
            prevTipPos = tipPos;
            hasPrevTip = true;
            return;
        }

        Vector3 swingVector = tipPos - prevTipPos;
        float swingSpeed = swingVector.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        if (swingSpeed >= minSwingSpeed)
        {
            CheckSlice(prevTipPos, tipPos, swingVector.normalized);
        }

        prevTipPos = tipPos;
    }

    void CheckSlice(Vector3 previousTip, Vector3 currentTip, Vector3 swingDirection)
    {
        Collider[] hits = Physics.OverlapCapsule(previousTip, currentTip, hitRadius, layer, QueryTriggerInteraction.Collide);
        foreach (Collider hit in hits)
        {
            Cube cube = hit.GetComponentInParent<Cube>();
            if (cube == null)
            {
                continue;
            }

            if (!MatchesCutDirection(hit.transform, swingDirection))
            {
                cube.BadCut();
                continue;
            }

            SpawnHitEffect(hit.ClosestPoint(currentTip));
            PlayHaptic();
            cube.Hit();
            break;
        }
    }

    bool MatchesCutDirection(Transform note, Vector3 swingDirection)
    {
        Vector3 required = note.up;
        float angle = Vector3.Angle(swingDirection, required);
        return angle <= directionTolerance;
    }

    void SpawnHitEffect(Vector3 position)
    {
        if (hitEffectPrefab == null)
        {
            return;
        }

        GameObject effect = Instantiate(hitEffectPrefab, position, transform.rotation);
        effect.transform.localScale *= hitEffectScale;
        Destroy(effect, hitEffectLifetime);
    }

    void PlayHaptic()
    {
        if (hapticPlayer == null)
        {
            hapticPlayer = GetComponent("HapticImpulsePlayer");
        }

        if (hapticPlayer == null)
        {
            return;
        }

        var method = hapticPlayer.GetType().GetMethod("SendHapticImpulse", new[] { typeof(float), typeof(float) });
        method?.Invoke(hapticPlayer, new object[] { hapticAmplitude, hapticDuration });
    }

    void AutoBindBladePoints()
    {
        if (bladeRoot == null)
        {
            foreach (Transform child in GetComponentsInChildren<Transform>(true))
            {
                if (child != transform && child.GetComponent<Renderer>() != null && child.GetComponent<Collider>() != null)
                {
                    bladeRoot = child;
                    break;
                }
            }
        }

        if (bladeRoot == null)
        {
            bladeRoot = transform;
        }

        if (bladeTip == null)
        {
            Transform existingTip = bladeRoot.Find("Blade Tip");
            if (existingTip != null)
            {
                bladeTip = existingTip;
            }
        }
    }

    void BuildSaberVisual()
    {
        bool isBlue = layer.value == (1 << LayerMask.NameToLayer("Blue"));
        Color bladeColor = isBlue ? new Color(0f, 0.86f, 1f, 1f) : new Color(1f, 0.06f, 0.08f, 1f);

        bladeMaterial = CreateMaterial(bladeColor, bladeColor, 4.4f);
        whiteCoreMaterial = CreateMaterial(Color.white, Color.white, 2.4f);
        gripMaterial = CreateMaterial(new Color(0.025f, 0.025f, 0.03f, 1f), new Color(0.08f, 0.08f, 0.1f, 1f), 0.4f);

        if (bladeRoot != null && bladeRoot != transform)
        {
            bladeRoot.name = isBlue ? "Blue Energy Blade" : "Red Energy Blade";
            bladeRoot.localPosition = new Vector3(0f, 0f, 0.62f);
            bladeRoot.localScale = new Vector3(0.026f, 0.026f, 1.42f);

            var renderer = bladeRoot.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = bladeMaterial;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }

            AddBladeCore();
            ConfigureTrail(bladeColor);
        }

        AddHilt(bladeColor);
    }

    void AddBladeCore()
    {
        if (bladeRoot.Find("White Blade Core") != null)
        {
            return;
        }

        var core = GameObject.CreatePrimitive(PrimitiveType.Cube);
        core.name = "White Blade Core";
        core.transform.SetParent(bladeRoot, false);
        core.transform.localPosition = Vector3.zero;
        core.transform.localScale = new Vector3(0.44f, 0.44f, 1.04f);
        Destroy(core.GetComponent<Collider>());

        var renderer = core.GetComponent<Renderer>();
        renderer.material = whiteCoreMaterial;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    void ConfigureTrail(Color bladeColor)
    {
        var trail = bladeRoot.GetComponent<TrailRenderer>();
        if (trail == null)
        {
            trail = bladeRoot.gameObject.AddComponent<TrailRenderer>();
        }

        trail.time = 0.22f;
        trail.minVertexDistance = 0.008f;
        trail.widthMultiplier = 0.23f;
        trail.material = bladeMaterial;
        trail.widthCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.55f, 0.55f), new Keyframe(1f, 0f));

        var gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(bladeColor, 0.24f),
                new GradientColorKey(bladeColor, 1f)
            },
            new[]
            {
                new GradientAlphaKey(0.85f, 0f),
                new GradientAlphaKey(0.52f, 0.35f),
                new GradientAlphaKey(0f, 1f)
            });
        trail.colorGradient = gradient;
    }

    void AddHilt(Color accentColor)
    {
        if (transform.Find("Neon Saber Hilt") != null)
        {
            return;
        }

        var hiltRoot = new GameObject("Neon Saber Hilt");
        hiltRoot.transform.SetParent(transform, false);
        hiltRoot.transform.localPosition = new Vector3(0f, 0f, -0.03f);

        AddHiltPart(hiltRoot.transform, "Handle", PrimitiveType.Cylinder, new Vector3(0f, 0f, -0.12f), new Vector3(0.06f, 0.22f, 0.06f), Quaternion.Euler(90f, 0f, 0f), gripMaterial);
        AddHiltPart(hiltRoot.transform, "Emitter Ring", PrimitiveType.Cylinder, new Vector3(0f, 0f, 0.1f), new Vector3(0.095f, 0.035f, 0.095f), Quaternion.Euler(90f, 0f, 0f), bladeMaterial);
    }

    void AddHiltPart(Transform parent, string name, PrimitiveType primitive, Vector3 localPosition, Vector3 localScale, Quaternion localRotation, Material material)
    {
        var part = GameObject.CreatePrimitive(primitive);
        part.name = name;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = localPosition;
        part.transform.localScale = localScale;
        part.transform.localRotation = localRotation;
        Destroy(part.GetComponent<Collider>());

        var renderer = part.GetComponent<Renderer>();
        renderer.material = material;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    Material CreateMaterial(Color baseColor, Color emissionColor, float emission)
    {
        var material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        material.SetColor("_BaseColor", baseColor);
        material.SetColor("_EmissionColor", emissionColor * emission);
        material.EnableKeyword("_EMISSION");
        return material;
    }

    Vector3 GetTipPosition()
    {
        if (bladeTip != null)
        {
            return bladeTip.position;
        }

        if (bladeRoot != null)
        {
            return bladeRoot.TransformPoint(0f, 0f, bladeTipOffset);
        }

        return transform.position + transform.forward * bladeTipOffset;
    }

    void OnDrawGizmosSelected()
    {
        if (bladeRoot == null)
        {
            return;
        }

        Gizmos.color = Color.cyan;
        Vector3 tipPosition = GetTipPosition();
        Gizmos.DrawWireSphere(tipPosition, hitRadius);
        Gizmos.DrawLine(bladeRoot.position, tipPosition);
    }
}
