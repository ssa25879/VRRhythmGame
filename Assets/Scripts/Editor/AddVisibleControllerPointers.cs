using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AddVisibleControllerPointers
{
    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Intro.unity");

        var changed = false;
        RemoveOldPointer("Left_NearFarInteractor");
        RemoveOldPointer("Right_NearFarInteractor");
        changed |= EnsurePointer("Left Controller", new Color(0f, 0.55f, 1f, 0.95f));
        changed |= EnsurePointer("Right Controller", new Color(1f, 0.2f, 0.2f, 0.95f));

        if (changed)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
        }

        Debug.Log($"Visible controller pointer setup complete. Changed={changed}");
    }

    private static bool EnsurePointer(string controllerName, Color color)
    {
        var controller = GameObject.Find(controllerName);
        if (controller == null)
        {
            Debug.LogError($"Controller not found: {controllerName}");
            return false;
        }

        var canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("Canvas not found.");
            return false;
        }

        var pointer = controller.transform.Find("VisibleUIPointer");
        GameObject pointerObject;
        if (pointer == null)
        {
            pointerObject = new GameObject("VisibleUIPointer");
            pointerObject.transform.SetParent(controller.transform, false);
            pointerObject.transform.localPosition = Vector3.zero;
            pointerObject.transform.localRotation = Quaternion.identity;
            pointerObject.transform.localScale = Vector3.one;
        }
        else
        {
            pointerObject = pointer.gameObject;
            pointerObject.SetActive(true);
        }

        var visualizer = pointerObject.GetComponent<ControllerPointerVisualizer>();
        if (visualizer == null)
        {
            visualizer = pointerObject.AddComponent<ControllerPointerVisualizer>();
        }

        var visualizerSerialized = new SerializedObject(visualizer);
        visualizerSerialized.FindProperty("rayOrigin").objectReferenceValue = controller.transform;
        visualizerSerialized.FindProperty("targetPlane").objectReferenceValue = canvas.transform;
        visualizerSerialized.FindProperty("maxLength").floatValue = 8f;
        visualizerSerialized.FindProperty("lineWidth").floatValue = 0.014f;
        visualizerSerialized.FindProperty("lineColor").colorValue = color;
        visualizerSerialized.FindProperty("hitColor").colorValue = Color.white;
        visualizerSerialized.ApplyModifiedProperties();
        EditorUtility.SetDirty(visualizer);

        var lineRenderer = pointerObject.GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
        }

        Debug.Log($"Visible UI pointer ready: {GetPath(pointerObject.transform)}");
        return true;
    }

    private static void RemoveOldPointer(string interactorName)
    {
        var interactor = GameObject.Find(interactorName);
        if (interactor == null)
        {
            return;
        }

        var pointer = interactor.transform.Find("VisibleUIPointer");
        if (pointer != null)
        {
            Object.DestroyImmediate(pointer.gameObject);
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
}
