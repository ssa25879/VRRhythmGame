using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public class StageEntry
{
    [Tooltip("Intro 씬 UI에 표시될 스테이지 이름")]
    public string stageName = "New Stage";

    [Tooltip("선택 화면 썸네일 이미지 (Sprite)")]
    public Sprite thumbnail;

    [Tooltip("360° 배경 동영상 (.mp4) — RenderTexture360에 출력됩니다")]
    public VideoClip backgroundVideo;

    [Tooltip("영상 대신 사용할 Skybox Material")]
    public Material skyboxMaterial;

    [Tooltip("Retrowave/Synthwave용 바닥 Grid Material")]
    public Material gridMaterial;

    [Tooltip("이 스테이지의 BGM")]
    public AudioClip bgm;

    [Tooltip("BGM의 BPM — 노트 생성 간격 계산에 사용됩니다 (beat = 60 / bpm)")]
    public float bpm = 120f;
}
