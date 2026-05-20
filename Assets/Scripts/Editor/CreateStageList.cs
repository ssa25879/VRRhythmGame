using UnityEngine;
using UnityEngine.Video;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class CreateStageList
{
    [MenuItem("VRBeatSaber/Create Stage List Asset")]
    public static void Execute()
    {
        // Assets/Data 폴더 생성
        if (!AssetDatabase.IsValidFolder("Assets/Data"))
            AssetDatabase.CreateFolder("Assets", "Data");

        // 기존 에셋이 있으면 재사용
        var existing = AssetDatabase.LoadAssetAtPath<StageListSO>("Assets/Data/StageList.asset");
        if (existing != null)
        {
            Debug.Log("[CreateStageList] StageList.asset 이미 존재 — 기존 에셋 사용");
            Selection.activeObject = existing;
            return;
        }

        // 새 에셋 생성
        var asset = ScriptableObject.CreateInstance<StageListSO>();

        // 기본 스테이지 1개: motion.mp4 + About That Oldie.mp3
        var defaultVideo = AssetDatabase.LoadAssetAtPath<VideoClip>(
            "Assets/360 Music/motion.mp4");
        var defaultBgm = AssetDatabase.LoadAssetAtPath<AudioClip>(
            "Assets/360 Music/About That Oldie - Vibe Tracks.mp3");

        asset.stages = new StageEntry[]
        {
            new StageEntry
            {
                stageName       = "About That Oldie",
                backgroundVideo = defaultVideo,
                bgm             = defaultBgm
            }
        };

        AssetDatabase.CreateAsset(asset, "Assets/Data/StageList.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = asset;
        Debug.Log("[CreateStageList] StageList.asset 생성 완료 — Assets/Data/StageList.asset");

        // 빌드 세팅도 함께 구성
        SetupBuildSettings();
    }

    static void SetupBuildSettings()
    {
        var scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/Intro.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Game.unity",  true)
        };
        EditorBuildSettings.scenes = scenes;
        Debug.Log("[CreateStageList] 빌드 세팅: Intro(0) → Game(1) 적용 완료");
    }
}
