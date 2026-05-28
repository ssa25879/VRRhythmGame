using System.Collections.Generic;
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
    public bool enableHitEffect;
    public bool enableBladeTrail;
    public float hitRadius = 0.24f;
    public float minSwingSpeed = 0.35f;
    public float directionTolerance = 85f;
    public float bladeTipOffset = 0.58f;
    public Vector3 bladeLocalPosition = new Vector3(0f, 0f, 0.61f);
    public Vector3 bladeLocalEulerAngles = Vector3.zero;
    public Vector3 bladeLocalScale = new Vector3(0.026f, 0.026f, 1.08f);
    public float hitBladeStart = 0.3f;
    public float hitBladeEnd = 1f;
    public float minSwingToBladeAngle = 45f;
    public float hapticAmplitude = 0.35f;
    public float hapticDuration = 0.08f;

    Vector3 prevTipPos;
    Vector3 prevBasePos;
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
            if (bladeRoot == null || bladeTip == null)
            {
                hasPrevTip = false;
                return;
            }
        }

        Vector3 basePos = GetBladeBasePosition();
        Vector3 tipPos = GetTipPosition();
        if (!hasPrevTip)
        {
            prevBasePos = basePos;
            prevTipPos = tipPos;
            hasPrevTip = true;
            return;
        }

        Vector3 swingVector = tipPos - prevTipPos;
        float swingSpeed = swingVector.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        if (swingSpeed >= minSwingSpeed)
        {
            CheckSlice(prevBasePos, prevTipPos, basePos, tipPos, swingVector.normalized);
        }

        prevBasePos = basePos;
        prevTipPos = tipPos;
    }

    void CheckSlice(Vector3 previousBase, Vector3 previousTip, Vector3 currentBase, Vector3 currentTip, Vector3 swingDirection)
    {
        if (!IsSwingCutMotion(swingDirection))
        {
            return;
        }

        var cubes = new HashSet<Cube>();

        const int sampleCount = 4;
        for (int i = 0; i < sampleCount; i++)
        {
            float t = sampleCount == 1 ? 1f : i / (float)(sampleCount - 1);
            t = Mathf.Lerp(hitBladeStart, hitBladeEnd, t);
            Vector3 previousPoint = Vector3.Lerp(previousBase, previousTip, t);
            Vector3 currentPoint = Vector3.Lerp(currentBase, currentTip, t);
            CollectCubesBetween(cubes, previousPoint, currentPoint);
        }

        foreach (Cube cube in cubes)
        {
            if (cube == null)
            {
                continue;
            }

            if (!MatchesCutDirection(cube.transform, swingDirection))
            {
                cube.BadCut();
                break;
            }

            SpawnHitEffect(cube.transform.position);
            PlayHaptic();
            cube.Hit();
            break;
        }
    }

    bool IsSwingCutMotion(Vector3 swingDirection)
    {
        if (bladeRoot == null)
        {
            return true;
        }

        float thrustAngle = Vector3.Angle(swingDirection, bladeRoot.forward);
        float reverseThrustAngle = Vector3.Angle(swingDirection, -bladeRoot.forward);
        return thrustAngle >= minSwingToBladeAngle && reverseThrustAngle >= minSwingToBladeAngle;
    }

    void CollectCubesBetween(HashSet<Cube> cubes, Vector3 start, Vector3 end)
    {
        Collider[] hits = Physics.OverlapCapsule(start, end, hitRadius, layer, QueryTriggerInteraction.Collide);
        foreach (Collider hit in hits)
        {
            Cube cube = hit.GetComponentInParent<Cube>();
            if (cube != null)
            {
                cubes.Add(cube);
            }
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
        if (!enableHitEffect || hitEffectPrefab == null)
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
            bladeRoot = FindNamedBladeRoot();
        }

        if (bladeRoot == null)
        {
            Debug.LogWarning($"[Saber] Blade root is missing on {name}. Hit detection is disabled until a Neon Blade child is assigned.");
            return;
        }

        if (bladeTip == null)
        {
            Transform existingTip = bladeRoot.Find("Blade Tip");
            if (existingTip != null)
            {
                bladeTip = existingTip;
            }
        }

        if (bladeTip == null && bladeRoot != null)
        {
            var tip = new GameObject("Blade Tip").transform;
            tip.SetParent(bladeRoot, false);
            tip.localPosition = new Vector3(0f, 0f, bladeTipOffset);
            bladeTip = tip;
        }
    }

    Transform FindNamedBladeRoot()
    {
        bool isBlue = layer.value == (1 << LayerMask.NameToLayer("Blue"));
        string preferredName = isBlue ? "Blue Neon Blade" : "Red Neon Blade";
        Transform namedBlade = FindChildByName(transform, preferredName);
        if (namedBlade != null)
        {
            return namedBlade;
        }

        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child == transform)
            {
                continue;
            }

            if ((child.name.Contains("Neon Blade") || child.name.Contains("Energy Blade")) &&
                child.GetComponent<Renderer>() != null)
            {
                return child;
            }
        }

        return null;
    }

    Transform FindChildByName(Transform root, string childName)
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == childName)
            {
                return child;
            }
        }

        return null;
    }

    void BuildSaberVisual()
    {
        bool isBlue = layer.value == (1 << LayerMask.NameToLayer("Blue"));
        Color bladeColor = isBlue ? new Color(0f, 0.86f, 1f, 1f) : new Color(1f, 0.06f, 0.08f, 1f);
        string colorName = isBlue ? "Blue" : "Red";

        bladeMaterial = CreateMaterial(bladeColor, bladeColor, 4.4f);
        whiteCoreMaterial = CreateMaterial(Color.white, Color.white, 2.4f);
        gripMaterial = CreateMaterial(new Color(0.025f, 0.025f, 0.03f, 1f), new Color(0.08f, 0.08f, 0.1f, 1f), 0.4f);

        if (bladeRoot != null && bladeRoot != transform)
        {
            bladeRoot.name = $"{colorName} Energy Blade";
            bladeRoot.localPosition = bladeLocalPosition;
            bladeRoot.localRotation = Quaternion.Euler(bladeLocalEulerAngles);
            bladeRoot.localScale = bladeLocalScale;

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

        AddOrUpdateHilt(bladeColor);
        gameObject.name = $"{colorName} Saber Controller";
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
        if (!enableBladeTrail)
        {
            if (trail != null)
            {
                trail.enabled = false;
            }

            return;
        }

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

    void AddOrUpdateHilt(Color accentColor)
    {
        var hiltRoot = transform.Find("Neon Saber Hilt");
        if (hiltRoot == null)
        {
            hiltRoot = new GameObject("Neon Saber Hilt").transform;
        }

        hiltRoot.transform.SetParent(transform, false);
        hiltRoot.transform.localPosition = new Vector3(0f, 0f, -0.03f);

        AddOrUpdateHiltPart(hiltRoot, "Handle", PrimitiveType.Cylinder, new Vector3(0f, 0f, -0.12f), new Vector3(0.06f, 0.22f, 0.06f), Quaternion.Euler(90f, 0f, 0f), gripMaterial);
        AddOrUpdateHiltPart(hiltRoot, "Emitter Ring", PrimitiveType.Cylinder, new Vector3(0f, 0f, 0.1f), new Vector3(0.095f, 0.035f, 0.095f), Quaternion.Euler(90f, 0f, 0f), bladeMaterial);
    }

    void AddOrUpdateHiltPart(Transform parent, string name, PrimitiveType primitive, Vector3 localPosition, Vector3 localScale, Quaternion localRotation, Material material)
    {
        var partTransform = parent.Find(name);
        GameObject part;
        if (partTransform == null)
        {
            part = GameObject.CreatePrimitive(primitive);
            part.name = name;
        }
        else
        {
            part = partTransform.gameObject;
        }

        part.transform.SetParent(parent, false);
        part.transform.localPosition = localPosition;
        part.transform.localScale = localScale;
        part.transform.localRotation = localRotation;

        var collider = part.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }

        var renderer = part.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }
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

    Vector3 GetBladeBasePosition()
    {
        if (bladeRoot != null)
        {
            return bladeRoot.position;
        }

        return transform.position;
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
