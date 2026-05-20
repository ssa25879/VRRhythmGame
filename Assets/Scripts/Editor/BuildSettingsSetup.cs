using UnityEditor;

public class BuildSettingsSetup
{
    public static void Execute()
    {
        var scenes = new[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/Intro.unity",  true),
            new EditorBuildSettingsScene("Assets/Scenes/Game/Game.unity", true),
        };
        EditorBuildSettings.scenes = scenes;
        UnityEngine.Debug.Log("[BuildSettings] Intro(0) → Game(1) 빌드 세팅 완료");
    }
}
