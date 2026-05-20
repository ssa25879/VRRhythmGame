using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.UI;
using TMPro;

/// <summary>
/// VRBeatSaber 메뉴 → "Build Intro Scene UI" 실행 시
/// Intro 씬의 Canvas를 스테이지 선택 UI로 재구성합니다.
/// </summary>
public class IntroSceneBuilder
{
    [MenuItem("VRBeatSaber/Build Intro Scene UI")]
    public static void Execute()
    {
        // ── Canvas 리사이즈 (800×550, scale 0.001) ──────────────────────────
        var canvasGo = GameObject.Find("Canvas");
        if (canvasGo == null) { Debug.LogError("[IntroSceneBuilder] Canvas 없음"); return; }

        var rt = canvasGo.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(800f, 550f);
        canvasGo.transform.SetPositionAndRotation(
            new Vector3(0f, 1.5f, 2.5f), Quaternion.identity);
        canvasGo.transform.localScale = Vector3.one * 0.001f;

        // 기존 Button → StartButton 으로 이름 변경 후 위치 재배치
        var oldBtn = canvasGo.transform.Find("Button");
        if (oldBtn != null)
        {
            oldBtn.name = "StartButton";
            var oldRT = oldBtn.GetComponent<RectTransform>();
            oldRT.anchoredPosition = new Vector2(0f, -210f);
            oldRT.sizeDelta        = new Vector2(300f, 70f);
        }

        // ── Title ──────────────────────────────────────────────────────────
        var titleGo = CreateText(canvasGo.transform, "TitleText", "VR BEAT SABER",
            new Vector2(0f, 240f), new Vector2(700f, 70f), 54f, Color.white,
            FontStyles.Bold);

        // ── 이전/다음 스테이지 버튼 ────────────────────────────────────────
        var prevBtn = CreateButton(canvasGo.transform, "PrevButton", "◀",
            new Vector2(-310f, 30f), new Vector2(70f, 180f),
            new Color(0.12f, 0.12f, 0.25f, 0.9f), 36f);

        var nextBtn = CreateButton(canvasGo.transform, "NextButton", "▶",
            new Vector2(310f, 30f), new Vector2(70f, 180f),
            new Color(0.12f, 0.12f, 0.25f, 0.9f), 36f);

        // ── 썸네일 이미지 ────────────────────────────────────────────────
        var thumbBg = CreatePanel(canvasGo.transform, "ThumbnailBG",
            new Vector2(0f, 40f), new Vector2(240f, 180f),
            new Color(0.10f, 0.10f, 0.20f, 0.9f));

        var thumbImgGo = new GameObject("ThumbnailImage");
        thumbImgGo.transform.SetParent(thumbBg.transform, false);
        var thumbRT = thumbImgGo.AddComponent<RectTransform>();
        thumbRT.anchoredPosition = Vector2.zero;
        thumbRT.sizeDelta        = new Vector2(230f, 170f);
        var thumbImg = thumbImgGo.AddComponent<Image>();
        thumbImg.color = new Color(0.18f, 0.18f, 0.30f);
        thumbImg.preserveAspect = true;

        // ── 스테이지 이름 텍스트 ──────────────────────────────────────────
        var stageNameGo = CreateText(canvasGo.transform, "StageNameText", "Stage 1",
            new Vector2(0f, -95f), new Vector2(620f, 60f), 32f,
            new Color(0.8f, 0.9f, 1f), FontStyles.Normal);

        // ── Start 버튼 텍스트 설정 ────────────────────────────────────────
        var startBtn = canvasGo.transform.Find("StartButton");
        if (startBtn != null)
        {
            var lblTf = startBtn.Find("Text (TMP)") ?? startBtn.Find("Label");
            if (lblTf != null)
            {
                var tmp = lblTf.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.text      = "▶  PLAY";
                    tmp.fontSize  = 30f;
                    tmp.fontStyle = FontStyles.Bold;
                }
            }
            // StartButton 배경색 지정
            var img = startBtn.GetComponent<Image>();
            if (img != null) img.color = new Color(0.15f, 0.45f, 1f, 0.95f);
        }

