using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class ApplyIntroSciFiUIStyle
{
    const string IntroScenePath = "Assets/Scenes/Intro.unity";
    const string ButtonActivePath = "Assets/Sci-Fi UI/_SciFi_GUISkin_/Skin_Assets/buttons/button_active.png";
    const string ButtonPressedPath = "Assets/Sci-Fi UI/_SciFi_GUISkin_/Skin_Assets/buttons/button_pushed.png";
    const string WindowPath = "Assets/Sci-Fi UI/_SciFi_GUISkin_/Skin_Assets/window/window_transparent.png";
    const string ArrowLeftPath = "Assets/Sci-Fi UI/_SciFi_GUISkin_/Bonus_Assets/arrows/arrow1_left.png";
    const string ArrowRightPath = "Assets/Sci-Fi UI/_SciFi_GUISkin_/Bonus_Assets/arrows/arrow1_right.png";

    public static void Execute()
    {
        EditorSceneManager.OpenScene(IntroScenePath, OpenSceneMode.Single);

        var canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("[ApplyIntroSciFiUIStyle] Canvas를 찾지 못했습니다.");
            return;
        }

        var activeSprite = LoadSprite(ButtonActivePath, new Vector4(18f, 18f, 18f, 18f));
        var pressedSprite = LoadSprite(ButtonPressedPath, new Vector4(18f, 18f, 18f, 18f));
        var windowSprite = LoadSprite(WindowPath, new Vector4(28f, 28f, 28f, 28f));
        var leftArrowSprite = LoadSprite(ArrowLeftPath, Vector4.zero);
        var rightArrowSprite = LoadSprite(ArrowRightPath, Vector4.zero);
        var fontAsset = LoadOrCreateFontAsset();

        var canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(1456f, 988f);

        var panel = EnsureImage(canvas.transform, "SciFiMenuPanel", new Vector2(0f, 0f), new Vector2(1352f, 858f));
        panel.transform.SetAsFirstSibling();
        panel.sprite = windowSprite;
        panel.type = Image.Type.Sliced;
        panel.color = new Color(0.05f, 0.12f, 0.20f, 0.86f);
        panel.raycastTarget = false;

        var titleGlow = EnsureImage(canvas.transform, "SciFiTitleGlow", new Vector2(0f, 338f), new Vector2(1092f, 143f));
        titleGlow.transform.SetSiblingIndex(1);
        titleGlow.sprite = windowSprite;
        titleGlow.type = Image.Type.Sliced;
        titleGlow.color = new Color(0.0f, 0.72f, 1f, 0.18f);
        titleGlow.raycastTarget = false;

        StyleText("TitleText", fontAsset, 94f, new Color(0.75f, 0.96f, 1f, 1f), FontStyles.Bold, new Vector2(0f, 348f), new Vector2(1170f, 125f));
        StyleText("StageNameText", fontAsset, 55f, new Color(1f, 0.92f, 0.68f, 1f), FontStyles.Bold, new Vector2(0f, -169f), new Vector2(1066f, 96f));

        var subtitle = EnsureText(canvas.transform, "SciFiSubtitle", "SELECT STAGE", new Vector2(0f, 254f), new Vector2(832f, 60f));
        subtitle.font = fontAsset;
        subtitle.fontSize = 36f;
        subtitle.fontStyle = FontStyles.Bold;
        subtitle.color = new Color(0.52f, 0.82f, 1f, 0.92f);
        subtitle.alignment = TextAlignmentOptions.Center;

        StylePanel("ThumbnailBG", windowSprite, new Color(0.02f, 0.07f, 0.13f, 0.90f), new Vector2(0f, 62f), new Vector2(494f, 338f));
        StyleButton("StartButton", windowSprite, activeSprite, pressedSprite, fontAsset, new Vector2(0f, -372f), new Vector2(559f, 125f), "PLAY", 55f);
        StyleButton("MuteButton", windowSprite, activeSprite, pressedSprite, fontAsset, new Vector2(449f, -372f), new Vector2(319f, 96f), null, 34f);
        StyleArrowButton("PrevButton", leftArrowSprite, activeSprite, pressedSprite, fontAsset, new Vector2(-585f, 62f), new Vector2(146f, 317f), "<", 68f);
        StyleArrowButton("NextButton", rightArrowSprite, activeSprite, pressedSprite, fontAsset, new Vector2(585f, 62f), new Vector2(146f, 317f), ">", 68f);

        var vignetteTop = EnsureImage(canvas.transform, "SciFiTopAccent", new Vector2(0f, 465f), new Vector2(1196f, 13f));
        vignetteTop.color = new Color(0f, 0.85f, 1f, 0.56f);
        vignetteTop.raycastTarget = false;

        var vignetteBottom = EnsureImage(canvas.transform, "SciFiBottomAccent", new Vector2(0f, -465f), new Vector2(1196f, 13f));
        vignetteBottom.color = new Color(1f, 0.15f, 0.65f, 0.45f);
        vignetteBottom.raycastTarget = false;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        Debug.Log("[ApplyIntroSciFiUIStyle] Intro UI Sci-Fi 스타일 적용 완료");
    }

    static Sprite LoadSprite(string path, Vector4 border)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            var changed = false;
            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                changed = true;
            }
            if (importer.spriteImportMode != SpriteImportMode.Single)
            {
                importer.spriteImportMode = SpriteImportMode.Single;
                changed = true;
            }
            if (importer.alphaIsTransparency == false)
            {
                importer.alphaIsTransparency = true;
                changed = true;
            }
            if (importer.spriteBorder != border)
            {
                importer.spriteBorder = border;
                changed = true;
            }
            if (changed)
            {
                importer.SaveAndReimport();
            }
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    static TMP_FontAsset LoadOrCreateFontAsset()
    {
        return null;
    }

    static Image EnsureImage(Transform parent, string name, Vector2 anchoredPosition, Vector2 size)
    {
        var child = parent.Find(name);
        if (child != null && child.GetComponent<RectTransform>() == null)
        {
            Object.DestroyImmediate(child.gameObject);
            child = null;
        }

        var go = child != null ? child.gameObject : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        if (child == null)
        {
            go.transform.SetParent(parent, false);
        }

        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        return go.GetComponent<Image>() ?? go.AddComponent<Image>();
    }

    static TextMeshProUGUI EnsureText(Transform parent, string name, string text, Vector2 anchoredPosition, Vector2 size)
    {
        var child = parent.Find(name);
        if (child != null && child.GetComponent<RectTransform>() == null)
        {
            Object.DestroyImmediate(child.gameObject);
            child = null;
        }

        var go = child != null ? child.gameObject : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        if (child == null)
        {
            go.transform.SetParent(parent, false);
        }

        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        var tmp = go.GetComponent<TextMeshProUGUI>() ?? go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.raycastTarget = false;
        return tmp;
    }

    static void StylePanel(string name, Sprite sprite, Color color, Vector2 anchoredPosition, Vector2 size)
    {
        var go = GameObject.Find(name);
        if (go == null)
        {
            return;
        }

        var rect = go.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
        }

        var image = go.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = sprite;
            image.type = Image.Type.Sliced;
            image.color = color;
        }
    }

    static void StyleText(string name, TMP_FontAsset font, float size, Color color, FontStyles style, Vector2 anchoredPosition, Vector2 rectSize)
    {
        var go = GameObject.Find(name);
        if (go == null)
        {
            return;
        }

        var rect = go.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = rectSize;
        }

        var tmp = go.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            if (font != null)
            {
                tmp.font = font;
            }
            tmp.fontSize = size;
            tmp.fontStyle = style;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false;
        }
    }

    static void StyleButton(string name, Sprite normal, Sprite highlighted, Sprite pressed, TMP_FontAsset font, Vector2 anchoredPosition, Vector2 size, string labelOverride, float fontSize)
    {
        var go = GameObject.Find(name);
        if (go == null)
        {
            return;
        }

        var rect = go.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
        }

        var image = go.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = normal;
            image.type = Image.Type.Sliced;
            image.color = new Color(0.04f, 0.28f, 0.42f, 0.92f);
        }

        var button = go.GetComponent<Button>();
        if (button != null)
        {
            button.targetGraphic = image;
            button.transition = Selectable.Transition.SpriteSwap;
            button.spriteState = new SpriteState
            {
                highlightedSprite = highlighted,
                selectedSprite = highlighted,
                pressedSprite = pressed
            };
        }

        var label = go.transform.Find("Text (TMP)") ?? go.transform.Find("Label");
        if (label == null)
        {
            return;
        }

        var labelRect = label.GetComponent<RectTransform>();
        if (labelRect != null)
        {
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
        }

        var tmp = label.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            if (!string.IsNullOrEmpty(labelOverride))
            {
                tmp.text = labelOverride;
            }
            if (font != null)
            {
                tmp.font = font;
            }
            tmp.fontSize = fontSize;
            tmp.fontStyle = FontStyles.Bold;
            tmp.color = new Color(0.72f, 0.95f, 1f, 1f);
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false;
        }
    }

    static void StyleArrowButton(string name, Sprite arrowSprite, Sprite highlighted, Sprite pressed, TMP_FontAsset font, Vector2 anchoredPosition, Vector2 size, string fallbackLabel, float fontSize)
    {
        StyleButton(name, arrowSprite, highlighted, pressed, font, anchoredPosition, size, fallbackLabel, fontSize);
        var image = GameObject.Find(name)?.GetComponent<Image>();
        if (image != null)
        {
            image.type = Image.Type.Simple;
            image.preserveAspect = true;
            image.color = new Color(0.74f, 0.95f, 1f, 1f);
        }
    }
}
