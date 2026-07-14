# VR Beat Saber — 작업 로그

## 2026-05-28 Retrowave VHS 뮤트 Play Mode 테스트

### 작업 시간
- 시작시간: 2026-05-28 21:03:00 +09:00
- 종료시간: 2026-05-28 21:08:53 +09:00

### 확인 내용
- [x] 프로젝트 내 Beat Sage `.zip` / `Info.dat` 파일 존재 여부 확인.
  - 현재 프로젝트에는 Beat Sage 맵 파일 없음.
- [x] `SelectedStage=3`으로 Retrowave VHS 선택.
- [x] `BgmMuted=1`로 BGM 뮤트 상태 설정.
- [x] Game 씬 Play Mode 단기 테스트 진행.
- [x] 테스트 후 Intro 씬으로 복귀.

### 결과
- Retrowave VHS 적용 확인: `stage=Retrowave VHS`, `bgm=Retrowave VHS`, `muted=True`.
- Spawner 적용 확인: `beatDuration=0.455`, `noteSpeed=3.60`, `beatsPerSpawn=2.00`.
- Score 예상 노트 수 로그 확인: `expectedNotes=134`.
- 콘솔에 Unity Editor Inspector/toolbar 관련 메시지가 표시됨. 현재 노트 튜닝 코드 직접 오류로 보이는 로그는 확인되지 않음.

## 2026-05-28 BGM별 노트 속도/스폰 밀도 튜닝 기능 추가

### 작업 시간
- 시작시간: 2026-05-28 20:54:38 +09:00
- 종료시간: 2026-05-28 20:58:28 +09:00

### 작업 내용
- [x] 수정 전 백업 생성.
  - `Backup/Scripts/20260528_before_stage_note_tuning/`
  - `Backup/Data/20260528_before_stage_note_tuning/`
- [x] `StageEntry`에 BGM별 튜닝값 추가.
  - `noteSpeed`
  - `beatsPerSpawn`
  - `spawnLeadBeats`
- [x] `Spawner`가 선택된 스테이지의 튜닝값을 적용하도록 수정.
- [x] `Cube`에 런타임 노트 속도 적용 메서드 추가.
- [x] 점수 예상 노트 수 계산이 스테이지별 `beatsPerSpawn`을 반영하도록 수정.
- [x] `StageList.asset`에 추천 초기값 반영.

### 적용값
- About That Oldie: noteSpeed 3.2, beatsPerSpawn 2.0
- Retrowave Vapor: noteSpeed 3.4, beatsPerSpawn 2.0
- Retrowave Orange: noteSpeed 3.0, beatsPerSpawn 2.0
- Retrowave VHS: noteSpeed 3.6, beatsPerSpawn 2.0

### 확인
- [x] Unity 컴파일 후 신규 컴파일 에러 없음 확인.
- [x] Game 씬 Play Mode 단기 테스트에서 `noteSpeed=3.20`, `beatsPerSpawn=2.00` 적용 로그 확인.
- [x] 테스트 후 Intro 씬으로 복귀.

## 2026-05-28 RED/BLUE 노트 프리팹 Unity 렌더 확인

### 작업 시간
- 시작시간: 2026-05-28 20:41:00 +09:00
- 종료시간: 2026-05-28 20:48:35 +09:00

### 확인 내용
- [x] Unity MCP 직접 세션 초기화 성공 확인.
- [x] `Assets/Prefab/RED.prefab`, `Assets/Prefab/BLUE.prefab`를 Unity Editor 내부 API로 로드.
- [x] `PreviewRenderUtility`로 RED/BLUE 프리팹 전용 미리보기 렌더링 생성.
- [x] 확인용 이미지 저장: `Assets/Screenshots/Codex_RED_BLUE_PrefabPreview_Static.png`.
- [x] Unity Console Error 0건 확인.

### 결과
- 파일 구조와 Unity 렌더 결과 기준 RED/BLUE 노트 프리팹의 기본 시각 구성은 정상으로 판단.

---

## 협업 지침 (Collaboration Guidelines)

> 이 파일은 Claude Code, 다른 AI, 팀원 누구나 읽고 이어서 작업할 수 있도록 작성되었습니다.

### 이 파일의 역할
- 프로젝트 전체 작업 이력을 날짜별로 기록합니다.
- 각 세션 종료 시 **완료 항목 / 미완료 항목 / 다음 할 일**을 반드시 기록합니다.
- 새 작업자는 이 파일을 위에서 아래로 읽으면 현재 상태를 파악할 수 있습니다.

### 작업 인계 시 확인 순서
1. `Log.md` (이 파일) — 전체 이력 및 현재 상태 파악
2. `Task.md` — 남은 작업 목록 및 스테이지 추가/삭제 가이드
3. Unity 에디터에서 `Assets/Scenes/Intro.unity` 열기
4. 컴파일 에러 없는지 확인 후 작업 시작

### 기록 규칙
- 날짜 헤더: `## YYYY-MM-DD`
- 완료 항목: `- [x]`, 미완료: `- [ ]`
- 파일 경로는 프로젝트 루트 기준 상대경로로 표기
- 세션 종료 시 **현재 상태 요약** 섹션 업데이트 필수
- `Log.md`, `Task.md` 수정 시 별도 검토 요청 없이 바로 수정하고, 완료 후 수정 사실만 보고

---

## 프로젝트 개요

| 항목 | 내용 |
|------|------|
| 프로젝트명 | VR Beat Saber |
| 엔진 | Unity (XR / VR) |
| 주요 씬 | `Assets/Scenes/Intro.unity`, `Assets/Scenes/Game.unity` |
| 핵심 데이터 | `Assets/Data/StageList.asset` |
| 빌드 순서 | Intro (index 0) → Game (index 1) |

### 핵심 구조 요약
```
StageListSO (ScriptableObject)
  └── StageEntry[] stages
        └── StageEntry: name / thumbnail / backgroundVideo / bgm

Intro 씬
  └── IntroManager: PrevStage() / NextStage() / OnStartGame()
        → PlayerPrefs.SetInt("SelectedStage", index)
        → SceneManager.LoadScene("Game")

Game 씬
  └── GameBackgroundController.Awake()
        → PlayerPrefs.GetInt("SelectedStage", 0)
        → VideoPlayer + AudioSource 적용
```

---

## 2026-05-06

### Claude Code 환경 설정
- [x] `D:\Plugins\unity-harness-plugin` 위치 플러그인 설치
  - 문제: `plugin.json`의 `author` 필드가 문자열 → 객체 형식으로 수정 후 설치 성공
- [x] Claude Code 재시작 후 Unity 스킬 7개 활성화 확인
  - `/unity-harness:compile`, `/unity-harness:brief` 등

### 컴파일 확인
- [x] `/unity-harness:compile` 실행 — 에러 없음

---

### Intro 씬 초기 구현 — 360° 스카이돔 + VR UI

#### 생성된 스크립트

| 파일 | 역할 |
|------|------|
| `Assets/Scripts/IntroManager.cs` | 메인 컨트롤러 (곡 선택, 배경 전환, 페이드, 씬 이동) |
| `Assets/Scripts/SkyRotator.cs` | 스카이돔 Y축 회전 (speed: 1.5°/s) |
| `Assets/Scripts/Editor/IntroSceneSetup.cs` | SkyDome360, VideoPlayer360, BGM 오브젝트 생성 |
| `Assets/Scripts/Editor/IntroUISetup.cs` | World Space Canvas UI 생성 |
| `Assets/Scripts/Editor/IntroFinalSetup.cs` | TrackedDeviceGraphicRaycaster 추가, SongButton onClick 연결 |
| `Assets/Scripts/Editor/BuildSettingsSetup.cs` | 빌드 세팅 Intro(0) → Game(1) |
| `Assets/Scripts/Editor/AttachSkyRotator.cs` | SkyDome360에 SkyRotator 컴포넌트 부착 |
| `Assets/Scripts/Editor/WireVideoPlayer.cs` | IntroManager.skyVideoPlayer 레퍼런스 자동 연결 |

#### 씬 적용 내용 (Assets/Scenes/Intro.unity — 초기)

| 오브젝트 | 내용 |
|----------|------|
| `SkyDome360` | Sphere (scale -100,100,100), Material360.mat, SkyRotator 부착 |
| `VideoPlayer360` | motion.mp4 루프 재생 → SkyDome MainTex 출력 |
| `BGM` | About That Oldie.mp3, 2D, loop, volume 0.6 |
| `IntroPanel` | World Space Canvas (pos: 0,1.5,2.5 / scale: 0.001) |
| ↳ `TrackedDeviceGraphicRaycaster` | VR 컨트롤러로 UI 버튼 클릭 가능 |
| ↳ `SongButton_0` | onClick → IntroManager.SelectSong0() |
| ↳ `StartButton` | onClick → IntroManager.OnStartGame() |
| ↳ `FadeOverlay` | CanvasGroup, 씬 시작/종료 페이드 |
| `Directional Light` | 파란빛 (0.4, 0.45, 0.9), intensity 0.4 |

#### 배경 전환 구조 (초기)
- 곡마다 다른 360° 배경 영상 자동 적용
- `SongEntry.backgroundVideo` (VideoClip) 필드 드래그 → 자동 전환
- `IntroManager.defaultBackgroundVideo` = motion.mp4 (fallback)

---

## 2026-05-14

### 배경+BGM 스테이지 선택 시스템 전면 재구축

#### 배경 (작업 전 상태)
- `Assets/Scenes/Intro.unity`: Button onClick 비어있음, 씬 전환 로직 없음
- `Assets/Scenes/Game.unity`: Video 오브젝트 있으나 배경 컨트롤러 없음
- 구버전 IntroManager: 씬에 미부착 상태

#### 신규 스크립트

| 파일 | 역할 |
|------|------|
| `Assets/Scripts/StageEntry.cs` | 스테이지 데이터 (name / thumbnail / backgroundVideo / bgm) |
| `Assets/Scripts/StageListSO.cs` | ScriptableObject — 스테이지 목록 중앙 관리 |
| `Assets/Scripts/IntroManager.cs` | Intro 씬 컨트롤러 재작성 (Prev/Next/Start/Fade) |
| `Assets/Scripts/GameBackgroundController.cs` | Game 씬 배경+BGM 적용 (PlayerPrefs 수신) |
| `Assets/Scripts/Editor/IntroSceneBuilder.cs` | Intro 씬 UI 자동 구성 |
| `Assets/Scripts/Editor/GameSceneSetup.cs` | Game 씬 GameBackgroundController 자동 생성 |
| `Assets/Scripts/Editor/CreateStageList.cs` | StageList.asset + 빌드 세팅 자동 생성 |
| `Assets/Scripts/Editor/WireIntroRefs.cs` | IntroManager bgmSource 자동 연결 |
| `Assets/Scripts/Editor/WireStageList.cs` | 양쪽 씬에 StageList.asset 자동 연결 |
| `Assets/Scripts/Editor/SaveCurrentScene.cs` | 현재 씬을 원래 경로로 저장 (coplay save 오작동 대응) |

#### 구버전 Editor 스크립트 비활성화 (스텁 처리)
- `Editor/WireVideoPlayer.cs` — 구 skyVideoPlayer 필드 참조 → 스텁
- `Editor/IntroUISetup.cs` — 구 SongEntry 참조 → 스텁
- `Editor/IntroFinalSetup.cs` — 구 SelectSong0 참조 → 스텁

#### Intro 씬 변경사항 (Assets/Scenes/Intro.unity)

| 오브젝트 | 내용 |
|----------|------|
| `Canvas` | 800×550 World Space, scale 0.001, pos (0, 1.5, 2.5) |
| `Canvas/TitleText` | "VR BEAT SABER" TMP 타이틀 |
| `Canvas/PrevButton` | ◀ — IntroManager.PrevStage() 연결 |
| `Canvas/NextButton` | ▶ — IntroManager.NextStage() 연결 |
| `Canvas/ThumbnailBG/ThumbnailImage` | 선택된 스테이지 썸네일 표시 |
| `Canvas/StageNameText` | 선택된 스테이지 이름 표시 |
| `Canvas/StartButton` | "PLAY" — IntroManager.OnStartGame() 연결 |
| `Canvas/FadeOverlay` | 페이드인/아웃 CanvasGroup |
| `IntroManager` | 모든 레퍼런스 + StageList.asset + VideoPlayer + AudioSource 연결 |

#### Game 씬 변경사항 (Assets/Scenes/Game.unity)

| 오브젝트 | 내용 |
|----------|------|
| `GameBackgroundController` | 신규 Empty GO, GameBackgroundController + AudioSource 부착 |
| ↳ stageList | Assets/Data/StageList.asset 연결 |
| ↳ backgroundPlayer | 기존 "Video" 오브젝트의 VideoPlayer 연결 |
| ↳ bgmSource | 자체 AudioSource (loop, vol 0.7) |

#### 배경 표시 문제 수정

**발견된 문제**
1. Intro 씬에 SkyDome360 없음 → 배경이 검게 보임
2. Cube, Cube(1), Spawner 오브젝트가 Intro 씬에 잘못 포함
3. Material360.mat 셰이더가 `Skybox/Panoramic` → 메시 적용 불가

**수정 완료**
- `Editor/FixIntroScene.cs` 실행: SkyDome360 구체 생성 (scale: -100,100,100), Cube/Spawner 제거
- `Editor/FixMaterial.cs` 실행: Material360.mat 셰이더 → `Unlit/Texture`, mainTexture: RenderTexture360 유지
- 씬 저장 완료

#### 백업 파일
- `Assets/Scenes/Intro_backup_20260514.unity`
- `Assets/Scenes/Game_backup_20260514.unity`

#### 데이터 에셋
- `Assets/Data/StageList.asset` — Stage 0: "About That Oldie" (motion.mp4 + About That Oldie.mp3)

#### 빌드 세팅
- Intro (index 0) → Game (index 1) 확인 완료

---

## 2026-05-15 (세션 1)

### Intro → Game 씬 흐름 최종 검증

#### 작업 내용
- [x] **Intro 씬 시각적 확인**: Game View에서 SkyDome(360 비디오) 및 UI 패널 정상 출력 확인
- [x] **스테이지 전환 로직 확인**: `Next/Prev` 시뮬레이션을 통해 스테이지 데이터 연동 확인
- [x] **씬 전환 및 데이터 전달 확인**: `Start` 버튼 클릭 시 `PlayerPrefs`를 통해 선택된 스테이지 인덱스가 `Game` 씬의 `GameBackgroundController`로 정상 전달되어 배경/BGM이 재생됨을 확인

#### 발견된 기술적 이슈 및 해결 시도
- **이슈 1: Play 모드 중 타임아웃/연결 끊김**: 비디오 재생 및 VR 시스템 부하로 인해 MCP 툴 응답 지연 발생.
  - 조치: 무거운 작업 시 Play 모드 일시 정지 권장. 툴 호출 시 paged/summary-first 전략 강화.
- **이슈 2: `execute_code` 툴 경로 길이 제한**: Windows OS의 260자 경로 제한으로 인해 Unity `mono.exe` 호출 실패.
  - 제안: 프로젝트를 `D:\VBS` 등 더 짧은 경로로 이동하거나, Windows Registry에서 "Long Paths" 활성화를 권장함.

---

## 2026-05-15 (세션 2)

### 배경 렌더링 방식 — VRBeat 레퍼런스 조사 및 정렬

#### 조사 결과
- `D:\work\VRBeat\VRBeat` 레퍼런스 프로젝트 분석 완료:
  - 방식: `VideoPlayer` → `RenderTexture` → `Skybox/Panoramic` 셰이더 적용 머티리얼.
  - 적용: 씬 스카이박스(`RenderSettings.skybox`)에 해당 머티리얼 할당 및 카메라 `clearFlags=Skybox` 설정.
  - 장점: 메시(SkyDome) 없이도 전방위 360도 배경을 왜곡 없이 렌더링 가능.

#### 수정 및 동기화 사항
- [x] **머티리얼 업그레이드**: `Assets/360 Music/Material360.mat` 셰이더를 `Unlit/Texture` → **`Skybox/Panoramic`**으로 변경.
- [x] **속성 최적화**: 매핑 방식을 **`Latitude Longitude Layout`**으로 설정하여 VR 몰입감 개선.
- [x] **씬 환경 일치화**: `SyncSkybox.cs` 에디터 스크립트를 제작 및 실행하여 `Intro`와 `Game` 씬 모두 레퍼런스 방식으로 배경 설정 통일.
- [x] **시각적 검증**: 양쪽 씬 모두 Game View에서 배경 영상이 전방위에 정상 출력됨을 확인.

---

## 2026-05-20

### Log.md / Task.md 인계 내용 확인 및 미진행 항목 점검

