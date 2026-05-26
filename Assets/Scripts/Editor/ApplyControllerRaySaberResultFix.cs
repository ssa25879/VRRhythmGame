using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ApplyControllerRaySaberResultFix
{
    public static void Execute()
    {
        FixIntroControllerRays();
        SaveGameScene();
        AssetDatabase.SaveAssets();
        Debug.Log("[ControllerRaySaberResultFix] Controller UI rays, saber visual refresh, and result flow fix applied.");
    }

    private static void FixIntroControllerRays()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Intro.unity", OpenSceneMode.Single);
        var canvas = GameObject.Find("Canvas");

        FixControllerRay("Left Controller", "Left_NearFarInteractor", canvas);
        FixControllerRay("Right Controller", "Right_NearFarInteractor", canvas);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void FixControllerRay(string controllerName, string nearFarName, GameObject canvas)
    {
        var controller = GameObject.Find(controllerName);
        if (controller == null)
        {
            Debug.LogError($"[ControllerRaySaberResultFix] Controller not found: {controllerName}");
            return;
        }

        var teleport = controller.transform.Find("Teleport Interactor");
        if (teleport != null)
        {
            teleport.gameObject.SetActive(false);
            EditorUtility.SetDirty(teleport.gameObject);
        }

        var nearFar = controller.transform.Find(nearFarName);
        if (nearFar != null)
        {
            nearFar.gameObject.SetActive(true);
            foreach (var behaviour in nearFar.GetComponentsInChildren<MonoBehaviour>(true))
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
                SetObjectReference(serialized, "m_RayOriginTransform", controller.transform);
                SetBool(serialized, "m_EnableUIInteraction", true);
                SetBool(serialized, "m_BlockUIOnInteractableSelection", false);
                serialized.ApplyModifiedProperties();
                EditorUtility.SetDirty(behaviour);
            }
        }

        var pointer = controller.transform.Find("VisibleUIPointer");
        if (pointer != null)
        {
            var visualizer = pointer.GetComponent<ControllerPointerVisualizer>();
            if (visualizer != null)
            {
                var serialized = new SerializedObject(visualizer);
                SetObjectReference(serialized, "rayOrigin", controller.transform);
                if (canvas != null)
                {
                    SetObjectReference(serialized, "targetPlane", canvas.transform);
                }

                serialized.ApplyModifiedProperties();
                EditorUtility.SetDirty(visualizer);
            }
        }

        Debug.Log($"[ControllerRaySaberResultFix] Controller ray fixed: {controllerName}");
    }

    private static void SaveGameScene()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Game.unity", OpenSceneMode.Single);
        foreach (var saber in UnityEngine.Object.FindObjectsByType<Saber>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            EditorUtility.SetDirty(saber);
        }

        foreach (var controller in UnityEngine.Object.FindObjectsByType<GameSongEndController>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            EditorUtility.SetDirty(controller);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
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
}
