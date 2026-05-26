using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class WireIntroMuteButton
{
    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Intro.unity", OpenSceneMode.Single);
        var manager = Object.FindFirstObjectByType<IntroManager>();
        var canvas = Object.FindFirstObjectByType<Canvas>();
        if (manager == null || canvas == null)
        {
            Debug.LogError($"[IntroMute] Missing references. manager={manager != null}, canvas={canvas != null}");
            return;
        }

        var button = EnsureButton(canvas.transform);
        var label = button.GetComponentInChildren<TextMeshProUGUI>(true);

        button.onClick.RemoveAllListeners();
        UnityEventTools.AddPersistentListener(button.onClick, manager.ToggleMute);

        var serializedObject = new SerializedObject(manager);
        serializedObject.FindProperty("muteButtonText").objectReferenceValue = label;
        serializedObject.ApplyModifiedProperties();

        PlayerPrefs.SetInt("BgmMuted", 1);
        PlayerPrefs.Save();

        EditorUtility.SetDirty(manager);
        EditorUtility.SetDirty(button);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        Debug.Log("[IntroMute] Mute button wired. Default BgmMuted=1.");
    }

    private static Button EnsureButton(Transform canvasTransform)
    {
        var existing = canvasTransform.Find("MuteButton");
        GameObject buttonObject;
        if (existing != null)
        {
            buttonObject = existing.gameObject;
        }
        else
        {
            buttonObject = new GameObject("MuteButton");
            buttonObject.transform.SetParent(canvasTransform, false);
            buttonObject.AddComponent<RectTransform>();
            buttonObject.AddComponent<Image>();
            buttonObject.AddComponent<Button>();
        }

        var rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(310f, -220f);
        rect.sizeDelta = new Vector2(150f, 46f);

        var image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.05f, 0.08f, 0.14f, 0.92f);

        var button = buttonObject.GetComponent<Button>();
        if (button == null)
        {
            button = buttonObject.AddComponent<Button>();
        }

        var label = buttonObject.transform.Find("Label");
        GameObject labelObject;
        if (label != null)
        {
            labelObject = label.gameObject;
        }
        else
        {
            labelObject = new GameObject("Label");
            labelObject.transform.SetParent(buttonObject.transform, false);
            labelObject.AddComponent<RectTransform>();
            labelObject.AddComponent<TextMeshProUGUI>();
        }

        var labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        var text = labelObject.GetComponent<TextMeshProUGUI>();
        text.text = "MUTE ON";
        text.fontSize = 22f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;

        return button;
    }
}
