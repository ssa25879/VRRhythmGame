# VR Beat Saber — 작업 현황 및 가이드

---

## 협업 지침 (Collaboration Guidelines)

> 이 파일은 Claude Code, 다른 AI, 팀원 누구나 읽고 바로 작업에 투입될 수 있도록 작성되었습니다.

### 이 파일의 역할

- **현재 남은 작업 목록**을 우선순위 순으로 관리합니다.
- **스테이지(배경+BGM) 추가·삭제 방법**을 안내합니다.
- 작업을 완료하면 `- [ ]` → `- [x]`로 체크하고 `Log.md`에 이력을 기록합니다.

### 작업 인계 시 확인 순서

1. `Log.md` — 전체 이력 및 현재 상태 파악
2. `Task.md` (이 파일) — 남은 작업 및 가이드 확인
3. Unity 에디터에서 `Assets/Scenes/Intro.unity` 열기
4. 플레이 모드 종료 후 컴파일 에러 확인 → 작업 시작

### 작업 기록 규칙

- 완료: `- [x]`, 미완료: `- [ ]`, 진행 중: `- [~]`
- 작업 완료 후 반드시 `Log.md`에 날짜 + 내용 추가
- 새 작업은 이 파일 **남은 작업** 섹션에 추가
- `Log.md`, `Task.md` 같은 로그/관리 문서는 백업하지 않음
- `Log.md`, `Task.md` 수정 시 별도 검토 요청 없이 바로 수정하고, 완료 후 수정 사실만 보고
- 씬 백업은 `Assets/Scenes/Backup` 폴더에 저장함
- 기존 `Assets/Scenes/*_backup_20260514.unity` 백업은 유지함

### Unity 스크립트 실행 규칙

> **`execute_code` 툴 사용 금지 — `execute_script` 툴만 사용할 것**

