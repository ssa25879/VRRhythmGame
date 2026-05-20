# VR Beat Saber — 작업 로그

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
