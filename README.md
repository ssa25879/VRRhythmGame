# VRRhythmGame

Unity 기반 VR 리듬 게임 프로토타입입니다. Meta Quest 3S 환경에서 컨트롤러 세이버로 날아오는 노트를 베는 Beat Saber 스타일의 플레이를 목표로 제작 중입니다.

## 시연 영상

- 테스트 플레이 일부공개 영상: https://youtu.be/3g2RF_wvLGw

## 포트폴리오 문서

- [VR 리듬 액션 게임 포트폴리오](Docs/Portfolio.md)

## 현재 구현 내용

- Intro 씬에서 스테이지 선택 후 Game 씬으로 전환
- 컨트롤러 기반 좌/우 세이버 시각화
- 빨간색/파란색 노트 프리팹 및 색상별 판정
- Beat Sage 기반 Retrowave VHS 노트 차트 적용
- 노트 방향 판정, 휘두르기 속도 판정, 세이버 판정 범위 조정
- 플레이어 뒤쪽 통과 기준 Miss 판정 및 Combo 초기화
- Score / Combo / HP / Miss HUD 표시
- Hit 성공 시 Combo 기반 HP 회복
- 곡 종료, 노트 출력 완료, HP 조건을 이용한 Result 화면 출력
- Result 화면에서 불필요한 Ray/포인터 표시 제거
- Intro / Game UI Sci-Fi 스타일 통일 작업
- Retrowave 배경 스테이지 3종 구성
  - Retrowave Vapor
  - Retrowave Orange
  - Retrowave VHS

## 현재 상태

프로토타입 기준으로 기본 플레이 루프는 구성되어 있습니다.

- 완료: 스테이지 선택, Game 전환, 노트 스폰, 세이버 판정, 점수/콤보/HP, HP 회복, Miss 처리, Result 출력, Retrowave VHS Beat Sage 차트 적용
- 개선 완료: 세이버 위치/각도, 판정 후함 조정, 노트 속도 조정, Result 출력 조건, Game HUD 시각 개선
- 확인 필요: Quest 3S 실기 기준 장시간 플레이 안정성, UI 가독성, BGM 볼륨, 노트 난이도 체감

## 사용 에셋 및 출처

- 음악: About That Oldie - Vibe Tracks, Retrowave 3곡 - Gemini
- 노트 맵: Beat Sage 자동 생성
- 배경: RETROWAVE SKIES Lite - Suggo Creations
- VFX: Free Game VFX - Eric VFX Studio, Sci-Fi Loading Screen Effects FREE - VisualX Studio
- UI: Sci-Fi UI
- 폰트: Liberation Sans, SIL Open Font License 1.1
- 개발 환경: Unity, XR Interaction Toolkit, TextMesh Pro, OpenXR

## 개발 환경

- Unity 6000.3.10f1
- Universal Render Pipeline
- XR Interaction Toolkit
- OpenXR
- Meta Quest 3S 테스트 기준

## AI 도구 사용

기능 구현과 프로젝트 정리 과정에서 Claude Code와 Codex를 활용해 제작 속도를 높였습니다. 코드 수정, Unity 에디터 작업 보조, 작업 로그 정리, GitHub 관리 작업에 사용했습니다.

## 작업 문서

- `AGENTS.md`: 프로젝트 공통 작업 지침
- `Task.md`: 현재 작업 상태와 남은 작업
- `Log.md`: 작업 이력과 확인 결과

## 참고

`Library`, `Temp`, `Logs`, `UserSettings`, 로컬 백업, 검증용 스크린샷 등 Unity 재생성 파일과 로컬 작업 파일은 저장소에 포함하지 않습니다.
