using UnityEditor;

public static class ExitPlayMode
{
    public static void Execute()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.ExitPlaymode();
        }
    }
}