#### 확인 내용
- [x] `Log.md`와 `Task.md`를 UTF-8 기준으로 확인하고, 이 프로젝트 전용 작업 이력/작업 목록 문서로 관리하기로 정리.
- [x] Unity MCP 연결 확인: 프로젝트 루트가 `D:\work\VRBeatSaber`로 감지됨.
- [x] Unity Editor 상태 확인: Play Mode 꺼짐, 컴파일 에러 없음, 활성 에셋 `Assets/Scenes/Intro.unity`.
- [x] `Assets/Data/StageList.asset` 확인: 현재 스테이지는 `About That Oldie` 1개.
- [x] 스테이지 확장 가능 여부 확인: `Assets/360 Music/`에 현재 `motion.mp4`, `About That Oldie - Vibe Tracks.mp3`만 있어 새 스테이지 추가는 추가 미디어 필요.
- [ ] Windows Long Path 활성화 시도: 현재 `LongPathsEnabled=0`; 레지스트리 변경 시 `Access is denied`로 실패. 관리자 권한 필요.

#### 완료 항목
- [x] 현재 프로젝트 상태와 남은 작업을 재확인.
- [x] `Task.md`에 2026-05-20 기준 상태를 반영.

#### 미완료 항목
- [ ] 실제 VR 하드웨어 테스트: 헤드셋/컨트롤러 실기 필요.
- [ ] 스테이지 데이터 확장: 추가 360도 배경 영상, BGM, 선택 썸네일 필요.
- [ ] Windows Long Path 활성화: 관리자 권한 필요.

### Meta Quest 3S 실기 테스트 피드백 반영

#### 발견된 문제
- [x] Meta Quest 3S 실기 테스트 중 Intro 씬 UI가 눈높이보다 높게 보이는 문제 확인.

#### 수정 내용
- [x] `Assets/Scenes/Intro.unity`의 World Space `Canvas` 위치를 `y=0`에서 `y=-0.35`로 낮춤.
- [x] 수정 전 백업 생성:
  - `Assets/Scenes/Intro_backup_20260520_ui_height_before_update.unity`
  - `Log_backup_20260520_ui_height_before_update.md`
  - `Task_backup_20260520_ui_height_before_update.md`

#### 다음 확인
- [ ] Quest 3S에서 Intro 씬 UI 높이가 눈높이에 맞는지 재테스트.

### 프로토타입 배경 구조 반영

#### 기준 프로젝트
- [x] 프로토타입 경로 확인: `D:\work\VRBeat\VRBeat`
- [x] 기준 씬 확인: `D:\work\VRBeat\VRBeat\Assets\Scenes\Game.unity`

#### 백업 정리
- [x] 20260514 씬 백업은 유지.
- [x] 20260514 외 기존 씬 백업(`Intro_backup_20260520_ui_height_before_update.unity`) 삭제.
- [x] 신규 씬 백업 폴더 생성: `Assets/Scenes/Backup`
- [x] 현재 씬 백업 저장:
  - `Assets/Scenes/Backup/Intro_backup_20260520_before_prototype_background.unity`
  - `Assets/Scenes/Backup/Game_backup_20260520_before_prototype_background.unity`

#### 수정 내용
- [x] `Assets/360 Music/Material360.mat` 설정을 프로토타입 `mat360.mat`에 맞춤.
  - `_ImageType: 1`
  - `_Mapping: 1`
  - `_MirrorOnBack: 1`
  - `_MainTex: RenderTexture360`
- [x] `Assets/Scenes/Intro.unity`, `Assets/Scenes/Game.unity`에 프로토타입식 `Background` 루트 구성 적용.
- [x] `Background/Cube`: 위치 `(0, -1.3, 0)`, 스케일 `(5, 1, 5)`, `MaterialBlack` 적용.
- [x] `Background/Cube (1)`: 위치 `(0, -1.3, 12)`, 스케일 `(5, 1, 13)`, `MaterialBlack` 적용.
- [x] `Game.unity`의 `Spawner`를 `Background` 하위로 이동하여 프로토타입 구조와 일치.
- [x] 카메라 Clear Flags를 Skybox로 유지하고 `RenderSettings.skybox`를 `Material360`으로 설정.

#### 확인 결과
- [x] Unity Editor 상태 확인: Play Mode 꺼짐, 컴파일 에러 없음.
- [ ] Quest 3S 실기에서 프로토타입처럼 보이는지 재확인 필요.

### Quest 3S 컨트롤러 UI 클릭 문제 수정

#### 발견된 문제
- [x] Canvas에는 `TrackedDeviceGraphicRaycaster`, EventSystem에는 `XRUIInputModule`이 정상 적용되어 있음.
- [x] 버튼 3개(`PrevButton`, `NextButton`, `StartButton`) 모두 `interactable=true`, `raycastTarget=true` 확인.
- [x] 실제 원인 확인: 좌/우 컨트롤러의 `XRRayInteractor`에서 `m_EnableUIInteraction=False`로 되어 있어 레이로 World Space UI를 클릭할 수 없는 상태.

#### 수정 내용
- [x] 수정 전 씬 백업 생성: `Assets/Scenes/Backup/Intro_backup_20260520_before_xr_ui_fix.unity`
- [x] `Assets/Scenes/Intro.unity`의 좌/우 컨트롤러 `Teleport Interactor`에 `m_EnableUIInteraction=True` 적용.
- [x] 확인 로그: 변경된 `XRRayInteractor` 2개.
- [x] Unity Editor 상태 확인: Play Mode 꺼짐, 컴파일 에러 없음.

#### 다음 확인
- [ ] Quest 3S에서 컨트롤러 레이로 Intro UI 버튼 클릭이 정상 동작하는지 재테스트.

### Quest 3S 컨트롤러 UI 클릭 추가 수정

#### 재확인된 문제
- [x] `Teleport Interactor`의 `m_EnableUIInteraction`을 켰지만, 실기에서 Intro UI 버튼 상호작용이 계속 동작하지 않음.
- [x] XRI Starter Assets 기준 실제 UI/원거리 상호작용용 프리팹은 `NearFarInteractor` 계열임을 확인.

#### 수정 내용
- [x] 수정 전 씬 백업 생성: `Assets/Scenes/Backup/Intro_backup_20260520_before_nearfar_ui_fix.unity`
- [x] `XR Origin Hands (XR Rig)/Camera Offset/Left Controller` 하위에 `Left_NearFarInteractor` 추가.
- [x] `XR Origin Hands (XR Rig)/Camera Offset/Right Controller` 하위에 `Right_NearFarInteractor` 추가.
- [x] 추가한 Near-Far Interactor는 `UI Press` 액션과 `m_EnableUIInteraction=True`가 포함된 XRI Starter Assets 프리팹을 사용.
- [x] Unity Editor 상태 확인: Play Mode 꺼짐, 컴파일 에러 없음.

#### 다음 확인
- [ ] Quest 3S에서 컨트롤러 레이/포인터가 Intro UI 버튼을 hover/click 하는지 재테스트.

### 컨트롤러 조준 레이 표시 추가

#### 발견된 문제
- [x] Quest 3S 실기에서 컨트롤러가 가리키는 방향을 알 수 없고, 레이 자체가 보이지 않아 UI 버튼을 조준하기 어려움.

#### 수정 내용
- [x] 수정 전 씬 백업 생성: `Assets/Scenes/Backup/Intro_backup_20260520_before_visible_pointer.unity`
- [x] `Assets/Scripts/ControllerPointerVisualizer.cs` 추가.
- [x] `Assets/Scripts/Editor/AddVisibleControllerPointers.cs` 추가 및 실행.
- [x] `Left_NearFarInteractor` 하위에 `VisibleUIPointer` 추가.
- [x] `Right_NearFarInteractor` 하위에 `VisibleUIPointer` 추가.
- [x] 런타임에 왼쪽은 파란 레이, 오른쪽은 빨간 레이, 끝점은 흰색 포인터로 표시되도록 구성.
- [x] Unity Editor 상태 확인: Play Mode 꺼짐, 컴파일 에러 없음.

#### 다음 확인
- [ ] Quest 3S에서 컨트롤러 방향 레이가 보이고 UI 버튼 조준이 가능한지 재테스트.

### 컨트롤러 기준 UI 평면 레이로 수정

#### 발견된 문제
- [x] 보조 레이가 컨트롤러가 아닌 헤드에서 나가는 것처럼 느껴져 조준 기준이 불명확함.

#### 수정 내용
- [x] 수정 전 씬 백업 생성: `Assets/Scenes/Backup/Intro_backup_20260520_before_controller_to_ui_ray.unity`
- [x] `ControllerPointerVisualizer`를 컨트롤러 위치에서 시작하도록 수정.
- [x] 레이 끝점을 고정 길이가 아니라 `Canvas`가 있는 UI 평면과 만나는 지점으로 계산하도록 수정.
- [x] 기존 `NearFarInteractor` 하위 `VisibleUIPointer`를 제거하고, `Left Controller` / `Right Controller` 하위에 `VisibleUIPointer`를 재배치.
- [x] Unity Editor 상태 확인: Play Mode 꺼짐, 컴파일 에러 없음.

#### 다음 확인
- [ ] Quest 3S에서 레이가 컨트롤러에서 UI 평면까지 자연스럽게 보이는지 재테스트.

---

## 현재 상태 (2026-05-20 기준)

| 항목 | 상태 |
|------|------|
| 컴파일 에러 | 없음 |
| 활성 씬 | Assets/Scenes/Intro.unity |
| 배경 렌더링 | **Skybox/Panoramic 방식** (레퍼런스 동일, 최적화 완료) |
| 씬 전환 로직 | Intro ↔ Game 연동 및 데이터 전달 확인 완료 |
| Intro UI 위치 | Canvas `y=-0.35`, `z=2.5` |
| 배경 구조 | 프로토타입식 `Background` 루트 + 검은 바닥 큐브 2개 적용 |
| XR UI 입력 | 좌/우 `NearFarInteractor` 추가, `XRUIInputModule`/`TrackedDeviceGraphicRaycaster` 유지 |
| 컨트롤러 조준 표시 | 컨트롤러 기준 `VisibleUIPointer` 레이, UI 평면까지 표시 |
| 스테이지 데이터 | `About That Oldie` 1개 구성 |
| 주요 미진행 | Quest 3S UI 클릭/배경/높이 재테스트, 추가 스테이지 확장, Windows Long Path 관리자 권한 설정 |

### 다음 세션에서 할 일 → Task.md 참고

### Intro 씬 TMP 기호 깨짐 대응

#### 발견된 문제
- [x] Intro 씬의 일부 TMP 텍스트가 `▶`, `◀` 기호를 사용해 Quest/기본 TMP 폰트 환경에서 깨져 보일 가능성이 있음.

#### 수정 내용
- [x] 수정 전 씬 백업 생성: `Assets/Scenes/Backup/Intro_backup_20260520_before_text_fix.unity`
- [x] `Assets/Scenes/Intro.unity`의 `▶` 텍스트를 `>`로 변경.
- [x] `Assets/Scenes/Intro.unity`의 `▶  PLAY` 텍스트를 `PLAY`로 변경.
- [x] `Assets/Scenes/Intro.unity`의 `◀` 텍스트를 `<`로 변경.
- [x] Unity Editor 상태 확인: Play Mode 꺼짐, 컴파일 에러 없음.

#### 다음 확인
- [ ] Quest 3S에서 Intro 씬 버튼/타이틀/스테이지 텍스트가 깨짐 없이 보이는지 재테스트.

### 에셋 임포트 오류 대응

#### 발견된 문제
- [x] 6번 에셋 `Projectile Factory for PRO Effects Sci-Fi FX` 임포트 후 `Knife - PRO Effects - Sci-Fi FX` 연동 prefab들이 누락된 부모/중첩 prefab을 참조해 import error 발생.
- [x] 프로젝트 컴파일 자체는 깨지지 않았고, 문제는 별도 `PRO Effects Sci-Fi FX` 패키지를 기대하는 연동용 샘플 prefab에 한정됨.

#### 수정 내용
- [x] 문제 연동 폴더를 `Assets` 밖 백업 위치로 이동:
  - `Backup/AssetImports/20260520_ProjectileFactoryBrokenIntegration/Knife - PRO Effects - Sci-Fi FX`
- [x] `AssetDatabase.Refresh()` 재실행 후 신규 import error 미발생 확인.
- [x] Unity Editor 상태 확인: Play Mode 꺼짐, 컴파일 에러 없음.

#### 다음 확인
- [ ] 1번/3번 에셋 prefab 중 Game 씬 세이버/타격 연출에 쓸 후보 선별.

### 1번/3번 에셋 적용 후속 작업

#### 수정 내용
- [x] 수정 전 백업 생성:
  - `Assets/Scenes/Backup/Game_backup_20260520_before_neon_saber.unity`
  - `Assets/Scenes/Backup/Intro_backup_20260520_before_loading_vfx.unity`
  - `Backup/Scripts/Saber_backup_20260520_before_hit_vfx.cs`
  - `Backup/Scripts/IntroManager_backup_20260520_before_loading_vfx.cs`
- [x] 1번 `Free Game VFX Collection(URP)` 기반으로 Game 씬 세이버 2개를 네온 블레이드 형태로 변경.
- [x] Game 씬 세이버 블레이드에 `TrailRenderer`, `Point Light`, `SaberGlowPulse`를 추가해 휘두를 때 빛 잔상이 보이도록 구성.
- [x] `Saber.cs`에 블록 타격 시 hit VFX prefab을 생성하는 로직 추가.
- [x] 왼쪽/파란 세이버에는 `FX_LootDrop_Blue`, 오른쪽/빨간 세이버에는 `FX_Purple_Hit_02`를 타격 VFX로 연결.
- [x] 3번 `Sci-Fi Loading Screen VFX FREE`의 `Loading_free_blue` prefab을 Intro 씬 `PLAY` 전환 시 재생되도록 연결.
- [x] Unity Editor 상태 확인: Play Mode 꺼짐, 컴파일 에러 없음.

#### 다음 확인
- [ ] Quest 3S에서 세이버 Trail/빛 강도/타격 VFX가 과하지 않은지 실기 확인.
- [ ] Intro 씬에서 `PLAY` 클릭 시 로딩 VFX 위치와 페이드 타이밍 확인.

### Game 씬 노트 크기 및 세이버 판정 개선

#### 발견된 문제
- [x] 실기 플레이 중 노트가 크게 느껴지고, 블레이드 색상/노트 방향에 맞춰 휘둘러도 성공 판정 느낌이 약함.

#### 수정 내용
- [x] 수정 전 백업 생성:
  - `Assets/Scenes/Backup/Game_backup_20260520_before_saber_judgement_fix.unity`
  - `Backup/Scripts/Saber_backup_20260520_before_judgement_fix.cs`
  - `Backup/Scripts/Spawner_backup_20260520_before_note_scale_fix.cs`
  - `Backup/Scripts/Cube_backup_20260520_before_note_feedback_fix.cs`
  - `Backup/Scripts/RED_backup_20260520_before_note_scale_fix.prefab`
  - `Backup/Scripts/BLUE_backup_20260520_before_note_scale_fix.prefab`
- [x] RED/BLUE 노트 prefab 기본 스케일을 `0.5`에서 `0.38`로 축소.
- [x] Spawner의 생성 노트 스케일도 `noteScale=0.38`로 고정.
- [x] `Saber.cs` 판정을 기존 단일 Raycast에서 블레이드 끝점 이동 구간 기반 `OverlapCapsule` 판정으로 변경.
- [x] 세이버별 기존 LayerMask는 유지하여 왼쪽 파란 블레이드는 파란 노트, 오른쪽 빨간 블레이드는 빨간 노트만 판정.
- [x] 방향 판정 허용 각도를 `85도`로 완화하고, 최소 스윙 속도를 `0.35`로 낮춤.
- [x] 성공 시 hit VFX, 노트 축소 후 제거, 짧은 컨트롤러 햅틱 호출을 추가.
- [x] Unity Editor 상태 확인: Play Mode 꺼짐, 컴파일 에러 없음.

#### 다음 확인
- [ ] Quest 3S에서 노트 크기와 방향 판정 체감 재테스트.

### 노트/세이버 런타임 디자인 변경

#### 수정 내용
- [x] 수정 전 백업 생성:
  - `Backup/Design/RED_backup_20260520_before_visual_redesign.prefab`
  - `Backup/Design/BLUE_backup_20260520_before_visual_redesign.prefab`
  - `Assets/Scenes/Backup/Game_backup_20260520_before_visual_redesign.unity`
  - `Backup/Scripts/Cube_backup_20260520_before_runtime_visual_design.cs`
  - `Backup/Scripts/Saber_backup_20260520_before_runtime_visual_design.cs`
- [x] `Cube.cs`에 런타임 노트 비주얼 생성 추가.
  - 기존 단색 큐브를 에너지 블록 느낌으로 보이게 core/emission material 적용.
  - 전면 네온 프레임과 절단 방향 화살표 추가.
  - 기존 방향 표시용 Sphere는 렌더링 비활성화.
- [x] `Saber.cs`에 런타임 세이버 비주얼 생성 추가.
  - 블레이드를 더 길고 얇은 에너지 블레이드로 조정.
  - 흰색 블레이드 코어, 컬러 글로우, TrailRenderer 보강.
  - 손잡이, emitter ring, cross guard 추가.
- [x] Game 씬 Play Mode 실행 후 스크린샷 저장:
  - `Assets/Screenshots/test_play_game.png`
