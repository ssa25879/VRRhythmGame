using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

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
    [SerializeField] private float hitHpRecoverPerCombo = 0.35f;
    [SerializeField] private float maxHitHpRecover = 6f;
    [SerializeField] private float hpFeedbackDuration = 0.75f;

    [Header("Result")]
    [SerializeField] private string introSceneName = "Intro";
    [SerializeField] private float failResultDelay = 0.6f;
    [SerializeField] private float clearResultDelay = 3f;

    [Header("Scene UI")]
    [SerializeField] private Sprite panelSprite;
    [SerializeField] private Sprite panelOutlineSprite;
    [SerializeField] private Sprite hpFrameSprite;
    [SerializeField] private Sprite sciFiButtonActiveSprite;
    [SerializeField] private Sprite sciFiButtonPressedSprite;
    [SerializeField] private TMP_FontAsset hudFont;
    [SerializeField] private GameObject hudRoot;
    [SerializeField] private GameObject scoreHudRoot;
    [SerializeField] private GameObject comboHudRoot;
    [SerializeField] private GameObject hpHudRoot;
    [SerializeField] private GameObject resultRoot;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI missText;
    [SerializeField] private TextMeshProUGUI resultTitleText;
    [SerializeField] private TextMeshProUGUI resultScoreText;
    [SerializeField] private TextMeshProUGUI resultStatsText;
    [SerializeField] private Image hpVerticalFill;
    [SerializeField] private RectTransform hpVerticalFillRect;
    [SerializeField] private Button resultOkButton;
    [SerializeField] private GameObject[] gameplayModeObjects;
    [SerializeField] private GameObject[] resultModeObjects;
    [SerializeField] private Behaviour[] gameplayModeBehaviours;
    [SerializeField] private Behaviour[] resultModeBehaviours;

    private GameObject[] resultRayObjects;
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
    private float lastHpRecovery;
    private float hpFeedbackVisibleUntil;
    private float baseScorePerNote;
    private float comboScorePool;
    private float maxComboWeight;
    private bool gameFailed;

    private float hpVerticalFillMaxHeight;
    private bool resultScreenShown;
    private bool resultInputEnabled;
    private bool resultConfirmWasReleased;
    private float resultInputEnabledAt;
    private string resultIntroSceneName;
    private bool clearResultQueued;

    private static readonly Color SciFiCyan = new Color(0.72f, 0.95f, 1f, 1f);
    private static readonly Color SciFiBlue = new Color(0.52f, 0.82f, 1f, 0.92f);
    private static readonly Color SciFiGold = new Color(1f, 0.92f, 0.68f, 1f);
    private static readonly Color SciFiScore = new Color(0.94f, 1f, 1f, 1f);
    private static readonly Color SciFiPanel = new Color(0.05f, 0.12f, 0.20f, 0.86f);

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
        if (!resultScreenShown && !gameFailed)
        {
            TryQueueClearResult();
        }

        if (lastHpRecovery > 0f && Time.time > hpFeedbackVisibleUntil)
        {
            ClearHpFeedback();
            RefreshHud();
        }

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
        float recoveredHp = RecoverHp(GetHitHpRecovery(combo));
        judgementLabel = recoveredHp > 0f ? $"HIT +{recoveredHp:0.#} HP" : "HIT";

        float comboScore = comboScorePool * combo / maxComboWeight;
        scoreValue = Mathf.Min(maxScore, scoreValue + baseScorePerNote + comboScore);
        score = Mathf.Min(maxScore, Mathf.RoundToInt(scoreValue));
        RefreshHud();
        TryQueueClearResult();
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
        TryQueueClearResult();
    }

    public void RegisterBad()
    {
        if (gameFailed)
        {
            return;
        }

        badCount++;
        judgementLabel = "BAD";
        ClearHpFeedback();
        ApplyHpDamage(missHpDamage * badHpDamageRatio);
        RefreshHud();
        TryQueueClearResult();
    }

    private void TryQueueClearResult()
    {
        if (clearResultQueued || resultScreenShown || gameFailed)
        {
            return;
        }

        if (hp <= 0f)
        {
            return;
        }

        if (spawner == null)
        {
            return;
        }

        if (!spawner.HasFinishedSpawning || !spawner.HasSongEnded)
        {
            return;
        }

        clearResultQueued = true;
        StartCoroutine(ShowClearResultAfterDelay());
    }

    private IEnumerator ShowClearResultAfterDelay()
    {
        yield return new WaitForSeconds(clearResultDelay);

        if (!gameFailed)
        {
            ShowResultScreen(introSceneName);
        }
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
        ShowSceneResultScreen();
        resultInputEnabledAt = Time.unscaledTime + 0.25f;
        resultInputEnabled = true;
    }

    private void ApplyHpDamage(float damage)
    {
        ClearHpFeedback();
        hp = Mathf.Max(0f, hp - damage);
        if (hp <= 0f)
        {
            FailGame();
        }
    }

    private float GetHitHpRecovery(int currentCombo)
    {
        float scaledRecovery = Mathf.Max(0f, hitHpRecoverPerCombo) * Mathf.Max(0, currentCombo);
        return Mathf.Min(maxHitHpRecover, scaledRecovery);
    }

    private float RecoverHp(float amount)
    {
        if (amount <= 0f || hp >= maxHp)
        {
            ClearHpFeedback();
            return 0f;
        }

        float previousHp = hp;
        hp = Mathf.Min(maxHp, hp + amount);
        lastHpRecovery = hp - previousHp;
        hpFeedbackVisibleUntil = Time.time + hpFeedbackDuration;
        return lastHpRecovery;
    }

    private void ClearHpFeedback()
    {
        lastHpRecovery = 0f;
        hpFeedbackVisibleUntil = 0f;
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
            StageEntry currentStage = backgroundController.CurrentStage;
            if (currentStage.useBeatSageChart && currentStage.noteChart != null && currentStage.noteChart.PlayableNoteCount > 0)
            {
                expectedNotes = Mathf.Max(1, currentStage.noteChart.GetPlayableNoteCount(currentStage.beatSageMinBeatGap));
                ApplyScorePools();
                Debug.Log($"[Score] expectedNotes={expectedNotes}, source=BeatSageChart, minBeatGap={currentStage.beatSageMinBeatGap:0.00}, basePerNote={baseScorePerNote:0.00}, comboPool={comboScorePool:0.00}");
                return;
            }

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

        float beatsPerSpawn = spawner != null ? Mathf.Max(0.25f, spawner.beatsPerSpawn) : 1f;
        if (backgroundController != null && backgroundController.CurrentStage != null && backgroundController.CurrentStage.beatsPerSpawn > 0f)
        {
            beatsPerSpawn = Mathf.Max(0.25f, backgroundController.CurrentStage.beatsPerSpawn);
        }

        float beatDuration = 60f / bpm;
        expectedNotes = Mathf.Max(1, Mathf.FloorToInt(clipLength / (beatDuration * beatsPerSpawn)) + 1);

        ApplyScorePools();

        Debug.Log($"[Score] expectedNotes={expectedNotes}, basePerNote={baseScorePerNote:0.00}, comboPool={comboScorePool:0.00}");
    }

    private void ApplyScorePools()
    {
        float baseScorePool = maxScore * baseScoreRatio;
        comboScorePool = maxScore - baseScorePool;
        baseScorePerNote = baseScorePool / expectedNotes;
        maxComboWeight = expectedNotes * (expectedNotes + 1f) * 0.5f;
    }

    private void CreateHud()
    {
        BindSceneUiReferences();

        if (scoreText == null || comboText == null || hpText == null || missText == null || hpVerticalFill == null || hpVerticalFillRect == null)
        {
            Debug.LogWarning("[Score] Scene UI references are missing. HUD update is disabled until Game UI Root is wired.");
            return;
        }

        if (hudRoot != null)
        {
            hudRoot.SetActive(true);
        }

        SetGameplayHudVisible(true);

        if (resultRoot != null)
        {
            resultRoot.SetActive(false);
        }

        ApplyGameplayHudVisualStyle();
        hpVerticalFillMaxHeight = hpVerticalFillRect.sizeDelta.y;

        if (resultOkButton != null)
        {
            resultOkButton.onClick.RemoveListener(LoadIntroFromResult);
            resultOkButton.onClick.AddListener(LoadIntroFromResult);
        }

        SetResultMode(false);
    }

    private void BindSceneUiReferences()
    {
        if (hudRoot == null)
        {
            var root = GameObject.Find("Game UI Root");
            if (root != null)
            {
                hudRoot = root;
            }
        }

        if (resultRoot == null)
        {
            var result = FindInactiveChild("Result HUD");
            if (result != null)
            {
                resultRoot = result.gameObject;
            }
        }

        scoreHudRoot ??= FindInactiveChild("Score HUD")?.gameObject;
        comboHudRoot ??= FindInactiveChild("Combo HUD")?.gameObject;
        hpHudRoot ??= FindInactiveChild("HP HUD")?.gameObject;

        scoreText ??= FindText("ScoreText");
        comboText ??= FindText("ComboText");
        hpText ??= FindText("HpText");
        missText ??= FindText("MissText");
        resultTitleText ??= FindText("ResultTitle");
        resultScoreText ??= FindText("ResultScore");
        resultStatsText ??= FindText("ResultStats");
        hpVerticalFill ??= FindImage("HpBarFill");
        hpVerticalFillRect ??= hpVerticalFill != null ? hpVerticalFill.rectTransform : null;
        PopulateModeReferences();
        RemoveResultRayObjects();

        if (resultOkButton == null)
        {
            var ok = FindInactiveChild("ResultOkButton");
            if (ok != null)
            {
                resultOkButton = ok.GetComponent<Button>();
            }
        }
    }

    private void PopulateModeReferences()
    {
        if (resultModeObjects == null || resultModeObjects.Length == 0)
        {
            var resultObjects = new System.Collections.Generic.List<GameObject>();
            foreach (var transform in FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (transform.name == "Result HUD")
                {
                    resultObjects.Add(transform.gameObject);
                }
            }

            resultModeObjects = resultObjects.ToArray();
        }

        if (gameplayModeBehaviours == null || gameplayModeBehaviours.Length == 0)
        {
            gameplayModeBehaviours = FindObjectsByType<Saber>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        }

        if (gameplayModeObjects == null || gameplayModeObjects.Length == 0)
        {
            var visuals = new System.Collections.Generic.List<GameObject>();
            foreach (var saber in FindObjectsByType<Saber>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                foreach (var renderer in saber.GetComponentsInChildren<Renderer>(false))
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

            gameplayModeObjects = visuals.ToArray();
        }
    }

    private void RemoveResultRayObjects()
    {
        if (resultModeObjects == null || resultModeObjects.Length == 0)
        {
            return;
        }

        var filtered = new System.Collections.Generic.List<GameObject>();
        foreach (var obj in resultModeObjects)
        {
            if (obj == null)
            {
                continue;
            }

            if (obj.name == "VisibleUIPointer" ||
                obj.name == "Left_NearFarInteractor" ||
                obj.name == "Right_NearFarInteractor")
            {
                AddResultRayObject(obj);
                continue;
            }

            filtered.Add(obj);
        }

        resultModeObjects = filtered.ToArray();
    }

    private void AddResultRayObject(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        if (resultRayObjects == null)
        {
            resultRayObjects = new[] { obj };
            return;
        }

        var rays = new System.Collections.Generic.List<GameObject>(resultRayObjects);
        if (!rays.Contains(obj))
        {
            rays.Add(obj);
            resultRayObjects = rays.ToArray();
        }
    }

    private TextMeshProUGUI FindText(string objectName)
    {
        var target = FindInactiveChild(objectName);
        return target != null ? target.GetComponent<TextMeshProUGUI>() : null;
    }

    private Image FindImage(string objectName)
    {
        var target = FindInactiveChild(objectName);
        return target != null ? target.GetComponent<Image>() : null;
    }

    private Transform FindInactiveChild(string objectName)
    {
        foreach (var rect in FindObjectsByType<RectTransform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (rect.name == objectName)
            {
                return rect.transform;
            }
        }

        return null;
    }

    private void RefreshHud()
    {
        if (scoreText == null || comboText == null || hpText == null || missText == null)
        {
            return;
        }

        scoreText.text = $"SCORE {score:000000}";
        comboText.text = $"{combo} COMBO";
        bool showHpRecovery = lastHpRecovery > 0f && Time.time <= hpFeedbackVisibleUntil;
        hpText.text = showHpRecovery
            ? $"HP {Mathf.CeilToInt(hp)}  +{lastHpRecovery:0.#}"
            : $"HP {Mathf.CeilToInt(hp)}";
        missText.text = gameFailed ? "FAILED" : $"{judgementLabel}   HIT {hitCount}  BAD {badCount}  MISS {missCount}";
        ApplyComboTextColor();

        if (hpVerticalFill != null)
        {
            float hpRatio = Mathf.Clamp01(hp / maxHp);
            hpVerticalFill.color = showHpRecovery ? new Color(0.52f, 1f, 0.78f, 1f) : GetHpColor(hpRatio);

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

    private void ApplyGameplayHudVisualStyle()
    {
        PositionHudRoot(scoreHudRoot, new Vector3(0f, 0.54f, 2.08f), new Vector2(500f, 168f), 0.00112f);
        PositionHudRoot(comboHudRoot, new Vector3(-0.43f, 0.23f, 2.08f), new Vector2(330f, 190f), 0.00115f);
        PositionHudRoot(hpHudRoot, new Vector3(0.43f, 0.23f, 2.08f), new Vector2(210f, 292f), 0.00115f);

        StyleHudPanel(scoreHudRoot, "Score Panel", new Color(0.04f, 0.14f, 0.24f, 0.94f), new Color(0f, 0.95f, 1f, 0.55f));
        StyleHudPanel(comboHudRoot, "Combo Panel", new Color(0.06f, 0.07f, 0.16f, 0.86f), new Color(1f, 0.15f, 0.65f, 0.34f));
        StyleHudPanel(hpHudRoot, "HP Panel", new Color(0.02f, 0.09f, 0.14f, 0.86f), new Color(0.1f, 1f, 0.62f, 0.34f));

        StyleHudText(scoreText, 34f, SciFiScore);
        StyleHudText(comboText, 34f, SciFiGold);
        StyleHudText(hpText, 22f, SciFiCyan);
        StyleHudText(missText, 14f, SciFiBlue);

        SetRect(scoreText, new Vector2(0f, 24f), new Vector2(460f, 58f));
        SetRect(comboText, new Vector2(0f, 8f), new Vector2(270f, 64f));
        SetRect(hpText, new Vector2(0f, 82f), new Vector2(150f, 30f));
        SetRect(missText, new Vector2(0f, -30f), new Vector2(420f, 28f));

        if (hpVerticalFillRect != null)
        {
            hpVerticalFillRect.sizeDelta = new Vector2(32f, 150f);
        }
    }

    private void ApplyComboTextColor()
    {
        if (comboText == null)
        {
            return;
        }

        float comboRatio = Mathf.Clamp01(combo / 30f);
        comboText.color = Color.Lerp(new Color(0.92f, 0.98f, 1f, 1f), new Color(1f, 0.18f, 0.32f, 1f), comboRatio);
    }

    private void PositionHudRoot(GameObject root, Vector3 localPosition, Vector2 size, float scale)
    {
        if (root == null)
        {
            return;
        }

        root.transform.localPosition = localPosition;
        root.transform.localRotation = Quaternion.identity;
        root.transform.localScale = Vector3.one * scale;

        if (root.transform is RectTransform rect)
        {
            rect.sizeDelta = size;
        }
    }

    private void StyleHudPanel(GameObject root, string panelName, Color panelColor, Color outlineColor)
    {
        if (root == null)
        {
            return;
        }

        Vector2 panelSize = root.transform is RectTransform rootRect ? rootRect.sizeDelta : Vector2.zero;

        foreach (var image in root.GetComponentsInChildren<Image>(true))
        {
            if (image.name == panelName)
            {
                ApplySlicedSprite(image, panelSprite);
                image.color = panelColor;
                if (panelSize != Vector2.zero)
                {
                    image.rectTransform.sizeDelta = panelSize;
                }
            }
            else if (image.name == panelName + " Outline")
            {
                ApplySlicedSprite(image, panelSprite != null ? panelSprite : panelOutlineSprite);
                image.color = outlineColor;
                if (panelSize != Vector2.zero)
                {
                    image.rectTransform.sizeDelta = panelSize + new Vector2(12f, 12f);
                }
            }
        }
    }

    private void ApplySlicedSprite(Image image, Sprite sprite)
    {
        if (image == null || sprite == null)
        {
            return;
        }

        image.sprite = sprite;
        image.type = Image.Type.Sliced;
        image.raycastTarget = false;
    }

    private void StyleHudText(TextMeshProUGUI text, float fontSize, Color color)
    {
        if (text == null)
        {
            return;
        }

        text.fontSize = fontSize;
        text.enableAutoSizing = true;
        text.fontSizeMin = Mathf.Max(10f, fontSize * 0.5f);
        text.fontSizeMax = fontSize;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.color = color;
        text.outlineWidth = 0.18f;
        text.outlineColor = new Color(0f, 0f, 0f, 0.82f);
    }

    private void SetRect(TextMeshProUGUI text, Vector2 anchoredPosition, Vector2 size)
    {
        if (text == null)
        {
            return;
        }

        text.rectTransform.anchoredPosition = anchoredPosition;
        text.rectTransform.sizeDelta = size;
    }

    private void ShowSceneResultScreen()
    {
        BindSceneUiReferences();

        if (hudRoot != null)
        {
            hudRoot.SetActive(true);
        }

        SetGameplayHudVisible(false);

        if (resultRoot == null || resultTitleText == null || resultScoreText == null || resultStatsText == null)
        {
            Debug.LogWarning("[Score] Result UI references are missing. Check Game UI Root in the Game scene.");
            return;
        }

        resultRoot.SetActive(true);
        ApplyResultHudVisualStyle();
        resultTitleText.text = gameFailed ? "FAILED" : "RESULT";
        resultScoreText.text = $"SCORE {score:000000}";
        resultStatsText.text = $"MAX COMBO {maxCombo}\nHIT {hitCount}   BAD {badCount}   MISS {missCount}\nACCURACY {Accuracy * 100f:0.0}%";

        if (resultOkButton != null)
        {
            resultOkButton.onClick.RemoveListener(LoadIntroFromResult);
            resultOkButton.onClick.AddListener(LoadIntroFromResult);
        }

        SetResultMode(true);
    }

    private void ApplyResultHudVisualStyle()
    {
        PositionHudRoot(resultRoot, new Vector3(0f, 0.02f, 2.05f), new Vector2(620f, 540f), 0.00118f);
        StyleHudPanel(resultRoot, "Result Panel", SciFiPanel, new Color(0f, 0.85f, 1f, 0.42f));

        StyleHudText(resultTitleText, 38f, gameFailed ? new Color(1f, 0.28f, 0.34f, 1f) : SciFiCyan);
        StyleHudText(resultScoreText, 34f, SciFiGold);
        StyleHudText(resultStatsText, 19f, SciFiBlue);

        SetRect(resultTitleText, new Vector2(0f, 138f), new Vector2(500f, 46f));
        SetRect(resultScoreText, new Vector2(0f, 66f), new Vector2(500f, 44f));
        SetRect(resultStatsText, new Vector2(0f, -28f), new Vector2(500f, 108f));
        StyleResultButton();
    }

    private void StyleResultButton()
    {
        if (resultOkButton == null)
        {
            return;
        }

        var buttonTransform = resultOkButton.transform;
        if (buttonTransform is RectTransform rect)
        {
            rect.anchoredPosition = new Vector2(0f, -152f);
            rect.sizeDelta = new Vector2(250f, 76f);
        }

        if (resultOkButton.targetGraphic is Image image)
        {
            ApplySlicedSprite(image, panelSprite);
            image.color = new Color(0.04f, 0.28f, 0.42f, 0.92f);
            image.raycastTarget = true;
        }

        if (sciFiButtonActiveSprite != null || sciFiButtonPressedSprite != null)
        {
            resultOkButton.transition = Selectable.Transition.SpriteSwap;
            resultOkButton.spriteState = new SpriteState
            {
                highlightedSprite = sciFiButtonActiveSprite,
                selectedSprite = sciFiButtonActiveSprite,
                pressedSprite = sciFiButtonPressedSprite
            };
        }

        var label = resultOkButton.transform.Find("Label");
        if (label != null && label.TryGetComponent<TextMeshProUGUI>(out var labelText))
        {
            StyleHudText(labelText, 26f, SciFiCyan);
            SetRect(labelText, Vector2.zero, new Vector2(190f, 46f));
        }
    }

    private void SetGameplayHudVisible(bool isVisible)
    {
        if (scoreHudRoot != null)
        {
            scoreHudRoot.SetActive(isVisible);
        }

        if (comboHudRoot != null)
        {
            comboHudRoot.SetActive(isVisible);
        }

        if (hpHudRoot != null)
        {
            hpHudRoot.SetActive(isVisible);
        }
    }

    private void SetResultMode(bool isResultMode)
    {
        SetObjectsActive(gameplayModeObjects, !isResultMode);
        SetObjectsActive(resultModeObjects, isResultMode);
        if (isResultMode)
        {
            SetObjectsActive(resultRayObjects, false);
        }

        SetBehavioursEnabled(gameplayModeBehaviours, !isResultMode);
        SetBehavioursEnabled(resultModeBehaviours, isResultMode);
    }

    private void SetObjectsActive(GameObject[] objects, bool isActive)
    {
        if (objects == null)
        {
            return;
        }

        foreach (var obj in objects)
        {
            if (obj != null)
            {
                obj.SetActive(isActive);
            }
        }
    }

    private void SetBehavioursEnabled(Behaviour[] behaviours, bool isEnabled)
    {
        if (behaviours == null)
        {
            return;
        }

        foreach (var behaviour in behaviours)
        {
            if (behaviour != null)
            {
                behaviour.enabled = isEnabled;
            }
        }
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
