using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

public static class BuildGameSceneUiObjects
{
    private const string ScenePath = "Assets/Scenes/Game.unity";

    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        var controller = Object.FindFirstObjectByType<GameScoreController>();
        if (controller == null)
        {
            Debug.LogError("[BuildGameSceneUI] GameScoreController not found.");
            return;
        }

        var camera = Camera.main != null ? Camera.main : Object.FindFirstObjectByType<Camera>();
        if (camera == null)
        {
            Debug.LogError("[BuildGameSceneUI] Camera not found.");
            return;
        }

        var panelSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VRTemplateAssets/Sprites/UI/Round Radius 10.png");
        var panelOutlineSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VRTemplateAssets/Sprites/UI/Round Radius 10 Outline.png");
        var hpFrameSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/VRTemplateAssets/Sprites/UI/Circle_60x60_Vertical.png");
        var hudFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/VRTemplateAssets/Fonts/Inter/Inter-Regular SDF.asset");

        var existingRoot = camera.transform.Find("Game UI Root");
        if (existingRoot != null)
        {
            Object.DestroyImmediate(existingRoot.gameObject);
        }

        var rootObject = new GameObject("Game UI Root", typeof(RectTransform));
        rootObject.transform.SetParent(camera.transform, false);
        var root = rootObject.transform;
        root.localPosition = Vector3.zero;
        root.localRotation = Quaternion.identity;
        root.localScale = Vector3.one;

        var scoreHud = CreateCanvas(root, "Score HUD", new Vector3(0f, 0.46f, 2.15f), new Vector2(420f, 96f), 0.0011f, camera);
        var comboHud = CreateCanvas(root, "Combo HUD", new Vector3(-0.68f, 0f, 2.15f), new Vector2(260f, 132f), 0.0011f, camera);
        var hpHud = CreateCanvas(root, "HP HUD", new Vector3(0.72f, 0f, 2.15f), new Vector2(150f, 240f), 0.0011f, camera);
        var resultHud = CreateCanvas(root, "Result HUD", new Vector3(0f, 0f, 2.07f), new Vector2(560f, 420f), 0.00118f, camera);

        ConfigurePanel(scoreHud, "Score Panel", panelSprite, panelOutlineSprite, new Color(0.02f, 0.05f, 0.1f, 0.52f), new Color(0f, 0.82f, 1f, 0.7f));
        ConfigurePanel(comboHud, "Combo Panel", panelSprite, panelOutlineSprite, new Color(0.02f, 0.04f, 0.09f, 0.42f), new Color(1f, 0.1f, 0.18f, 0.65f));
        ConfigurePanel(hpHud, "HP Panel", panelSprite, panelOutlineSprite, new Color(0.02f, 0.04f, 0.09f, 0.42f), new Color(0.1f, 1f, 0.62f, 0.65f));
        ConfigurePanel(resultHud, "Result Panel", panelSprite, panelOutlineSprite, new Color(0.015f, 0.02f, 0.045f, 0.86f), new Color(0f, 0.82f, 1f, 0.82f));

        var scoreText = CreateText(scoreHud, "ScoreText", Vector2.zero, new Vector2(400f, 58f), 30f, TextAlignmentOptions.Center, hudFont);
        var missText = CreateText(scoreHud, "MissText", new Vector2(0f, -34f), new Vector2(400f, 34f), 17f, TextAlignmentOptions.Center, hudFont);
        var comboText = CreateText(comboHud, "ComboText", Vector2.zero, new Vector2(250f, 80f), 34f, TextAlignmentOptions.Center, hudFont);
        var hpText = CreateText(hpHud, "HpText", new Vector2(0f, 98f), new Vector2(140f, 36f), 21f, TextAlignmentOptions.Center, hudFont);
        var hpFill = CreateHpBar(hpHud, panelSprite, hpFrameSprite);

        var resultTitle = CreateText(resultHud, "ResultTitle", new Vector2(0f, 150f), new Vector2(500f, 54f), 36f, TextAlignmentOptions.Center, hudFont);
        var resultScore = CreateText(resultHud, "ResultScore", new Vector2(0f, 78f), new Vector2(500f, 64f), 40f, TextAlignmentOptions.Center, hudFont);
        var resultStats = CreateText(resultHud, "ResultStats", new Vector2(0f, -12f), new Vector2(500f, 100f), 22f, TextAlignmentOptions.Center, hudFont);
        var okButton = CreateButton(resultHud, "ResultOkButton", new Vector2(0f, -142f), new Vector2(210f, 68f), panelSprite, hudFont);

