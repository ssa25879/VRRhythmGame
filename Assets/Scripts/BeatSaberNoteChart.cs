using UnityEngine;

[CreateAssetMenu(fileName = "BeatSaberNoteChart", menuName = "VRBeatSaber/Beat Saber Note Chart")]
public class BeatSaberNoteChart : ScriptableObject
{
    public float beatsPerMinute = 120f;
    public float noteJumpMovementSpeed = 10f;
    public float noteJumpStartBeatOffset;
    public BeatSaberNote[] notes = System.Array.Empty<BeatSaberNote>();

    public int PlayableNoteCount
    {
        get
        {
            if (notes == null)
            {
                return 0;
            }

            int count = 0;
            for (int i = 0; i < notes.Length; i++)
            {
                if (notes[i].IsPlayable)
                {
                    count++;
                }
            }

            return count;
        }
    }

    public int GetPlayableNoteCount(float minBeatGap)
    {
        if (notes == null)
        {
            return 0;
        }

        minBeatGap = Mathf.Max(0f, minBeatGap);
        float lastAcceptedBeat = float.NegativeInfinity;
        int count = 0;

        for (int i = 0; i < notes.Length; i++)
        {
            if (!notes[i].IsPlayable)
            {
                continue;
            }

            if (notes[i].beat - lastAcceptedBeat < minBeatGap)
            {
                continue;
            }

            lastAcceptedBeat = notes[i].beat;
            count++;
        }

        return count;
    }
}

[System.Serializable]
public struct BeatSaberNote
{
    public float beat;
    public int lineIndex;
    public int lineLayer;
    public int type;
    public int cutDirection;

    public bool IsPlayable => type == 0 || type == 1;

    public float GetTimeSeconds(float bpm)
    {
        return beat * 60f / Mathf.Max(1f, bpm);
    }
}
