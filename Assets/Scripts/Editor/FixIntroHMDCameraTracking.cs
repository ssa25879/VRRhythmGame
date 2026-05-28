using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public static class FixIntroHMDCameraTracking
{
    private const string IntroScenePath = "Assets/Scenes/Intro.unity";
    private const string XriDefaultInputActionsPath = "Assets/Samples/XR Interaction Toolkit/3.3.0/Starter Assets/XRI Default Input Actions.inputactions";
    private const string XrOriginPath = "XR Origin Hands (XR Rig)";
    private const string CameraOffsetPath = "XR Origin Hands (XR Rig)/Camera Offset";
    private const string MainCameraPath = "XR Origin Hands (XR Rig)/Camera Offset/Main Camera";

    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene(IntroScenePath);

        var xrOriginObject = GameObject.Find(XrOriginPath);
        var cameraOffsetObject = GameObject.Find(CameraOffsetPath);
        var cameraObject = GameObject.Find(MainCameraPath);

        if (xrOriginObject == null || cameraOffsetObject == null || cameraObject == null)
        {
            Debug.LogError($"[Intro HMD Camera Fix] Missing XR camera hierarchy. origin={xrOriginObject != null}, offset={cameraOffsetObject != null}, camera={cameraObject != null}");
            return;
        }

        cameraObject.SetActive(true);
        cameraObject.tag = "MainCamera";
        cameraObject.transform.localPosition = Vector3.zero;
        cameraObject.transform.localRotation = Quaternion.identity;

        var camera = cameraObject.GetComponent<Camera>();
        if (camera == null)
        {
            camera = cameraObject.AddComponent<Camera>();
        }

        camera.enabled = true;
        camera.stereoTargetEye = StereoTargetEyeMask.Both;

        var listener = cameraObject.GetComponent<AudioListener>();
        if (listener == null)
        {
            listener = cameraObject.AddComponent<AudioListener>();
        }

        listener.enabled = true;
        DisableOtherMainCamerasAndListeners(cameraObject, listener);

        var trackedPoseDriver = cameraObject.GetComponent<TrackedPoseDriver>();
        if (trackedPoseDriver == null)
        {
            trackedPoseDriver = cameraObject.AddComponent<TrackedPoseDriver>();
        }

        trackedPoseDriver.enabled = true;
        trackedPoseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
        trackedPoseDriver.updateType = TrackedPoseDriver.UpdateType.UpdateAndBeforeRender;
        trackedPoseDriver.ignoreTrackingState = false;
        var positionReference = LoadActionReference("XRI Head/Position");
        var rotationReference = LoadActionReference("XRI Head/Rotation");
        var trackingStateReference = LoadActionReference("XRI Head/Tracking State");
        trackedPoseDriver.positionInput = new InputActionProperty(positionReference);
        trackedPoseDriver.rotationInput = new InputActionProperty(rotationReference);
        trackedPoseDriver.trackingStateInput = new InputActionProperty(trackingStateReference);
        ForceActionReferences(trackedPoseDriver, positionReference, rotationReference, trackingStateReference);
        RemoveTransientActionReferences();

        RelinkXrOriginCamera(xrOriginObject, camera, cameraOffsetObject);
        RelinkWorldSpaceCanvases(camera);

        EditorUtility.SetDirty(cameraObject);
        EditorUtility.SetDirty(xrOriginObject);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        Debug.Log("[Intro HMD Camera Fix] Main Camera now uses Tracked Pose Driver (Input System) with XRHMD center eye bindings.");
    }

    private static InputActionReference LoadActionReference(string actionPath)
    {
        var asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(XriDefaultInputActionsPath);
        if (asset == null)
        {
            Debug.LogError($"[Intro HMD Camera Fix] Missing input actions asset: {XriDefaultInputActionsPath}");
            return null;
        }

        var action = asset.FindAction(actionPath, true);
        foreach (var actionReference in AssetDatabase.LoadAllAssetsAtPath(XriDefaultInputActionsPath))
        {
            if (actionReference is InputActionReference reference && reference.action != null && reference.action.id == action.id)
            {
                return reference;
            }
        }

        return InputActionReference.Create(action);
    }

    private static void ForceActionReferences(
        TrackedPoseDriver trackedPoseDriver,
        InputActionReference positionReference,
        InputActionReference rotationReference,
        InputActionReference trackingStateReference)
    {
        var serializedObject = new SerializedObject(trackedPoseDriver);
        SetActionReference(serializedObject, "m_PositionInput", positionReference);
        SetActionReference(serializedObject, "m_RotationInput", rotationReference);
        SetActionReference(serializedObject, "m_TrackingStateInput", trackingStateReference);
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(trackedPoseDriver);
    }

    private static void SetActionReference(SerializedObject serializedObject, string propertyName, InputActionReference reference)
    {
        serializedObject.FindProperty($"{propertyName}.m_UseReference").boolValue = true;
        serializedObject.FindProperty($"{propertyName}.m_Reference").objectReferenceValue = reference;
    }

    private static void RemoveTransientActionReferences()
    {
        foreach (var reference in Resources.FindObjectsOfTypeAll<InputActionReference>())
        {
            if (reference == null || AssetDatabase.Contains(reference))
            {
                continue;
            }

            if (reference.name.StartsWith("XRI Head/"))
            {
                Object.DestroyImmediate(reference, true);
            }
        }
    }

    private static void DisableOtherMainCamerasAndListeners(GameObject activeCameraObject, AudioListener activeListener)
    {
        foreach (var camera in Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (camera.gameObject == activeCameraObject)
            {
                continue;
            }

            if (camera.CompareTag("MainCamera"))
            {
                camera.tag = "Untagged";
            }
        }

        foreach (var listener in Object.FindObjectsByType<AudioListener>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (listener != activeListener)
            {
                listener.enabled = false;
            }
        }
    }

    private static void RelinkXrOriginCamera(GameObject xrOriginObject, Camera camera, GameObject cameraOffsetObject)
    {
        foreach (var component in xrOriginObject.GetComponents<MonoBehaviour>())
        {
            if (component == null || component.GetType().Name != "XROrigin")
            {
                continue;
            }

            var serializedObject = new SerializedObject(component);
            var cameraProperty = serializedObject.FindProperty("m_Camera");
            if (cameraProperty != null)
            {
                cameraProperty.objectReferenceValue = camera;
            }

            var cameraOffsetProperty = serializedObject.FindProperty("m_CameraFloorOffsetObject");
            if (cameraOffsetProperty != null)
            {
                cameraOffsetProperty.objectReferenceValue = cameraOffsetObject;
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(component);
        }
    }

    private static void RelinkWorldSpaceCanvases(Camera camera)
    {
        foreach (var canvas in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                canvas.worldCamera = camera;
                EditorUtility.SetDirty(canvas);
            }
        }
    }
}
