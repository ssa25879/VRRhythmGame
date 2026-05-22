using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class RecordTestPlayVideo
{
    private const string GameScenePath = "Assets/Scenes/Game.unity";
    private const string OutputRoot = "Assets/Screenshots/TestPlayVideo";
    private const string RecordingKey = "VRBeatSaber.TestPlayVideo.Recording";
    private const string OutputFolderKey = "VRBeatSaber.TestPlayVideo.OutputFolder";
    private const string FrameIndexKey = "VRBeatSaber.TestPlayVideo.FrameIndex";
    private const string StartTimeKey = "VRBeatSaber.TestPlayVideo.StartTime";
    private const string NextFrameTimeKey = "VRBeatSaber.TestPlayVideo.NextFrameTime";
    private const int FrameRate = 10;
    private const float DurationSeconds = 10f;
    private const float WarmupSeconds = 1f;

    private static string outputFolder;
    private static int frameIndex;
    private static double startTime;
    private static double nextFrameTime;
    private static bool isRecording;

    static RecordTestPlayVideo()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

        if (SessionState.GetBool(RecordingKey, false))
        {
            RestoreState();
            EditorApplication.update -= UpdateRecording;
            EditorApplication.update += UpdateRecording;
        }
    }

    public static void Execute()
    {
        if (EditorApplication.isCompiling)
        {
            Debug.LogError("[TestPlayVideo] Unity is compiling. Try again after compilation finishes.");
            return;
        }

        if (EditorApplication.isPlaying)
        {
            Debug.LogError("[TestPlayVideo] Stop Play Mode before starting a new recording.");
            return;
        }

        Directory.CreateDirectory(OutputRoot);
        outputFolder = Path.Combine(OutputRoot, DateTime.Now.ToString("yyyyMMdd_HHmmss")).Replace("\\", "/");
        Directory.CreateDirectory(outputFolder);
        frameIndex = 0;
        isRecording = false;
        SaveState();

        EditorSceneManager.OpenScene(GameScenePath);
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        EditorApplication.isPlaying = true;

        Debug.Log($"[TestPlayVideo] Recording requested. Output folder: {outputFolder}");
    }

    public static void CaptureCurrentPlayMode()
    {
        if (!EditorApplication.isPlaying)
        {
            Debug.LogError("[TestPlayVideo] Play Mode is not running.");
            return;
        }

        Directory.CreateDirectory(OutputRoot);
        outputFolder = Path.Combine(OutputRoot, DateTime.Now.ToString("yyyyMMdd_HHmmss")).Replace("\\", "/");
        Directory.CreateDirectory(outputFolder);
        frameIndex = 0;
        startTime = EditorApplication.timeSinceStartup + 0.5f;
        nextFrameTime = startTime;
        isRecording = true;
        SaveState();

        EditorApplication.update -= UpdateRecording;
        EditorApplication.update += UpdateRecording;
        Debug.Log($"[TestPlayVideo] Current Play Mode capture started. Output folder: {outputFolder}");
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            startTime = EditorApplication.timeSinceStartup + WarmupSeconds;
            nextFrameTime = startTime;
            isRecording = true;
            SaveState();
            EditorApplication.update -= UpdateRecording;
            EditorApplication.update += UpdateRecording;
            Debug.Log("[TestPlayVideo] Play Mode entered. Capturing frames.");
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            ClearState();
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.update -= UpdateRecording;
        }
    }

    private static void UpdateRecording()
    {
        if (!isRecording)
        {
            return;
        }

        double now = EditorApplication.timeSinceStartup;
        if (now < nextFrameTime)
        {
            return;
        }

        if (now >= startTime + DurationSeconds)
        {
            isRecording = false;
            ClearState();
            EditorApplication.update -= UpdateRecording;
            EditorApplication.isPlaying = false;
            AssetDatabase.Refresh();
            Debug.Log($"[TestPlayVideo] Capture complete. Frames: {frameIndex}, Folder: {outputFolder}");
            return;
        }

        string framePath = Path.Combine(outputFolder, $"frame_{frameIndex:D04}.png").Replace("\\", "/");
        ScreenCapture.CaptureScreenshot(framePath);
        frameIndex++;
        nextFrameTime += 1.0 / FrameRate;
        SaveState();
    }

    private static void RestoreState()
    {
        outputFolder = SessionState.GetString(OutputFolderKey, string.Empty);
        frameIndex = SessionState.GetInt(FrameIndexKey, 0);
        startTime = SessionState.GetFloat(StartTimeKey, 0f);
        nextFrameTime = SessionState.GetFloat(NextFrameTimeKey, 0f);
        isRecording = SessionState.GetBool(RecordingKey, false);
    }

    private static void SaveState()
    {
        SessionState.SetBool(RecordingKey, isRecording);
        SessionState.SetString(OutputFolderKey, outputFolder ?? string.Empty);
        SessionState.SetInt(FrameIndexKey, frameIndex);
        SessionState.SetFloat(StartTimeKey, (float)startTime);
        SessionState.SetFloat(NextFrameTimeKey, (float)nextFrameTime);
    }

    private static void ClearState()
    {
        SessionState.EraseBool(RecordingKey);
        SessionState.EraseString(OutputFolderKey);
        SessionState.EraseInt(FrameIndexKey);
        SessionState.EraseFloat(StartTimeKey);
        SessionState.EraseFloat(NextFrameTimeKey);
    }
}
