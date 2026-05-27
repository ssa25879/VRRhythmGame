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

    [Header("Result")]
    [SerializeField] private string introSceneName = "Intro";
    [SerializeField] private float failResultDelay = 0.6f;

    [Header("Scene UI")]
    [SerializeField] private Sprite panelSprite;
    [SerializeField] private Sprite panelOutlineSprite;
    [SerializeField] private Sprite hpFrameSprite;
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
        ShowSceneResultScreen();
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
                if (transform.name == "VisibleUIPointer" ||
                    transform.name == "Left_NearFarInteractor" ||
                    transform.name == "Right_NearFarInteractor")
                {
                    resultObjects.Add(transform.gameObject);
                }
            }

            resultModeObjects = resultObjects.ToArray();
        }

        if (gameplayModeBehaviours == null || gameplayModeBehaviours.Length == 0)
        {
            gameplayModeBehaviours = FindObjectsByType<Saber>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }

        if (gameplayModeObjects == null || gameplayModeObjects.Length == 0)
        {
            var visuals = new System.Collections.Generic.List<GameObject>();
            foreach (var saber in FindObjectsByType<Saber>(FindObjectsInactive.Include, FindObjectsSortMode.None))
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

            gameplayModeObjects = visuals.ToArray();
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
