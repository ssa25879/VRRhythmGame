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
