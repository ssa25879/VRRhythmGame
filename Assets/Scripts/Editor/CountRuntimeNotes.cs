using UnityEditor;
using UnityEngine;

public static class CountRuntimeNotes
{
    public static void Execute()
    {
        int red = 0;
        int blue = 0;
        int redLayer = LayerMask.NameToLayer("Red");
        int blueLayer = LayerMask.NameToLayer("Blue");

        foreach (var cube in Object.FindObjectsByType<Cube>(FindObjectsSortMode.None))
        {
            if (cube.gameObject.layer == redLayer)
            {
                red++;
            }
            else if (cube.gameObject.layer == blueLayer)
            {
                blue++;
            }
        }

        Debug.Log($"[RuntimeNoteCount] RED={red}, BLUE={blue}");
    }
}