- [x] Unity Editor 상태 확인: Play Mode 꺼짐, 컴파일 에러 없음.

#### 다음 확인
- [ ] Quest 3S에서 노트 전면 화살표 방향 가독성과 세이버 손잡이/블레이드 크기 체감 확인.

### 세이버 프리뷰 렌더링

#### 작업 내용
- [x] `CaptureSaberPreview.cs`에 세이버 단독 프리뷰 렌더링 기능 추가.
- [x] 세이버 프리뷰 이미지 생성:
  - `Assets/Screenshots/saber_preview.png`
- [x] Unity Editor 상태 확인: Play Mode 꺼짐, 컴파일 에러 없음.

### 세이버 크로스가드 제거

#### 수정 내용
- [x] 수정 전 백업 생성:
  - `Backup/Scripts/Saber_backup_20260520_before_remove_crossguard.cs`
  - `Backup/Scripts/CaptureSaberPreview_backup_20260520_before_remove_crossguard.cs`
- [x] `Saber.cs` 런타임 세이버 디자인에서 `Cross Guard` 생성 제거.
- [x] `CaptureSaberPreview.cs` 세이버 프리뷰에서도 `Cross Guard` 제거.
- [x] `Assets/Screenshots/saber_preview.png` 재생성.
- [x] Unity Editor 상태 확인: Play Mode 꺼짐, 컴파일 에러 없음.

### Retrowave Skies Lite 스테이지 적용

#### 수정 내용
- [x] 1번 에셋 `RETROWAVE SKIES Lite` 기반 `Neon Grid` 스테이지 추가.
- [x] `StageEntry.cs`에 스테이지별 skybox/grid material 필드 추가.
- [x] `GameBackgroundController.cs`에서 영상 배경이 없는 스테이지는 skybox + grid floor 방식으로 표시하도록 수정.
- [x] `IntroManager.cs`에서 영상 없는 스테이지 선택 시 VideoPlayer를 정지/비활성화하도록 수정.
- [x] Game 씬에 `Retrowave Grid Floor`를 추가하고 `Vapor_Skybox`, `M_Grid Vapor Lite`를 연결.
- [x] Play Mode 실행 후 `Assets/Screenshots/test_play_game.png`로 적용 화면 캡처 확인.
- [x] Unity Editor 상태 확인: Play Mode 꺼짐, 컴파일 에러 없음.
- [x] `RETROWAVE SKIES Lite` 데모 프리셋 3종 캡처 생성:
  - `Assets/Screenshots/RetrowavePresets/retrowave_vapor.png`
  - `Assets/Screenshots/RetrowavePresets/retrowave_orange.png`
  - `Assets/Screenshots/RetrowavePresets/retrowave_vhs.png`

### Retrowave 배경별 스테이지/BGM 추가

#### 수정 내용
- [x] 수정 전 백업 생성:
  - `Backup/Data/StageList_backup_20260520_before_retrowave_stage_variants.asset`
  - `Backup/Scripts/SetRetrowavePreviewStage_backup_20260520_before_stage_variant_names.cs`
  - `Backup/Scripts/GameBackgroundController_backup_20260520_before_retrowave_bgm_loop.cs`
  - `Backup/Scripts/IntroManager_backup_20260520_before_retrowave_bgm_loop.cs`
- [x] 기존 `Neon Grid`를 배경 프리셋별 Retrowave 스테이지 3종으로 정리:
  - `Retrowave Vapor` / BPM 124 / `Retrowave_Vapor_BGM.wav`
  - `Retrowave Orange` / BPM 110 / `Retrowave_Orange_BGM.wav`
  - `Retrowave VHS` / BPM 132 / `Retrowave_VHS_BGM.wav`
- [x] 각 스테이지에 대응되는 skybox/grid material 연결.
- [x] `Assets/Audio/Generated/`에 각 스테이지용 루프형 WAV BGM 생성.
- [x] `IntroManager.cs`, `GameBackgroundController.cs`에서 BGM AudioSource loop를 강제 활성화.
- [x] Unity Editor 상태 확인: Play Mode 꺼짐, 컴파일 에러 없음.

### 작업 로그 파일명 정책 반영

#### 수정 내용
- [x] 기존 통합 작업 로그 `D:\Codex\Log\20260520_Log.md`에서 VRBeatSaber 관련 내용만 분리.
- [x] 신규 프로젝트별 작업 로그 생성: `D:\Codex\Log\20260520_VRBeatSaber_Log.md`
- [x] 다른 프로젝트 작업 로그 확인: `D:\Codex\Log\20260520_일경험프로젝트_Log.md`
- [x] 프로젝트별 로그 분리가 완료된 것으로 판단하여 기존 날짜 전용 통합 로그 `D:\Codex\Log\20260520_Log.md` 삭제.

### 실기 불가 상황에서 에디터 검증 진행

#### 작업 내용
- [x] 사용자가 당장 Quest 3S 실기 테스트를 진행할 수 없는 상황임을 반영.
- [x] `VerifyEditorScopeTasks.cs`를 추가해 에디터에서 확인 가능한 범위 자동 검증.
  - Retrowave 3종 스테이지 skybox/grid/BGM/BPM 연결 확인.
  - Intro 씬 TMP 텍스트 5개 확인, `▶` / `◀` 특수 화살표 없음.
  - Game 씬 `GameBackgroundController`의 StageList/BGM/Grid 참조 확인.
- [x] Play Mode에서 Retrowave 3종 Game 씬 캡처 생성:
  - `Assets/Screenshots/RetrowaveGameStages/retrowave_game_vapor.png`
  - `Assets/Screenshots/RetrowaveGameStages/retrowave_game_orange.png`
  - `Assets/Screenshots/RetrowaveGameStages/retrowave_game_vhs.png`
- [x] Task.md에서 에디터 검증 완료 항목과 실기/관리자 권한 대기 항목을 구분.
- [x] Unity Editor 상태 확인: Play Mode 꺼짐, 컴파일 에러 없음.

### 파란 노트가 빨간색으로 보이는 문제 수정

#### 발견된 문제
- [x] Game 씬 Play Mode 캡처에서 노트가 빨간색으로만 보임.
- [x] RED/BLUE prefab은 각각 레이어 6/7로 정상 연결되어 있었고, Spawner에도 두 prefab이 모두 연결되어 있음을 확인.
- [x] 원인: 프로젝트 레이어 이름은 `Red`, `Blue`인데 `Cube.cs`에서 `LayerMask.NameToLayer("BLUE")`를 사용해 파란 노트 판정이 실패함.

#### 수정 내용
- [x] 수정 전 백업 생성:
  - `Backup/Scripts/Spawner_backup_20260520_before_balanced_note_color.cs`
  - `Backup/Scripts/Cube_backup_20260520_before_layer_name_fix.cs`
  - `Backup/Scripts/Saber_backup_20260520_before_layer_name_fix.cs`
  - `Backup/Scripts/CountRuntimeNotes_backup_20260520_before_layer_name_fix.cs`
- [x] `Cube.cs` 레이어 판정을 `Blue`로 수정해 파란 노트가 파란색 재질을 사용하도록 수정.
- [x] `Saber.cs`의 보조 레이어 판정도 `Blue` 기준으로 수정.
- [x] `Spawner.cs`를 완전 랜덤 대신 색상이 균형 있게 교차 생성되도록 보강.
- [x] Play Mode 검증: 런타임 노트 카운트 `RED=21`, `BLUE=21`.
- [x] 캡처 갱신: `Assets/Screenshots/RetrowaveGameStages/retrowave_game_vhs.png`.
- [x] Unity Editor 상태 확인: Play Mode 꺼짐, 컴파일 에러 없음.
- [x] 추가 확인: 일반 플레이 캡처에서는 노트 겹침 때문에 차이가 잘 안 보여, `CaptureNoteColorDiagnostic.cs`로 RED/BLUE 노트를 고정 배치한 근접 진단 캡처 생성.
- [x] 추가 검증: 런타임 노트 카운트 `RED=45`, `BLUE=46`.
- [x] 진단 캡처 생성: `Assets/Screenshots/note_color_diagnostic.png`.

### Retrowave BGM 런타임 재생 확인

#### 작업 내용
- [x] `CheckRuntimeBgm.cs`를 추가해 Play Mode에서 `GameBackgroundController.bgmSource` 상태 확인.
- [x] `Retrowave Vapor`: `Retrowave_Vapor_BGM`, 길이 15.48초, `isPlaying=True`, `loop=True`, volume 0.70, mute False.
- [x] `Retrowave Orange`: `Retrowave_Orange_BGM`, 길이 17.45초, `isPlaying=True`, `loop=True`, volume 0.70, mute False.
- [x] `Retrowave VHS`: `Retrowave_VHS_BGM`, 길이 14.55초, `isPlaying=True`, `loop=True`, volume 0.70, mute False.
- [x] Unity Editor 상태 확인: Play Mode 꺼짐, 컴파일 에러 없음.
- [ ] 실제 청감상 볼륨/분위기 적합성은 사용자 환경에서 확인 필요.

### 전체 저장 및 문서 정리 확인

#### 작업 내용
- [x] `SaveAllProjectState.cs`를 추가해 Unity AssetDatabase와 열린 씬 전체 저장 실행.
- [x] 저장 실행 결과: `[SaveAllProjectState] Saved assets and open scenes.`
- [x] `Log.md` / `Task.md`에 최근 작업 내용이 정리되어 있는지 확인.
- [x] 확인된 정리 항목:
  - Retrowave 배경별 스테이지/BGM 추가
  - 작업 로그 파일명 정책 반영
  - 실기 불가 상황에서 에디터 검증 진행
  - 파란 노트 색상 문제 수정 및 진단 캡처
  - Retrowave 3종 BGM 런타임 재생 확인
- [x] Unity Editor 상태 확인: Play Mode 꺼짐, 컴파일 에러 없음.

### GitHub private 저장소 연결 및 초기 업로드

#### 작업 내용
- [x] 로컬 git 저장소 초기화.
- [x] GitHub 원격 저장소 연결: `https://github.com/ssa25879/VRRhythmGame.git`
- [x] Unity 프로젝트용 `.gitignore` 추가 및 원격 Unity 템플릿과 병합.
  - `Library/`, `Temp/`, `Logs/`, `UserSettings/`, IDE 생성 파일 제외.
  - 로컬 `Backup/`, `Assets/Screenshots/`, `.codex/`, `.claude/` 제외.
- [x] Unity 파일 관리를 위한 `.gitattributes` 추가.
- [x] 스테이징 전 확인: 제외 대상 폴더와 90MB 초과 파일이 커밋 후보에 없음.
- [x] 첫 커밋 생성: `0df79bc Initial Unity VR rhythm game project`
- [x] 원격 초기 커밋 병합: `9a927b7 Merge remote-tracking branch 'origin/main'`
- [x] `main` 브랜치를 `origin/main`으로 push 완료.
- [x] Git `safe.directory` 예외 추가: `D:/work/VRBeatSaber`

### README 설명문 수정 및 업로드

#### 작업 내용
- [x] 수정 전 백업 생성: `Backup/Docs/README_backup_20260520_before_simple_description.md`
- [x] `README.md`에 프로젝트 간단 설명, 현재 구현 내용, 개발 환경, 저장소 제외 대상 설명 추가.
- [x] 커밋 생성: `5426b44 Update README project description`
- [x] GitHub 원격 저장소 `main` 브랜치로 push 완료.

### AGENTS.md 지침 파일 생성 및 업로드

#### 작업 내용
- [x] 프로젝트 루트에 `AGENTS.md` 생성.
- [x] 한국어 응답, 문서 관리, 외부 작업 로그, 백업 정책, Unity 작업 규칙, Git/GitHub 관리 지침 정리.
- [x] 다른 AI와 번갈아 작업할 때도 먼저 확인할 수 있는 공통 지침 파일로 관리하도록 정리.

### README 작업 지침 안내 추가

#### 작업 내용
- [x] 수정 전 백업 생성: `Backup/Docs/README_backup_20260520_before_agents_note.md`
- [x] `README.md`에 클론 후 작업 시 먼저 확인할 문서 안내 추가.
  - `AGENTS.md`: 프로젝트 공통 작업 지침
  - `Task.md`: 현재 작업 상태와 대기 항목
  - `Log.md`: 지금까지의 작업 이력

### README AI 도구 활용 표기 추가

#### 작업 내용
- [x] 수정 전 백업 생성: `Backup/Docs/README_backup_20260520_before_ai_tools_note.md`
- [x] `README.md`에 AI 도구 활용 섹션 추가.
- [x] 개발 과정에서 AI 도구를 사용해 코드 수정, Unity 에디터 작업 보조, 작업 로그 정리, GitHub 관리 작업을 진행하고 있음을 표기.

### AGENTS 경로 지침 보정

#### 작업 내용
- [x] `AGENTS.md`의 프로젝트 범위를 절대 경로 고정이 아닌 저장소 루트 기준으로 수정.
- [x] 현재 PC의 `D:\work\VRBeatSaber` 경로는 로컬 예시이며, 다른 환경에서는 프로젝트 루트 기준 상대 경로를 우선 사용하도록 명시.
- [x] 외부 작업 로그 경로도 현재 작업 환경 기준이며, 다른 환경에서는 사용자가 지정한 동등한 작업 로그 경로를 사용할 수 있도록 보정.

### 테스트 플레이 영상 캡처

#### 작업 내용
- [x] Unity MCP 연결 상태 확인: Play Mode 실행 가능, 컴파일 에러 없음.
- [x] `RecordTestPlayVideo.cs` 에디터 유틸리티 생성.
- [x] Game 씬 Play Mode에서 10초 분량 100프레임 캡처.
- [x] 캡처 프레임 위치: `Assets/Screenshots/TestPlayVideo/20260520_160059/`
- [x] GIF 미리보기 생성: `Assets/Screenshots/TestPlayVideo/test_play_preview.gif`

### Intro 씬 PC 클릭 테스트 가시성 수정

#### 작업 내용
- [x] 수정 전 씬 백업 생성: `Assets/Scenes/Backup/Intro_backup_20260520_before_pc_click_visibility.unity`
- [x] PC Game View에서 UI가 보이지 않는 원인 확인.
  - Main Camera 위치: `(0, 1.5, 0)`
  - 기존 Canvas 위치: `(0, -0.35, 2.5)`
  - PC 카메라 시야 기준 UI가 화면 아래 밖에 배치되어 있었음.
- [x] Intro Canvas 위치를 `(0, 1.2, 2.5)`로 조정.
- [x] Canvas `worldCamera`를 `Main Camera`로 지정해 PC 마우스 클릭 판정 안정화.
- [x] Play Mode 캡처로 UI 표시 확인: `Assets/Screenshots/intro_pc_click_visibility.png`

