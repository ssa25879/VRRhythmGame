using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class FixGameResultXRUIInput
{
    private const string ScenePath = "Assets/Scenes/Game.unity";
    private const string LeftPrefabPath = "Assets/Samples/XR Interaction Toolkit/3.3.0/Starter Assets/Prefabs/Interactors/Left_NearFarInteractor.prefab";
    private const string RightPrefabPath = "Assets/Samples/XR Interaction Toolkit/3.3.0/Starter Assets/Prefabs/Interactors/Right_NearFarInteractor.prefab";

    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        var resultHud = FindTransform("Result HUD");
        if (resultHud == null)
        {
            Debug.LogError("[GameResultXRUI] Result HUD not found.");
            return;
        }

        var leftUi = EnsureControllerUi("Left Controller", "Left_NearFarInteractor", LeftPrefabPath, resultHud, new Color(0f, 0.55f, 1f, 0.95f));
        var rightUi = EnsureControllerUi("Right Controller", "Right_NearFarInteractor", RightPrefabPath, resultHud, new Color(1f, 0.2f, 0.2f, 0.95f));
        WireModeSwitch(leftUi, rightUi);
        FixIntroControllerRays();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[GameResultXRUI] Game result XR UI input fixed.");
    }

    private static GameObject[] EnsureControllerUi(string controllerName, string interactorName, string prefabPath, Transform targetPlane, Color pointerColor)
    {
        var controller = FindTransform(controllerName);
        if (controller == null)
        {
            Debug.LogError($"[GameResultXRUI] Controller not found: {controllerName}");
            return Array.Empty<GameObject>();
        }

        var interactor = controller.Find(interactorName);
        GameObject interactorObject;
        if (interactor == null)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogError($"[GameResultXRUI] NearFar prefab not found: {prefabPath}");
                return Array.Empty<GameObject>();
            }

            interactorObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            interactorObject.name = interactorName;
            interactorObject.transform.SetParent(controller, false);
        }
        else
        {
            interactorObject = interactor.gameObject;
        }

        interactorObject.SetActive(true);
        interactorObject.transform.localPosition = Vector3.zero;
        interactorObject.transform.localRotation = Quaternion.identity;
        interactorObject.transform.localScale = Vector3.one;
        EnableUiInteraction(interactorObject, controller);
        var pointerObject = EnsureVisiblePointer(controller, targetPlane, pointerColor);

        var teleport = controller.Find("Teleport Interactor");
        if (teleport != null)
        {
            teleport.gameObject.SetActive(false);
            EditorUtility.SetDirty(teleport.gameObject);
        }

        EditorUtility.SetDirty(interactorObject);
        Debug.Log($"[GameResultXRUI] Ready: {GetPath(interactorObject.transform)}");
        interactorObject.SetActive(false);
        pointerObject.SetActive(false);
        return new[] { interactorObject, pointerObject };
    }

    private static void EnableUiInteraction(GameObject root, Transform controller)
    {
        foreach (var behaviour in root.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (behaviour == null)
            {
                continue;
            }

            var typeName = behaviour.GetType().FullName ?? string.Empty;
            if (!typeName.Contains("NearFarInteractor", StringComparison.Ordinal) &&
                !typeName.Contains("XRRayInteractor", StringComparison.Ordinal))
            {
                continue;
            }

            behaviour.enabled = true;
            var serialized = new SerializedObject(behaviour);
            SetBool(serialized, "m_EnableUIInteraction", true);
            SetBool(serialized, "m_BlockUIOnInteractableSelection", false);
            SetObjectReference(serialized, "m_RayOriginTransform", controller);
            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(behaviour);
        }
    }

    private static GameObject EnsureVisiblePointer(Transform controller, Transform targetPlane, Color pointerColor)
    {
        var pointer = controller.Find("VisibleUIPointer");
        GameObject pointerObject;
        if (pointer == null)
        {
            pointerObject = new GameObject("VisibleUIPointer");
            pointerObject.transform.SetParent(controller, false);
        }
        else
        {
            pointerObject = pointer.gameObject;
        }

        pointerObject.SetActive(true);
        pointerObject.transform.localPosition = Vector3.zero;
        pointerObject.transform.localRotation = Quaternion.identity;
        pointerObject.transform.localScale = Vector3.one;

        var visualizer = pointerObject.GetComponent<ControllerPointerVisualizer>();
        if (visualizer == null)
        {
            visualizer = pointerObject.AddComponent<ControllerPointerVisualizer>();
        }

        var serialized = new SerializedObject(visualizer);
        SetObjectReference(serialized, "rayOrigin", controller);
        SetObjectReference(serialized, "targetPlane", targetPlane);
        serialized.FindProperty("maxLength").floatValue = 8f;
        serialized.FindProperty("lineWidth").floatValue = 0.014f;
        serialized.FindProperty("lineColor").colorValue = pointerColor;
        serialized.FindProperty("hitColor").colorValue = Color.white;
        serialized.ApplyModifiedProperties();
        EditorUtility.SetDirty(visualizer);
        return pointerObject;
    }

    private static void WireModeSwitch(GameObject[] leftUiObjects, GameObject[] rightUiObjects)
    {
        var scoreController = UnityEngine.Object.FindFirstObjectByType<GameScoreController>();
        if (scoreController == null)
        {
            Debug.LogError("[GameResultXRUI] GameScoreController not found.");
            return;
        }

        var resultObjects = Combine(leftUiObjects, rightUiObjects);
        var saberBehaviours = UnityEngine.Object.FindObjectsByType<Saber>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var saberVisuals = FindSaberVisualObjects();

        var serialized = new SerializedObject(scoreController);
        SetObjectArray(serialized.FindProperty("resultModeObjects"), resultObjects);
        SetObjectArray(serialized.FindProperty("gameplayModeObjects"), saberVisuals);
        SetObjectArray(serialized.FindProperty("gameplayModeBehaviours"), saberBehaviours);
        SetObjectArray(serialized.FindProperty("resultModeBehaviours"), Array.Empty<Behaviour>());
        serialized.ApplyModifiedProperties();
        EditorUtility.SetDirty(scoreController);

        foreach (var obj in resultObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
                EditorUtility.SetDirty(obj);
            }
        }

        foreach (var obj in saberVisuals)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                EditorUtility.SetDirty(obj);
            }
        }
    }

    private static GameObject[] FindSaberVisualObjects()
    {
        var visuals = new System.Collections.Generic.List<GameObject>();
        foreach (var saber in UnityEngine.Object.FindObjectsByType<Saber>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            foreach (var renderer in saber.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer.transform == saber.transform)
                {
                    continue;
                }

                if (!visuals.Contains(renderer.gameObject))
                {
                    visuals.Add(renderer.gameObject);
                }
            }
        }

        return visuals.ToArray();
    }

    private static T[] Combine<T>(T[] first, T[] second)
    {
        var firstLength = first != null ? first.Length : 0;
        var secondLength = second != null ? second.Length : 0;
        var result = new T[firstLength + secondLength];
        if (firstLength > 0)
        {
            Array.Copy(first, result, firstLength);
        }

        if (secondLength > 0)
        {
            Array.Copy(second, 0, result, firstLength, secondLength);
        }

        return result;
    }

    private static void SetObjectArray(SerializedProperty property, UnityEngine.Object[] values)
    {
        if (property == null)
        {
            return;
        }

        property.arraySize = values != null ? values.Length : 0;
        for (int i = 0; i < property.arraySize; i++)
        {
            property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        }
    }

    private static void FixIntroControllerRays()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Intro.unity", OpenSceneMode.Single);
        var canvas = FindTransform("Canvas");
        FixIntroControllerRay("Left Controller", "Left_NearFarInteractor", canvas);
        FixIntroControllerRay("Right Controller", "Right_NearFarInteractor", canvas);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
    }

    private static void FixIntroControllerRay(string controllerName, string interactorName, Transform targetPlane)
    {
        var controller = FindTransform(controllerName);
        if (controller == null)
        {
            Debug.LogError($"[GameResultXRUI] Intro controller not found: {controllerName}");
            return;
        }

        var interactor = controller.Find(interactorName);
        if (interactor != null)
        {
            EnableUiInteraction(interactor.gameObject, controller);
        }

        var pointer = controller.Find("VisibleUIPointer");
        if (pointer != null)
        {
            var visualizer = pointer.GetComponent<ControllerPointerVisualizer>();
            if (visualizer != null)
            {
                var serialized = new SerializedObject(visualizer);
                SetObjectReference(serialized, "rayOrigin", controller);
                if (targetPlane != null)
                {
                    SetObjectReference(serialized, "targetPlane", targetPlane);
                }

                serialized.ApplyModifiedProperties();
                EditorUtility.SetDirty(visualizer);
            }
        }

        var teleport = controller.Find("Teleport Interactor");
        if (teleport != null)
        {
            teleport.gameObject.SetActive(false);
            EditorUtility.SetDirty(teleport.gameObject);
        }
    }

    private static void SetBool(SerializedObject serialized, string propertyName, bool value)
    {
        var property = serialized.FindProperty(propertyName);
        if (property != null)
        {
            property.boolValue = value;
        }
    }

    private static void SetObjectReference(SerializedObject serialized, string propertyName, UnityEngine.Object value)
    {
        var property = serialized.FindProperty(propertyName);
        if (property != null)
        {
            property.objectReferenceValue = value;
        }
    }

    private static string GetPath(Transform transform)
    {
        var path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }

        return path;
    }

    private static Transform FindTransform(string objectName)
    {
        foreach (var transform in Resources.FindObjectsOfTypeAll<Transform>())
        {
            if (transform.name == objectName && transform.gameObject.scene.IsValid())
            {
                return transform;
            }
        }

        return null;
    }
}
