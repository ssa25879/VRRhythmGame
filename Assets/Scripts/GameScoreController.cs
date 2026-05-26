using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public int Score => score;
    public int Combo => combo;
    public int MaxCombo => maxCombo;
    public int HitCount => hitCount;
    public int BadCount => badCount;
    public int MissCount => missCount;
    public float Hp => hp;
    public bool IsFailed => gameFailed;

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
        hp = Mathf.Max(0f, hp - missHpDamage);

        if (hp <= 0f)
        {
            gameFailed = true;
        }

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
        hp = Mathf.Max(0f, hp - missHpDamage * badHpDamageRatio);

        if (hp <= 0f)
        {
            gameFailed = true;
        }

        RefreshHud();
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
        image.color = new Color(0.1f, 1f, 0.62f, 0.92f);
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Vertical;
        image.fillOrigin = (int)Image.OriginVertical.Bottom;
        image.raycastTarget = false;

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
            hpVerticalFill.fillAmount = hpRatio;
            hpVerticalFill.color = GetHpColor(hpRatio);
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
}
