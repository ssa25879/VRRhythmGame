using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] cube;
    public Transform[] point;
    public StageListSO stageList;
    public float noteScale = 0.38f;
    public bool alternateNoteColors = true;

    float beat = 0.5f;
    float timer = 0;
    int nextCubeIndex;

    void Start()
    {
        nextCubeIndex = Random.Range(0, Mathf.Max(1, cube.Length));
        if (stageList != null)
        {
            int idx = PlayerPrefs.GetInt("SelectedStage", 0);
            if (idx < stageList.stages.Length)
            {
                float bpm = stageList.stages[idx].bpm;
                if (bpm > 0f)
                    beat = 60f / bpm;
            }
        }
    }

    void Update()
    {
        if (timer > beat)
        {
            if (cube == null || cube.Length == 0 || point == null || point.Length == 0)
            {
                return;
            }

            GameObject obj = Instantiate(GetNextCubePrefab(), point[Random.Range(0, point.Length)]);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one * noteScale;
            obj.transform.Rotate(transform.forward, 90 * Random.Range(0, 4));
            timer -= beat;
        }

        timer += Time.deltaTime;
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