        // ── FadeOverlay (기존 것 제거 후 새로 생성) ───────────────────────
        var existingFade = canvasGo.transform.Find("FadeOverlay");
        if (existingFade != null) Object.DestroyImmediate(existingFade.gameObject);
        var fadeGo = CreatePanel(canvasGo.transform, "FadeOverlay",
                         Vector2.zero, new Vector2(800f, 550f),
                         new Color(0f, 0f, 0f, 1f));
        fadeGo.transform.SetAsLastSibling();
        var cg = fadeGo.AddComponent<CanvasGroup>();
        cg.alpha          = 1f;
        cg.blocksRaycasts = false;
        cg.interactable   = false;

        // ── IntroManager 컴포넌트 부착 ────────────────────────────────────
        var mgrGo = GameObject.Find("IntroManager") ?? new GameObject("IntroManager");
        var mgr   = mgrGo.GetComponent<IntroManager>()
                    ?? mgrGo.AddComponent<IntroManager>();

        // VideoPlayer 연결
        var videoPl = GameObject.Find("Video")?.GetComponent<VideoPlayer>();
        mgr.backgroundPlayer = videoPl;

        // UI 레퍼런스 연결
        mgr.fadeOverlay    = cg;
        mgr.stageNameText  = stageNameGo.GetComponent<TextMeshProUGUI>();
        mgr.thumbnailImage = thumbImg;

        // ── Prev/Next/Start 버튼 onClick 연결 ────────────────────────────
        ConnectButton(prevBtn.GetComponent<Button>(), mgr.PrevStage);
        ConnectButton(nextBtn.GetComponent<Button>(), mgr.NextStage);

        if (startBtn != null)
        {
            var sb = startBtn.GetComponent<Button>();
            ConnectButton(sb, mgr.OnStartGame);
        }

        // ── 씬 저장 ────────────────────────────────────────────────────────
        EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("[IntroSceneBuilder] Intro 씬 UI 구성 완료 — Ctrl+S 로 저장하세요.\n" +
                  "남은 작업: IntroManager → Stage List 필드에 StageList.asset 드래그");
    }

    // ── 헬퍼 ──────────────────────────────────────────────────────────────────

    static void ConnectButton(Button btn, UnityEngine.Events.UnityAction action)
    {
        if (btn == null) return;
        btn.onClick.RemoveAllListeners();
        UnityEventTools.AddPersistentListener(btn.onClick, action);
    }

    static GameObject CreatePanel(Transform parent, string name,
        Vector2 pos, Vector2 size, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rT  = go.AddComponent<RectTransform>();
        rT.anchoredPosition = pos;
        rT.sizeDelta        = size;
        var img = go.AddComponent<Image>();
        img.color = color;
        return go;
    }

    static GameObject CreateText(Transform parent, string name, string text,
        Vector2 pos, Vector2 size, float fontSize, Color color, FontStyles style)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rT  = go.AddComponent<RectTransform>();
        rT.anchoredPosition = pos;
        rT.sizeDelta        = size;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = fontSize;
        tmp.color     = color;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        return go;
    }

    static GameObject CreateButton(Transform parent, string name, string label,
        Vector2 pos, Vector2 size, Color bgColor, float fontSize)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rT  = go.AddComponent<RectTransform>();
        rT.anchoredPosition = pos;
        rT.sizeDelta        = size;
        var img = go.AddComponent<Image>();
        img.color = bgColor;
        go.AddComponent<Button>();

        var lbl = new GameObject("Label");
        lbl.transform.SetParent(go.transform, false);
        var lRT = lbl.AddComponent<RectTransform>();
        lRT.anchoredPosition = Vector2.zero;
        lRT.sizeDelta        = size;
        var tmp = lbl.AddComponent<TextMeshProUGUI>();
        tmp.text      = label;
        tmp.fontSize  = fontSize;
        tmp.color     = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        return go;
    }
}
