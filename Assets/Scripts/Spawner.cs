using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] cube;
    public Transform[] point;
    public StageListSO stageList;
    public AudioSource bgmSource;
    public float noteScale = 0.38f;
    public bool alternateNoteColors = true;
    public bool syncToBgm = true;
    public float noteSpeed = 3.2f;
    public float beatsPerSpawn = 2f;
    public float spawnLeadBeats = 0f;
    public float firstSpawnBeat = 1f;
    public int maxCatchUpSpawnsPerFrame = 4;
    public Transform missReferenceTransform;
    public float missBehindDistance = 0.6f;

    float beatDuration = 0.5f;
    float fallbackTimer;
    int nextCubeIndex;
    float nextSpawnBeat;
    BeatSaberNoteChart activeChart;
    int nextChartNoteIndex;
    float beatSageMinBeatGap;
    float lastSpawnedChartBeat = float.NegativeInfinity;
    int previousTimeSamples;
    double audioLoopOffsetSeconds;

    public bool HasFinishedSpawning
    {
        get
        {
            if (activeChart != null)
            {
                return activeChart.notes == null || nextChartNoteIndex >= activeChart.notes.Length;
            }

            if (syncToBgm && bgmSource != null && bgmSource.clip != null && !bgmSource.loop)
            {
                return !bgmSource.isPlaying || bgmSource.timeSamples >= bgmSource.clip.samples - 1;
            }

            return false;
        }
    }

    public bool HasSongEnded
    {
        get
        {
            if (bgmSource == null || bgmSource.clip == null)
            {
                return false;
            }

            if (bgmSource.loop)
            {
                return false;
            }

            return !bgmSource.isPlaying || bgmSource.timeSamples >= bgmSource.clip.samples - 1;
        }
    }

    void Awake()
    {
        ApplySelectedStageTuning();
    }

    void Start()
    {
        nextCubeIndex = Random.Range(0, Mathf.Max(1, cube.Length));
        noteSpeed = Mathf.Max(0.1f, noteSpeed);
        beatsPerSpawn = Mathf.Max(0.25f, beatsPerSpawn);
        firstSpawnBeat = Mathf.Max(0f, firstSpawnBeat);
        maxCatchUpSpawnsPerFrame = Mathf.Max(1, maxCatchUpSpawnsPerFrame);
        nextSpawnBeat = firstSpawnBeat;

        ResolveBgmSource();
        ResolveMissReference();
        ApplySelectedStageTuning();

        if (bgmSource != null && bgmSource.clip != null)
        {
            previousTimeSamples = bgmSource.timeSamples;
        }

        Debug.Log($"[Spawner] BPM sync initialized. beatDuration={beatDuration:0.000}, noteSpeed={noteSpeed:0.00}, beatsPerSpawn={beatsPerSpawn:0.00}, syncToBgm={syncToBgm}, bgm={(bgmSource != null && bgmSource.clip != null ? bgmSource.clip.name : "null")}");
    }

    void Update()
    {
        if (syncToBgm && bgmSource != null && bgmSource.clip != null)
        {
            if (activeChart != null)
            {
                SpawnFromChartClock();
                return;
            }

            SpawnFromAudioClock();
            return;
        }

        SpawnFromFallbackTimer();
    }

    void SpawnFromAudioClock()
    {
        if (!bgmSource.isPlaying)
        {
            return;
        }

        if (!bgmSource.loop && bgmSource.clip != null && bgmSource.timeSamples >= bgmSource.clip.samples - 1)
        {
            return;
        }

        double songTime = GetSongTimeSeconds();
        double targetTime = songTime + spawnLeadBeats * beatDuration;
        int spawned = 0;

        while (nextSpawnBeat * beatDuration <= targetTime && spawned < maxCatchUpSpawnsPerFrame)
        {
            SpawnNote();
            nextSpawnBeat += beatsPerSpawn;
            spawned++;
        }
    }

    void SpawnFromChartClock()
    {
        if (!bgmSource.isPlaying || activeChart.notes == null)
        {
            return;
        }

        if (!bgmSource.loop && bgmSource.clip != null && bgmSource.timeSamples >= bgmSource.clip.samples - 1)
        {
            return;
        }

        double songTime = GetSongTimeSeconds();
        double targetTime = songTime + spawnLeadBeats * beatDuration;
        int spawned = 0;

        while (nextChartNoteIndex < activeChart.notes.Length && spawned < maxCatchUpSpawnsPerFrame)
        {
            BeatSaberNote chartNote = activeChart.notes[nextChartNoteIndex];
            float noteTime = chartNote.GetTimeSeconds(activeChart.beatsPerMinute);
            if (noteTime > targetTime)
            {
                break;
            }

            nextChartNoteIndex++;
            if (!chartNote.IsPlayable)
            {
                continue;
            }

            if (chartNote.beat - lastSpawnedChartBeat < beatSageMinBeatGap)
            {
                continue;
            }

            SpawnChartNote(chartNote);
            lastSpawnedChartBeat = chartNote.beat;
            spawned++;
        }
    }

    double GetSongTimeSeconds()
    {
        int currentSamples = bgmSource.timeSamples;
        if (currentSamples < previousTimeSamples && bgmSource.clip != null)
        {
            audioLoopOffsetSeconds += bgmSource.clip.length;
        }

        previousTimeSamples = currentSamples;
        return audioLoopOffsetSeconds + currentSamples / (double)bgmSource.clip.frequency;
    }

    void SpawnFromFallbackTimer()
    {
        fallbackTimer += Time.deltaTime;
        while (fallbackTimer >= beatDuration)
        {
            SpawnNote();
            fallbackTimer -= beatDuration;
        }
    }

    void SpawnNote()
    {
        if (cube == null || cube.Length == 0 || point == null || point.Length == 0)
        {
            return;
        }

        GameObject obj = Instantiate(GetNextCubePrefab(), point[Random.Range(0, point.Length)]);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one * noteScale;
        obj.transform.Rotate(transform.forward, 90 * Random.Range(0, 4));
        if (obj.TryGetComponent<Cube>(out var note))
        {
            note.SetMoveSpeed(noteSpeed);
            note.SetMissReference(missReferenceTransform, missBehindDistance);
        }
        GameScoreController.Instance?.RegisterNoteSpawned();
    }

    void SpawnChartNote(BeatSaberNote chartNote)
    {
        if (cube == null || cube.Length == 0 || point == null || point.Length == 0)
        {
            return;
        }

        GameObject obj = Instantiate(GetChartCubePrefab(chartNote.type), GetChartSpawnPoint(chartNote));
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one * noteScale;
        obj.transform.Rotate(transform.forward, GetCutDirectionAngle(chartNote.cutDirection));
        if (obj.TryGetComponent<Cube>(out var note))
        {
            note.SetMoveSpeed(noteSpeed);
            note.SetMissReference(missReferenceTransform, missBehindDistance);
        }

        GameScoreController.Instance?.RegisterNoteSpawned();
    }

    void ResolveBgmSource()
    {
        if (bgmSource != null)
        {
            return;
        }

        GameBackgroundController backgroundController = FindFirstObjectByType<GameBackgroundController>();
        if (backgroundController != null)
        {
            bgmSource = backgroundController.bgmSource;
        }
    }

    void ResolveMissReference()
    {
        if (missReferenceTransform != null)
        {
            return;
        }

        if (Camera.main != null)
        {
            missReferenceTransform = Camera.main.transform;
        }
    }

    void ApplySelectedStageTuning()
    {
        if (stageList == null || stageList.stages == null || stageList.stages.Length == 0)
        {
            return;
        }

        int index = Mathf.Clamp(PlayerPrefs.GetInt("SelectedStage", 0), 0, stageList.stages.Length - 1);
        StageEntry stage = stageList.stages[index];
        activeChart = stage.useBeatSageChart ? stage.noteChart : null;
        nextChartNoteIndex = 0;
        lastSpawnedChartBeat = float.NegativeInfinity;
        beatSageMinBeatGap = Mathf.Max(0f, stage.beatSageMinBeatGap);
        float bpm = stage.bpm;
        if (activeChart != null && activeChart.beatsPerMinute > 0f)
        {
            bpm = activeChart.beatsPerMinute;
        }

        if (bpm > 0f)
        {
            beatDuration = 60f / bpm;
        }

        if (stage.noteSpeed > 0f)
        {
            noteSpeed = stage.noteSpeed;
        }

        if (stage.beatsPerSpawn > 0f)
        {
            beatsPerSpawn = stage.beatsPerSpawn;
        }

        spawnLeadBeats = Mathf.Max(0f, stage.spawnLeadBeats);

        if (activeChart != null)
        {
            int filteredNotes = activeChart.GetPlayableNoteCount(beatSageMinBeatGap);
            Debug.Log($"[Spawner] Beat Sage chart active. notes={activeChart.PlayableNoteCount}, filteredNotes={filteredNotes}, bpm={activeChart.beatsPerMinute:0.0}, minBeatGap={beatSageMinBeatGap:0.00}");
        }
    }

    GameObject GetNextCubePrefab()
    {
        if (!alternateNoteColors || cube.Length == 1)
        {
            return cube[Random.Range(0, cube.Length)];
        }

        GameObject prefab = cube[nextCubeIndex % cube.Length];
        nextCubeIndex = (nextCubeIndex + 1) % cube.Length;
        return prefab;
    }

    GameObject GetChartCubePrefab(int noteType)
    {
        string targetName = noteType == 0 ? "RED" : "BLUE";
        for (int i = 0; i < cube.Length; i++)
        {
            if (cube[i] != null && cube[i].name.IndexOf(targetName, System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return cube[i];
            }
        }

        int fallbackIndex = noteType == 0 ? 0 : Mathf.Min(1, cube.Length - 1);
        return cube[Mathf.Clamp(fallbackIndex, 0, cube.Length - 1)];
    }

    Transform GetChartSpawnPoint(BeatSaberNote chartNote)
    {
        float targetX = chartNote.lineIndex < 2 ? -0.5f : 0.5f;
        float targetY = chartNote.lineLayer <= 0 ? -0.5f : 0f;
        Transform bestPoint = point[0];
        float bestDistance = float.MaxValue;

        for (int i = 0; i < point.Length; i++)
        {
            if (point[i] == null)
            {
                continue;
            }

            Vector3 local = point[i].localPosition;
            float distance = (new Vector2(local.x, local.y) - new Vector2(targetX, targetY)).sqrMagnitude;
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestPoint = point[i];
            }
        }

        return bestPoint;
    }

    float GetCutDirectionAngle(int cutDirection)
    {
        switch (cutDirection)
        {
            case 0:
                return 0f;
            case 1:
                return 180f;
            case 2:
                return 90f;
            case 3:
                return -90f;
            case 4:
                return 45f;
            case 5:
                return -45f;
            case 6:
                return 135f;
            case 7:
                return -135f;
            default:
                return 0f;
        }
    }
}
