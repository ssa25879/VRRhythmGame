using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections.Generic;

public static class RemoveSaberWeaponVFX
{
    private const string GameScenePath = "Assets/Scenes/Game.unity";
    private const string WeaponVfxName = "Saber Weapon VFX";

    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene(GameScenePath);
        var targets = new List<GameObject>();

        foreach (var transform in Object.FindObjectsByType<Transform>(
                     FindObjectsInactive.Include,
                     FindObjectsSortMode.None))
        {
            if (transform.name != WeaponVfxName)
            {
                continue;
            }

            targets.Add(transform.gameObject);
        }

        foreach (var target in targets)
        {
            Object.DestroyImmediate(target);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log($"[Saber Weapon VFX] Removed {targets.Count} swing VFX object(s).");
    }
}
