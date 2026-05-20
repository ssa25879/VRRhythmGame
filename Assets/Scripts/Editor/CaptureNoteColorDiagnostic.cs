using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class CaptureNoteColorDiagnostic
{
    private const string RedPrefabPath = "Assets/Prefab/RED.prefab";
    private const string BluePrefabPath = "Assets/Prefab/BLUE.prefab";
    private const string ScreenshotPath = "Assets/Screenshots/note_color_diagnostic.png";

    public static void Spawn()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("[NoteColorDiagnostic] Enter Play Mode before spawning diagnostic notes.");
            return;
        }

        var redPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(RedPrefabPath);
        var bluePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BluePrefabPath);
        if (redPrefab == null || bluePrefab == null)
        {
            Debug.LogError($"[NoteColorDiagnostic] Missing prefabs. red={redPrefab != null}, blue={bluePrefab != null}");
            return;
        }

        CreateDiagnosticNote(redPrefab, "Diagnostic RED", new Vector3(-0.65f, 1.35f, 2.15f), Quaternion.Euler(0f, 180f, 0f));
        CreateDiagnosticNote(bluePrefab, "Diagnostic BLUE", new Vector3(0.65f, 1.35f, 2.15f), Quaternion.Euler(0f, 180f, 0f));

        Debug.Log("[NoteColorDiagnostic] Spawned fixed red/blue diagnostic notes.");
        EditorCoroutineRunner.Start(CaptureAfterDelay());
    }

    private static GameObject CreateDiagnosticNote(GameObject prefab, string name, Vector3 position, Quaternion rotation)
    {
        var note = Object.Instantiate(prefab, position, rotation);
        note.name = name;
        note.transform.localScale = Vector3.one * 0.55f;

        return note;
    }

    private static IEnumerator CaptureAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        Directory.CreateDirectory("Assets/Screenshots");
        ScreenCapture.CaptureScreenshot(ScreenshotPath);
        Debug.Log($"[NoteColorDiagnostic] Screenshot requested: {ScreenshotPath}");
    }

    private sealed class EditorCoroutineRunner : MonoBehaviour
    {
        public static void Start(IEnumerator routine)
        {
            var runner = new GameObject("Editor Coroutine Runner").AddComponent<EditorCoroutineRunner>();
            runner.StartCoroutine(routine);
        }
    }
}
