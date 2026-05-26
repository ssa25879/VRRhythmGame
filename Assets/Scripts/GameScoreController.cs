using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class GameScoreController : MonoBehaviour
{
    public static GameScoreController Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameBackgroundController backgroundController;
    [SerializeField] private Spawner spawner;

    [Header("Score")]
    [SerializeField] private int maxScore = 100000;
    [SerializeField, Range(0.1f, 0.95f)] private float baseScoreRatio = 0.7f;

    [Header("HP")]
    [SerializeField] private float maxHp = 100f;
    [SerializeField] private float missHpDamage = 12f;
    [SerializeField] private float badHpDamageRatio = 0.333f;

    [Header("Result")]
    [SerializeField] private string introSceneName = "Intro";
    [SerializeField] private float failResultDelay = 0.6f;

    [Header("Runtime HUD")]
    [SerializeField] private float hudDistance = 1.9f;
    [SerializeField] private float hudScale = 0.0011f;
    [SerializeField] private Sprite panelSprite;
    [SerializeField] private Sprite panelOutlineSprite;
    [SerializeField] private Sprite hpFrameSprite;
    [SerializeField] private TMP_FontAsset hudFont;

    private int score;
    private int combo;
    private int maxCombo;
    private int hitCount;
    private int badCount;
    private int missCount;
    private int spawnedCount;
    private int expectedNotes = 1;
    private float hp;
    private float scoreValue;
    private string judgementLabel = "READY";
    private float baseScorePerNote;
    private float comboScorePool;
    private float maxComboWeight;
    private bool gameFailed;

    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI comboText;
    private TextMeshProUGUI hpText;
    private TextMeshProUGUI missText;
    private Image hpVerticalFill;
    private RectTransform hpVerticalFillRect;
    private float hpVerticalFillMaxHeight;
    private bool resultScreenShown;
    private bool resultInputEnabled;
    private bool resultConfirmWasReleased;
    private float resultInputEnabledAt;
    private string resultIntroSceneName;

    public int Score => score;
    public int Combo => combo;
    public int MaxCombo => maxCombo;
    public int HitCount => hitCount;
    public int BadCount => badCount;
    public int MissCount => missCount;
    public float Hp => hp;
    public bool IsFailed => gameFailed;
    public float Accuracy => Mathf.Clamp01(hitCount + badCount + missCount > 0 ? hitCount / (float)(hitCount + badCount + missCount) : 0f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (backgroundController == null)
        {
            backgroundController = FindFirstObjectByType<GameBackgroundController>();
        }

        if (spawner == null)
        {
            spawner = FindFirstObjectByType<Spawner>();
        }

        hp = maxHp;
    }

    private void Start()
    {
        RecalculateExpectedNotes();
        CreateHud();
        RefreshHud();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (!resultInputEnabled || Time.unscaledTime < resultInputEnabledAt)
        {
            return;
        }

        bool xrConfirmPressed = WasXRConfirmPressed();
        if (!resultConfirmWasReleased)
        {
            resultConfirmWasReleased = !xrConfirmPressed;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetKeyDown(KeyCode.Space) ||
            xrConfirmPressed)
        {
            LoadIntroFromResult();
        }
    }

    public void RegisterNoteSpawned()
    {
        spawnedCount++;
    }

    public void RegisterHit()
    {
        if (gameFailed)
        {
            return;
        }

        combo++;
        maxCombo = Mathf.Max(maxCombo, combo);
        hitCount++;
        judgementLabel = "HIT";

        float comboScore = comboScorePool * combo / maxComboWeight;
        scoreValue = Mathf.Min(maxScore, scoreValue + baseScorePerNote + comboScore);
        score = Mathf.Min(maxScore, Mathf.RoundToInt(scoreValue));
        RefreshHud();
    }

    public void RegisterMiss(string label = "MISS")
    {
        if (gameFailed)
        {
            return;
        }

        combo = 0;
        missCount++;
        judgementLabel = label;
        ApplyHpDamage(missHpDamage);
        RefreshHud();
    }

    public void RegisterBad()
    {
        if (gameFailed)
        {
            return;
        }

        badCount++;
        judgementLabel = "BAD";
        ApplyHpDamage(missHpDamage * badHpDamageRatio);
        RefreshHud();
    }

    public void ShowResultScreen(string introSceneName)
    {
        if (resultScreenShown)
        {
            return;
        }

        resultScreenShown = true;
        resultConfirmWasReleased = false;
        resultIntroSceneName = introSceneName;
        CreateResultScreen(introSceneName);
        resultInputEnabledAt = Time.unscaledTime + 0.25f;
        resultInputEnabled = true;
    }

    private void ApplyHpDamage(float damage)
    {
        hp = Mathf.Max(0f, hp - damage);
        if (hp <= 0f)
        {
            FailGame();
        }
    }

    private void FailGame()
    {
        if (gameFailed)
        {
            return;
        }

        gameFailed = true;
        judgementLabel = "FAILED";

        if (spawner != null)
        {
            spawner.enabled = false;
        }

        if (backgroundController != null && backgroundController.bgmSource != null)
        {
            backgroundController.bgmSource.Stop();
        }

        foreach (var note in FindObjectsByType<Cube>(FindObjectsSortMode.None))
        {
            Destroy(note.gameObject);
        }

        StartCoroutine(ShowFailResultAfterDelay());
    }

    private IEnumerator ShowFailResultAfterDelay()
    {
        yield return new WaitForSeconds(failResultDelay);
        ShowResultScreen(introSceneName);
    }

    private void RecalculateExpectedNotes()
    {
        float bpm = 120f;
        float clipLength = 0f;

        if (backgroundController != null && backgroundController.CurrentStage != null)
        {
            bpm = Mathf.Max(1f, backgroundController.CurrentStage.bpm);
            if (backgroundController.CurrentStage.bgm != null)
            {
                clipLength = backgroundController.CurrentStage.bgm.length;
            }
        }
        else if (backgroundController != null && backgroundController.bgmSource != null && backgroundController.bgmSource.clip != null)
        {
            clipLength = backgroundController.bgmSource.clip.length;
        }

        int beatsPerSpawn = spawner != null ? Mathf.Max(1, spawner.beatsPerSpawn) : 1;
        float beatDuration = 60f / bpm;
        expectedNotes = Mathf.Max(1, Mathf.FloorToInt(clipLength / (beatDuration * beatsPerSpawn)) + 1);

        float baseScorePool = maxScore * baseScoreRatio;
        comboScorePool = maxScore - baseScorePool;
        baseScorePerNote = baseScorePool / expectedNotes;
        maxComboWeight = expectedNotes * (expectedNotes + 1f) * 0.5f;

        Debug.Log($"[Score] expectedNotes={expectedNotes}, basePerNote={baseScorePerNote:0.00}, comboPool={comboScorePool:0.00}");
    }

    private void CreateHud()
    {
        if (scoreText != null)
        {
            return;
        }

        var cameraTransform = Camera.main != null ? Camera.main.transform : transform;
        var scoreRect = CreateHudCanvas(cameraTransform, "Score HUD", new Vector3(0f, 0.46f, hudDistance), new Vector2(420f, 96f));
        var comboRect = CreateHudCanvas(cameraTransform, "Combo HUD", new Vector3(-0.68f, 0f, hudDistance), new Vector2(260f, 132f));
        var hpRect = CreateHudCanvas(cameraTransform, "HP HUD", new Vector3(0.72f, 0f, hudDistance), new Vector2(150f, 240f));

        AddPanel(scoreRect, "Score Panel", new Color(0.02f, 0.05f, 0.1f, 0.52f), new Color(0f, 0.82f, 1f, 0.7f));
        AddPanel(comboRect, "Combo Panel", new Color(0.02f, 0.04f, 0.09f, 0.42f), new Color(1f, 0.1f, 0.18f, 0.65f));
        AddPanel(hpRect, "HP Panel", new Color(0.02f, 0.04f, 0.09f, 0.42f), new Color(0.1f, 1f, 0.62f, 0.65f));

        scoreText = CreateText(scoreRect, "ScoreText", Vector2.zero, new Vector2(400f, 58f), 30f, TextAlignmentOptions.Center);
        missText = CreateText(scoreRect, "MissText", new Vector2(0f, -34f), new Vector2(400f, 34f), 17f, TextAlignmentOptions.Center);
        comboText = CreateText(comboRect, "ComboText", Vector2.zero, new Vector2(250f, 80f), 34f, TextAlignmentOptions.Center);
        hpText = CreateText(hpRect, "HpText", new Vector2(0f, 98f), new Vector2(140f, 36f), 21f, TextAlignmentOptions.Center);
        hpVerticalFill = CreateHpBar(hpRect);
    }

    private RectTransform CreateHudCanvas(Transform parent, string name, Vector3 localPosition, Vector2 size)
    {
        var canvasObject = new GameObject(name);
        canvasObject.transform.SetParent(parent, false);
        canvasObject.transform.localPosition = localPosition;
        canvasObject.transform.localRotation = Quaternion.identity;
        canvasObject.transform.localScale = Vector3.one * hudScale;

        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        var rect = canvasObject.GetComponent<RectTransform>();
        rect.sizeDelta = size;

        canvasObject.AddComponent<CanvasScaler>().dynamicPixelsPerUnit = 14f;
        return rect;
    }

    private void AddPanel(RectTransform parent, string name, Color backgroundColor, Color outlineColor)
    {
        CreatePanelImage(parent, name, panelSprite, backgroundColor, Vector2.zero, parent.sizeDelta);
        CreatePanelImage(parent, name + " Outline", panelOutlineSprite, outlineColor, Vector2.zero, parent.sizeDelta + new Vector2(12f, 12f));
    }

    private Image CreatePanelImage(RectTransform parent, string name, Sprite sprite, Color color, Vector2 anchoredPosition, Vector2 size)
    {
        var imageObject = new GameObject(name);
        imageObject.transform.SetParent(parent, false);
        imageObject.transform.SetAsFirstSibling();

        var rect = imageObject.AddComponent<RectTransform>();
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        var image = imageObject.AddComponent<Image>();
        image.sprite = sprite;
        image.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private TextMeshProUGUI CreateText(RectTransform parent, string name, Vector2 anchoredPosition, Vector2 size, float fontSize, TextAlignmentOptions alignment)
    {
        var textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);

        var rect = textObject.AddComponent<RectTransform>();
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        var text = textObject.AddComponent<TextMeshProUGUI>();
        if (hudFont != null)
        {
            text.font = hudFont;
        }

        text.fontSize = fontSize;
        text.fontStyle = FontStyles.Bold;
        text.alignment = alignment;
        text.color = Color.white;
        text.outlineWidth = 0.16f;
        text.outlineColor = new Color(0f, 0f, 0f, 0.75f);
        text.raycastTarget = false;
        return text;
    }

    private Image CreateHpBar(RectTransform parent)
    {
        var background = new GameObject("HpBarBackground");
        background.transform.SetParent(parent, false);

        var backgroundRect = background.AddComponent<RectTransform>();
        backgroundRect.anchoredPosition = new Vector2(0f, -16f);
        backgroundRect.sizeDelta = new Vector2(34f, 170f);

        var backgroundImage = background.AddComponent<Image>();
        backgroundImage.sprite = hpFrameSprite;
        backgroundImage.type = hpFrameSprite != null ? Image.Type.Sliced : Image.Type.Simple;
        backgroundImage.color = new Color(0f, 0f, 0f, 0.72f);
        backgroundImage.raycastTarget = false;

        var fill = new GameObject("HpBarFill");
        fill.transform.SetParent(backgroundRect, false);

        var fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0f, 0f);
        fillRect.anchorMax = new Vector2(1f, 1f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        var image = fill.AddComponent<Image>();
        image.sprite = panelSprite;
        image.color = new Color(0.1f, 1f, 0.62f, 0.92f);
        image.type = panelSprite != null ? Image.Type.Sliced : Image.Type.Simple;
        image.raycastTarget = false;

        hpVerticalFillRect = fillRect;
        hpVerticalFillMaxHeight = backgroundRect.sizeDelta.y - 8f;
        fillRect.anchorMin = new Vector2(0.5f, 0f);
        fillRect.anchorMax = new Vector2(0.5f, 0f);
        fillRect.pivot = new Vector2(0.5f, 0f);
        fillRect.anchoredPosition = new Vector2(0f, 4f);
        fillRect.sizeDelta = new Vector2(backgroundRect.sizeDelta.x - 8f, hpVerticalFillMaxHeight);

        var warningLine = new GameObject("HpWarningLine");
        warningLine.transform.SetParent(backgroundRect, false);

        var warningRect = warningLine.AddComponent<RectTransform>();
        warningRect.anchorMin = new Vector2(0f, 0.35f);
        warningRect.anchorMax = new Vector2(1f, 0.35f);
        warningRect.offsetMin = new Vector2(-5f, -1.5f);
        warningRect.offsetMax = new Vector2(5f, 1.5f);

        var warningImage = warningLine.AddComponent<Image>();
        warningImage.color = new Color(1f, 1f, 1f, 0.75f);
        warningImage.raycastTarget = false;
        return image;
    }

    private void RefreshHud()
    {
        if (scoreText == null)
        {
            return;
        }

        scoreText.text = $"SCORE {score:000000}";
        comboText.text = combo > 0 ? $"{combo} COMBO" : "COMBO";
        hpText.text = $"HP {Mathf.CeilToInt(hp)}";
        missText.text = gameFailed ? "FAILED" : $"{judgementLabel}   HIT {hitCount}  BAD {badCount}  MISS {missCount}";

        if (hpVerticalFill != null)
        {
            float hpRatio = Mathf.Clamp01(hp / maxHp);
            hpVerticalFill.color = GetHpColor(hpRatio);

            if (hpVerticalFillRect != null)
            {
                hpVerticalFillRect.sizeDelta = new Vector2(
                    hpVerticalFillRect.sizeDelta.x,
                    hpVerticalFillMaxHeight * hpRatio);
            }
        }
    }

    private Color GetHpColor(float hpRatio)
    {
        if (hpRatio > 0.55f)
        {
            return new Color(0.1f, 1f, 0.62f, 0.92f);
        }

        if (hpRatio > 0.35f)
        {
            return new Color(1f, 0.82f, 0.1f, 0.95f);
        }

        return new Color(1f, 0.12f, 0.08f, 0.95f);
    }

    private void CreateResultScreen(string introSceneName)
    {
        var cameraTransform = Camera.main != null ? Camera.main.transform : transform;
        var resultRect = CreateHudCanvas(cameraTransform, "Result HUD", new Vector3(0f, 0f, hudDistance - 0.08f), new Vector2(560f, 420f));
        resultRect.localScale = Vector3.one * (hudScale * 1.08f);

        var graphicRaycaster = resultRect.gameObject.GetComponent<GraphicRaycaster>();
        if (graphicRaycaster == null)
        {
            graphicRaycaster = resultRect.gameObject.AddComponent<GraphicRaycaster>();
        }

        if (resultRect.gameObject.GetComponent<TrackedDeviceGraphicRaycaster>() == null)
        {
            resultRect.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
        }

        AddPanel(resultRect, "Result Panel", new Color(0.015f, 0.02f, 0.045f, 0.86f), new Color(0f, 0.82f, 1f, 0.82f));

        CreateText(resultRect, "ResultTitle", new Vector2(0f, 150f), new Vector2(500f, 54f), 36f, TextAlignmentOptions.Center)
            .text = gameFailed ? "FAILED" : "RESULT";
        CreateText(resultRect, "ResultScore", new Vector2(0f, 78f), new Vector2(500f, 64f), 40f, TextAlignmentOptions.Center)
            .text = $"SCORE {score:000000}";
        CreateText(resultRect, "ResultStats", new Vector2(0f, -12f), new Vector2(500f, 100f), 22f, TextAlignmentOptions.Center)
            .text = $"MAX COMBO {maxCombo}\nHIT {hitCount}   BAD {badCount}   MISS {missCount}\nACCURACY {Accuracy * 100f:0.0}%";

        CreateOkButton(resultRect, introSceneName);
    }

    private void CreateOkButton(RectTransform parent, string introSceneName)
    {
        var buttonObject = new GameObject("ResultOkButton");
        buttonObject.transform.SetParent(parent, false);

        var rect = buttonObject.AddComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0f, -142f);
        rect.sizeDelta = new Vector2(210f, 68f);

        var image = buttonObject.AddComponent<Image>();
        image.sprite = panelSprite;
        image.type = panelSprite != null ? Image.Type.Sliced : Image.Type.Simple;
        image.color = new Color(0f, 0.7f, 1f, 0.88f);

        var button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(LoadIntroFromResult);

        var label = CreateText(rect, "Label", Vector2.zero, new Vector2(190f, 54f), 28f, TextAlignmentOptions.Center);
        label.text = "OK";
    }

    private void LoadIntroFromResult()
    {
        resultInputEnabled = false;
        SceneManager.LoadScene(string.IsNullOrWhiteSpace(resultIntroSceneName) ? "Intro" : resultIntroSceneName);
    }

    private bool WasXRConfirmPressed()
    {
        return WasXRButtonPressed(XRNode.LeftHand) || WasXRButtonPressed(XRNode.RightHand);
    }

    private bool WasXRButtonPressed(XRNode node)
    {
        var device = InputDevices.GetDeviceAtXRNode(node);
        if (!device.isValid)
        {
            return false;
        }

        return IsPressed(device, CommonUsages.triggerButton) ||
               IsPressed(device, CommonUsages.primaryButton);
    }

    private bool IsPressed(InputDevice device, InputFeatureUsage<bool> usage)
    {
        return device.TryGetFeatureValue(usage, out bool pressed) && pressed;
    }
}
