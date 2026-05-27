using UnityEngine;

public class ControllerPointerVisualizer : MonoBehaviour
{
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private Transform targetPlane;
    [SerializeField] private float maxLength = 8f;
    [SerializeField] private float lineWidth = 0.012f;
    [SerializeField] private Color lineColor = new Color(0f, 0.9f, 1f, 0.95f);
    [SerializeField] private Color hitColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private bool aimAtTargetPlaneCenter = true;

    private LineRenderer line;
    private Transform reticle;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        if (line == null)
        {
            line = gameObject.AddComponent<LineRenderer>();
        }

        line.positionCount = 2;
        line.useWorldSpace = true;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth * 0.35f;
        line.numCapVertices = 6;
        Shader lineShader = Shader.Find("Universal Render Pipeline/Particles/Unlit")
                         ?? Shader.Find("Particles/Standard Unlit")
                         ?? Shader.Find("Sprites/Default");
        line.material = new Material(lineShader);
        line.startColor = lineColor;
        line.endColor = new Color(lineColor.r, lineColor.g, lineColor.b, 0.25f);

        var reticleObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        reticleObject.name = "PointerReticle";
        reticleObject.transform.SetParent(transform, false);
        reticleObject.transform.localScale = Vector3.one * 0.045f;

        var collider = reticleObject.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }

        var renderer = reticleObject.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
            {
                color = hitColor
            };
        }

        reticle = reticleObject.transform;
    }

    private void LateUpdate()
    {
        var origin = rayOrigin != null ? rayOrigin : transform;
        var start = origin.position;
        var direction = GetPointerDirection(origin, start);
        var end = start + direction * maxLength;

        if (targetPlane != null)
        {
            var plane = new Plane(targetPlane.forward, targetPlane.position);
            var ray = new Ray(start, direction);
            if (plane.Raycast(ray, out var distance) && distance > 0f && distance <= maxLength)
            {
                end = ray.GetPoint(distance);
            }
        }

        line.SetPosition(0, start);
        line.SetPosition(1, end);

        if (reticle != null)
        {
            reticle.position = end;
        }
    }

    private Vector3 GetPointerDirection(Transform origin, Vector3 start)
    {
        if (aimAtTargetPlaneCenter && targetPlane != null)
        {
            var toPlane = targetPlane.position - start;
            if (toPlane.sqrMagnitude > 0.0001f)
            {
                return toPlane.normalized;
            }
        }

        return origin.forward;
    }
}
