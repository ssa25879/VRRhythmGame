using UnityEditor;
using UnityEngine;
using System.Reflection;

public static class RefreshAssetDatabaseForImportCheck
{
    public static void Execute()
    {
        AssetDatabase.Refresh();
        Debug.Log("[ImportCheck] AssetDatabase refresh complete.");
    }

    public static void ClearAndRefresh()
    {
        var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
        var clearMethod = logEntries?.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
        clearMethod?.Invoke(null, null);

        AssetDatabase.Refresh();
        Debug.Log("[ImportCheck] Console cleared and AssetDatabase refresh complete.");
    }
}