### Intro 씬 스테이지 변경/음악 확인 및 시각 피드백 보강

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Backup/Scripts/IntroManager_backup_20260520_before_intro_stage_visuals.cs`
  - `Assets/Scenes/Backup/Intro_backup_20260520_before_intro_stage_visuals.unity`
- [x] Intro 런타임 진단 결과 확인.
  - `StageList` 4개 스테이지 연결 확인.
  - 버튼 이벤트: `PrevStage`, `NextStage`, `OnStartGame` 연결 확인.
  - BGM AudioSource: `isPlaying=True`, `volume=0.7`, `mute=False`, `spatialBlend=0` 확인.
  - AudioListener: `Main Camera`에 존재 확인.
- [x] Retrowave 스테이지가 Intro 씬에서 배경 변화가 거의 없어 보이던 문제 수정.
  - `IntroManager`에 skybox/grid 적용 로직 추가.
  - 스테이지별 fallback thumbnail 색상 적용.
  - `StageGridPreview` 생성 후 `IntroManager.gridRenderer`에 연결.
- [x] Play Mode 검증 완료.
  - `About That Oldie`: 기존 터널 영상 표시.
  - `Retrowave Vapor`: Vapor skybox/grid 표시.
  - `Retrowave Orange`: Orange skybox/grid 표시.
  - 각 스테이지 BGM clip 전환 및 `playing=True` 로그 확인.
- [x] 확인 캡처 생성.
  - `Assets/Screenshots/IntroStageStates/intro_about_that_oldie.png`
  - `Assets/Screenshots/IntroStageStates/intro_retrowave_vapor.png`
  - `Assets/Screenshots/IntroStageStates/intro_retrowave_orange.png`

### 불필요 백업 검토 및 삭제

#### 작업 내용
- [x] 백업 파일 해시 비교로 완전 중복 후보 확인.
- [x] 사용자 승인 후 삭제 진행.
- [x] 삭제 파일/폴더:
  - `Assets/Scenes/Backup/Intro_backup_20260520_before_retrowave_stage.unity`
  - `Assets/Scenes/Backup/Intro_backup_20260520_before_retrowave_stage.unity.meta`
  - `Backup/AssetImports/20260520_ProjectileFactoryBrokenIntegration/`
- [x] 삭제 후 대상 경로가 존재하지 않음을 확인.

### 음악 생성 프롬프트 정리 및 BPM 기반 노트 스폰 보강

#### 작업 내용
- [x] 음악 생성 프롬프트 md 파일 생성: `D:\Codex\VRBeatSaber_MusicPrompts.md`
  - Retrowave Vapor / Orange / VHS 프롬프트 작성.
  - 2분~2분 30초 full-length 조건과 30초 preview/sample 금지 조건 추가.
- [x] 수정 전 백업 생성: `Backup/Scripts/Spawner_backup_20260520_before_bpm_audio_sync.cs`
- [x] `Spawner.cs` 노트 생성 로직 보강.
  - 선택 스테이지 BPM으로 `beatDuration = 60 / bpm` 계산.
  - `GameBackgroundController.bgmSource`를 자동 참조.
  - 기존 `Time.deltaTime` 누적 timer 중심 생성에서 BGM AudioSource 재생 시간 기준 생성으로 변경.
  - 루프 재생 시 `timeSamples` 래핑을 감지해 누적 재생 시간을 유지.
  - `beatsPerSpawn`, `spawnLeadBeats`, `maxCatchUpSpawnsPerFrame`, `syncToBgm` 옵션 추가.
- [x] Play Mode 검증.
  - Retrowave Vapor 선택 후 Game 씬 실행.
  - 124 BPM 기준 `beatDuration=0.484` 로그 확인.
  - BGM `Retrowave_Vapor_BGM` 재생 중 확인.
  - 런타임 노트 생성 확인: `RED=24`, `BLUE=23`.

### About That Oldie 기본 스테이지 동작 확인

#### 작업 내용
- [x] `SelectedStage=0`으로 설정 후 Game 씬 Play Mode 실행.
- [x] BGM 확인.
  - clip: `About That Oldie - Vibe Tracks`
  - length: `114.08`
  - `isPlaying=True`, `loop=True`, `volume=0.70`, `mute=False`
- [x] BPM 노트 스폰 확인.
  - `beatDuration=0.500`
  - BGM 기준 동기화 활성화: `syncToBgm=True`
- [x] 런타임 노트 생성 확인: `RED=27`, `BLUE=27`.

### Game 씬 노래 종료 후 Intro 복귀 로직 추가

#### 작업 내용
- [x] 기존 구조 확인.
  - Game 씬에서 노래 종료 후 Intro/노래 선택 화면으로 돌아가는 로직은 없었음.
  - `GameBackgroundController`가 BGM `loop=true`를 강제하고 있어 노래가 끝나지 않는 구조였음.
- [x] 수정 전 백업 생성.
  - `Backup/Scripts/GameBackgroundController_backup_20260522_before_song_end_return.cs`
  - `Backup/Scripts/Spawner_backup_20260522_before_song_end_stop.cs`
  - `Assets/Scenes/Backup/Game_backup_20260522_before_song_end_return.unity`
- [x] `GameBackgroundController.cs` 수정.
  - BGM loop를 `false`로 변경.
  - 현재 스테이지 정보 확인용 `CurrentStageIndex`, `CurrentStage` 추가.
- [x] `GameSongEndController.cs` 추가.
  - BGM 재생 시작 후 곡 끝을 감지하면 `Intro` 씬으로 복귀.
  - 기본 복귀 지연 시간: `1.5초`.
- [x] `Spawner.cs` 보강.
  - BGM이 non-loop 상태에서 끝난 경우 추가 노트 생성을 중단.
- [x] Game 씬에 `GameSongEndController` 연결.
- [x] Play Mode 검증.
  - Retrowave Vapor 임시 BGM으로 곡 종료 후 `Intro` 씬 복귀 확인.
  - 로그 확인: `[GameSongEnd] Song ended. Returning to Intro scene.`

### DOTween 임포트 전 에디터 오류 원인 확인 및 XR 인터랙터 정리

#### 작업 내용
- [x] DOTween 관련 파일/네임스페이스 검색.
  - 현재 프로젝트 안에서는 `DOTween`, `Demigiant`, `DG.Tweening` 임포트 흔적 없음.
  - 현재 에디터 Error는 DOTween이 아니라 XR Interaction Toolkit 입력 등록 문제로 확인.
- [x] 오류 원인 확인.
  - `No available indices for pointer registration.`
  - Intro 씬에 `XRPokeInteractor`, 기본 `Near-Far Interactor`, 추가 `Left_NearFarInteractor`/`Right_NearFarInteractor`, 손 추적용 인터랙터가 동시에 활성화되어 XR UI pointer 등록 수가 고갈됨.
- [x] 수정 전 씬 백업 생성.
  - `Assets/Scenes/Backup/Intro_backup_20260522_before_xr_pointer_registration_fix.unity`
- [x] Intro 씬 XR 인터랙터 정리.
  - 좌/우 컨트롤러 UI용 `Left_NearFarInteractor`, `Right_NearFarInteractor`는 유지.
  - 중복 `XRPokeInteractor`와 기본 `Near-Far Interactor`는 비활성화.
  - `Teleport Interactor`의 UI Interaction은 비활성화.
- [x] 검증.
  - 수정 후 Intro Play Mode 진입/종료 확인.
  - 14:02 이후 신규 Error 없음.
  - 기존 콘솔에 남은 13:47/14:00 Error는 수정 전 발생 로그로 확인.

### BGM 후보 문서화

#### 작업 내용
- [x] 다른 환경에서도 확인할 수 있도록 프로젝트 내부 문서 생성.
  - `Docs/BGM_Candidates.md`
- [x] Retrowave 3종 스테이지별 BGM 후보와 출처 URL, 라이선스 확인 포인트, 적용 메모 정리.
  - Pixabay `Game Synthwave`
  - OpenGameArt `Synthwave House Loop`
  - OpenGameArt `Eyeless (Retrowave)`
  - StreamBeats / Nihilore / Pixabay Synthwave 추가 탐색 후보
- [x] `README.md` 작업 지침 문서 목록에 BGM 후보 문서 링크 추가.

### Score / Combo / HP / Miss 시스템 1차 구성

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Backup/Scripts/Cube_backup_20260526_before_score_system.cs`
  - `Backup/Scripts/Spawner_backup_20260526_before_score_system.cs`
  - `Assets/Scenes/Backup/Game_backup_20260526_before_score_system.unity`
- [x] `GameScoreController.cs` 추가.
  - 곡별 최대 점수 `100,000` 기준.
  - DJMAX식 곡별 고정 만점 구조를 참고해 `70% 기본 Hit 점수 + 30% 콤보 성장 점수`로 배분.
  - 전체 노트 성공 시 이론상 `100,000점`에 도달하도록 예상 노트 수와 콤보 누적 가중치 기반 계산.
  - Score / Combo / HP / Hit / Miss 런타임 HUD 자동 생성.
- [x] `Cube.cs` 수정.
  - 노트 성공 시 `RegisterHit()` 호출.
  - 노트를 놓치면 `MISS` 처리 후 콤보 초기화 및 HP 감소.
  - 방향이 맞지 않은 타격은 `BAD CUT`으로 처리해 콤보 초기화 및 HP 감소.
- [x] `Saber.cs` 수정.
  - 휘두르는 방향이 노트 방향과 맞을 때만 성공.
  - 방향이 틀린 경우 노트를 `BadCut()` 처리.
- [x] `Spawner.cs` 수정.
  - 노트 생성 시 Score 시스템에 생성 카운트 전달.
- [x] `WireGameScoreController.cs`로 Game 씬에 `GameScoreController` 연결.
- [x] 검증.
  - Unity 컴파일 에러 없음.
  - Game 씬에 `GameScoreController` 연결 확인.
  - Play Mode 진입 시 `[Score] expectedNotes=229` 초기화 로그 확인.

### BAD 판정 튜닝

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Backup/Scripts/GameScoreController_backup_20260526_before_bad_judgement_tuning.cs`
  - `Backup/Scripts/Cube_backup_20260526_before_bad_judgement_tuning.cs`
- [x] 방향이 틀린 타격 판정명을 `BAD CUT`에서 `BAD`로 변경.
- [x] `BAD` 판정은 콤보를 초기화하지 않도록 변경.
- [x] `BAD` 판정 HP 감소량을 `MISS`의 약 1/3로 설정.
  - `missHpDamage=12`
  - `badHpDamageRatio=0.333`
  - BAD 1회 HP 감소량 약 `4`
- [x] Game 씬 `GameScoreController` 직렬화 값 갱신.
- [x] Unity 컴파일 에러 없음 확인.

### 판정 Play Mode 검증

#### 작업 내용
- [x] 수정 전 씬 백업 생성.
  - `Assets/Scenes/Backup/Game_backup_20260526_before_judgement_playtest.unity`
- [x] `RunJudgementPlaytest.cs`, `CheckJudgementInPlayMode.cs`, `DiagnoseScoreControllerRuntime.cs` 진단 스크립트 추가.
- [x] 초기 자동 테스트 실패 원인 확인.
  - 테스트 실행 지연 중 노트가 자동 MISS 처리되어 `miss=9`, `hp=0`, `failed=True` 상태가 된 뒤 판정 호출이 들어감.
- [x] Score 상태를 테스트 직전에 초기화한 뒤 판정 단위 검증 완료.
  - `Hit`: score 증가, combo `1`, HP 변화 없음.
  - `BAD`: combo 유지, HP `4` 감소.
  - `MISS`: combo `0` 초기화, HP `12` 감소.
  - 결과 로그: `hitOk=True, badOk=True, missOk=True, damageRatioOk=True`.
- [~] 실제 Quest 컨트롤러를 휘둘러 방향 판정 체감 확인은 실기 테스트 필요.

### 노트 방향 표시 확인 및 가시성 수정

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Backup/Scripts/Cube_backup_20260526_before_note_direction_visibility_fix.cs`
  - `Backup/Scripts/Saber_backup_20260526_before_note_direction_visibility_fix.cs`
- [x] Play Mode 캡처로 기존 노트 방향 표시 확인.
  - 기존 상태에서는 전면 `Energy Glow`가 화살표를 가리고, 화살표 위치가 뒤쪽/내부에 가까워 방향이 거의 보이지 않음.
- [x] `Cube.cs` 수정.
  - 방향 화살표를 노트 전면으로 이동.
  - 화살표 크기를 키워 접근 중에도 보이도록 조정.
  - 전면 glow 크기를 줄여 화살표를 덜 가리도록 조정.
- [x] `Saber.cs` 수정.
  - 성공 판정 방향을 시각 화살표 방향과 일치하도록 `note.up` 기준으로 변경.
- [x] Play Mode 재캡처 확인.
  - `Assets/Screenshots/note_color_diagnostic.png`
  - 방향 표시가 확인 가능함.
  - 단, 현재 화살표는 크게 보이므로 추후 크기/두께 체감 튜닝 권장.
- [x] Unity 컴파일 에러 없음 확인.

### 수직 HP바 추가

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Backup/Scripts/GameScoreController_backup_20260526_before_vertical_hp_bar.cs`
- [x] `GameScoreController.cs` HUD 수정.
  - 기존 숫자 HP 표기는 유지.
  - 오른쪽에 세로 HP바 추가.
  - HP가 높으면 초록, 중간이면 노랑, 낮으면 빨강으로 표시.
  - 35% 지점에 경고선을 추가해 위험 구간을 직관적으로 확인할 수 있도록 구성.
- [x] Play Mode 캡처 확인.
  - `Assets/Screenshots/test_play_game.png`
  - 테스트 중 HP가 낮아진 상태에서 오른쪽 세로 HP바가 빨간색으로 표시됨 확인.
- [x] Unity 컴파일 에러 없음 확인.

### 직접 테스트 전 진단 스크립트 정리

#### 작업 내용
- [x] 직접 테스트 플레이에 방해될 수 있는 판정 검증용 Editor 스크립트 제거.
  - `Assets/Scripts/Editor/RunJudgementPlaytest.cs`
  - `Assets/Scripts/Editor/CheckJudgementInPlayMode.cs`
  - `Assets/Scripts/Editor/DiagnoseScoreControllerRuntime.cs`
- [x] 제거 이유.
  - `RunJudgementPlaytest.cs`: Play Mode를 강제로 시작하고 판정 테스트를 실행함.
  - `CheckJudgementInPlayMode.cs`: 런타임 Score/HP 상태를 초기화하고 Hit/BAD/MISS를 강제로 호출함.
  - `DiagnoseScoreControllerRuntime.cs`: 런타임 상태 진단 전용으로 실제 테스트에는 불필요함.
- [x] 확인.
  - 제거 대상 파일이 존재하지 않음 확인.
  - Unity Play Mode 꺼짐 확인.
  - Unity 컴파일 에러 없음 확인.

### 테스트 플레이용 Game BGM 음소거

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Backup/Scripts/GameBackgroundController_backup_20260526_before_mute_bgm_test.cs`
  - `Assets/Scenes/Backup/Game_backup_20260526_before_mute_bgm_test.unity`
- [x] `GameBackgroundController.cs` 수정.
  - `muteBgmForTesting` 옵션 추가.
  - BGM AudioSource는 재생하되 `mute=true`로 설정해 노트 스폰/곡 종료 타이밍은 유지하고 소리만 끔.
- [x] Game 씬 설정.
  - `muteBgmForTesting: true`
- [x] Unity Play Mode 꺼짐 및 컴파일 에러 없음 확인.

### Intro 씬 Mute 토글 버튼 추가

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Backup/Scripts/IntroManager_backup_20260526_before_mute_button.cs`
  - `Backup/Scripts/GameBackgroundController_backup_20260526_before_mute_button.cs`
  - `Assets/Scenes/Backup/Intro_backup_20260526_before_mute_button.unity`
- [x] `IntroManager.cs` 수정.
  - `BgmMuted` PlayerPrefs 기반 Mute 상태 저장 추가.
  - `ToggleMute()` 추가.
  - Intro BGM mute 적용 및 버튼 텍스트 `MUTE ON` / `MUTE OFF` 갱신.
- [x] `GameBackgroundController.cs` 수정.
  - Game 씬 BGM도 `BgmMuted` PlayerPrefs 값을 읽어 mute 적용.
  - 기본값은 현재 테스트 상황에 맞춰 mute ON.
- [x] Intro 씬 Canvas에 `MuteButton` 추가.
  - 버튼 이벤트를 `IntroManager.ToggleMute()`에 연결.
  - `IntroManager.muteButtonText` 연결.
- [x] `SetGameBgmMuteForTesting.cs` 제거.
  - Intro Mute 버튼으로 대체되어 별도 테스트용 음소거 스크립트가 불필요해짐.
- [x] 확인.
  - Intro 씬에 `MuteButton`, `ToggleMute` 이벤트, `MUTE ON` 텍스트, `muteButtonText` 참조 반영 확인.
  - Unity 컴파일 에러 없음.

### 테스트 피드백 반영 - UI 높이, 색상 판정, 이펙트, HUD 조정

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Assets/Scenes/Backup/Intro_backup_20260526_before_playtest_feedback_fix.unity`
  - `Assets/Scenes/Backup/Game_backup_20260526_before_playtest_feedback_fix.unity`
  - `Backup/Scripts/Saber_backup_20260526_before_playtest_feedback_fix.cs`
  - `Backup/Scripts/GameScoreController_backup_20260526_before_playtest_feedback_fix.cs`
- [x] Intro 씬 UI 위치 조정.
  - Canvas Y 위치를 더 낮춰 `-0.22`로 조정.
- [x] 세이버 색상 판정 확인 및 정리.
  - `Red` 레이어: `64`
  - `Blue` 레이어: `128`
  - `RED.prefab`: Red 레이어
  - `BLUE.prefab`: Blue 레이어
  - Game 씬 `Blue Neon Blade`: Blue 레이어 마스크
  - Game 씬 `Red Neon Blade`: Red 레이어 마스크
- [x] 히트 이펙트 화면 가림 완화.
  - `Saber.hitEffectScale=0.18`
  - `Saber.hitEffectLifetime=0.55`
- [x] Game HUD 화면 가림 완화.
  - HUD 위치를 오른쪽 아래로 이동.
  - HUD 전체 크기와 스케일 축소.
  - 수직 HP바 크기 축소.
- [x] Unity Play Mode 꺼짐 및 컴파일 에러 없음 확인.

### 테스트 피드백 반영 - HUD 분리 배치 및 블레이드 색상 수정

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Backup/Scripts/GameScoreController_backup_20260526_before_hud_layout_split.cs`
  - `Backup/Scripts/Saber_backup_20260526_before_blade_color_swap.cs`
  - `Assets/Scenes/Backup/Game_backup_20260526_before_hud_layout_blade_color_fix.unity`
- [x] `GameScoreController.cs` HUD 생성 구조 변경.
  - Score HUD: 중앙 상단.
  - Combo HUD: 좌측 중앙.
  - HP HUD: 우측 중앙.
  - 기존처럼 한 Canvas 안에 몰아서 표시하지 않고, 카메라 하위에 HUD Canvas 3개를 분리 생성.
- [x] HUD 폰트 가독성 보강.
  - TMP Bold 적용.
  - 검은 outline 추가.
- [x] `Saber.cs` 블레이드 색상 결정 기준 수정.
  - 오브젝트 이름/손 기준 대신 실제 판정 LayerMask 기준으로 블레이드 색상 결정.
  - Blue 레이어 마스크면 파란 블레이드, Red 레이어 마스크면 빨간 블레이드.
- [x] Game 씬 직렬화 값 반영.
  - `hudDistance=1.9`
  - `hudScale=0.0011`
  - Blue/Red 세이버 LayerMask 및 이펙트 축소값 유지.
- [x] Unity Play Mode 꺼짐 및 컴파일 에러 없음 확인.

### 현재 보유 에셋 기반 HUD 스타일 적용

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Backup/Scripts/GameScoreController_backup_20260526_before_asset_based_hud.cs`
  - `Assets/Scenes/Backup/Game_backup_20260526_before_asset_based_hud.unity`