이 프로젝트 경로(`D:\work\VRBeatSaber`)는 Windows 260자 경로 제한으로 인해
`execute_code` 툴(인라인 C# 코드 실행)이 동작하지 않습니다.

**올바른 방법:**
1. 실행할 C# 코드를 `Assets/Scripts/Editor/` 아래 `.cs` 파일로 저장
2. `execute_script` 툴로 해당 파일 경로를 지정해서 실행

```
// 예시
execute_script("Assets/Scripts/Editor/MySetupScript.cs")
```

기존 Editor 스크립트 예시: `SaveCurrentScene.cs`, `FixIntroScene.cs`, `CreateStageList.cs` 등

---

## 현재 상태 (2026-05-27 기준)

| 항목 | 상태 |
|------|------|
| 컴파일 에러 | 없음 |
| 활성 씬 | `Assets/Scenes/Intro.unity` |
| 배경 렌더링 | **Skybox/Panoramic** (최적화 완료) |
| 기능 검증 | 스테이지 선택 및 씬 전환 연동 완료 |
| Intro UI 위치 | Canvas `y=-0.22`, `z=2.5` |
| 배경 구조 | 프로토타입식 `Background` 루트 + 검은 바닥 큐브 2개 적용 |
| XR UI 입력 | 좌/우 컨트롤러 `Left_NearFarInteractor` / `Right_NearFarInteractor` 활성 |
| 컨트롤러 조준 표시 | 기본 XR Ray 사용, 보조 `VisibleUIPointer`는 비활성 |
| Intro Ray 원점 | `Left_NearFarInteractor`, `Right_NearFarInteractor` 컨트롤러 Transform 기준 ✅ |
| 비활성화된 Ray 오브젝트 | 구형 `Near-Far Interactor` (Left/Right Controller), 손 추적용 `Near-Far Interactor` (Left/Right Hand), Teleport Stabilized Origin 2개, Gaze/Teleport Interactor |
| 스테이지 데이터 | `Assets/Data/StageList.asset`에 `About That Oldie` + Retrowave 3종 4개 |
| Score/Combo/HP/MISS | 구현 완료, Rank 미구현 |
| 결과 화면 | 구현 완료 (FAILED/결과 화면, OK 버튼) |
| Game 씬 Ray | 게임 중 비활성화, 결과 화면 시 활성화 |
| Windows Long Path | 현재 비활성화(`LongPathsEnabled=0`), 관리자 권한 필요 |

---

## 남은 작업 (우선순위 순)

- [~] **1. 실제 VR 하드웨어 테스트**
  - VR 헤드셋을 착용하고 실제 컨트롤러로 UI 버튼 트리거 및 씬 전환 시 멀미/부하 여부 최종 확인
  - 2026-05-20 Quest 3S 실기 테스트: Intro UI가 높게 보여 `Canvas` Y 위치를 `0` → `-0.35`로 낮춤
  - 2026-05-20 프로토타입식 배경 구조 적용: `Background` 루트와 검은 바닥 큐브 2개 구성
  - 2026-05-20 컨트롤러 UI 클릭 문제 수정: 좌/우 `XRRayInteractor`의 `m_EnableUIInteraction`을 켬
  - 2026-05-20 추가 수정: 좌/우 컨트롤러에 XRI Starter Assets `NearFarInteractor` 프리팹 추가
  - 2026-05-20 조준 레이 추가: 좌/우 `NearFarInteractor` 하위에 `VisibleUIPointer` 추가
  - 2026-05-20 레이 기준 수정: `VisibleUIPointer`를 좌/우 컨트롤러 하위로 옮기고 UI 평면까지만 표시
  - 2026-05-27 **Intro Ray 원점 재정리**: 구형 `Near-Far Interactor` (Controller/Hand 계열), Teleport Stabilized Origin 비활성화. `Left_NearFarInteractor`/`Right_NearFarInteractor`는 컨트롤러 기준 활성 유지, 보조 `VisibleUIPointer`는 비활성화
  - 2026-05-20 현재 상황: 사용자가 당장 Quest 3S 실기 테스트를 진행할 수 없어, 에디터에서 확인 가능한 범위만 검증 완료
  - 다음 확인: Quest 3S에서 UI Ray가 **컨트롤러에서만** 나가는지, 버튼 클릭(Prev/Next/Start/Mute)이 정상 동작하는지 재테스트

- [x] **2. 스테이지 데이터 확장**
  - `Assets/Data/StageList.asset`에 다양한 배경 영상과 BGM을 추가하여 리스트 순환 기능 테스트
  - 2026-05-20 `RETROWAVE SKIES Lite` 기반 Retrowave 3종 스테이지와 전용 BGM 추가 완료

- [~] **3. 시스템 설정 (환경 변수)**
  - Windows Long Path 설정을 시스템에 적용하여 `execute_code` 활성화 시도 (관리자 권한 필요)
  - 2026-05-20 확인: `LongPathsEnabled=0`; 활성화 시도는 `Access is denied`로 실패
  - 현재 Codex가 일반 권한에서 완료할 수 없는 작업이므로 관리자 권한 작업 대기

---

## 완료된 작업 (2026-05-20 세션)

- [x] **Log.md / Task.md 내용 확인**: 두 파일을 이 프로젝트 전용 작업 이력/작업 목록 문서로 관리하기로 정리
- [x] **Unity MCP 연결 확인**: MCP 프로젝트 루트가 `D:\work\VRBeatSaber`로 감지됨
- [x] **Unity Editor 상태 확인**: Play Mode 꺼짐, 컴파일 에러 없음, 활성 에셋 `Assets/Scenes/Intro.unity`
- [x] **스테이지 확장 가능 여부 확인**: 현재 전용 미디어는 `motion.mp4`, `About That Oldie - Vibe Tracks.mp3`만 확인됨
- [x] **Windows Long Path 상태 확인**: 현재 비활성화 상태와 관리자 권한 필요 여부 확인
- [x] **Quest 3S 실기 피드백 반영**: Intro 씬 UI 높이를 낮추기 위해 `Canvas` 위치를 `y=-0.35`로 조정
- [x] **프로토타입 배경 구조 반영**: Intro/Game 씬에 `Background` 루트, 검은 바닥 큐브 2개, Skybox/Panoramic 설정 적용
- [x] **씬 백업 정책 변경**: 신규 씬 백업은 `Assets/Scenes/Backup` 폴더에 저장하도록 정리
- [x] **컨트롤러 UI 클릭 문제 수정**: Intro 씬 좌/우 컨트롤러 `XRRayInteractor`의 UI Interaction 활성화
- [x] **컨트롤러 UI 클릭 추가 수정**: Intro 씬 좌/우 컨트롤러에 `Left_NearFarInteractor`, `Right_NearFarInteractor` 추가
- [x] **컨트롤러 조준 레이 표시 추가**: 런타임에 좌/우 컨트롤러 방향 레이와 끝점 포인터 표시
- [x] **컨트롤러 기준 UI 평면 레이로 수정**: 레이가 컨트롤러에서 시작해 UI 평면까지 표시되도록 변경

---

## 완료된 작업 (2026-05-15 세션)

- [x] **Game View 배경 표시 확인**: 360 비디오 정상 출력 확인
- [x] **UI 버튼 및 씬 전환 연동**: `PlayerPrefs` 기반 스테이지 연동 완료
- [x] **렌더링 방식 최적화**: `Skybox/Panoramic` 셰이더 적용 및 레퍼런스 프로젝트와 동기화 완료
- [x] **기술 이슈 분석**: Windows 경로 제한 및 타임아웃 원인 파악 및 대응 가이드 작성
- [x] **Log.md / Task.md 최종 업데이트 완료**

---

## 스테이지(배경+BGM) 추가 방법

### 스테이지란?

하나의 스테이지 = **360° 배경 영상 + BGM + 썸네일**의 묶음입니다.
`Assets/Data/StageList.asset` 하나만 수정하면 Intro 씬과 Game 씬 양쪽에 자동 반영됩니다.

---

### Step 1 — 파일 준비

`Assets/360 Music/` 폴더에 아래 파일을 추가합니다.

| 파일 종류 | 형식 | 예시 |
|-----------|------|------|
| 360° 배경 영상 | `.mp4` | `stage2_bg.mp4` |
| BGM | `.mp3` / `.wav` | `stage2_bgm.mp3` |
| 썸네일 이미지 (선택) | `.png` / `.jpg` | `stage2_thumb.png` |

> 썸네일 임포트 후: Inspector → Texture Type → **Sprite (2D and UI)** → Apply

---

### Step 2 — StageList.asset에 항목 추가

1. Project 창에서 `Assets/Data/StageList.asset` 선택
2. Inspector → **Stages** → **Size** 를 1 늘린다 (예: 1 → 2)
3. 새 항목(Element 1)을 채운다

| 필드 | 입력 내용 |
|------|-----------|
| Stage Name | 스테이지 이름 (UI에 표시됨) |
| Thumbnail | 썸네일 Sprite 드래그 (선택사항) |
| Background Video | 360° 배경 영상 (.mp4) 드래그 |
| Bgm | BGM 오디오 클립 드래그 |

4. **Ctrl+S** 로 저장

> 이것만 하면 끝입니다. 씬 재빌드나 스크립트 수정 불필요.

---

## 스테이지 삭제 방법

1. `Assets/Data/StageList.asset` 선택
2. Inspector → **Stages** → 삭제할 항목 우클릭 → **Delete Array Element**
3. **Ctrl+S** 저장

---

## 씬 구조 참고

### Intro 씬 (Assets/Scenes/Intro.unity)

```
Skybox Material     — Material360.mat (Skybox/Panoramic 셰이더 사용)
Video               — VideoPlayer, RenderTexture360 출력
Canvas (World Space, 800x550, scale 0.001, pos 0,1.5,2.5)
  ├── TitleText         — "VR BEAT SABER"
  ├── PrevButton (◀)   — IntroManager.PrevStage()
  ├── NextButton (▶)   — IntroManager.NextStage()
  ├── ThumbnailBG
  │   └── ThumbnailImage  — 선택된 스테이지 썸네일
  ├── StageNameText     — 선택된 스테이지 이름
  ├── StartButton       — IntroManager.OnStartGame()
  └── FadeOverlay       — 페이드인/아웃 (CanvasGroup)
IntroManager (Empty GO)
  ├── stageList         → Assets/Data/StageList.asset
  ├── backgroundPlayer  → Video 오브젝트의 VideoPlayer
  ├── bgmSource         → 자체 AudioSource
  ├── thumbnailImage    → ThumbnailImage
  ├── stageNameText     → StageNameText
  └── fadeOverlay       → FadeOverlay
```

### Game 씬 (Assets/Scenes/Game.unity)

```
GameBackgroundController (Empty GO)
  ├── stageList         → Assets/Data/StageList.asset
  ├── backgroundPlayer  → Video 오브젝트의 VideoPlayer
  └── bgmSource         → 자체 AudioSource (loop, vol 0.7)
```

---

## 씬 전환 흐름

```
[Intro 씬]
  PrevButton / NextButton
    → IntroManager.PrevStage() / NextStage()
    → stageList.stages[index] 로 VideoPlayer + BGM 미리보기

  StartButton
    → IntroManager.OnStartGame()
    → PlayerPrefs.SetInt("SelectedStage", index)
    → FadeOut → SceneManager.LoadScene("Game")

[Game 씬]
  GameBackgroundController.Awake()
    → PlayerPrefs.GetInt("SelectedStage", 0)
    → stageList.stages[index].backgroundVideo → VideoPlayer.clip
    → stageList.stages[index].bgm → AudioSource.clip → Play()
```

---

## 빌드 세팅

| 인덱스 | 씬 경로 |
|--------|---------|
| 0 | `Assets/Scenes/Intro.unity` |
| 1 | `Assets/Scenes/Game.unity` |

---

## 관련 스크립트 파일

| 파일 | 역할 |
|------|------|
| `Assets/Scripts/StageEntry.cs` | 스테이지 데이터 구조체 |
| `Assets/Scripts/StageListSO.cs` | ScriptableObject — 스테이지 목록 |
| `Assets/Scripts/IntroManager.cs` | Intro 씬 컨트롤러 |
| `Assets/Scripts/GameBackgroundController.cs` | Game 씬 배경+BGM 적용 |
| `Assets/Scripts/SkyRotator.cs` | 스카이돔 Y축 자동 회전 |

---

## 2026-05-20 추가 확인 작업

- [x] Intro 씬 TMP 특수기호 깨짐 가능성 확인 및 ASCII 텍스트로 수정
  - `▶` → `>`
  - `▶  PLAY` → `PLAY`
  - `◀` → `<`
- [x] 수정 전 씬 백업 생성: `Assets/Scenes/Backup/Intro_backup_20260520_before_text_fix.unity`
- [x] Unity Editor 상태 확인: 컴파일 에러 없음
- [~] Quest 3S에서 Intro 씬 텍스트가 깨지지 않는지 실기 재확인
  - 에디터 검증: TMP 텍스트 5개 확인, `▶` / `◀` 특수 화살표 없음
  - 현재 실기 테스트 불가로 대기
- [x] 6번 에셋 임포트 오류 원인 확인
  - `Knife - PRO Effects - Sci-Fi FX` 연동 prefab들이 별도 누락 prefab을 참조함
  - 문제 폴더를 `Backup/AssetImports/20260520_ProjectileFactoryBrokenIntegration/`로 이동
  - AssetDatabase 새로고침 후 신규 import error 없음
- [x] Game 씬 Cube 세이버를 광선검/네온 블레이드 스타일로 교체
  - `Saber_Neon_Blue.mat`, `Saber_Neon_Red.mat` 생성
  - 세이버 블레이드에 `TrailRenderer`, `Point Light`, `SaberGlowPulse` 적용
  - 1번 에셋의 hit VFX prefab을 `Saber.cs` 타격 시 생성하도록 연결
- [x] 3번 에셋 `Loading_free_blue`를 Intro 씬 `PLAY` 전환 VFX로 연결
- [x] Game 씬 노트 크기 및 세이버 판정 개선
  - RED/BLUE 노트 prefab 스케일 `0.38`로 축소
  - Spawner 생성 스케일 `noteScale=0.38` 적용
  - 세이버 판정을 단일 Raycast에서 블레이드 이동 구간 `OverlapCapsule` 방식으로 변경
  - 색상별 LayerMask 유지
  - 방향 판정 허용 각도 `85도`, 최소 스윙 속도 `0.35`로 조정
  - 성공 시 hit VFX, 노트 축소 제거, 햅틱 호출 추가
- [x] 노트/세이버 디자인 변경
  - 노트: 에너지 블록 core, 네온 프레임, 절단 방향 화살표 런타임 생성
  - 세이버: 에너지 블레이드, 흰색 코어, 손잡이, emitter ring, cross guard 런타임 생성
  - Play Mode 실행 후 `Assets/Screenshots/test_play_game.png` 캡처 확인
- [x] 세이버 단독 프리뷰 렌더링
  - `Assets/Screenshots/saber_preview.png` 생성
- [x] 세이버 크로스가드 제거
  - 런타임 세이버 디자인과 프리뷰 렌더링 양쪽에서 `Cross Guard` 제거
  - `Assets/Screenshots/saber_preview.png` 재생성
- [x] 1번 에셋 `RETROWAVE SKIES Lite` 기반 `Neon Grid` 스테이지 추가
  - `Vapor_Skybox`와 `M_Grid Vapor Lite`를 Game 씬에 연결
  - 영상 없는 스테이지에서도 Intro/Game 배경 처리가 깨지지 않도록 수정
  - Game 씬 Play Mode 캡처로 적용 확인: `Assets/Screenshots/test_play_game.png`
- [x] `RETROWAVE SKIES Lite` 배경 프리셋별 스테이지 3종 추가
  - `Retrowave Vapor`: Vapor skybox/grid, 전용 synth BGM, BPM 124
  - `Retrowave Orange`: Orange skybox/grid, 전용 sunset BGM, BPM 110
  - `Retrowave VHS`: VHS skybox/grid, 전용 darker BGM, BPM 132
  - 생성 BGM 위치: `Assets/Audio/Generated/`
  - Intro/Game BGM AudioSource loop 강제 활성화
- [x] 에디터에서 확인 가능한 범위 검증
  - Retrowave 3종 스테이지 skybox/grid/BGM/BPM 연결 확인
  - Intro 씬 TMP 텍스트 5개 확인, 기존 특수 화살표 문자 없음
  - Game 씬 `GameBackgroundController`의 StageList/BGM/Grid 참조 확인
  - Play Mode에서 Retrowave 3종 화면 캡처 생성
    - `Assets/Screenshots/RetrowaveGameStages/retrowave_game_vapor.png`
    - `Assets/Screenshots/RetrowaveGameStages/retrowave_game_orange.png`
    - `Assets/Screenshots/RetrowaveGameStages/retrowave_game_vhs.png`
- [x] 파란 노트가 빨간색으로 보이는 문제 수정
  - 원인: 프로젝트 레이어 이름은 `Blue`인데 `Cube.cs`에서 `BLUE`로 조회해 파란색 판정 실패
  - `Cube.cs`, `Saber.cs` 레이어 이름 판정 수정
  - `Spawner.cs`를 색상 균형 교차 생성 방식으로 보강
  - Play Mode 검증: `RED=21`, `BLUE=21`
  - 추가 진단 캡처: `Assets/Screenshots/note_color_diagnostic.png`
  - 추가 Play Mode 검증: `RED=45`, `BLUE=46`
- [x] Retrowave 3종 BGM 런타임 재생 확인
  - `Retrowave Vapor`: `isPlaying=True`, `loop=True`, volume 0.70
  - `Retrowave Orange`: `isPlaying=True`, `loop=True`, volume 0.70
  - `Retrowave VHS`: `isPlaying=True`, `loop=True`, volume 0.70
- [x] 전체 저장 및 문서 정리 확인
  - `SaveAllProjectState.cs`로 Unity AssetDatabase와 열린 씬 전체 저장 실행
  - `Log.md` / `Task.md`에 최근 작업 내용 정리 여부 확인
- [x] GitHub private 저장소 연결 및 초기 업로드
  - 원격 저장소: `https://github.com/ssa25879/VRRhythmGame.git`
  - Unity용 `.gitignore` / `.gitattributes` 추가
  - `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `Backup/`, `Assets/Screenshots/` 등 제외 확인
  - 초기 프로젝트 커밋 및 원격 `main` push 완료
- [x] README 설명문 수정 및 GitHub 업로드
  - 프로젝트 소개, 현재 구현 내용, 개발 환경, 제외 대상 설명 추가
  - 커밋 `5426b44 Update README project description` push 완료
- [x] `AGENTS.md` 지침 파일 생성
  - 한국어 응답, 문서 관리, 외부 작업 로그, 백업 정책, Unity 작업 규칙, Git/GitHub 관리 지침 정리
  - 다른 AI와 번갈아 작업할 때도 루트 지침 파일로 확인 가능하도록 구성
- [x] README에 작업 지침 문서 안내 추가
  - 클론 후 `AGENTS.md`, `Task.md`, `Log.md`를 먼저 확인하도록 안내
- [x] README에 AI 도구 활용 표기 추가
  - 개발 과정에서 AI 도구를 사용해 코드 수정, Unity 에디터 작업 보조, 작업 로그 정리, GitHub 관리 작업을 진행하고 있음을 표기
- [x] `AGENTS.md` 경로 지침 보정
  - 다른 환경에서 클론해도 사용할 수 있도록 절대 경로 대신 저장소 루트 기준 상대 경로 우선으로 수정
  - 현재 PC 경로와 외부 작업 로그 경로는 로컬 예시/현재 환경 기준으로 정리
- [x] 테스트 플레이 영상 캡처
  - Game 씬 Play Mode 기준 10초 분량 100프레임 캡처
  - GIF 미리보기 생성: `Assets/Screenshots/TestPlayVideo/test_play_preview.gif`
- [x] Intro 씬 PC 클릭 테스트 가시성 수정
  - PC Game View에서 UI가 보이지 않던 원인을 Canvas 높이 문제로 확인
  - Intro Canvas 위치를 PC 카메라 시야 안으로 조정
  - Canvas `worldCamera`를 `Main Camera`로 지정
  - Play Mode 캡처로 UI 표시 확인
- [x] Intro 씬 스테이지 변경/음악 확인 및 시각 피드백 보강
  - StageList, 버튼 이벤트, BGM AudioSource, AudioListener 연결 확인
  - IntroManager에 skybox/grid 적용 로직 추가
  - `StageGridPreview`를 생성해 Retrowave 스테이지별 바닥 그리드 표시
  - thumbnail 이미지가 없는 스테이지는 색상 패널로 구분되도록 수정
  - Play Mode에서 About That Oldie, Retrowave Vapor, Retrowave Orange 화면 변화와 BGM 전환 로그 확인
- [x] 불필요 백업 검토 및 삭제
  - 완전 중복 Intro 씬 백업 1개와 `.meta` 삭제
  - 깨진 에셋 격리 백업 폴더 `Backup/AssetImports/20260520_ProjectileFactoryBrokenIntegration/` 삭제
- [x] 음악 생성 프롬프트 정리 및 BPM 기반 노트 스폰 보강
  - `D:\Codex\VRBeatSaber_MusicPrompts.md`에 Retrowave 3종 음악 생성 프롬프트 작성
  - 프롬프트에 2분~2분 30초 길이 조건과 30초 preview/sample 금지 조건 추가
  - `Spawner.cs`를 BGM AudioSource 재생 시간 기준 BPM 동기화 방식으로 수정
  - Retrowave Vapor 124 BPM 기준 `beatDuration=0.484` 로그 확인
  - Game 씬 Play Mode에서 BGM 재생 및 노트 생성 확인
- [x] About That Oldie 기본 스테이지 동작 확인
  - `SelectedStage=0`에서 Game 씬 Play Mode 실행
  - `About That Oldie - Vibe Tracks` 재생 확인
  - BGM 길이 `114.08`, `isPlaying=True`, `loop=True`, `volume=0.70` 확인
  - BPM 동기화 `beatDuration=0.500`, 런타임 노트 `RED=27`, `BLUE=27` 확인
- [x] Game 씬 노래 종료 후 Intro 복귀 로직 추가
  - 기존에는 노래 종료 후 선택 화면 복귀 로직이 없고 BGM loop가 강제되어 있었음
  - `GameBackgroundController`의 BGM loop를 비활성화
  - `GameSongEndController`를 추가해 곡 종료 후 `Intro` 씬으로 복귀하도록 구현
  - 곡 종료 후 `Spawner`가 추가 노트를 만들지 않도록 보강
  - Retrowave Vapor 임시 BGM으로 종료 후 Intro 복귀 확인
- [x] DOTween 임포트 전 에디터 오류 원인 확인 및 XR 인터랙터 정리
  - 현재 프로젝트에는 DOTween 임포트 흔적 없음
  - 에디터 Error 원인은 Intro 씬의 중복 XR UI 인터랙터 pointer 등록 고갈로 확인
  - 수정 전 백업 생성: `Assets/Scenes/Backup/Intro_backup_20260522_before_xr_pointer_registration_fix.unity`
  - 컨트롤러 UI용 `Left_NearFarInteractor`, `Right_NearFarInteractor`는 유지
  - 중복 `XRPokeInteractor`, 기본 `Near-Far Interactor`, `Teleport Interactor` UI Interaction 비활성화
  - 수정 후 Intro Play Mode 진입/종료에서 신규 Error 없음
- [x] BGM 후보 문서화
  - 다른 환경에서도 확인 가능하도록 `Docs/BGM_Candidates.md` 생성
  - 스테이지별 후보 URL, 라이선스 확인 포인트, 적용 메모 정리
  - `README.md` 작업 지침 문서 목록에 BGM 후보 문서 추가
- [x] Score / Combo / HP / Miss 시스템 1차 구성
  - `GameScoreController.cs` 추가
  - 곡별 최대 점수 `100,000` 기준으로 `70% 기본 Hit 점수 + 30% 콤보 성장 점수` 배분
  - 전체 노트 성공 시 이론상 `100,000점`에 도달하도록 예상 노트 수와 콤보 누적 가중치 기반 계산
  - Hit 시 Score/Combo 증가, Miss/Bad Cut 시 Combo 초기화 및 HP 감소
  - 노트를 놓치면 `MISS`, 방향이 틀린 타격은 `BAD CUT`으로 처리
  - Game 씬에 런타임 Score/Combo/HP/Miss HUD 자동 생성
  - Unity 컴파일 에러 없음, Game 씬 연결 확인
- [x] BAD 판정 튜닝
  - 방향이 틀린 타격 판정명을 `BAD CUT`에서 `BAD`로 변경
  - `BAD`는 콤보를 초기화하지 않음
  - `BAD` HP 감소량은 `MISS`의 약 1/3로 설정
- [x] 판정 Play Mode 검증
  - Score 상태 초기화 후 Hit/BAD/MISS 단위 판정 검증 완료
  - `Hit`: 점수 증가, 콤보 증가, HP 유지
  - `BAD`: 콤보 유지, HP 약 `4` 감소
  - `MISS`: 콤보 `0` 초기화, HP `12` 감소
  - 결과: `hitOk=True`, `badOk=True`, `missOk=True`, `damageRatioOk=True`
- [x] 노트 방향 표시 확인 및 가시성 수정
  - 기존 캡처에서 방향 화살표가 glow와 배치 문제로 거의 보이지 않음을 확인
  - 방향 화살표를 노트 전면으로 이동하고 크게 조정
  - 성공 판정 방향을 시각 화살표 방향과 같은 `note.up` 기준으로 변경
  - 수정 후 캡처에서 방향 표시 확인 가능
- [x] 수직 HP바 추가
  - 기존 숫자 HP 표기는 유지
  - 오른쪽에 세로 HP바 추가
  - HP 상태에 따라 초록/노랑/빨강으로 표시
  - 35% 경고선을 추가해 위험 구간을 확인할 수 있도록 구성
  - Play Mode 캡처로 표시 확인: `Assets/Screenshots/test_play_game.png`
- [x] 직접 테스트 전 진단 스크립트 정리
  - 판정 테스트용 `RunJudgementPlaytest.cs`, `CheckJudgementInPlayMode.cs`, `DiagnoseScoreControllerRuntime.cs` 제거
  - 직접 Play Mode 테스트 중 Score/HP 상태를 강제로 바꾸는 코드가 남지 않도록 정리
  - Unity Play Mode 꺼짐, 컴파일 에러 없음 확인
- [x] 테스트 플레이용 Game BGM 음소거
  - `GameBackgroundController.muteBgmForTesting=true` 적용
  - BGM AudioSource는 재생하되 mute 처리해 노트 스폰/곡 종료 타이밍은 유지
  - 소리만 나오지 않도록 설정
- [x] Intro 씬 Mute 토글 버튼 추가
  - Intro Canvas에 `MuteButton` 추가
  - `IntroManager.ToggleMute()` 연결
  - 버튼 텍스트는 `MUTE ON` / `MUTE OFF`로 표시
  - `BgmMuted` PlayerPrefs로 Intro/Game 씬 BGM mute 상태 공유
  - 현재 기본값은 테스트 환경에 맞춰 `MUTE ON`
- [x] 테스트 피드백 반영 - UI 높이, 색상 판정, 이펙트, HUD 조정
  - Intro Canvas Y 위치를 `-0.22`로 낮춤
  - RED/BLUE 노트 prefab 레이어와 세이버 LayerMask를 재확인하고 같은 색상끼리 히트하도록 정리
  - 히트 이펙트 크기 `0.18`, 유지시간 `0.55초`로 축소
  - Game HUD 위치를 오른쪽 아래로 이동하고 전체 크기/스케일 축소
  - Unity Play Mode 꺼짐, 컴파일 에러 없음 확인
- [x] 테스트 피드백 반영 - HUD 분리 배치 및 블레이드 색상 수정
  - Score HUD를 중앙 상단에 배치
  - Combo HUD를 좌측 중앙에 배치
  - HP HUD를 우측 중앙에 배치
  - HUD를 카메라 하위 Canvas 3개로 분리 생성
  - TMP Bold와 outline을 적용해 폰트 가독성 보강
  - 블레이드 시각 색상을 오브젝트 이름이 아니라 실제 판정 LayerMask 기준으로 결정하도록 수정
  - Unity Play Mode 꺼짐, 컴파일 에러 없음 확인
- [x] 현재 보유 에셋 기반 HUD 스타일 적용
  - `VRTemplateAssets`의 둥근 패널/외곽선/세로 UI 스프라이트와 Inter TMP 폰트를 활용
  - Score/Combo/HP HUD에 반투명 패널과 색상 외곽선 적용
  - Game 씬 `GameScoreController`에 HUD 에셋 참조 저장
  - Unity 컴파일 에러 없음, 현재 작업 관련 신규 Error 없음 확인
- [x] 테스트 피드백 반영 - 컨트롤러 Ray, 세이버 색상, 결과 화면
  - Intro 씬에서 메뉴용 컨트롤러 Ray가 머리 위/Teleport Ray처럼 보이지 않도록 `Teleport Interactor` 비활성화
  - `Left_NearFarInteractor`, `Right_NearFarInteractor`는 활성 유지하고 Ray 원점을 컨트롤러 Transform 기준으로 재연결
  - 세이버 시각 색상을 실제 판정 LayerMask 기준으로 재갱신하도록 수정
  - 곡 종료 시 결과 화면을 표시하고, `OK` 버튼으로 Intro 씬으로 이동하도록 구현
  - Unity 컴파일 에러 없음 확인
- [x] 결과 화면 OK 입력 보강
  - OK 버튼 클릭 외에 PC 키보드 `Enter` / `Space`로 Intro 복귀 가능
  - Quest 컨트롤러 `triggerButton` / `primaryButton`으로 Intro 복귀 가능
  - 결과 화면 표시 직후 누르고 있던 입력이 바로 처리되지 않도록 입력 해제 후 재입력 방식으로 보강
  - Unity 컴파일 에러 없음 확인
- [x] HP바 소모 표시 및 HP 0 실패 처리 보강
  - HP바 채움 높이가 HP 비율에 따라 실제로 줄어들도록 수정
  - HP 0 이하에서 스포너 비활성화, BGM 정지, 남은 노트 제거
  - HP 0 실패 시 `FAILED` 결과 화면 표시
  - Unity 컴파일 에러 없음 확인
- [x] Game 씬 UI 오브젝트 기반 관리 구조로 전환
  - `Game UI Root` 아래에 `Score HUD`, `Combo HUD`, `HP HUD`, `Result HUD`를 실제 씬 오브젝트로 생성
  - `GameScoreController`가 런타임 UI를 생성하지 않고, 씬 오브젝트 참조를 갱신하도록 수정
  - 게임 중에는 Score/Combo/HP HUD 활성화, 결과 화면에서는 해당 HUD 비활성화 후 `Result HUD` 활성화
  - `ResultOkButton`도 Game 씬 오브젝트로 생성해 직접 위치/크기/스타일 수정 가능
  - Unity 컴파일 에러 없음 확인
- [x] Game 씬 결과 화면 컨트롤러 OK 입력 보강
  - Game 씬 양쪽 컨트롤러에 UI용 `NearFarInteractor` 추가
  - 양쪽 컨트롤러에 `VisibleUIPointer` 추가
  - Result HUD에 컨트롤러 Ray가 닿도록 목표 평면 연결
  - Unity 컴파일 에러 없음 확인
- [x] Ray 표시 모드 전환, Intro Ray 원점, 초기 노트 겹침 수정
  - Game 씬에서 게임 중에는 Result UI Ray/Pointer 비활성화
  - 결과 화면에서는 Result UI Ray/Pointer 활성화, 세이버 컴포넌트/시각물 비활성화
  - Intro 씬 Ray Origin을 헤드가 아니라 각 컨트롤러 Transform 기준으로 재연결
  - `Spawner.firstSpawnBeat=1`로 시작 직후 노트 겹침 완화
  - Unity 컴파일 에러 없음 확인
- [x] Game 씬 스포너 높이 조정
  - 플레이어 시점에서 노트가 낮게 들어오는 문제를 완화하기 위해 `Spawner` 로컬 Y 위치를 `1.0`에서 `1.45`로 조정
  - 전체 스폰 포인트가 함께 상승하도록 `Spawner` 본체 위치 기준으로 수정
- [x] **Intro 씬 XR UI Ray 원점 완전 정리** (2026-05-27)
  - 구형 `Near-Far Interactor` (Left Controller / Right Controller 하위) — GO 비활성화
  - 손 추적용 `Near-Far Interactor` (Left Hand / Right Hand 하위) — GO 비활성화
  - `Left Controller Teleport Stabilized Origin`, `Right Controller Teleport Stabilized Origin` — GO 비활성화
  - Gaze Interactor, Gaze Stabilized, Left/Right Teleport Interactor — 이미 비활성 확인
  - `Left_NearFarInteractor`, `Right_NearFarInteractor`, 좌/우 `VisibleUIPointer` 활성 유지 확인
  - `VisibleUIPointer.rayOrigin` → 각 컨트롤러 Transform ✓
  - `NearFarInteractor.castOrigin` → 카메라 아닌 컨트롤러 기준 ✓
  - `FixIntroRayOrigin.cs` Editor 스크립트로 일괄 처리 및 씬 저장 완료
  - 백업: `Assets/Scenes/Intro.unity.bak`
  - Unity 컴파일 에러 없음 확인
- [x] Intro 씬 컨트롤러 Pointer Ray 방향 보강
  - `ControllerPointerVisualizer`가 `rayOrigin.forward`만 사용하지 않고, Intro UI 평면이 연결되어 있으면 컨트롤러 위치에서 UI 중심 방향으로 Ray를 그리도록 수정
  - 컨트롤러 루트 forward 축 때문에 Ray가 헤드/정면 기준처럼 느껴지는 상황을 완화
  - Unity 컴파일 에러 없음 확인
- [x] Intro 씬 보조 Pointer 선 비활성화
  - 기본 XR Ray가 정상으로 보이는 상태에서 정중앙에 고정된 보조선이 남아 있어 좌/우 `VisibleUIPointer` 비활성화
  - `Video`, `Cube (1)` 등 다른 오브젝트 활성 상태 유지 확인
  - Unity 컴파일 에러 없음 확인
- [x] Intro 씬 기본 XR Ray 복구
  - 보조선 `VisibleUIPointer`는 비활성 유지
  - 좌/우 `Left_NearFarInteractor` / `Right_NearFarInteractor` GameObject 활성화
  - Unity 컴파일 에러 없음 확인
- [x] Intro/Game 씬 카메라 정적 점검
  - Ray가 헤드 기준처럼 보인 원인이 Ray 자체가 아니라 HMD 기준 카메라가 아닌 일반 카메라 시점 사용 가능성으로 정리됨
  - 파일 기준 `Assets/Scenes/Intro.unity`에는 별도 일반 Camera가 직렬화되어 있지 않고, XR Origin 프리팹의 Camera(`fileID: 300037366`)를 Canvas가 참조함
  - 파일 기준 `Assets/Scenes/Game.unity`도 XR Origin 프리팹의 Camera(`fileID: 441087510`)를 UI Canvas들이 참조함
  - Unity Play Mode 꺼짐, 컴파일 에러 없음 확인
- [x] Intro/Game 씬 XR 카메라 참조 보정 실행
  - `Assets/Scripts/Editor/FixXRCameraReferences.cs` 추가
  - Intro 씬 기준 카메라를 `XR Origin Hands (XR Rig)/Camera Offset/Main Camera`로 확인 및 보정
  - Game 씬 기준 카메라를 `XR Origin (XR Rig)/Camera Offset/Main Camera`로 확인 및 보정
  - Screen Space Overlay가 아닌 Canvas의 `worldCamera`를 XR Origin 카메라로 재지정
  - XR Origin 카메라를 `MainCamera` 태그/AudioListener 기준으로 정리
  - Unity 컴파일 에러 없음 확인
- [x] README 현재 완성도 문구 추가
  - 프로토타입 기준 핵심 플레이 흐름 약 70% 구현 상태로 표기
  - 완료/부분 완료/확인 필요/개선 예정 항목 정리
- [x] 커밋 전 ignore 규칙 보강
  - `Assets/_Recovery/`, `Assets/_Recovery.meta`, `*.unity.bak`, `*.unity.bak.meta` 제외
- [x] 세이버 중앙 상시 VFX 제거
  - Game 씬 좌/우 세이버 하위 `Saber Weapon VFX` 프리팹 인스턴스 제거
  - `ApplyNeonSaberVisuals` 재실행 시 `FX_Weapon Effect`가 다시 붙지 않도록 로직 제거
  - Unity 컴파일 에러 없음 확인
- [x] Intro 씬 HMD 카메라 추적 보정
  - `XR Origin Hands (XR Rig)/Camera Offset/Main Camera`에 Input System `Tracked Pose Driver` 활성화
  - `XRI Default Input Actions`의 `XRI Head/Position`, `XRI Head/Rotation`, `XRI Head/Tracking State` 참조 연결
  - Intro UI Canvas의 `worldCamera`를 XR Origin 하위 Main Camera로 재연결
  - Unity 컴파일 에러 없음 확인
- [x] Intro 씬 Sci-Fi UI 스타일 적용
  - `Sci-fi GUI skin` 무료 에셋 기반으로 Intro Canvas 패널/버튼/화살표/텍스트 색상 스타일 적용
  - `SciFiMenuPanel`, `SciFiTitleGlow`, `SciFiSubtitle`, `SciFiTopAccent`, `SciFiBottomAccent` 추가
  - `StartButton`, `MuteButton`, `PrevButton`, `NextButton`, `ThumbnailBG`, `TitleText`, `StageNameText` 위치/색상/스프라이트 정리
  - 적용 결과 캡처: `Assets/Screenshots/IntroSciFiUI_20260527.png`
  - Unity 컴파일 에러 없음 확인
- [x] Intro 씬 UI 크기 확대 조정
  - Canvas 내부 기준 크기를 `1120x760`으로 확대
  - 제목, 스테이지명, 썸네일, 시작/음소거/이전/다음 버튼 크기와 위치 재배치
  - Mute 버튼을 안쪽으로 조정해 우측 가장자리 겹침 위험 완화
  - 적용 결과 캡처 갱신: `Assets/Screenshots/IntroSciFiUI_20260527.png`
  - Unity 컴파일 에러 없음 확인
- [x] Intro 씬 UI 추가 확대 조정
  - 기존 확대 UI 기준으로 Canvas 내부 크기와 주요 UI 요소를 약 1.3배 추가 확대
  - 제목, 스테이지명, 썸네일, 시작/음소거/이전/다음 버튼 위치와 크기 재배치
  - 적용 결과 캡처 갱신: `Assets/Screenshots/IntroSciFiUI_20260527.png`
  - Unity 컴파일 에러 없음 확인
- [x] 노트 프리팹 런타임 비주얼 동기화
  - `Assets/Prefab/RED.prefab`, `Assets/Prefab/BLUE.prefab`에 실제 플레이 중 보이던 프레임/방향 화살표/글로우 구성을 직접 추가
  - 기존 `Sphere` 렌더러는 실제 런타임과 동일하게 비활성화
  - `Cube.cs`는 프리팹에 비주얼이 이미 있으면 중복 생성하지 않도록 수정
  - `ApplyRuntimeNoteVisualToPrefabs.cs` 추가
  - Unity 컴파일 에러 없음 확인
- [x] 노트 방향 가시성 개선
  - RED/BLUE 프리팹의 전면 방향 화살표를 더 굵게 확대
  - 뒤쪽에서도 방향을 확인할 수 있도록 Back 방향 화살표 추가
  - 흰색 `Direction Guide`를 추가해 휘두를 방향 중심선을 더 명확하게 표시
  - `Cube.cs` fallback 비주얼과 `ApplyRuntimeNoteVisualToPrefabs.cs` 재적용 스크립트도 동일 구조로 수정
  - Unity 컴파일 에러 없음 확인
- [x] 노트 방향 마커 꺾쇠형 개선
  - Y자처럼 보이던 `Cut Arrow` / `Direction Guide` 구조 제거
  - RED/BLUE 프리팹에 `Direction Chevron Left/Right`, `Direction Chevron Left/Right Back` 추가
  - 기본 방향은 위쪽 꺾쇠이며, 스폰 시 90도 회전으로 좌/우/아래 방향도 명확히 보이도록 유지
  - `Cube.cs` fallback 비주얼과 `ApplyRuntimeNoteVisualToPrefabs.cs` 재적용 스크립트도 동일 구조로 수정
  - Unity 컴파일 에러 없음 확인
  - 커밋 전 RED/BLUE 프리팹에 꺾쇠형 방향 마커가 저장되어 있고 기존 Y자형 마커가 제거됨 재확인
- [~] Quest 3S 실기에서 HMD 기준 카메라 적용 확인
  - 실제 플레이 시 렌더링 기준이 `XR Origin Hands (XR Rig)` 하위 HMD/Main Camera인지 확인 필요
  - Intro 씬에서 HMD 움직임에 따라 카메라 위치/회전이 따라오는지 실기 확인 필요
  - 일반 Camera가 Hierarchy에 활성 상태로 남아 있거나 `MainCamera` 태그/Audio Listener를 점유하는지 실기/에디터 Hierarchy에서 최종 확인 필요
- [~] Quest 3S 실기에서 Intro Sci-Fi UI 체감 확인
  - UI 크기, 버튼 가독성, Ray 입력 영역이 실제 착용 상태에서 편한지 확인 필요
- [ ] 노트 방향 표시 크기/두께 체감 튜닝
  - 현재 수정 후 화살표는 잘 보이지만 다소 크게 느껴질 수 있음
  - Quest 3S 실기에서 접근 거리 기준으로 크기/두께 조정 필요
- [ ] Score / Combo / HP / Miss 체감 튜닝
  - Miss HP 감소량 `12`, Bad HP 감소량 약 `4`, 방향 판정 허용 각도 `85도`, 노트 miss 시간 `8초` 실기/플레이 체감 기준 재조정 필요
  - 결과 화면과 HP 0 실패 처리는 구현 완료, Rank 표시는 아직 미구현
- [~] Quest 3S에서 Game 씬 세이버 시각 효과, 노트 판정, Intro 로딩 VFX 실기 확인
  - 현재 실기 테스트 불가로 대기
- [~] Quest 3S에서 `Retrowave Vapor` 스테이지의 바닥 높이, 그리드 밀도, 하늘 밝기 실기 확인
  - 현재 실기 테스트 불가로 대기
- [~] Quest 3S에서 Retrowave 3종 스테이지의 배경 가독성, BGM 볼륨, 노트 판정 체감 확인
  - 현재 실기 테스트 불가로 대기
## 2026-05-28 현재 테스트 대기 항목

- [ ] BGM별 노트 속도/스폰 밀도 실기 체감 확인
  - About That Oldie: noteSpeed 3.2, beatsPerSpawn 2.0
  - Retrowave Vapor: noteSpeed 3.4, beatsPerSpawn 2.0
  - Retrowave Orange: noteSpeed 3.0, beatsPerSpawn 2.0
  - Retrowave VHS: noteSpeed 3.0, Beat Sage minBeatGap 1.0
- [ ] Play Mode 자동 테스트는 BGM 뮤트 상태로 진행
- [ ] Quest 3S 실기 테스트에서 노트 속도, 스폰 밀도, 판정 체감 재확인

## 2026-05-28 Beat Sage Retrowave VHS 테스트 대기 항목

- [x] Beat Sage ZIP 파일을 `Assets/Audio/StageNote/RetrowaveVHS_BeatSage/`에 정리
- [x] Retrowave VHS용 Beat Sage Normal 차트 에셋 생성
- [x] Retrowave VHS 스테이지를 Beat Sage `song.ogg`, BPM 120, 274개 노트 차트로 연결
- [x] 난이도 완화를 위해 Retrowave VHS Beat Sage 노트를 최소 1비트 간격으로 필터링
  - 현재 예상 플레이 노트: 114개
- [ ] Unity Editor에서 AssetDatabase Refresh 후 스크립트 컴파일 오류 여부 확인
- [ ] Play Mode에서 BGM muted 상태로 Retrowave VHS 차트 스폰 확인
  - 첫 노트가 약 11.1초 지점부터 시작하므로 최소 14초 이상 확인 필요
- [ ] Quest 3S 실기에서 노트 밀도, 방향, 좌우 레인 체감 확인

## 2026-05-28 세이버 히트 판정 확인 대기 항목

- [x] 칼끝 중심 판정을 칼날 전체 스윕 판정으로 수정
- [x] 양손 세이버 판정 반경/최소 속도/방향 허용각 재조정
  - 현재 테스트값: hitRadius 0.22, minSwingSpeed 0.65, directionTolerance 50
- [x] 정지 접촉 상태가 HIT로 처리되지 않도록 현재 칼날 접촉 검사를 제거하고 프레임 간 스윕만 검사
- [x] 잘못된 방향 접촉 시 첫 대상만 BAD 처리하도록 조정
- [x] 양손 `Saber.bladeRoot`를 실제 `Blue Neon Blade` / `Red Neon Blade` Transform에 직접 연결
- [x] 블레이드 기준점을 못 찾으면 컨트롤러 중심으로 판정하지 않도록 `Saber.cs` 보정
- [ ] Unity Editor에서 스크립트 재컴파일 확인
- [ ] Quest Link/Air Link 또는 Editor Play Mode에서 노트 히트/HUD HIT 증가 확인
- [ ] 방향을 일부러 반대로 휘둘렀을 때 BAD가 증가하는지 확인

## 2026-05-28 세이버 파티클/트레일 확인 대기 항목

- [x] 양손 세이버 칼날 TrailRenderer 비활성화
- [x] 양손 세이버 히트 성공 이펙트는 다시 활성화
- [x] 런타임에서 TrailRenderer가 다시 켜지지 않도록 `Saber.cs` 보정
- [ ] Unity Editor 재컴파일 확인
- [ ] Quest Link/Air Link에서 칼 중앙 파티클/잔상이 사라졌는지 확인

## 2026-05-28 Result 화면 Ray 제거 확인 대기 항목

- [x] Result 화면 표시 목록에서 `VisibleUIPointer`, `Left_NearFarInteractor`, `Right_NearFarInteractor` 제외 처리
- [x] Result 모드 진입 시점에만 해당 Ray 오브젝트를 비활성화하도록 보정
- [x] Game 씬의 양쪽 `VisibleUIPointer` / `ControllerPointerVisualizer` 비활성화
- [ ] Unity Editor 재컴파일 확인
- [ ] Play Mode에서 결과 화면 중앙에 Ray 2개가 남지 않는지 확인

## 2026-05-28 Play 화면 세이버 중복 표시 확인 대기 항목

- [x] `GameScoreController`의 Play 모드 오브젝트 자동 수집 범위를 현재 활성 세이버/Renderer로 제한
- [ ] Unity Editor 재컴파일 확인
- [ ] Play Mode에서 하늘에 떠 있는 추가 세이버 2개가 사라졌는지 확인

## 2026-05-28 세이버 블레이드 길이/각도 체감 확인 대기 항목

- [x] 블레이드 런타임 길이 축소
  - bladeLocalScale z: 1.42 -> 1.08
  - bladeTipOffset: 0.72 -> 0.58
- [x] 블레이드가 손잡이에서 떨어져 보이지 않도록 접합 위치 재조정
  - bladeLocalPosition: `{x: 0, y: 0.025, z: 0.52}`
- [x] 블레이드를 위쪽으로 약 18도 기울이도록 조정
  - bladeLocalEulerAngles x: -18
- [x] 판정 구간을 블레이드 중간~끝으로 제한
  - hitBladeStart: 0.18, hitBladeEnd: 1.0
- [x] 찌르기보다 휘두르기 판정에 가깝도록 블레이드 방향과 거의 평행한 움직임 제외
  - minSwingToBladeAngle: 32
- [ ] Unity Editor 재컴파일 확인
- [ ] Play Mode에서 칼을 든 느낌, 손 뒤쪽 블레이드 길이, 찌르는 느낌 완화 여부 확인

## 2026-05-28 VHS 스테이지 최신 확인 대기 항목

- [x] Retrowave VHS 노트 속도 1.4배 증가 반영
  - noteSpeed: `3.0` -> `4.2`
- [x] 클리어 Result 조건 반영
  - BGM 종료
  - 노트 출력 완료
  - HP > 0
  - 조건 충족 후 clearResultDelay `3`초 대기
  - Spawner 참조가 없으면 조건 확인 불가로 Result 예약하지 않음
- [x] 세이버 블레이드 일체형 재조정
  - bladeLocalPosition: `{x: 0, y: 0, z: 0.61}`
  - bladeLocalEulerAngles: `{x: 0, y: 0, z: 0}`
  - hitBladeStart: `0.3`, hitBladeEnd: `1.0`
  - minSwingToBladeAngle: `45`
- [ ] Unity Editor 재컴파일 확인
- [ ] Play Mode에서 Retrowave VHS BGM muted 상태로 노트 속도와 Result 출력 타이밍 확인
- [ ] Quest 3S 실기에서 블레이드가 손잡이와 정확히 붙어 보이는지 확인
- [ ] Quest 3S 실기에서 휘두르기 판정 체감 재확인

## 2026-05-28 Miss / Combo / HUD 최신 확인 대기 항목

- [x] Miss 판정을 시간초과 기준에서 플레이어 뒤 좌표 통과 기준으로 변경
  - 기본 기준점: `Camera.main.transform`
  - 뒤 통과 거리: `0.6m`
- [x] Miss 발생 시 기존 `RegisterMiss()` 흐름으로 Combo `0` 초기화 유지
- [x] Combo가 0일 때도 `0 COMBO`로 표시
- [x] HP/Combo HUD 위치와 네온 패널 톤을 런타임 보정
- [ ] Unity Editor 재컴파일 확인
- [ ] Play Mode에서 노트가 플레이어 뒤로 지나가면 Miss + Combo 0 처리되는지 확인
- [ ] Play Mode/Quest 3S에서 HP/Combo HUD 가독성과 Intro UI와의 톤 차이 확인

## 2026-05-28 Hit HP 회복 / HUD 피드백 확인 대기 항목

- [x] Hit 성공 시 HP 회복 추가
  - 추천/적용값: `0.35 * Combo`
  - Hit 1회 최대 회복: `6`
  - maxHp 초과 회복 방지
- [x] HP 회복량 HUD 표시 추가
  - 예: `HP 84  +2.1`
  - 표시 시간: `0.75`초
- [x] 회복 중 HP 바 민트색 강조
- [x] Combo 수에 따라 Combo 텍스트 네온 톤 변화
- [ ] Unity Editor 재컴파일 확인
- [ ] Play Mode에서 Hit 시 HP 회복량과 회복 표시 확인
- [ ] 실기에서 회복량이 너무 후하거나 부족한지 체감 확인

## 2026-05-28 Game 씬 UI Sci-Fi 비주얼 확인 대기 항목

- [x] Intro UI와 동일 계열 Sci-Fi GUI skin 에셋 확인
  - `window_transparent.png`
  - `button_active.png`
  - `button_pushed.png`
- [x] Game HUD Score / Combo / HP 패널을 어두운 Sci-Fi 패널 톤으로 보정
- [x] Result HUD 패널/텍스트/OK 버튼을 Intro UI 계열로 보정
- [x] Game UI 재생성용 `BuildGameSceneUiObjects`도 Sci-Fi 스타일 기준으로 수정
- [x] `Game.unity` GameScoreController에 Sci-Fi 스프라이트 참조 연결
- [ ] Unity Editor 재컴파일 확인
- [ ] Play Mode에서 Game HUD와 Result HUD가 Intro UI와 톤이 맞는지 확인
- [ ] Quest 3S 실기에서 HP/Combo/Result UI 가독성 확인

## 2026-05-28 Game 씬 UI 실제 오브젝트 재확인 대기 항목

- [x] `Game.unity` 실제 UI 패널/아웃라인 Image 스프라이트가 Sci-Fi `window_transparent.png` 계열로 교체되었는지 재확인
- [x] Result OK 버튼의 highlighted/pressed/selected 스프라이트가 Sci-Fi 버튼 에셋으로 연결되었는지 재확인
- [x] HP 바 배경/Fill이 Sci-Fi bar 에셋으로 교체되었는지 재확인
- [x] 빈 `m_LocalScale`, 빈 `m_text` 직렬화 값 없음 확인
- [ ] Unity Editor에서 Game 씬을 리로드/포커스 갱신한 뒤 HUD와 Result UI가 실제로 바뀌어 보이는지 확인
- [ ] Play Mode에서 Game HUD, HP 바, Result OK 버튼 시각 상태 확인

## 2026-05-28 Game 씬 UI 텍스트 이탈 확인 대기 항목

- [x] Game HUD / Result HUD 텍스트 RectTransform 크기 축소
- [x] 실제 `Game.unity` TMP 자동 크기 조절 활성화
- [x] 텍스트 overflow Ellipsis 적용
- [x] `GameScoreController` 런타임 보정 값 갱신
- [x] `BuildGameSceneUiObjects` 재생성 기준 갱신
- [ ] Unity Editor에서 Game 씬 리로드 후 텍스트가 패널 내부에 들어오는지 확인
- [ ] Play Mode에서 Score / Combo / HP / Result 화면 텍스트 잘림 여부 확인

## 2026-05-28 Game 씬 UI 패널 세로 여백 확인 대기 항목

- [x] Score / Combo / HP / Result HUD 패널 높이 확대
- [x] 패널 아웃라인 높이 동시 확대
- [x] 런타임 패널 크기 보정 로직 추가
- [x] UI 재생성 스크립트 기준 값 갱신
- [ ] Unity Editor에서 Game 씬 리로드 후 패널 위아래 여백 확인
- [ ] Play Mode에서 텍스트와 패널 스프라이트가 자연스럽게 맞는지 확인

## 2026-05-28 Game 씬 UI 패널 추가 확대 / Score 가시성 확인 대기 항목

- [x] Score / Combo / HP / Result HUD 패널 높이 추가 확대
- [x] 패널 아웃라인 높이 동시 확대
- [x] Score 패널 배경/아웃라인 대비 강화
- [x] Score 텍스트 크기, 박스, 색상 보정
- [x] 런타임 보정 및 UI 재생성 기준 값 갱신
- [ ] Unity Editor에서 Game 씬 리로드 후 위아래 여백과 Score 가시성 확인
- [ ] Play Mode에서 VR 시야 기준 Score / Combo / HP 위치와 가독성 확인

## 2026-05-29 main 브랜치 업로드 전 현재 작업 상태

### 완료된 주요 작업
- [x] Intro 씬 스테이지 선택 및 Game 씬 전환
- [x] Retrowave Vapor / Orange / VHS 스테이지 구성
- [x] Retrowave VHS Beat Sage 노트 맵 적용
- [x] 노트 색상별 세이버 판정
- [x] 방향 판정, 휘두르기 속도 판정, 세이버 판정 범위 조정
- [x] 플레이어 뒤쪽 통과 기준 Miss 판정 및 Combo 0 처리
- [x] Hit 성공 시 Combo 기반 HP 회복
- [x] 곡 종료, 노트 출력 완료, HP 조건 기반 Result 화면 출력
- [x] Result 화면 Ray/포인터 잔존 문제 보정
- [x] Game HUD / Result UI Sci-Fi 스타일 보정
- [x] Score / Combo / HP / Result 패널 크기 및 Score 가시성 보정
- [x] README / Log / Task 현재 상태 정리
- [x] 테스트 시연 영상 링크 기록: https://youtu.be/3g2RF_wvLGw

### 남은 확인 항목
- [ ] Quest 3S 실기 기준 장시간 플레이 안정성 확인
- [ ] Quest 3S 실기 기준 UI 가독성 확인
- [ ] Retrowave VHS 난이도, 노트 속도, BGM 볼륨 체감 확인
- [ ] 유튜브 설명란에 사용 에셋 및 출처 표기 유지
- [ ] 필요 시 main 브랜치 기준 릴리즈/태그 정리

## 2026-05-29 포트폴리오 문서 정리

- [x] 포트폴리오 제출용 Markdown 문서 추가: `Docs/Portfolio.md`
- [x] `README.md`에서 포트폴리오 문서로 이동할 수 있도록 링크 추가
- [x] 포트폴리오 문서에 테스트 플레이 일부공개 영상 링크 포함: https://youtu.be/3g2RF_wvLGw
- [x] 게임 개요, 담당 역할, 수행 내용, 구현 기능, 개발 환경, 사용 에셋 및 향후 보완 사항 정리
- [ ] 포트폴리오 제출 플랫폼에 맞춰 문서 서식 최종 조정
