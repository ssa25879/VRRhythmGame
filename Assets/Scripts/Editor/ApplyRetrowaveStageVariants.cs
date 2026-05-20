using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ApplyRetrowaveStageVariants
{
    private const string StageListPath = "Assets/Data/StageList.asset";
    private const string AudioFolder = "Assets/Audio/Generated";
    private const int SampleRate = 44100;

    private struct StageVariant
    {
        public string stageName;
        public string skyboxPath;
        public string gridPath;
        public string bgmPath;
        public float bpm;
        public BgmMood mood;
    }

    private enum BgmMood
    {
        Vapor,
        Orange,
        Vhs
    }

    private static readonly StageVariant[] Variants =
    {
        new StageVariant
        {
            stageName = "Retrowave Vapor",
            skyboxPath = "Assets/Suggo Creations/RETROWAVE SKIES Lite/Skybox Materials/Vapor_Skybox.mat",
            gridPath = "Assets/Suggo Creations/RETROWAVE SKIES Lite/Materials/M_Grid Vapor Lite.mat",
            bgmPath = AudioFolder + "/Retrowave_Vapor_BGM.wav",
            bpm = 124f,
            mood = BgmMood.Vapor
        },
        new StageVariant
        {
            stageName = "Retrowave Orange",
            skyboxPath = "Assets/Suggo Creations/RETROWAVE SKIES Lite/Skybox Materials/Orange_Skybox.mat",
            gridPath = "Assets/Suggo Creations/RETROWAVE SKIES Lite/Materials/M_Grid Orange Lite.mat",
            bgmPath = AudioFolder + "/Retrowave_Orange_BGM.wav",
            bpm = 110f,
            mood = BgmMood.Orange
        },
        new StageVariant
        {
            stageName = "Retrowave VHS",
            skyboxPath = "Assets/Suggo Creations/RETROWAVE SKIES Lite/Skybox Materials/VHS_Skybox.mat",
            gridPath = "Assets/Suggo Creations/RETROWAVE SKIES Lite/Materials/M_Grid VHS Lite.mat",
            bgmPath = AudioFolder + "/Retrowave_VHS_BGM.wav",
            bpm = 132f,
            mood = BgmMood.Vhs
        }
    };

    public static void Execute()
    {
        Directory.CreateDirectory(AudioFolder);
        foreach (var variant in Variants)
        {
            WriteLoopWav(variant.bgmPath, variant.bpm, variant.mood);
            AssetDatabase.ImportAsset(variant.bgmPath, ImportAssetOptions.ForceUpdate);
        }

        var stageList = AssetDatabase.LoadAssetAtPath<StageListSO>(StageListPath);
        if (stageList == null)
        {
            Debug.LogError("[RetrowaveStages] StageList.asset was not found.");
            return;
        }

        var stages = new List<StageEntry>();
        if (stageList.stages != null)
        {
            foreach (var stage in stageList.stages)
            {
                if (stage.stageName == "Neon Grid" ||
                    stage.stageName == "Retrowave Vapor" ||
                    stage.stageName == "Retrowave Orange" ||
                    stage.stageName == "Retrowave VHS")
                {
                    continue;
                }

                stages.Add(stage);
            }
        }

        foreach (var variant in Variants)
        {
            var skybox = AssetDatabase.LoadAssetAtPath<Material>(variant.skyboxPath);
            var grid = AssetDatabase.LoadAssetAtPath<Material>(variant.gridPath);
            var bgm = AssetDatabase.LoadAssetAtPath<AudioClip>(variant.bgmPath);
            if (skybox == null || grid == null || bgm == null)
            {
                Debug.LogError($"[RetrowaveStages] Missing asset for {variant.stageName}. skybox={skybox != null}, grid={grid != null}, bgm={bgm != null}");
                continue;
            }

            stages.Add(new StageEntry
            {
                stageName = variant.stageName,
                thumbnail = null,
                backgroundVideo = null,
                skyboxMaterial = skybox,
                gridMaterial = grid,
                bgm = bgm,
                bpm = variant.bpm
            });
        }

        stageList.stages = stages.ToArray();
        EditorUtility.SetDirty(stageList);
        AssetDatabase.SaveAssets();
        Debug.Log("[RetrowaveStages] Retrowave Vapor, Orange, VHS stages and generated BGM applied.");
    }

    private static void WriteLoopWav(string path, float bpm, BgmMood mood)
    {
        const int bars = 8;
        const int beatsPerBar = 4;
        var duration = bars * beatsPerBar * 60f / bpm;
        var sampleCount = Mathf.CeilToInt(duration * SampleRate);
        var samples = new short[sampleCount * 2];
        var chordRoots = GetChordRoots(mood);

        for (int i = 0; i < sampleCount; i++)
        {
            var time = i / (float)SampleRate;
            var beat = time * bpm / 60f;
            var bar = Mathf.FloorToInt(beat / beatsPerBar) % chordRoots.Length;
            var beatInBar = beat - Mathf.Floor(beat / beatsPerBar) * beatsPerBar;
            var root = chordRoots[bar];

            var bass = Saw(root / 2f, time) * Envelope(beatInBar % 1f, 0.025f, 0.42f) * 0.23f;
            var pad = Chord(root, time, mood) * 0.16f;
            var arp = Arp(root, beat, time, mood) * 0.18f;
            var drums = DrumLayer(beat, mood) * 0.28f;
            var sidechain = 0.62f + 0.38f * Mathf.Clamp01((beat % 1f) * 2.5f);
            var value = (bass + (pad + arp) * sidechain + drums) * 0.62f;

            var stereoSpread = Mathf.Sin(time * 0.7f) * 0.08f;
            samples[i * 2] = FloatToPcm(value * (1f - stereoSpread));
            samples[i * 2 + 1] = FloatToPcm(value * (1f + stereoSpread));
        }

        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
        using var writer = new BinaryWriter(stream);
        var dataSize = samples.Length * sizeof(short);
        writer.Write(new[] { 'R', 'I', 'F', 'F' });
        writer.Write(36 + dataSize);
        writer.Write(new[] { 'W', 'A', 'V', 'E' });
        writer.Write(new[] { 'f', 'm', 't', ' ' });
        writer.Write(16);
        writer.Write((short)1);
        writer.Write((short)2);
        writer.Write(SampleRate);
        writer.Write(SampleRate * 2 * sizeof(short));
        writer.Write((short)(2 * sizeof(short)));
        writer.Write((short)16);
        writer.Write(new[] { 'd', 'a', 't', 'a' });
        writer.Write(dataSize);
        foreach (var sample in samples)
        {
            writer.Write(sample);
        }
    }

    private static float[] GetChordRoots(BgmMood mood)
    {
        return mood switch
        {
            BgmMood.Orange => new[] { 55f, 43.65f, 65.41f, 49f },
            BgmMood.Vhs => new[] { 46.25f, 36.71f, 55f, 41.2f },
            _ => new[] { 65.41f, 55f, 43.65f, 49f }
        };
    }

    private static float Chord(float root, float time, BgmMood mood)
    {
        var third = mood == BgmMood.Orange || mood == BgmMood.Vhs ? 1.1892f : 1.2599f;
        var fifth = 1.4983f;
        return SmoothSaw(root, time) + SmoothSaw(root * third, time) * 0.8f + SmoothSaw(root * fifth, time) * 0.7f;
    }

    private static float Arp(float root, float beat, float time, BgmMood mood)
    {
        var steps = mood == BgmMood.Vhs
            ? new[] { 1f, 1.4983f, 2f, 1.4983f, 1.1892f, 1.4983f, 2.3784f, 2f }
            : new[] { 1f, 1.2599f, 1.4983f, 2f, 1.4983f, 1.2599f, 2.5198f, 2f };
        var index = Mathf.FloorToInt(beat * 2f) % steps.Length;
        var gate = Envelope((beat * 2f) % 1f, 0.015f, 0.35f);
        return Mathf.Sin(2f * Mathf.PI * root * steps[index] * 2f * time) * gate;
    }

    private static float DrumLayer(float beat, BgmMood mood)
    {
        var kick = Mathf.Exp(-40f * (beat % 1f)) * Mathf.Sin(2f * Mathf.PI * 58f * (beat % 1f));
        var snarePhase = Mathf.Abs((beat % 4f) - 2f);
        var snare = snarePhase < 0.13f ? Noise(beat * 29.7f) * (1f - snarePhase / 0.13f) : 0f;
        var hat = Mathf.Abs((beat * 2f) % 1f) < 0.08f ? Noise(beat * 89.1f) * 0.35f : 0f;
        var snareGain = mood == BgmMood.Vhs ? 0.28f : 0.2f;
        return kick * 0.9f + snare * snareGain + hat * 0.18f;
    }

    private static float Envelope(float phase, float attack, float release)
    {
        if (phase < attack)
        {
            return phase / attack;
        }

        return Mathf.Clamp01(1f - (phase - attack) / release);
    }

    private static float Saw(float frequency, float time)
    {
        return 2f * (time * frequency - Mathf.Floor(time * frequency + 0.5f));
    }

    private static float SmoothSaw(float frequency, float time)
    {
        return (Saw(frequency, time) + Mathf.Sin(2f * Mathf.PI * frequency * time)) * 0.28f;
    }

    private static float Noise(float seed)
    {
        return Mathf.PerlinNoise(seed, seed * 0.31f) * 2f - 1f;
    }

    private static short FloatToPcm(float value)
    {
        return (short)(Mathf.Clamp(value, -0.95f, 0.95f) * short.MaxValue);
    }
}
