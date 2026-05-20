using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class FixIntroScene
{
    public static void Execute()
    {
        // ── 1. 불필요 오브젝트 제거 (Cube, Spawner) ───────────────────────
        string[] toRemove = { "Cube", "Cube (1)", "Spawner" };
        foreach (var name in toRemove)
        {
            var go = GameObject.Find(name);
            if (go != null)
            {
                Object.DestroyImmediate(go);
                Debug.Log($"[FixIntro] 제거: {name}");
            }
        }

        // ── 2. SkyDome360 생성 (이미 있으면 재사용) ───────────────────────
        var skyDome = GameObject.Find("SkyDome360");
        if (skyDome == null)
        {
            skyDome = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            skyDome.name = "SkyDome360";

            // 콜라이더 제거 (불필요)
            var col = skyDome.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);
        }

        // 스케일: X를 음수로 뒤집어 법선을 안쪽으로 향하게
        skyDome.transform.position   = Vector3.zero;
        skyDome.transform.localScale = new Vector3(-100f, 100f, 100f);

        // ── 3. Material360.mat 적용 ────────────────────────────────────────
        var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/360 Music/Material360.mat");
        if (mat != null)
        {
            var renderer = skyDome.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = mat;
            Debug.Log("[FixIntro] Material360.mat 적용 완료");
        }
        else
        {
            Debug.LogWarning("[FixIntro] Material360.mat 없음 — 수동으로 머티리얼 할당 필요");
        }

        // ── 4. IntroManager에 backgroundPlayer 재확인 ────────────────────
        var mgr = Object.FindFirstObjectByType<IntroManager>();
        if (mgr != null && mgr.backgroundPlayer == null)
        {
            var vp = GameObject.Find("Video")?.GetComponent<UnityEngine.Video.VideoPlayer>();
            if (vp != null) mgr.backgroundPlayer = vp;
            Debug.Log("[FixIntro] IntroManager.backgroundPlayer 재연결");
        }

        EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("[FixIntro] 완료 — SkyDome360 생성, 게임 오브젝트 제거");
    }
}
