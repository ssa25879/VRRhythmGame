using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class WireIntroRefs
{
    public static void Execute()
    {
        var mgr = Object.FindFirstObjectByType<IntroManager>();
        if (mgr == null) { Debug.LogError("[WireIntroRefs] IntroManager 없음"); return; }

        // bgmSource: IntroManager GameObject에 붙은 AudioSource 자동 연결
        var src = mgr.GetComponent<AudioSource>();
        if (src == null) src = mgr.gameObject.AddComponent<AudioSource>();
        src.loop   = true;
        src.volume = 0.7f;
        src.playOnAwake = false;
        mgr.bgmSource = src;

        EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("[WireIntroRefs] bgmSource 연결 완료");
    }
}
