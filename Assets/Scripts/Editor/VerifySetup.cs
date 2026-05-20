using UnityEngine;
using UnityEditor;

public class VerifySetup
{
    public static void Execute()
    {
        // 빌드 세팅 확인
        var scenes = EditorBuildSettings.scenes;
        Debug.Log($"[Verify] 빌드 씬 수: {scenes.Length}");
        for (int i = 0; i < scenes.Length; i++)
            Debug.Log($"[Verify]  [{i}] {scenes[i].path} enabled={scenes[i].enabled}");

        // StageList.asset 확인
        var sl = AssetDatabase.LoadAssetAtPath<StageListSO>("Assets/Data/StageList.asset");
        if (sl != null)
        {
            Debug.Log($"[Verify] StageList.asset: {sl.stages?.Length ?? 0}개 스테이지");
            if (sl.stages != null)
                foreach (var s in sl.stages)
                    Debug.Log($"[Verify]   - {s.stageName} | video={s.backgroundVideo != null} | bgm={s.bgm != null}");
        }
        else Debug.LogError("[Verify] StageList.asset 없음!");
    }
}
