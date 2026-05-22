using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class VerifyXRPointerPlayMode
{
    private static readonly List<string> Errors = new();
    private static double startedAt;
    private static bool waitingForPlayMode;
    private static bool running;

    public static void Execute()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.Log("[XR Pointer Play Verify] Already in play mode; skipping scene open.");
            return;
        }

        EditorSceneManager.OpenScene("Assets/Scenes/Intro.unity", OpenSceneMode.Single);
        ClearConsole();

        Errors.Clear();
        startedAt = EditorApplication.timeSinceStartup;
        waitingForPlayMode = true;
        running = true;

        Application.logMessageReceived -= OnLogMessageReceived;
        Application.logMessageReceived += OnLogMessageReceived;
        EditorApplication.update -= Update;
        EditorApplication.update += Update;

        EditorApplication.EnterPlaymode();
        Debug.Log("[XR Pointer Play Verify] Entering Intro play mode.");
    }

    private static void Update()
    {
        if (!running)
            return;

        if (waitingForPlayMode)
        {
            if (!EditorApplication.isPlaying)
                return;

            waitingForPlayMode = false;
            startedAt = EditorApplication.timeSinceStartup;
            return;
        }

        if (EditorApplication.timeSinceStartup - startedAt < 5.0d)
            return;

        running = false;
        Application.logMessageReceived -= OnLogMessageReceived;
        EditorApplication.update -= Update;

        Debug.Log($"[XR Pointer Play Verify] Complete. New error count={Errors.Count}");
        foreach (var error in Errors)
        {
            Debug.Log($"[XR Pointer Play Verify] Error: {error}");
        }

        EditorApplication.ExitPlaymode();
    }

    private static void OnLogMessageReceived(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
        {
            Errors.Add(condition);
        }
    }

    private static void ClearConsole()
    {
        var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
        var clear = logEntries?.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
        clear?.Invoke(null, null);
    }
}