- [x] 현재 프로젝트에 포함된 `VRTemplateAssets` UI 에셋을 HUD에 적용.
  - 패널: `Round Radius 10.png`
  - 패널 외곽선: `Round Radius 10 Outline.png`
  - HP 프레임: `Circle_60x60_Vertical.png`
  - 폰트: `Inter-Regular SDF.asset`
- [x] `GameScoreController.cs` 런타임 HUD 생성 로직 수정.
  - Score/Combo/HP HUD에 반투명 패널과 색상 외곽선 추가.
  - HP바 배경에 세로 UI 스프라이트 적용.
  - HUD 텍스트에 Inter TMP 폰트 연결 슬롯 추가.
- [x] Game 씬 `GameScoreController`에 에셋 참조 저장.
  - `panelSprite`, `panelOutlineSprite`, `hpFrameSprite`, `hudFont` 직렬화 참조 반영 확인.
- [x] Unity 콘솔 확인.
  - 컴파일 에러 없음.
  - 현재 작업 관련 신규 Error 없음.

### 테스트 피드백 반영 - 컨트롤러 Ray, 세이버 색상, 결과 화면

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Assets/Scenes/Backup/Intro_backup_20260526_before_ray_saber_result_fix.unity`
  - `Assets/Scenes/Backup/Game_backup_20260526_before_ray_saber_result_fix.unity`
  - `Backup/Scripts/Saber_backup_20260526_before_ray_saber_result_fix.cs`
  - `Backup/Scripts/GameScoreController_backup_20260526_before_result_panel.cs`
  - `Backup/Scripts/GameSongEndController_backup_20260526_before_result_panel.cs`
- [x] Intro 씬 컨트롤러 Ray 정리.
  - 기존 `Teleport Interactor`가 메뉴에서 별도 Ray를 표시하지 않도록 비활성화.
  - `Left_NearFarInteractor`, `Right_NearFarInteractor`는 활성 유지.
  - 컨트롤러용 `VisibleUIPointer`의 Ray 원점을 각 컨트롤러 Transform으로 재연결.
- [x] 세이버 색상 갱신 로직 보강.
  - 실제 판정 LayerMask 기준으로 컨트롤러/블레이드 이름과 시각 색상을 매번 갱신.
  - 기존에 생성된 손잡이/Emitter 색상도 재사용 시 현재 색상으로 갱신되도록 수정.
- [x] 곡 종료 결과 화면 추가.
  - 곡 종료 후 바로 Intro로 이동하지 않고 `RESULT` 화면을 먼저 표시.
  - Score, Max Combo, Hit, Bad, Miss, Accuracy 표시.
  - `OK` 버튼을 누르면 Intro 씬으로 이동.
- [x] 확인.
  - Unity Play Mode 꺼짐.
  - 컴파일 에러 없음.
  - 수정 후 Intro의 Teleport Interactor 비활성, NearFar UI Interactor 활성 확인.

### 결과 화면 OK 입력 보강

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Backup/Scripts/GameScoreController_backup_20260526_before_result_ok_input_fix.cs`
  - `Assets/Scenes/Backup/Game_backup_20260526_before_result_ok_input_fix.unity`
- [x] 결과 화면 입력 보강.
  - OK 버튼 클릭 외에 키보드 `Enter`, `Numpad Enter`, `Space`로 Intro 복귀 가능하도록 추가.
  - Quest 컨트롤러의 `triggerButton`, `primaryButton` 입력으로 Intro 복귀 가능하도록 추가.
  - 결과 화면이 뜨는 순간 트리거를 누르고 있던 상태로 바로 넘어가지 않도록, 입력을 한 번 놓은 뒤 다시 누를 때만 처리.
- [x] 확인.
  - Unity Play Mode 꺼짐.
  - 컴파일 에러 없음.
  - 현재 작업 관련 신규 Error 없음.

### HP바 소모 표시 및 HP 0 실패 처리 보강

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Backup/Scripts/GameScoreController_backup_20260526_before_hp_deplete_fail_stop.cs`
- [x] HP바 표시 방식 수정.
  - 기존 색상 변경 중심 표시에서 `HpBarFill` RectTransform 높이가 HP 비율에 따라 줄어드는 방식으로 변경.
  - HP 100에서는 전체 높이, HP 0에서는 채움 높이 0으로 표시되도록 수정.
- [x] HP 0 실패 처리 추가.
  - HP가 0 이하가 되면 `gameFailed=true` 처리.
  - `Spawner` 비활성화.
  - BGM AudioSource 정지.
  - 남아있는 노트 제거.
  - 잠시 후 `FAILED` 결과 화면 표시.
- [x] 확인.
  - Unity Play Mode 꺼짐.
  - 컴파일 에러 없음.
  - 현재 작업 관련 신규 Error 없음.

### Game 씬 UI 오브젝트 기반 관리 구조로 전환

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Backup/Scripts/GameScoreController_backup_20260527_before_scene_ui_objects.cs`
  - `Assets/Scenes/Backup/Game_backup_20260527_before_scene_ui_objects.unity`
- [x] Game 씬에 직접 편집 가능한 UI 오브젝트 생성.
  - `Game UI Root`
  - `Score HUD`
  - `Combo HUD`
  - `HP HUD`
  - `Result HUD`
  - `ResultOkButton`
- [x] `GameScoreController.cs` 구조 변경.
  - 런타임 UI 생성 방식 제거.
  - 씬에 배치된 UI 오브젝트 참조를 받아 텍스트/HP바만 갱신하도록 수정.
  - 게임 중에는 `Score HUD`, `Combo HUD`, `HP HUD` 활성화.
  - 결과 표시 시에는 Score/Combo/HP HUD 비활성화 후 `Result HUD` 활성화.
- [x] `BuildGameSceneUiObjects.cs` 추가.
  - Game 씬 UI 오브젝트 생성 및 `GameScoreController` 참조 연결용 에디터 스크립트.
- [x] 확인.
  - Game 씬에 UI 오브젝트 생성 및 직렬화 참조 연결 확인.
  - Unity Play Mode 꺼짐.
  - 컴파일 에러 없음.

### Game 씬 결과 화면 컨트롤러 OK 입력 보강

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Assets/Scenes/Backup/Game_backup_20260527_before_game_result_xr_ui_fix.unity`
  - `Backup/Scripts/ControllerPointerVisualizer_backup_20260527_before_game_result_xr_ui_fix.cs`
- [x] Game 씬 컨트롤러 UI 입력 구성 추가.
  - `Left Controller` 아래 `Left_NearFarInteractor` 추가.
  - `Right Controller` 아래 `Right_NearFarInteractor` 추가.
  - 양쪽 컨트롤러에 `VisibleUIPointer` 추가.
  - `VisibleUIPointer`의 목표 평면을 `Result HUD`로 연결.
  - 컨트롤러 UI Interactor의 `m_BlockUIOnInteractableSelection` 비활성화 확인.
- [x] 확인.
  - Game 씬 직렬화에서 `Left_NearFarInteractor`, `Right_NearFarInteractor`, `VisibleUIPointer` 확인.
  - Unity Play Mode 꺼짐.
  - 컴파일 에러 없음.

### Ray 표시 모드 전환, Intro Ray 원점, 초기 노트 겹침 수정

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Backup/Scripts/GameScoreController_backup_20260527_before_mode_toggle.cs`
  - `Backup/Scripts/Spawner_backup_20260527_before_initial_spawn_delay.cs`
  - `Assets/Scenes/Backup/Game_backup_20260527_before_mode_toggle_spawn_fix.unity`
  - `Assets/Scenes/Backup/Intro_backup_20260527_before_controller_ray_origin_fix.unity`
- [x] Game 씬 Ray/세이버 표시 모드 전환 추가.
  - 게임 중에는 `VisibleUIPointer`, `NearFarInteractor` 계열 Result UI Ray를 비활성화.
  - 결과 화면 표시 시 Result UI Ray를 활성화.
  - 결과 화면 표시 시 `Saber` 컴포넌트와 세이버 시각 Renderer 계열을 비활성화.
- [x] Intro 씬 Ray 원점 재정리.
  - `Left_NearFarInteractor`, `Right_NearFarInteractor`의 Ray Origin을 각 컨트롤러 Transform 기준으로 재연결.
  - `VisibleUIPointer`의 Ray Origin도 각 컨트롤러 Transform 기준으로 재연결.
  - `Teleport Interactor`는 비활성 유지.
- [x] Retrowave VHS 초기 노트 겹침 완화.
  - `Spawner.firstSpawnBeat=1` 추가.
  - BPM 동기화 스폰 시작 beat를 0이 아닌 1로 초기화해 게임 시작 직후 즉시 겹쳐 나오는 노트를 방지.
- [x] 확인.
  - Game 씬에 `firstSpawnBeat: 1` 저장 확인.
  - Unity Play Mode 꺼짐.
  - 컴파일 에러 없음.

### Game 씬 스포너 높이 조정

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Assets/Scenes/Backup/Game_backup_20260527_before_spawner_height_fix.unity`
- [x] Game 씬 `Spawner` 위치 조정.
  - 플레이어 시점에서 노트가 낮게 들어오는 문제를 완화하기 위해 `Spawner` 로컬 Y 위치를 `1.0`에서 `1.45`로 올림.
  - 네 개 스폰 포인트가 `Spawner` 하위 오브젝트이므로 전체 노트 라인이 함께 상승하도록 처리.

### Intro 씬 컨트롤러 Pointer Ray 방향 보강

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Backup/Scripts/ControllerPointerVisualizer_backup_20260527_before_pointer_direction_fix.cs`
- [x] `ControllerPointerVisualizer`의 Ray 방향 계산 보강.
  - 기존에는 `rayOrigin.forward` 기준으로 Ray를 그려 XR 컨트롤러 루트 축이 실제 가리키는 방향과 다르면 헤드/정면 기준처럼 느껴질 수 있었음.
  - `targetPlane`이 있을 때는 `rayOrigin` 위치에서 UI 평면 중심 방향으로 Ray를 그리도록 변경.
  - Ray 시작점은 기존처럼 컨트롤러 기준을 유지.
- [x] 확인.
  - Unity Play Mode 꺼짐.
  - 컴파일 에러 없음.

### Intro 씬 보조 Pointer 선 비활성화

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Assets/Scenes/Backup/Intro_backup_20260527_before_remove_visible_ui_pointer.unity`
- [x] Intro 씬 보조선 제거.
  - 기본 XR Ray가 정상으로 보이는 상태에서 정중앙에 고정된 보조선이 남아 있어 `VisibleUIPointer` 두 개를 비활성화.
  - `Video`, `Cube (1)` 등 다른 오브젝트 활성 상태는 유지하도록 재확인.
- [x] 확인.
  - `VisibleUIPointer` 두 개의 `m_IsActive: 0` 확인.
  - Unity Play Mode 꺼짐.
  - 컴파일 에러 없음.

---

## 2026-05-27

### Intro 씬 XR UI Ray 원점 수정 — 컨트롤러 기준으로 고정

#### 배경
- Intro 씬에서 UI Ray가 컨트롤러가 아닌 헤드/카메라 쪽에서 나가는 것처럼 보이는 문제 보고.
- XR Origin 아래에 구형 `Near-Far Interactor`(NearFarInteractor 컴포넌트 disabled)와 신형 `Left_NearFarInteractor`/`Right_NearFarInteractor`가 공존하는 구조였음.
- 구형 인터랙터의 `CurveVisualController`가 활성 상태로 남아 있어 런타임에서 시각적 혼선 발생 가능.
- 손 추적용 `Near-Far Interactor`(Left/Right Hand 하위)의 GO도 활성 상태였음.

#### 분석 결과

| 오브젝트 | 기존 상태 | 문제 |
|---|---|---|
| `Camera Offset/Gaze Interactor` | 이미 비활성 | — |
| `Camera Offset/Gaze Stabilized` | 이미 비활성 | — |
| `Left Controller/Teleport Interactor` | 이미 비활성 | — |
| `Right Controller/Teleport Interactor` | 이미 비활성 | — |
| `Left Controller/Near-Far Interactor` | **GO 활성, 컴포넌트 disabled** | CurveVisualController 활성으로 시각 혼선 가능 |
| `Right Controller/Near-Far Interactor` | **GO 활성, 컴포넌트 disabled** | 동일 |
| `Left Hand/Near-Far Interactor` | **GO 활성, 컴포넌트 disabled** | Pinch 기반 lineOrigin이 Head 위치로 fallback 가능 |
| `Right Hand/Near-Far Interactor` | **GO 활성, 컴포넌트 disabled** | 동일 |
| `Left Controller Teleport Stabilized Origin` | **GO 활성** | 불필요한 Helper GO |
| `Right Controller Teleport Stabilized Origin` | **GO 활성** | 불필요한 Helper GO |

#### 검증 결과 (유지 대상)

| 오브젝트 | 상태 | 비고 |
|---|---|---|
| `Left Controller/Left_NearFarInteractor` | ✅ 활성 | NearFarInteractor.enabled=true, castOrigin=컨트롤러 기준 |
| `Right Controller/Right_NearFarInteractor` | ✅ 활성 | NearFarInteractor.enabled=true, castOrigin=컨트롤러 기준 |
| `Left Controller/VisibleUIPointer` | ✅ 활성 | rayOrigin=Left Controller ✓ |
| `Right Controller/VisibleUIPointer` | ✅ 활성 | rayOrigin=Right Controller ✓ |

#### 수정 내용
- [x] 수정 전 씬 백업 생성.
  - `Assets/Scenes/Intro.unity.bak` (프로젝트 루트 인접, 동일 경로)
- [x] `Assets/Scripts/Editor/FixIntroRayOrigin.cs` 추가.
  - Unity Editor API 기반 일괄 수정 스크립트.
  - 비활성화 대상 찾아 `SetActive(false)` 적용 + 활성 유지 대상 검증 + rayOrigin/castOrigin 검증.
  - 수정 후 Intro 씬 자동 저장.
- [x] `FixIntroRayOrigin.Execute()` 실행 결과:
  - **신규 비활성화 6개**:
    - `Camera Offset/Left Controller/Near-Far Interactor`
    - `Camera Offset/Right Controller/Near-Far Interactor`
    - `Camera Offset/Left Hand/Near-Far Interactor`
    - `Camera Offset/Right Hand/Near-Far Interactor`
    - `Camera Offset/Left Controller Teleport Stabilized Origin`
    - `Camera Offset/Right Controller Teleport Stabilized Origin`
  - **이미 비활성 확인 4개**: Gaze Interactor, Gaze Stabilized, Left/Right Teleport Interactor
  - **활성 유지 확인 4개**: Left_NearFarInteractor, Right_NearFarInteractor, 좌/우 VisibleUIPointer
  - **rayOrigin 검증**: 좌/우 VisibleUIPointer → 각 컨트롤러 Transform 참조 ✓
  - **castOrigin 검증**: 좌/우 NearFarInteractor → 카메라 아닌 컨트롤러 기준 ✓
- [x] `Assets/Scenes/Intro.unity` 저장 완료.
- [x] 컴파일 에러 없음 확인.

#### 다음 확인
- [ ] Quest 3S 실기에서 컨트롤러에서만 UI Ray가 나가는지 확인.
- [ ] 컨트롤러 Ray로 Intro 버튼(Prev/Next/Start/Mute) 클릭 정상 동작 확인.

### Intro 씬 VisibleUIPointer Ray 불가시 문제 수정 (URP 셰이더)

#### 원인 분석

- 구형 `Near-Far Interactor`를 비활성화하기 전에는 해당 GO 하위 `LineVisual/CurveVisualController`가 XRI Starter Assets의 URP 호환 머티리얼을 사용해 Ray를 시각적으로 표시하고 있었음.
- 비활성화 후 남은 `VisibleUIPointer`의 `ControllerPointerVisualizer`가 런타임에 `LineRenderer`를 생성하지만, 셰이더로 `Sprites/Default`를 사용해 **URP 환경에서 렌더링되지 않는** 문제 발견.

#### 수정 내용

- [x] `Assets/Scripts/ControllerPointerVisualizer.cs` 수정.
  - 기존: `Shader.Find("Sprites/Default")` → URP에서 LineRenderer 불가시
  - 수정: `Shader.Find("Universal Render Pipeline/Particles/Unlit")` 우선 사용, fallback으로 `Particles/Standard Unlit` → `Sprites/Default` 순서로 탐색.
  - `Universal Render Pipeline/Particles/Unlit`은 LineRenderer의 vertex color 그라디언트를 지원하여 startColor/endColor 모두 정상 표시됨.

#### 확인

- [x] 컴파일 에러 없음 (문법 변경 없음, `Shader.Find()` + null 병합 연산자만 사용).
- [ ] Unity Editor Play Mode에서 Intro 씬 Ray가 보이는지 확인 필요.

### Intro 씬 기본 XR Ray 복구

#### 원인
- 정중앙에 고정된 보조선(`VisibleUIPointer`)을 비활성화한 뒤 기본 Ray까지 사라진 것처럼 보인 원인은, 기본 Ray를 담당하는 좌/우 `NearFarInteractor` GameObject가 이전 정리 과정에서 비활성화되어 있었기 때문으로 확인.

#### 수정 내용
- [x] 수정 전 씬 백업 생성.
  - `Assets/Scenes/Backup/Intro_backup_20260527_before_restore_default_xr_ray.unity`
- [x] 보조선 `VisibleUIPointer` 두 개는 비활성 상태 유지.
- [x] 좌/우 기본 XR Ray 대상 `Left_NearFarInteractor`, `Right_NearFarInteractor` GameObject만 다시 활성화.

#### 확인
- [x] `VisibleUIPointer` 두 개 `m_IsActive: 0` 유지 확인.
- [x] 좌/우 `NearFarInteractor` prefab instance `m_IsActive: 1` 확인.
- [x] Unity Play Mode 꺼짐, 컴파일 에러 없음 확인.

### Intro/Game 씬 카메라 정적 점검

#### 배경
- 사용자가 Ray가 헤드 기준처럼 보인 원인을 HMD 기준 카메라가 아닌 일반 카메라 시점 사용 가능성으로 확인.
- 실제 기기 없이 진행 가능한 범위에서 씬 파일 기준 카메라 참조를 점검.

#### 확인 내용
- [x] `Assets/Scenes/Intro.unity`
  - 별도 일반 Camera GameObject 직렬화 없음.
  - XR Origin 프리팹에서 온 Camera(`fileID: 300037366`)가 존재.
  - Intro UI Canvas가 해당 Camera를 참조함.
- [x] `Assets/Scenes/Game.unity`
  - 별도 일반 Camera GameObject 직렬화 없음.
  - XR Origin 프리팹에서 온 Camera(`fileID: 441087510`)가 존재.
  - Game UI Canvas들이 해당 Camera를 참조함.
- [x] Unity Play Mode 꺼짐, 컴파일 에러 없음 확인.

#### 남은 확인
- [~] Quest 3S 실기에서 실제 렌더링 기준이 `XR Origin Hands (XR Rig)` 하위 HMD/Main Camera인지 확인.
- [~] Hierarchy에서 일반 Camera가 활성 상태로 남아 `MainCamera` 태그나 Audio Listener를 점유하지 않는지 최종 확인.

### Intro/Game 씬 XR 카메라 참조 보정

#### 작업 내용
- [x] 수정 전 씬 백업 생성.
  - `Assets/Scenes/Backup/Intro_backup_20260527_before_xr_camera_fix.unity`
  - `Assets/Scenes/Backup/Game_backup_20260527_before_xr_camera_fix.unity`
- [x] `Assets/Scripts/Editor/FixXRCameraReferences.cs` 추가.
  - 씬별 XR Origin 하위 Camera를 기준 카메라로 탐색.
  - XR Origin 카메라를 활성화하고 `MainCamera` 태그 기준으로 정리.
  - XR Origin 카메라에 AudioListener가 없으면 추가하고, 다른 AudioListener는 비활성화.
  - Screen Space Overlay가 아닌 Canvas의 `worldCamera`를 XR Origin 카메라로 재지정.
- [x] `FixXRCameraReferences.Execute()` 실행.

#### 확인
- [x] Intro 씬 기준 카메라: `XR Origin Hands (XR Rig)/Camera Offset/Main Camera`.
- [x] Game 씬 기준 카메라: `XR Origin (XR Rig)/Camera Offset/Main Camera`.
- [x] Unity Play Mode 꺼짐, 컴파일 에러 없음 확인.

### README 완성도 문구 추가 및 커밋 준비

#### 작업 내용
- [x] 수정 전 README 백업 생성.
  - `Backup/Documents/README_backup_20260527_before_progress_update.md`
- [x] `README.md`에 현재 완성도 섹션 추가.
  - 프로토타입 기준 핵심 플레이 흐름 약 70% 구현 상태로 표기.
  - 완료/부분 완료/확인 필요/개선 예정 항목 정리.
- [x] 커밋 전 상태 확인.
  - `Assets/_Recovery`, `Assets/Scenes/Intro.unity.bak` 계열은 커밋 대상에서 제외 예정.
- [x] `.gitignore` 보강.
  - 수정 전 백업: `Backup/Documents/gitignore_backup_20260527_before_recovery_ignore.md`
  - `Assets/_Recovery/`, `Assets/_Recovery.meta`, `*.unity.bak`, `*.unity.bak.meta` 제외 규칙 추가.

### 세이버 중앙 상시 VFX 제거

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Assets/Scenes/Backup/Game_backup_20260527_before_remove_saber_weapon_vfx.unity`
  - `Backup/Scripts/ApplyNeonSaberVisuals_backup_20260527_before_remove_weapon_vfx.cs`
