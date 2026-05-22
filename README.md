# VRRhythmGame

Unity 기반 VR 리듬 게임 프로토타입입니다. Meta Quest 계열 기기에서 컨트롤러 세이버로 날아오는 노트를 베는 Beat Saber 스타일의 플레이를 목표로 제작 중입니다.

## 현재 구현 내용

- Intro 씬에서 스테이지 선택 후 Game 씬으로 전환
- 빨강/파랑 세이버와 노트 색상별 판정
- Retrowave 배경 스테이지 3종
  - Retrowave Vapor
  - Retrowave Orange
  - Retrowave VHS
- 스테이지별 BGM 재생 및 반복 처리
- XR 컨트롤러 UI 상호작용 및 조준 레이 표시

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
