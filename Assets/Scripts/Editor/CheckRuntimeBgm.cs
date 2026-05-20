using UnityEngine;

public static class CheckRuntimeBgm
{
    public static void Execute()
    {
        var controller = Object.FindFirstObjectByType<GameBackgroundController>();
        if (controller == null)
        {
            Debug.LogError("[BGMCheck] GameBackgroundController not found.");
            return;
        }

        var source = controller.bgmSource;
        if (source == null)
        {
            Debug.LogError("[BGMCheck] BGM AudioSource is missing.");
            return;
        }

        string clipName = source.clip != null ? source.clip.name : "NULL";
        float clipLength = source.clip != null ? source.clip.length : 0f;
        Debug.Log($"[BGMCheck] clip={clipName}, length={clipLength:0.00}, isPlaying={source.isPlaying}, loop={source.loop}, volume={source.volume:0.00}, mute={source.mute}, time={source.time:0.00}");
    }
}