- [x] Game 씬에서 좌/우 세이버 하위 `Saber Weapon VFX` 프리팹 인스턴스 제거.
- [x] `Assets/Scripts/Editor/ApplyNeonSaberVisuals.cs` 수정.
  - 세이버 비주얼 재적용 시 `FX_Weapon Effect` 프리팹을 다시 붙이지 않도록 관련 로직 제거.
- [x] `Assets/Scripts/Editor/RemoveSaberWeaponVFX.cs` 추가.
  - Game 씬의 `Saber Weapon VFX` 오브젝트를 일괄 제거하는 Editor 실행 스크립트.

#### 확인
- [x] `Assets/Scenes/Game.unity`에 `Saber Weapon VFX` / `FX_Weapon Effect` 참조가 남지 않음 확인.
- [x] Unity Play Mode 꺼짐, 컴파일 에러 없음 확인.

### Intro 씬 HMD 카메라 추적 보정

#### 작업 내용
- [x] 수정 전 씬 백업 생성.
  - `Assets/Scenes/Backup/Intro_backup_20260527_before_hmd_camera_tracking_fix.unity`
- [x] `Assets/Scripts/Editor/FixIntroHMDCameraTracking.cs` 추가 및 실행.
  - `XR Origin Hands (XR Rig)/Camera Offset/Main Camera`를 활성화.
  - Main Camera를 `MainCamera` 태그 / Stereo Target Eye Both로 정리.
  - Input System `Tracked Pose Driver`를 활성화.
  - `XRI Default Input Actions`의 `XRI Head/Position`, `XRI Head/Rotation`, `XRI Head/Tracking State` 액션 레퍼런스를 연결.
  - Intro UI Canvas의 `worldCamera`를 XR Origin 하위 Main Camera로 재연결.

#### 확인
- [x] Intro 씬 Tracked Pose Driver `m_Enabled: 1` 확인.
- [x] `m_PositionInput`, `m_RotationInput`, `m_TrackingStateInput`이 XRI Default Input Actions 참조를 사용하도록 저장 확인.
- [x] Unity Play Mode 꺼짐, 컴파일 에러 없음 확인.
- [~] Quest 3S 실기에서 HMD 움직임에 따라 Intro 카메라가 이동/회전하는지 확인 필요.

### Intro 씬 Sci-Fi UI 스타일 적용

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Assets/Scenes/Backup/Intro_backup_20260527_before_intro_scifi_ui.unity`
  - `Backup/AssetMeta/20260527_before_intro_scifi_ui/`
  - `Backup/Scripts/ApplyIntroSciFiUIStyle_backup_20260527_*.cs`
- [x] `Sci-fi GUI skin` 에셋 기반으로 Intro Canvas 스타일 적용.
  - `SciFiMenuPanel`, `SciFiTitleGlow`, `SciFiSubtitle`, `SciFiTopAccent`, `SciFiBottomAccent` 장식 오브젝트 추가.
  - `StartButton`, `MuteButton`, `PrevButton`, `NextButton`, `ThumbnailBG`, `TitleText`, `StageNameText` 위치/색상/스프라이트 스타일 정리.
  - 버튼/창/화살표 텍스처를 Sprite로 사용할 수 있도록 import 설정 보정.
- [x] `Assets/Scripts/Editor/ApplyIntroSciFiUIStyle.cs` 추가.
  - Intro UI 스타일을 재적용할 수 있는 Editor 실행 스크립트.
- [x] `Assets/Scripts/Editor/CaptureIntroSciFiUIPreview.cs` 추가.
  - Intro 스타일 적용 결과 캡처용 Editor 실행 스크립트.

#### 확인
- [x] Unity Play Mode 꺼짐, 컴파일 에러 없음 확인.
- [x] 적용 결과 캡처 저장: `Assets/Screenshots/IntroSciFiUI_20260527.png`.
- [~] Quest 3S 실기에서 UI 크기, 버튼 가독성, Ray 입력 영역 체감 확인 필요.

### Intro 씬 UI 크기 확대 조정

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Assets/Scenes/Backup/Intro_backup_20260527_before_intro_ui_scale_up.unity`
  - `Backup/Scripts/ApplyIntroSciFiUIStyle_backup_20260527_before_intro_ui_scale_up.cs`
- [x] Intro Canvas 내부 UI 기준 크기를 `920x620`에서 `1120x760`으로 확대.
- [x] 제목, 스테이지명, 썸네일, 시작/음소거/이전/다음 버튼 크기와 위치를 확대 기준으로 재배치.
- [x] Mute 버튼을 안쪽으로 조정해 우측 가장자리 겹침 위험 완화.

#### 확인
- [x] 스타일 적용 스크립트 재실행 및 씬 저장 완료.
- [x] 적용 결과 캡처 갱신: `Assets/Screenshots/IntroSciFiUI_20260527.png`.
- [x] Unity Play Mode 꺼짐, 컴파일 에러 없음 확인.

### 새 환경 MCP 및 GitHub push 지침 확인

#### 작업 내용
- [x] 2026-05-28 Unity MCP HTTP 서버 프로세스 확인: `python.exe`, `127.0.0.1:8080` LISTENING.
- [ ] 현재 Codex 세션의 MCP 리소스 목록에는 아직 `unityMCP` 리소스가 표시되지 않음. Codex 세션 재연결 또는 MCP 리소스 재로드 필요.
- [x] `AGENTS.md`의 GitHub push 지침을 최신 사용자 지시에 맞게 수정.
  - 원격 push는 사용자가 명시적으로 `업로드해줘`, `푸시해줘`, `GitHub에 올려줘`라고 요청한 경우에만 진행.
  - 명시 요청이 없으면 로컬 변경/상태 확인/커밋 준비까지만 진행.

## 2026-05-28

### 노트 프리팹 런타임 비주얼 동기화

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Backup/Prefabs/20260528_before_note_prefab_runtime_match/RED.prefab`
  - `Backup/Prefabs/20260528_before_note_prefab_runtime_match/BLUE.prefab`
  - `Backup/Scripts/Cube_backup_20260528_before_note_prefab_runtime_match.cs`
- [x] `Assets/Prefab/RED.prefab`, `Assets/Prefab/BLUE.prefab` 수정.
  - 실제 플레이 중 `Cube.cs`가 만들던 `Frame Top/Bottom/Left/Right`, `Cut Arrow Stem/Left/Right`, `Energy Glow` 구성을 프리팹에 직접 추가.
  - 기존 `Sphere` 렌더러는 실제 런타임과 동일하게 비활성화.
  - 루트 Cube 및 장식 렌더러 그림자 비활성화.
- [x] `Assets/Scripts/Cube.cs` 수정.
  - 프리팹에 런타임 노트 비주얼이 이미 있으면 중복 생성하지 않고 기존 자식 오브젝트를 사용하도록 보정.
- [x] `Assets/Scripts/Editor/ApplyRuntimeNoteVisualToPrefabs.cs` 추가.
  - RED/BLUE 프리팹을 런타임 노트 비주얼과 같은 구조로 재적용하는 Editor 실행 스크립트.

#### 확인
- [x] RED/BLUE 프리팹에 런타임 노트 구성요소 이름이 저장됨 확인.
- [x] Unity Play Mode 꺼짐, 컴파일 에러 없음 확인.

### 노트 방향 가시성 개선

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Backup/Prefabs/20260528_before_note_visibility_redesign/RED.prefab`
  - `Backup/Prefabs/20260528_before_note_visibility_redesign/BLUE.prefab`
  - `Backup/Scripts/Cube_backup_20260528_before_note_visibility_redesign.cs`
  - `Backup/Scripts/ApplyRuntimeNoteVisualToPrefabs_backup_20260528_before_note_visibility_redesign.cs`
- [x] RED/BLUE 노트 프리팹 방향 표시 개선.
  - 전면 `Cut Arrow` 막대 크기 확대.
  - 접근 방향 반대 상황에서도 확인 가능하도록 `Cut Arrow Stem/Left/Right Back` 추가.
  - 흰색 `Direction Guide`를 추가해 휘두를 방향 중심선을 더 잘 보이도록 조정.
- [x] `Cube.cs` 런타임 fallback 비주얼도 같은 구조로 보정.
- [x] `ApplyRuntimeNoteVisualToPrefabs.cs` 재적용 스크립트도 동일한 노트 구조를 만들도록 수정.

#### 확인
- [x] RED/BLUE 프리팹에 `Direction Guide`, `Cut Arrow Stem Back` 저장 확인.
- [x] Unity Play Mode 꺼짐, 컴파일 에러 없음 확인.