        scoreText.text = "SCORE 000000";
        missText.text = "READY   HIT 0  BAD 0  MISS 0";
        comboText.text = "COMBO";
        hpText.text = "HP 100";
        resultTitle.text = "RESULT";
        resultScore.text = "SCORE 000000";
        resultStats.text = "MAX COMBO 0\nHIT 0   BAD 0   MISS 0\nACCURACY 0.0%";

        root.gameObject.SetActive(true);
        scoreHud.gameObject.SetActive(true);
        comboHud.gameObject.SetActive(true);
        hpHud.gameObject.SetActive(true);
        resultHud.gameObject.SetActive(false);

        WireController(controller, root.gameObject, scoreHud.gameObject, comboHud.gameObject, hpHud.gameObject, resultHud.gameObject, scoreText, comboText, hpText, missText, resultTitle, resultScore, resultStats, hpFill, okButton, panelSprite, panelOutlineSprite, hpFrameSprite, hudFont);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[BuildGameSceneUI] Game scene UI objects created and wired.");
    }

    private static RectTransform CreateCanvas(Transform parent, string name, Vector3 localPosition, Vector2 size, float scale, Camera camera)
    {
        var obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        var transform = obj.transform;
        transform.localPosition = localPosition;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one * scale;

        var canvas = transform.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = transform.gameObject.AddComponent<Canvas>();
        }

        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = camera;

        var rect = transform.GetComponent<RectTransform>();

        rect.sizeDelta = size;

        var scaler = transform.GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = transform.gameObject.AddComponent<CanvasScaler>();
        }

        scaler.dynamicPixelsPerUnit = 14f;

        if (transform.GetComponent<GraphicRaycaster>() == null)
        {
            transform.gameObject.AddComponent<GraphicRaycaster>();
        }

        if (name == "Result HUD" && transform.GetComponent<TrackedDeviceGraphicRaycaster>() == null)
        {
            transform.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
        }

        return rect;
    }

    private static void ConfigurePanel(RectTransform parent, string name, Sprite panelSprite, Sprite outlineSprite, Color backgroundColor, Color outlineColor)
    {
        CreateImage(parent, name, Vector2.zero, parent.sizeDelta, panelSprite, backgroundColor, true).transform.SetAsFirstSibling();
        CreateImage(parent, name + " Outline", Vector2.zero, parent.sizeDelta + new Vector2(12f, 12f), outlineSprite, outlineColor, true).transform.SetAsFirstSibling();
    }

    private static TextMeshProUGUI CreateText(RectTransform parent, string name, Vector2 anchoredPosition, Vector2 size, float fontSize, TextAlignmentOptions alignment, TMP_FontAsset font)
    {
        var rect = GetOrCreateRect(parent, name);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        var text = rect.GetComponent<TextMeshProUGUI>();
        if (text == null)
        {
            text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        }

        text.font = font;
        text.fontSize = fontSize;
        text.fontStyle = FontStyles.Bold;
        text.alignment = alignment;
        text.color = Color.white;
        text.outlineWidth = 0.16f;
        text.outlineColor = new Color(0f, 0f, 0f, 0.75f);
        text.raycastTarget = false;
        return text;
    }

    private static Image CreateHpBar(RectTransform parent, Sprite fillSprite, Sprite frameSprite)
    {
        var background = CreateImage(parent, "HpBarBackground", new Vector2(0f, -16f), new Vector2(34f, 170f), frameSprite, new Color(0f, 0f, 0f, 0.72f), true);
        var backgroundRect = background.rectTransform;

        var fillRect = GetOrCreateRect(backgroundRect, "HpBarFill");
        fillRect.anchorMin = new Vector2(0.5f, 0f);
        fillRect.anchorMax = new Vector2(0.5f, 0f);
        fillRect.pivot = new Vector2(0.5f, 0f);
        fillRect.anchoredPosition = new Vector2(0f, 4f);
        fillRect.sizeDelta = new Vector2(26f, 162f);

        var fill = fillRect.GetComponent<Image>();
        if (fill == null)
        {
            fill = fillRect.gameObject.AddComponent<Image>();
        }

        fill.sprite = fillSprite;
        fill.type = fillSprite != null ? Image.Type.Sliced : Image.Type.Simple;
        fill.color = new Color(0.1f, 1f, 0.62f, 0.92f);
        fill.raycastTarget = false;

        CreateImage(backgroundRect, "HpWarningLine", new Vector2(0f, 162f * 0.35f + 4f), new Vector2(44f, 3f), null, new Color(1f, 1f, 1f, 0.75f), false);
        return fill;
    }

    private static Button CreateButton(RectTransform parent, string name, Vector2 anchoredPosition, Vector2 size, Sprite panelSprite, TMP_FontAsset font)
    {
        var image = CreateImage(parent, name, anchoredPosition, size, panelSprite, new Color(0f, 0.7f, 1f, 0.88f), true);
        image.raycastTarget = true;

        var button = image.GetComponent<Button>();
        if (button == null)
        {
            button = image.gameObject.AddComponent<Button>();
        }

        button.targetGraphic = image;
        var label = CreateText(image.rectTransform, "Label", Vector2.zero, new Vector2(190f, 54f), 28f, TextAlignmentOptions.Center, font);
        label.text = "OK";
        return button;
    }

    private static Image CreateImage(RectTransform parent, string name, Vector2 anchoredPosition, Vector2 size, Sprite sprite, Color color, bool sliced)
    {
        var rect = GetOrCreateRect(parent, name);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        var image = rect.GetComponent<Image>();
        if (image == null)
        {
            image = rect.gameObject.AddComponent<Image>();
        }

        image.sprite = sprite;
        image.type = sliced && sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static RectTransform GetOrCreateRect(RectTransform parent, string name)
    {
        var child = parent.Find(name);
        if (child == null)
        {
            var obj = new GameObject(name, typeof(RectTransform));
            obj.transform.SetParent(parent, false);
            child = obj.transform;
        }

        var rect = child.GetComponent<RectTransform>();
        return rect;
    }

    private static Transform GetOrCreateChild(Transform parent, string name)
    {
        var child = parent.Find(name);
        if (child != null)
        {
            return child;
        }

        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        return obj.transform;
    }

    private static void WireController(GameScoreController controller, GameObject hudRoot, GameObject scoreHudRoot, GameObject comboHudRoot, GameObject hpHudRoot, GameObject resultRoot, TextMeshProUGUI scoreText, TextMeshProUGUI comboText, TextMeshProUGUI hpText, TextMeshProUGUI missText, TextMeshProUGUI resultTitle, TextMeshProUGUI resultScore, TextMeshProUGUI resultStats, Image hpFill, Button okButton, Sprite panelSprite, Sprite panelOutlineSprite, Sprite hpFrameSprite, TMP_FontAsset hudFont)
    {
        var serialized = new SerializedObject(controller);
        serialized.FindProperty("hudRoot").objectReferenceValue = hudRoot;
        serialized.FindProperty("scoreHudRoot").objectReferenceValue = scoreHudRoot;
        serialized.FindProperty("comboHudRoot").objectReferenceValue = comboHudRoot;
        serialized.FindProperty("hpHudRoot").objectReferenceValue = hpHudRoot;
        serialized.FindProperty("resultRoot").objectReferenceValue = resultRoot;
        serialized.FindProperty("scoreText").objectReferenceValue = scoreText;
        serialized.FindProperty("comboText").objectReferenceValue = comboText;
        serialized.FindProperty("hpText").objectReferenceValue = hpText;
        serialized.FindProperty("missText").objectReferenceValue = missText;
        serialized.FindProperty("resultTitleText").objectReferenceValue = resultTitle;
        serialized.FindProperty("resultScoreText").objectReferenceValue = resultScore;
        serialized.FindProperty("resultStatsText").objectReferenceValue = resultStats;
        serialized.FindProperty("hpVerticalFill").objectReferenceValue = hpFill;
        serialized.FindProperty("hpVerticalFillRect").objectReferenceValue = hpFill.rectTransform;
        serialized.FindProperty("resultOkButton").objectReferenceValue = okButton;
        serialized.FindProperty("panelSprite").objectReferenceValue = panelSprite;
        serialized.FindProperty("panelOutlineSprite").objectReferenceValue = panelOutlineSprite;
        serialized.FindProperty("hpFrameSprite").objectReferenceValue = hpFrameSprite;
        serialized.FindProperty("hudFont").objectReferenceValue = hudFont;
        serialized.ApplyModifiedProperties();
        EditorUtility.SetDirty(controller);
    }
}
