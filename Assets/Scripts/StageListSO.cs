using UnityEngine;

// ★ 사용법:
//   Project 창 우클릭 → Create → VRBeatSaber → Stage List
//   생성된 에셋을 IntroManager와 GameBackgroundController 모두에 드래그
//   이 에셋 하나에서만 스테이지를 추가/삭제하면 양쪽에 자동 반영됩니다.
[CreateAssetMenu(fileName = "StageList", menuName = "VRBeatSaber/Stage List")]
public class StageListSO : ScriptableObject
{
    [Tooltip("스테이지 목록 — Size를 늘려 추가, 줄여서 제거")]
    public StageEntry[] stages = new StageEntry[0];
}
