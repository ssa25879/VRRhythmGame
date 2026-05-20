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

## 현재 상태 (2026-05-20 기준)

| 항목 | 상태 |
|------|------|
| 컴파일 에러 | 없음 |
| 활성 씬 | `Assets/Scenes/Intro.unity` |
| 배경 렌더링 | **Skybox/Panoramic** (최적화 완료) |
| 기능 검증 | 스테이지 선택 및 씬 전환 연동 완료 |
| Intro UI 위치 | Canvas `y=-0.35`, `z=2.5` |
| 배경 구조 | 프로토타입식 `Background` 루트 + 검은 바닥 큐브 2개 적용 |
| XR UI 입력 | 좌/우 컨트롤러에 `NearFarInteractor` 추가 |
| 컨트롤러 조준 표시 | 컨트롤러 기준 `VisibleUIPointer` 레이, UI 평면까지 표시 |
| 스테이지 데이터 | `Assets/Data/StageList.asset`에 `About That Oldie` 1개 |
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
  - 2026-05-20 현재 상황: 사용자가 당장 Quest 3S 실기 테스트를 진행할 수 없어, 에디터에서 확인 가능한 범위만 검증 완료
  - 다음 확인: Quest 3S에서 UI 높이, 배경 구조, 컨트롤러 레이 표시, 버튼 클릭이 의도대로 동작하는지 재테스트

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
- [~] Quest 3S에서 Game 씬 세이버 시각 효과, 노트 판정, Intro 로딩 VFX 실기 확인
  - 현재 실기 테스트 불가로 대기
- [~] Quest 3S에서 `Retrowave Vapor` 스테이지의 바닥 높이, 그리드 밀도, 하늘 밝기 실기 확인
  - 현재 실기 테스트 불가로 대기
- [~] Quest 3S에서 Retrowave 3종 스테이지의 배경 가독성, BGM 볼륨, 노트 판정 체감 확인
  - 현재 실기 테스트 불가로 대기