### 노트 방향 마커 꺾쇠형 개선

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Backup/Prefabs/20260528_before_note_chevron_marker/RED.prefab`
  - `Backup/Prefabs/20260528_before_note_chevron_marker/BLUE.prefab`
  - `Backup/Scripts/Cube_backup_20260528_before_note_chevron_marker.cs`
  - `Backup/Scripts/ApplyRuntimeNoteVisualToPrefabs_backup_20260528_before_note_chevron_marker.cs`
- [x] RED/BLUE 노트 프리팹의 방향 마커를 Y자형 막대 조합에서 큰 꺾쇠형 마커로 변경.
  - `Cut Arrow Stem/Left/Right`, `Direction Guide` 구조를 제거.
  - `Direction Chevron Left/Right`, `Direction Chevron Left/Right Back` 구조로 변경.
  - 기본 방향은 위쪽 꺾쇠(`^`)이며, 스폰 시 90도 회전으로 좌/우/아래 방향이 드러나도록 유지.
- [x] `Cube.cs` 런타임 fallback 비주얼과 `ApplyRuntimeNoteVisualToPrefabs.cs` 재적용 스크립트도 동일 구조로 수정.

#### 확인
- [x] RED/BLUE 프리팹에 `Direction Chevron` 오브젝트 저장 확인.
- [x] 기존 프리팹에서 `Cut Arrow Stem`, `Direction Guide`가 제거됨 확인.
- [x] Unity Play Mode 꺼짐, 컴파일 에러 없음 확인.

#### 커밋 전 최종 확인
- [x] `Assets/Prefab/RED.prefab`, `Assets/Prefab/BLUE.prefab`에 `Direction Chevron Left/Right`, `Direction Chevron Left/Right Back` 저장 확인.
- [x] RED/BLUE 프리팹에서 기존 `Cut Arrow Stem`, `Cut Arrow Left`, `Cut Arrow Right`, `Direction Guide` 검색 결과 없음 확인.
- [x] `Cube.cs`와 `ApplyRuntimeNoteVisualToPrefabs.cs`가 같은 꺾쇠형 방향 마커 구조를 사용함 확인.
- [x] Unity MCP 기준 Play Mode 꺼짐, 컴파일 에러 없음 확인.

### Intro 씬 UI 추가 확대 조정

#### 작업 내용
- [x] 수정 전 백업 생성.
  - `Assets/Scenes/Backup/Intro_backup_20260527_before_intro_ui_scale_1_3x.unity`
  - `Backup/Scripts/ApplyIntroSciFiUIStyle_backup_20260527_before_intro_ui_scale_1_3x.cs`
- [x] 기존 확대 UI 기준으로 Canvas 내부 크기와 주요 UI 요소를 약 1.3배 추가 확대.
- [x] 제목, 스테이지명, 썸네일, 시작/음소거/이전/다음 버튼 위치와 크기를 확대 기준으로 재배치.

#### 확인
- [x] 스타일 적용 스크립트 재실행 및 씬 저장 완료.
- [x] 적용 결과 캡처 갱신: `Assets/Screenshots/IntroSciFiUI_20260527.png`.
- [x] Unity Play Mode 꺼짐, 컴파일 에러 없음 확인.

## 2026-05-28 Beat Sage Retrowave VHS 테스트 구성

### 작업 시간
- 시작: 2026-05-28 21:10 KST
- 종료: 2026-05-28 21:19 KST

### 작업 내용
- [x] 사용자 제공 Beat Sage ZIP을 `Assets/Audio/StageNote/RetrowaveVHS_BeatSage/` 아래에 정리.
- [x] `Info.dat`, `Normal.dat`, `song.ogg`, `cover.jpg`, 원본 ZIP 보관.
- [x] `Normal.dat` 기준 Beat Saber/Beat Sage 노트 차트 ScriptableObject 생성.
  - `RetrowaveVHS_Normal_BeatSageChart.asset`
  - BPM: 120
  - 플레이 가능 노트 수: 274
- [x] Retrowave VHS 스테이지에 Beat Sage 차트와 ZIP 내부 `song.ogg`를 연결.
- [x] Beat Sage 차트 사용 시 생성형 BPM 스폰 대신 차트 시간표 기준으로 RED/BLUE 노트를 스폰하도록 `Spawner` 확장.
- [x] 점수 계산이 Beat Sage 차트 노트 수를 기준으로 기대 노트 수를 계산하도록 보정.

### 확인 결과
- [x] 파일 배치와 StageList 연결 값 확인.
- [~] Unity 배치 검증은 현재 동일 프로젝트가 Unity Editor에 열려 있어 중복 실행이 차단됨.
- [ ] Unity Editor에서 AssetDatabase Refresh 이후 컴파일/Play Mode 확인 필요.

## 2026-05-28 세이버 히트 판정 수정

### 작업 시간
- 시작: 2026-05-28 21:39 KST
- 종료: 2026-05-28 21:44 KST

### 작업 내용
- [x] 피드백: 오른손 파란 블레이드/왼손 빨간 블레이드로 방향에 맞춰 휘둘러도 노트 히트 반응이 없는 문제 확인.
- [x] 원인 후보 확인: 기존 `Saber` 판정이 칼날 전체가 아니라 칼끝 이동 경로만 검사해 실제 블레이드가 노트를 지나가도 판정 누락 가능.
- [x] `Saber.cs` 수정: 현재 칼날 전체와 이전 프레임-현재 프레임 사이의 칼날 샘플 지점을 함께 검사하도록 확장.
- [x] `Game.unity`의 양쪽 세이버 판정값 조정.
  - hitRadius: 0.18 -> 0.28
  - minSwingSpeed: 0.55 -> 0.30
  - directionTolerance: 75 -> 100

### 백업
- `Backup/Scripts/20260528_before_saber_hit_sweep_fix/Saber.cs`
- `Assets/Scenes/Backup/Game_backup_20260528_before_saber_hit_sweep_fix.unity`

### 확인 결과
- [x] Game 씬 설정 반영 확인.
- [~] Unity Editor가 아직 `Saber.cs` 변경 후 스크립트 어셈블리를 갱신하지 않은 상태로 보임. Editor에서 Play Mode 종료 후 Assets Refresh/재컴파일 필요.

## 2026-05-28 세이버 파티클/트레일 제거

### 작업 시간
- 시작: 2026-05-28 21:46 KST
- 종료: 2026-05-28 21:49 KST

### 작업 내용
- [x] 피드백: 칼을 휘두를 때 칼 중앙에서만 보이는 파티클/잔상 제거 요청.
- [x] `Saber.cs`에 `enableHitEffect`, `enableBladeTrail` 옵션 추가.
- [x] 기본값 비활성 상태에서 히트 파티클 생성과 칼날 TrailRenderer 자동 생성/활성화를 막도록 수정.
- [x] `Game.unity`의 양손 Saber `hitEffectPrefab` 참조는 이후 히트 성공 이펙트 복구를 위해 다시 연결.
- [x] `Game.unity`의 Blue/Red Neon Blade `TrailRenderer` 비활성화.

### 백업
- `Backup/Scripts/20260528_before_remove_saber_particles/Saber.cs`
- `Assets/Scenes/Backup/Game_backup_20260528_before_remove_saber_particles.unity`

### 확인 결과
- [x] Spawner는 활성 상태 유지 확인.
- [x] Blue/Red Blade MeshRenderer는 활성 상태 유지 확인.
- [x] Blue/Red Blade TrailRenderer만 비활성화 확인.
- [ ] Unity Editor에서 스크립트 재컴파일 후 실기 확인 필요.

## 2026-05-28 난이도 완화 / 히트 이펙트 복구 / Result Ray 제거

### 작업 시간
- 시작: 2026-05-28 21:49 KST
- 종료: 2026-05-28 21:52 KST

### 작업 내용
- [x] Retrowave VHS Beat Sage 차트 난이도 완화를 위해 `beatSageMinBeatGap` 옵션 추가.
- [x] Retrowave VHS 설정을 `noteSpeed: 3`, `beatSageMinBeatGap: 1`로 조정.
- [x] Beat Sage 원본 플레이 가능 노트 274개 중 최소 비트 간격 필터 적용 시 114개로 줄어드는 것 확인.
- [x] 양손 세이버 판정값을 추가 완화.
  - hitRadius: 0.34
  - minSwingSpeed: 0.25
  - directionTolerance: 120
- [x] 칼 중앙 TrailRenderer/잔상은 비활성 유지하면서 노트 히트 성공 이펙트는 다시 활성화.
- [x] Result 화면 진입 시 `VisibleUIPointer`, `Left_NearFarInteractor`, `Right_NearFarInteractor`가 Result 표시 목록에 포함되지 않도록 정리.
- [x] 시작 시점에 Ray 오브젝트를 바로 끄지 않고, Result 모드 진입 시점에만 비활성화되도록 보정.

### 백업
- `Backup/Scripts/20260528_before_easy_mode_hit_effect_restore/Saber.cs`
- `Backup/Scripts/20260528_before_easy_mode_hit_effect_restore/StageEntry.cs`
- `Backup/Scripts/20260528_before_easy_mode_hit_effect_restore/Spawner.cs`
- `Backup/Scripts/20260528_before_easy_mode_hit_effect_restore/GameScoreController.cs`
- `Backup/Scripts/20260528_before_easy_mode_hit_effect_restore/BeatSaberNoteChart.cs`
- `Backup/Data/20260528_before_easy_mode_hit_effect_restore/StageList.asset`
- `Assets/Scenes/Backup/Game_backup_20260528_before_easy_mode_hit_effect_restore.unity`
- `Backup/Scripts/20260528_before_result_ray_mode_refine/GameScoreController.cs`

### 확인 결과
- [x] `StageList.asset`에서 Retrowave VHS Beat Sage 설정 반영 확인.
- [x] `Game.unity`에서 양손 Saber `enableHitEffect: 1`, `enableBladeTrail: 0`, 히트 이펙트 프리팹 연결 확인.
- [x] `GameScoreController.cs`에서 Result 화면 Ray 오브젝트 제외 처리 확인.
- [ ] Unity Editor에서 스크립트 재컴파일 후 Play Mode로 히트 판정, 히트 이펙트, Result 화면 Ray 제거 확인 필요.

## 2026-05-28 Play 화면 세이버 중복 표시 / 방향 판정 재조정

### 작업 시간
- 시작: 2026-05-28 21:59 KST
- 종료: 2026-05-28 22:04 KST

### 작업 내용
- [x] Play 화면에서 이상한 세이버 2개가 하늘에 떠 보인다는 피드백 확인.
- [x] 원인 후보 정리: Result 전환용 `gameplayModeObjects` 자동 수집이 비활성 세이버 비주얼까지 포함해 다시 켤 수 있는 구조.
- [x] `GameScoreController.cs` 수정: 현재 활성화된 실제 `Saber`와 활성 Renderer만 수집하도록 제한.
- [x] Result 화면에서는 Ray 오브젝트가 Result 표시 목록에 포함되지 않도록 유지.
- [x] 세이버 판정을 접촉 중심이 아니라 실제 프레임 간 스윕 경로 기준으로만 검사하도록 조정.
- [x] 잘못된 방향으로 닿은 경우 여러 노트를 연쇄 판정하지 않고 첫 대상만 BAD 처리하도록 조정.
- [x] 양손 세이버 판정값 재조정.
  - hitRadius: 0.30
  - minSwingSpeed: 0.45
  - directionTolerance: 80

### 백업
- `Backup/Scripts/20260528_before_play_saber_direction_fix/GameScoreController.cs`
- `Backup/Scripts/20260528_before_play_saber_direction_fix/Saber.cs`
- `Assets/Scenes/Backup/Game_backup_20260528_before_play_saber_direction_fix.unity`

### 확인 결과
- [x] `GameScoreController.cs`에서 비활성 오브젝트 포함 수집 제거 확인.
- [x] `Saber.cs`에서 현재 칼날 정지 접촉 검사를 제거하고 스윕 검사만 남긴 것 확인.
- [x] `Game.unity`에서 양손 세이버 판정값 반영 확인.
- [ ] Unity Editor 재컴파일 및 Play Mode에서 떠 있는 세이버 제거, 방향 판정 체감 확인 필요.

## 2026-05-28 세이버 판정 기준점 명시 연결 / 포인터 비활성화

### 작업 시간
- 시작: 2026-05-28 22:05 KST
- 종료: 2026-05-28 22:12 KST

### 작업 내용
- [x] 피드백: 가운데 봉처럼 보이는 오브젝트가 닿으면 HIT, 실제 세이버는 판정 없음, Ray처럼 보이는 오브젝트가 계속 존재함.
- [x] 원인 확인: `Game.unity`의 양손 `Saber` 컴포넌트에서 `bladeRoot` / `bladeTip` 참조가 비어 있었음.
- [x] `Saber.cs` 수정: 자동 바인딩 시 Collider가 있는 임의 자식을 잡지 않고 `Blue Neon Blade` / `Red Neon Blade` 또는 `Neon Blade` / `Energy Blade` 이름의 Renderer만 찾도록 제한.
- [x] `Saber.cs` 수정: 실제 블레이드 기준점을 못 찾으면 컨트롤러 중심으로 판정하지 않고 판정을 중단하도록 수정.
- [x] `Game.unity` 수정: 양손 `Saber.bladeRoot`를 실제 `Blue Neon Blade`, `Red Neon Blade` Transform에 직접 연결.
- [x] `Game.unity` 수정: 양쪽 `VisibleUIPointer` GameObject와 `ControllerPointerVisualizer` 컴포넌트 비활성화.
- [x] 패치 중 잘못 꺼진 `Red Neon Blade`, HP/Score UI 이미지는 다시 활성 상태로 복구 확인.

### 백업
- `Backup/Scripts/20260528_before_explicit_saber_blade_binding/Saber.cs`
- `Assets/Scenes/Backup/Game_backup_20260528_before_explicit_saber_blade_binding.unity`

### 확인 결과
- [x] Blue Saber `bladeRoot` -> `Blue Neon Blade` Transform 연결 확인.
- [x] Red Saber `bladeRoot` -> `Red Neon Blade` Transform 연결 확인.
- [x] `VisibleUIPointer` 2개 비활성화 확인.
- [x] 실제 Blue/Red Neon Blade MeshRenderer 활성 상태 확인.
- [ ] Unity Editor 재컴파일 후 Play Mode에서 중앙 봉 HIT 제거, 실제 세이버 HIT 복구, Ray 제거 확인 필요.

## 2026-05-28 세이버 히트 판정 엄격도 테스트값 적용

### 작업 시간
- 시작: 2026-05-28 22:17 KST
- 종료: 2026-05-28 22:18 KST

### 작업 내용
- [x] 피드백: 현재 히트 판정이 너무 후하게 느껴짐.
- [x] 추천 테스트값을 양손 Saber에 동일 적용.
  - hitRadius: 0.30 -> 0.22
  - minSwingSpeed: 0.45 -> 0.65
  - directionTolerance: 80 -> 50

### 백업
- `Assets/Scenes/Backup/Game_backup_20260528_before_saber_strict_hit_tuning.unity`

### 확인 결과
- [x] Blue Saber 설정 반영 확인.
- [x] Red Saber 설정 반영 확인.
- [ ] Unity Editor Play Mode에서 HIT/BAD 체감 확인 필요.

## 2026-05-28 세이버 블레이드 길이 / 각도 튜닝

### 작업 시간
- 시작: 2026-05-28 22:24 KST
- 종료: 2026-05-28 22:26 KST

### 작업 내용
- [x] 피드백: 블레이드 뒷부분이 너무 길고, 칼을 들고 있는 느낌보다 찌르는 느낌이 강함.
- [x] `Saber.cs`에 블레이드 런타임 배치값을 노출.
  - bladeLocalPosition: `{x: 0, y: 0.06, z: 0.78}`
  - bladeLocalEulerAngles: `{x: -28, y: 0, z: 0}`
  - bladeLocalScale: `{x: 0.026, y: 0.026, z: 1.08}`
  - bladeTipOffset: `0.62`
- [x] 기존 런타임 블레이드 길이 `z scale 1.42`를 `1.08`로 줄여 손 뒤쪽으로 남는 느낌 완화.
- [x] 블레이드를 컨트롤러 정면 축에서 위로 약 28도 기울여 찌르는 느낌 완화.
- [x] 양손 Saber 컴포넌트에 동일 테스트값 반영.

### 백업
- `Backup/Scripts/20260528_before_saber_visual_angle_tuning/Saber.cs`
- `Assets/Scenes/Backup/Game_backup_20260528_before_saber_visual_angle_tuning.unity`

### 확인 결과
- [x] `Saber.cs` 런타임 배치값 반영 확인.
- [x] `Game.unity` 양손 Saber 배치값 반영 확인.
- [ ] Unity Editor 재컴파일 후 Play Mode에서 블레이드 길이/각도 체감 확인 필요.

## 2026-05-28 세이버 손잡이 접합 / 휘두르기 판정 범위 조정

### 작업 시간
- 시작: 2026-05-28 22:31 KST
- 종료: 2026-05-28 22:34 KST

### 작업 내용
- [x] 피드백: 손잡이는 정상 위치지만 블레이드가 앞쪽에 떨어져 보임.
- [x] 블레이드 중심을 손잡이 쪽으로 다시 당김.
  - bladeLocalPosition: `{x: 0, y: 0.06, z: 0.78}` -> `{x: 0, y: 0.025, z: 0.52}`
  - bladeLocalEulerAngles: `{x: -28, y: 0, z: 0}` -> `{x: -18, y: 0, z: 0}`
  - bladeTipOffset: `0.62` -> `0.58`
- [x] 히트 판정이 손잡이 근처까지 잡히지 않도록 블레이드 판정 구간을 중간~끝으로 제한.
  - hitBladeStart: `0.18`
  - hitBladeEnd: `1.0`
- [x] 찌르기 움직임보다 휘두르는 움직임에 맞도록 블레이드 방향과 거의 평행한 움직임은 판정하지 않도록 추가.
  - minSwingToBladeAngle: `32`
- [x] 기존 히트 엄격도 값 유지.
  - hitRadius: `0.22`
  - minSwingSpeed: `0.65`
  - directionTolerance: `50`

### 백업
- `Backup/Scripts/20260528_before_saber_attach_and_swing_range/Saber.cs`
- `Assets/Scenes/Backup/Game_backup_20260528_before_saber_attach_and_swing_range.unity`

### 확인 결과
- [x] `Saber.cs` 손잡이 접합/판정 구간/찌르기 제한 로직 반영 확인.
- [x] `Game.unity` 양손 Saber 설정 반영 확인.
- [ ] Unity Editor 재컴파일 후 Play Mode에서 손잡이-블레이드 접합, 휘두르기 판정 체감 확인 필요.

## 2026-05-28 Retrowave VHS 속도 / 클리어 조건 / 세이버 일체형 재조정

### 작업 시간
- 시작: 2026-05-28 22:36 KST
- 종료: 2026-05-28 22:48 KST

### 작업 내용
- [x] VHS 스테이지 실기 피드백 반영: 노트 날아오는 속도 1.4배 증가.
  - Retrowave VHS noteSpeed: `3.0` -> `4.2`
- [x] 클리어 화면 조건을 조정.
  - BGM이 끝났을 것
  - 노트 출력이 모두 끝났을 것
  - HP가 0보다 클 것
  - 조건 충족 후 약 3초 대기 뒤 Result 화면 표시
- [x] `Spawner.cs`에 BGM 종료 여부와 노트 출력 완료 여부를 외부에서 확인할 수 있는 상태값 추가.
- [x] `GameScoreController.cs`에 클리어 Result 지연 코루틴과 중복 호출 방지 플래그 추가.
- [x] `Spawner` 참조가 없으면 클리어 조건을 확인할 수 없으므로 Result를 예약하지 않도록 안전 처리.
- [x] 블레이드가 손잡이와 정확히 붙어 보이도록 세이버 배치를 다시 일체형 기준으로 조정.
  - bladeLocalPosition: `{x: 0, y: 0, z: 0.61}`
  - bladeLocalEulerAngles: `{x: 0, y: 0, z: 0}`
  - bladeLocalScale: `{x: 0.026, y: 0.026, z: 1.08}`
  - hitBladeStart: `0.3`
  - hitBladeEnd: `1.0`
  - minSwingToBladeAngle: `45`

### 백업
- `Backup/Scripts/20260528_before_vhs_speed_result_integrated_saber/Saber.cs`
- `Backup/Scripts/20260528_before_vhs_speed_result_integrated_saber/Spawner.cs`
- `Backup/Scripts/20260528_before_vhs_speed_result_integrated_saber/GameScoreController.cs`
- `Backup/Data/20260528_before_vhs_speed_result_integrated_saber/StageList.asset`
- `Assets/Scenes/Backup/Game_backup_20260528_before_vhs_speed_result_integrated_saber.unity`
- `Backup/Scripts/20260528_before_clear_result_song_end_rule/Spawner.cs`
- `Backup/Scripts/20260528_before_clear_result_song_end_rule/GameScoreController.cs`
- `Assets/Scenes/Backup/Game_backup_20260528_before_clear_result_song_end_rule.unity`
- `Backup/Scripts/20260528_before_require_spawner_for_clear/GameScoreController.cs`

### 확인 결과
- [x] `StageList.asset` Retrowave VHS noteSpeed `4.2` 반영 확인.
- [x] `Spawner.cs` BGM 종료 / 노트 출력 완료 상태값 반영 확인.
- [x] `GameScoreController.cs` HP > 0, BGM 종료, 노트 출력 완료, 3초 지연 Result 조건 반영 확인.
- [x] `Game.unity` clearResultDelay `3` 및 양손 Saber 일체형 배치값 반영 확인.
- [ ] Unity Editor 재컴파일 후 Retrowave VHS Play Mode 테스트 필요.
- [ ] Quest 3S 실기에서 노트 속도, 클리어 화면 타이밍, 손잡이-블레이드 일체감 확인 필요.

## 2026-05-28 Miss 판정 / Combo 0 표시 / Gameplay HUD 가독성 보정

### 작업 시간
- 시작: 2026-05-28 22:57 KST
- 종료: 2026-05-28 22:58 KST

### 작업 내용
- [x] 기존 시간초과 기반 Miss 판정을 플레이어 기준 좌표 뒤 통과 판정으로 변경.
  - 노트 이동 방향 기준으로 플레이어 기준점보다 `0.6m` 뒤로 지나가면 Miss 처리.
  - Miss 처리 시 기존 `RegisterMiss()` 흐름으로 Combo를 `0`으로 초기화.
- [x] `Spawner`에서 생성한 노트에 Miss 기준 Transform과 뒤 통과 거리값을 전달하도록 수정.
  - 기본 기준점은 `Camera.main.transform`.
  - 필요 시 `Spawner.missReferenceTransform`에 별도 플레이어 기준 Transform 연결 가능.
- [x] Combo가 0일 때도 `0 COMBO`로 표시되도록 수정.
- [x] HP/Combo HUD를 시야 중앙 쪽으로 당기고, Intro UI와 더 비슷한 어두운 네온 패널 톤으로 런타임 보정.

### 백업
- `Backup/Scripts/20260528_before_miss_plane_combo_hud/Cube.cs`
- `Backup/Scripts/20260528_before_miss_plane_combo_hud/Spawner.cs`
- `Backup/Scripts/20260528_before_miss_plane_combo_hud/GameScoreController.cs`

### 확인 결과
- [x] `Cube.cs` 좌표 기반 Miss 판정 로직 반영 확인.
- [x] `Spawner.cs` Miss 기준점 전달 로직 반영 확인.
- [x] `GameScoreController.cs` `0 COMBO` 표시 및 HUD 위치/스타일 런타임 보정 반영 확인.
- [ ] Unity Editor 재컴파일 확인 필요.
- [ ] Play Mode에서 노트가 플레이어 뒤로 지나갈 때 Miss + Combo 0 처리되는지 확인 필요.
- [ ] Play Mode/Quest 3S에서 HP/Combo HUD 위치와 가독성 확인 필요.

## 2026-05-28 Hit HP 회복 / HUD 회복 피드백 추가

### 작업 시간
- 시작: 2026-05-28 23:12 KST
- 종료: 2026-05-28 23:13 KST

### 추천 및 적용값
- [x] 현재 값 확인: Miss HP 피해 `12`, Bad HP 피해 약 `4`.
- [x] `0.5 * Combo`는 20콤보부터 Hit 1회 회복량이 `10`이 되어 난이도를 너무 낮출 수 있음.
- [x] 추천 초기값으로 `0.35 * Combo`, Hit 1회 최대 회복 `6` 적용.

### 작업 내용
- [x] Hit 성공 시 Combo 수에 비례해 HP 회복.
  - hitHpRecoverPerCombo: `0.35`
  - maxHitHpRecover: `6`
  - maxHp 초과 회복 방지
- [x] HP 회복량을 HUD에 약 `0.75`초 표시.
  - 예: `HP 84  +2.1`
  - 판정 라벨 예: `HIT +2.1 HP`
- [x] 회복 중 HP 바를 밝은 민트색으로 잠시 표시.
- [x] Combo 수가 올라갈수록 Combo 텍스트가 흰색에서 붉은 네온 톤으로 변하도록 보정.

### 백업
- `Backup/Scripts/20260528_before_hit_hp_recovery_ui/GameScoreController.cs`

### 확인 결과
- [x] `GameScoreController.cs` 회복 계산/상한/HUD 피드백 반영 확인.
- [ ] Unity Editor 재컴파일 확인 필요.
- [ ] Play Mode에서 Hit 시 HP 회복량, HP 상한, 회복 표시 체감 확인 필요.

## 2026-05-28 Game 씬 UI Sci-Fi 비주얼 업데이트

### 작업 시간
- 시작: 2026-05-28 23:17 KST
- 종료: 2026-05-28 23:19 KST

### 작업 내용
- [x] Intro 씬 UI에서 사용하는 Sci-Fi GUI skin 계열 스타일 확인.
  - `window_transparent.png`
  - `button_active.png`
  - `button_pushed.png`
  - 주요 색상: 청록, 골드, 어두운 반투명 패널
- [x] Game HUD의 Score / Combo / HP 패널을 Intro와 비슷한 Sci-Fi 패널 톤으로 보정.
- [x] Result HUD 패널, 제목, 점수, 통계 텍스트, OK 버튼을 같은 Sci-Fi 톤으로 보정.
- [x] `GameScoreController` 런타임 UI 보정에 Sci-Fi 색상/스프라이트 적용.
- [x] `BuildGameSceneUiObjects`가 이후 Game UI를 재생성할 때도 Sci-Fi UI 에셋을 사용하도록 수정.
- [x] `Game.unity`의 GameScoreController에 Sci-Fi 패널/버튼 스프라이트 참조 연결.

### 백업
- `Backup/Scripts/20260528_before_game_ui_scifi_visual/GameScoreController.cs`
- `Backup/Scripts/20260528_before_game_ui_scifi_visual/BuildGameSceneUiObjects.cs`
- `Assets/Scenes/Backup/Game_backup_20260528_before_game_ui_scifi_visual.unity`

### 확인 결과
- [x] Sci-Fi 스프라이트 에셋 존재 확인.
- [x] `GameScoreController.cs` Game HUD / Result HUD 스타일 보정 코드 반영 확인.
- [x] `BuildGameSceneUiObjects.cs` Sci-Fi UI 재생성 기준 반영 확인.
- [x] `Game.unity` Sci-Fi 스프라이트 참조 반영 확인.
- [ ] Unity Editor 재컴파일 확인 필요.
- [ ] Play Mode에서 Game HUD와 Result HUD가 Intro UI와 자연스럽게 이어지는지 확인 필요.

## 2026-05-28 Game 씬 UI 실제 오브젝트 재확인 및 보정

### 작업 시간
- 시작: 2026-05-28 23:27 KST
- 종료: 2026-05-28 23:32 KST

### 확인 내용
- 이전 반영은 `GameScoreController` 참조와 런타임 보정 중심이라, Unity Editor에서 실제 `Game.unity` UI 오브젝트가 기존 VRTemplate 스프라이트처럼 보일 수 있음을 확인.
- `Game.unity`의 실제 Score / Combo / HP / Result 패널 Image, 텍스트, OK 버튼 스프라이트 상태를 직접 재확인.

### 작업 내용
- Game 씬 UI 패널/아웃라인 실제 오브젝트 스프라이트를 Sci-Fi `window_transparent.png` 계열로 교체.
- Result OK 버튼의 highlighted/pressed/selected 스프라이트를 Sci-Fi 버튼 에셋으로 교체.
- HP 바 배경과 Fill을 Sci-Fi bar 에셋으로 교체.
- `BuildGameSceneUiObjects` 재생성 로직도 HP bar Sci-Fi 에셋을 사용하도록 보정.
- 보정 중 발견한 빈 `m_LocalScale`, 빈 기본 `m_text` 직렬화 값을 정상 값으로 복구.

### 확인 결과
- `Game.unity` 기준 기존 VRTemplate 패널/아웃라인/HP 프레임 GUID 잔존 없음 확인.
- 빈 `m_LocalScale`, 빈 `m_text` 없음 확인.
- Unity Editor 씬 리로드 및 Play Mode 시각 확인 필요.

## 2026-05-28 Game 씬 UI 텍스트 박스 이탈 보정

### 작업 시간
- 시작: 2026-05-28 23:36 KST
- 종료: 2026-05-28 23:42 KST

### 작업 내용
- Game HUD / Result HUD 내부 텍스트가 패널 밖으로 빠져나오는 문제 확인.
- TMP 텍스트 자동 크기 조절을 실제 Game 씬 저장 값에서도 활성화.
- Score / Combo / HP / Miss / Result / OK 버튼 Label 폰트 크기와 RectTransform 크기를 패널 안쪽 여백 기준으로 축소.
- 텍스트 줄바꿈을 끄고 overflow를 Ellipsis로 설정해 박스 밖으로 표시되지 않도록 보정.
- `GameScoreController` 런타임 보정과 `BuildGameSceneUiObjects` 재생성 기준도 동일 값으로 갱신.

### 백업
- Backup/Scripts/20260528_233608_before_ui_text_fit/GameScoreController.cs
- Backup/Scripts/20260528_233608_before_ui_text_fit/BuildGameSceneUiObjects.cs
- Assets/Scenes/Backup/Game_backup_20260528_233608_before_ui_text_fit.unity

### 확인 결과
- `Game.unity` 기준 대상 TMP 텍스트의 `m_enableAutoSizing: 1` 반영 확인.
- 대상 텍스트의 overflow Ellipsis 적용 확인.
- Unity Editor 씬 리로드 후 실제 시각 확인 필요.

## 2026-05-28 Game 씬 UI 패널 세로 여백 확대

### 작업 시간
- 시작: 2026-05-28 23:43 KST
- 종료: 2026-05-28 23:45 KST

### 작업 내용
- Game HUD / Result HUD 패널 스프라이트의 위아래 여백이 부족해 보이는 문제 보정.
- Score HUD: 높이 112 -> 132
- Combo HUD: 높이 136 -> 156
- HP HUD: 높이 232 -> 252
- Result HUD: 높이 442 -> 480
- 각 패널 아웃라인도 패널보다 12px 크게 유지되도록 함께 보정.
- `GameScoreController` 런타임 보정에서 패널/아웃라인 이미지가 HUD 루트 크기를 따라가도록 보강.
- `BuildGameSceneUiObjects` 재생성 기준도 동일한 패널 높이로 갱신.

### 백업
- Backup/Scripts/20260528_2345_before_panel_vertical_padding/GameScoreController.cs
- Backup/Scripts/20260528_2345_before_panel_vertical_padding/BuildGameSceneUiObjects.cs
- Assets/Scenes/Backup/Game_backup_20260528_2345_before_panel_vertical_padding.unity

### 확인 결과
- `Game.unity` 기준 패널/아웃라인 높이 반영 확인.
- 빈 `m_LocalScale`, 빈 `m_text` 없음 확인.
- Unity Editor 씬 리로드 후 시각 확인 필요.

## 2026-05-28 Game 씬 UI 패널 추가 확대 및 Score 가시성 보정

### 작업 시간
- 시작: 2026-05-28 23:50 KST
- 종료: 2026-05-28 23:53 KST

### 작업 내용
- 이전 확대 후에도 패널 위아래 여백이 좁아 보이는 문제 추가 보정.
- Score HUD: 높이 132 -> 168
- Combo HUD: 높이 156 -> 190
- HP HUD: 높이 252 -> 292
- Result HUD: 높이 480 -> 540
- 각 패널 아웃라인도 새 패널 높이에 맞춰 확대.
- Score 패널 배경 알파와 아웃라인 밝기를 올려 대비 강화.
- Score 텍스트 크기 28 -> 34, 텍스트 박스 420x42 -> 460x58, 색상은 더 밝은 백색 계열로 보정.
- `GameScoreController` 런타임 보정과 `BuildGameSceneUiObjects` 재생성 기준을 동일하게 갱신.

### 백업
- Backup/Scripts/20260528_2350_before_panel_padding_score_visibility/GameScoreController.cs
- Backup/Scripts/20260528_2350_before_panel_padding_score_visibility/BuildGameSceneUiObjects.cs
- Assets/Scenes/Backup/Game_backup_20260528_2350_before_panel_padding_score_visibility.unity

### 확인 결과
- `Game.unity` 기준 패널/아웃라인/Score 텍스트 값 반영 확인.
- 빈 `m_LocalScale`, 빈 `m_text` 없음 확인.
- Unity Editor 씬 리로드 후 시각 확인 필요.

## 2026-05-29 main 브랜치 업로드 전 문서 정리 및 시연 영상 기록

### 작업 시간
- 시작: 2026-05-29 KST
- 종료: 2026-05-29 KST

### 작업 내용
- 현재까지 진행한 VR 리듬 게임 프로토타입 내용을 `README.md`, `Task.md`, `Log.md` 기준으로 정리.
- `README.md`가 인코딩 깨짐 상태로 보여 한국어 기준으로 재작성.
- 사용 에셋 및 출처 표기 항목을 한국어로 정리.
- 테스트 시연 영상 일부공개 유튜브 영상 링크를 기록.

### 테스트 시연 영상
- https://youtu.be/3g2RF_wvLGw
- 위 링크는 테스트 시연 영상 일부공개 유튜브 영상 링크임.

### 현재 구현 요약
- Intro 씬 스테이지 선택 및 Game 씬 전환 구현.
- Retrowave Vapor / Orange / VHS 스테이지 구성.
- Retrowave VHS는 Beat Sage 자동 생성 노트 맵을 적용.
- 세이버 시각화, 색상별 노트 판정, 방향/휘두르기 판정, Miss 판정, Combo 초기화 구현.
- Hit 성공 시 Combo 기반 HP 회복 구현.
- 곡 종료, 노트 출력 완료, HP 조건 기반 Result 화면 출력 구현.
- Game HUD / Result UI를 Intro UI와 유사한 Sci-Fi 스타일로 보정.

### 남은 확인 항목
- Quest 3S 실기 기준 장시간 플레이 안정성 확인.
- Quest 3S 실기 기준 Score / Combo / HP / Result UI 가독성 확인.
- Retrowave VHS 난이도, 노트 속도, BGM 볼륨 체감 확인.
- YouTube 업로드 설명란에 에셋 출처 표기 유지.

## 2026-05-29 포트폴리오 Markdown 문서 추가 및 README 링크 정리

### 작업 시간
- 시작: 2026-05-29 00:35:29 +09:00
- 종료: 2026-05-29 00:36:04 +09:00

### 작업 내용
- 포트폴리오 제출용 Markdown 문서 `Docs/Portfolio.md`를 추가.
- `README.md`에 포트폴리오 문서 링크를 추가.
- 포트폴리오 문서에 테스트 플레이 일부공개 유튜브 링크를 포함.
  - https://youtu.be/3g2RF_wvLGw
  - 위 링크는 테스트 시연 영상 일부공개 유튜브 영상 링크임.
- 포트폴리오 문서에 게임 개요, 담당 역할, 수행 내용, 구현 기능, 핵심 기술 포인트, 문제 해결 경험, 개발 환경, 에셋 출처, 향후 보완 사항을 정리.

### 백업
- `Backup/Docs/20260529_before_portfolio_doc_update/README.md`

## 2026-07-14 GitHub 동기화 및 로컬 산출물 ignore 정리

### 작업 내용
- `origin/main`을 fetch 후 확인했고, 로컬 `main`이 원격보다 2커밋 뒤처진 상태임을 확인.
- `git pull --ff-only origin main`으로 로컬 `main`을 `origin/main` 최신 커밋 `5bc62f2358b40206fc01922279341d8504ff4d7a`와 동일하게 동기화.
- README는 최신 원격 커밋에서 시연 영상, 포트폴리오 문서, 구현 내용, 에셋 출처, AI 도구 사용 표기가 이미 정리되어 있어 추가 수정하지 않음.
- 포트폴리오 영상 캡처 산출물과 임시 캡처 스크립트가 Git 추적 후보로 남지 않도록 `.gitignore`에 로컬 산출물 ignore 규칙을 추가.

### 백업
- `Backup/GitIgnore/.gitignore_20260714_before_portfolio_ignore.bak`
