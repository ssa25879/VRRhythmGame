using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class DiagnoseXRUIInput
{
    public static void Execute()
    {
        DiagnoseScene("Assets/Scenes/Intro.unity");
    }

    private static void DiagnoseScene(string scenePath)
    {
        EditorSceneManager.OpenScene(scenePath);
        Debug.Log($"[XR UI Diagnose] Scene: {scenePath}");

        foreach (var eventSystem in UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None))
        {
            Debug.Log($"[XR UI Diagnose] EventSystem: {GetPath(eventSystem.transform)}");
            foreach (var component in eventSystem.GetComponents<Component>())
            {
                Debug.Log($"[XR UI Diagnose]   Component: {component.GetType().FullName}, enabled={GetEnabled(component)}");
            }
        }

        foreach (var canvas in UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            Debug.Log($"[XR UI Diagnose] Canvas: {GetPath(canvas.transform)}, renderMode={canvas.renderMode}, worldCamera={(canvas.worldCamera != null ? canvas.worldCamera.name : "null")}, pos={canvas.transform.position}, scale={canvas.transform.localScale}");
            foreach (var component in canvas.GetComponents<Component>())
            {
                Debug.Log($"[XR UI Diagnose]   Component: {component.GetType().FullName}, enabled={GetEnabled(component)}");
            }
        }

        foreach (var button in UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
        {
            var graphic = button.targetGraphic;
            Debug.Log($"[XR UI Diagnose] Button: {GetPath(button.transform)}, interactable={button.interactable}, targetGraphic={(graphic != null ? graphic.name : "null")}, raycastTarget={(graphic != null && graphic.raycastTarget)}");
        }

        var allBehaviours = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var behaviour in allBehaviours.Where(IsXRUIRelevant))
        {
            Debug.Log($"[XR UI Diagnose] XR/UI Component: {GetPath(behaviour.transform)}, type={behaviour.GetType().FullName}, enabled={behaviour.enabled}");
        }
    }

    private static bool IsXRUIRelevant(MonoBehaviour behaviour)
    {
        var name = behaviour.GetType().FullName ?? string.Empty;
        return name.Contains("RayInteractor", StringComparison.OrdinalIgnoreCase)
            || name.Contains("PokeInteractor", StringComparison.OrdinalIgnoreCase)
            || name.Contains("UIInputModule", StringComparison.OrdinalIgnoreCase)
            || name.Contains("TrackedDeviceGraphicRaycaster", StringComparison.OrdinalIgnoreCase)
            || name.Contains("InteractorLineVisual", StringComparison.OrdinalIgnoreCase)
            || name.Contains("InputActionManager", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetEnabled(Component component)
    {
        return component is Behaviour behaviour ? behaviour.enabled.ToString() : "n/a";
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
