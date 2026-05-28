using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ImportRetrowaveVhsBeatSage
{
    const string FolderPath = "Assets/Audio/StageNote/RetrowaveVHS_BeatSage";
    const string InfoPath = FolderPath + "/Info.dat";
    const string NormalPath = FolderPath + "/Normal.dat";
    const string SongPath = FolderPath + "/song.ogg";
    const string ChartPath = FolderPath + "/RetrowaveVHS_Normal_BeatSageChart.asset";
    const string StageListPath = "Assets/Data/StageList.asset";

    [MenuItem("VRBeatSaber/Import Retrowave VHS Beat Sage Normal")]
    public static void Import()
    {
        BeatSageInfo info = JsonUtility.FromJson<BeatSageInfo>(System.IO.File.ReadAllText(InfoPath));
        BeatSageMap map = JsonUtility.FromJson<BeatSageMap>(System.IO.File.ReadAllText(NormalPath));
        if (map == null || map._notes == null || map._notes.Length == 0)
        {
            Debug.LogError("[BeatSage] Normal.dat has no notes.");
            return;
        }

        AssetDatabase.ImportAsset(SongPath, ImportAssetOptions.ForceUpdate);

        BeatSaberNoteChart chart = AssetDatabase.LoadAssetAtPath<BeatSaberNoteChart>(ChartPath);
        if (chart == null)
        {
            chart = ScriptableObject.CreateInstance<BeatSaberNoteChart>();
            AssetDatabase.CreateAsset(chart, ChartPath);
        }

        chart.beatsPerMinute = info != null && info._beatsPerMinute > 0f ? info._beatsPerMinute : 120f;
        chart.noteJumpMovementSpeed = FindNormalDifficulty(info)?._noteJumpMovementSpeed ?? 10f;
        chart.noteJumpStartBeatOffset = FindNormalDifficulty(info)?._noteJumpStartBeatOffset ?? 0f;
        chart.notes = map._notes
            .OrderBy(note => note._time)
            .Select(note => new BeatSaberNote
            {
                beat = note._time,
                lineIndex = note._lineIndex,
                lineLayer = note._lineLayer,
                type = note._type,
                cutDirection = note._cutDirection
            })
            .ToArray();

        EditorUtility.SetDirty(chart);

        StageListSO stageList = AssetDatabase.LoadAssetAtPath<StageListSO>(StageListPath);
        AudioClip song = AssetDatabase.LoadAssetAtPath<AudioClip>(SongPath);
        if (stageList == null || stageList.stages == null)
        {
            Debug.LogError("[BeatSage] StageList asset is missing.");
            return;
        }

        StageEntry stage = stageList.stages.FirstOrDefault(item => item.stageName == "Retrowave VHS");
        if (stage == null)
        {
            Debug.LogError("[BeatSage] Retrowave VHS stage is missing.");
            return;
        }

        stage.bpm = chart.beatsPerMinute;
        stage.bgm = song;
        stage.useBeatSageChart = true;
        stage.noteChart = chart;

        EditorUtility.SetDirty(stageList);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[BeatSage] Imported Retrowave VHS chart. notes={chart.PlayableNoteCount}, bpm={chart.beatsPerMinute:0.0}, song={(song != null ? song.name : "null")}");
    }

    static BeatSageDifficulty FindNormalDifficulty(BeatSageInfo info)
    {
        if (info == null || info._difficultyBeatmapSets == null)
        {
            return null;
        }

        foreach (BeatSageDifficultySet set in info._difficultyBeatmapSets)
        {
            if (set._difficultyBeatmaps == null)
            {
                continue;
            }

            BeatSageDifficulty difficulty = set._difficultyBeatmaps.FirstOrDefault(item => item._difficulty == "Normal");
            if (difficulty != null)
            {
                return difficulty;
            }
        }

        return null;
    }

    [Serializable]
    class BeatSageInfo
    {
        public float _beatsPerMinute;
        public BeatSageDifficultySet[] _difficultyBeatmapSets;
    }

    [Serializable]
    class BeatSageDifficultySet
    {
        public BeatSageDifficulty[] _difficultyBeatmaps;
    }

    [Serializable]
    class BeatSageDifficulty
    {
        public string _difficulty;
        public float _noteJumpMovementSpeed;
        public float _noteJumpStartBeatOffset;
    }

    [Serializable]
    class BeatSageMap
    {
        public BeatSageNote[] _notes;
    }

    [Serializable]
    class BeatSageNote
    {
        public float _time;
        public int _lineIndex;
        public int _lineLayer;
        public int _type;
        public int _cutDirection;
    }
}
