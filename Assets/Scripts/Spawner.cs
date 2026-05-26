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
    public int beatsPerSpawn = 1;
    public float spawnLeadBeats = 0f;
    public int maxCatchUpSpawnsPerFrame = 4;

    float beatDuration = 0.5f;
    float fallbackTimer;
    int nextCubeIndex;
    int nextSpawnBeat;
    int previousTimeSamples;
    double audioLoopOffsetSeconds;

    void Start()
    {
        nextCubeIndex = Random.Range(0, Mathf.Max(1, cube.Length));
        beatsPerSpawn = Mathf.Max(1, beatsPerSpawn);
        maxCatchUpSpawnsPerFrame = Mathf.Max(1, maxCatchUpSpawnsPerFrame);

        ResolveBgmSource();
        ApplySelectedStageBpm();

        if (bgmSource != null && bgmSource.clip != null)
        {
            previousTimeSamples = bgmSource.timeSamples;
        }

        Debug.Log($"[Spawner] BPM sync initialized. beatDuration={beatDuration:0.000}, syncToBgm={syncToBgm}, bgm={(bgmSource != null && bgmSource.clip != null ? bgmSource.clip.name : "null")}");
    }

    void Update()
    {
        if (syncToBgm && bgmSource != null && bgmSource.clip != null)
        {
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

    void ApplySelectedStageBpm()
    {
        if (stageList == null || stageList.stages == null || stageList.stages.Length == 0)
        {
            return;
        }

        int index = Mathf.Clamp(PlayerPrefs.GetInt("SelectedStage", 0), 0, stageList.stages.Length - 1);
        float bpm = stageList.stages[index].bpm;
        if (bpm > 0f)
        {
            beatDuration = 60f / bpm;
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
}
