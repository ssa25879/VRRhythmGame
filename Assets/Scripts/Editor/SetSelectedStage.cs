using UnityEngine;

public static class SetSelectedStage
{
    public static void Execute()
    {
        PlayerPrefs.SetInt("SelectedStage", 1);
        PlayerPrefs.Save();
        Debug.Log("[SetSelectedStage] SelectedStage=1");
    }
}
