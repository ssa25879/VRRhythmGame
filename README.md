# VRRhythmGame

Unity 기반 VR 리듬 게임 프로토타입입니다. Meta Quest 계열 기기에서 컨트롤러 세이버로 날아오는 노트를 베는 Beat Saber 스타일의 플레이를 목표로 제작 중입니다.

## 현재 구현 내용

- Intro 씬에서 스테이지 선택 후 Game 씬으로 전환
- 빨강/파랑 세이버와 노트 색상별 판정
- 노트 방향 판정, Score / Combo / HP / Miss 기본 시스템
- 곡 종료 또는 HP 0 도달 시 결과 화면 표시
- Retrowave 배경 스테이지 3종
  - Retrowave Vapor
  - Retrowave Orange
  - Retrowave VHS
- 스테이지별 BGM 재생 및 반복 처리
- XR 컨트롤러 UI 상호작용 및 조준 레이 표시
- Game 씬 HUD 오브젝트 기반 관리 구조

## 현재 완성도

프로토타입 기준 핵심 플레이 흐름은 약 70% 정도 구현된 상태입니다.

- 완료: Intro 스테이지 선택, Game 씬 전환, 기본 노트 스폰, 세이버/노트 색상 판정, 방향 판정, 점수/콤보/HP/Miss, 결과 화면, Retrowave 스테이지 구성
- 부분 완료: XR 컨트롤러 UI Ray, 결과 화면 OK 입력, HUD 배치와 가독성, 스테이지별 BGM 연결
- 확인 필요: Quest 3S 실기에서 HMD 기준 카메라, 컨트롤러 Ray 체감, 노트 판정 체감, UI 위치와 크기, BGM 볼륨 밸런스
- 미구현/개선 예정: Rank 표시, 판정 밸런스 튜닝, 노트/세이버/VFX 연출 강화, 스테이지별 완성도 보강

## 개발 환경

- Unity 6 계열 프로젝트
- Universal Render Pipeline
- XR Interaction Toolkit
- OpenXR / Meta Quest 3S 테스트 기준

## AI 도구 활용

이 프로젝트는 개발 과정에서 AI 도구를 함께 사용해 코드 수정, Unity 에디터 작업 보조, 작업 로그 정리, GitHub 관리 작업을 진행하고 있습니다.

## 작업 지침

다른 환경에서 클론하거나 다른 AI 도구와 함께 작업할 때는 먼저 아래 문서를 확인합니다.

- `AGENTS.md`: 프로젝트 공통 작업 지침
- `Task.md`: 현재 작업 상태와 대기 항목
- `Log.md`: 지금까지의 작업 이력
- `Docs/BGM_Candidates.md`: 스테이지별 BGM 후보와 라이선스 확인 메모

## 참고

`Library`, `Temp`, `Logs`, `UserSettings`, 로컬 백업과 검증용 스크린샷은 저장소에 포함하지 않습니다.
