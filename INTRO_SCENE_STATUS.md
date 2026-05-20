# VR Intro 씬 작업 현황 (2026-05-06 17:16 저장)

## 전체 목표
VR Beat Saber의 Intro 씬을 360° 비디오 스카이돔 + World Space UI 패널로 개선.

---

## ✅ 완료된 작업

### 생성된 스크립트 파일
| 파일 | 내용 |
|------|------|
| `Assets/Scripts/IntroManager.cs` | 곡 선택, 페이드인/아웃, Game 씬 전환 |
| `Assets/Scripts/Editor/IntroSceneSetup.cs` | SkyDome360 + BGM 생성 |
| `Assets/Scripts/Editor/IntroUISetup.cs` | World Space Canvas UI 전체 생성 |
| `Assets/Scripts/Editor/IntroFinalSetup.cs` | TrackedDeviceGraphicRaycaster + SongButton 연결 + 스카이돔 회전 |
| `Assets/Scripts/Editor/BuildSettingsSetup.cs` | 빌드 세팅 (Intro→Game) |

### Unity 씬에 적용된 내용 (이전 세션에서 실행 완료)
- `SkyDome360` — 360° 구체, Material360.mat + motion.mp4 VideoPlayer
- `VideoPlayer360` — motion.mp4 루프 재생
- `BGM` — About That Oldie.mp3 배경음
- `IntroPanel` — World Space Canvas (pos: 0,1.5,2.5 / scale: 0.001)
  - 타이틀 "VR BEAT SABER", 서브타이틀, 구분선
  - SongList / SongButton_0 (About That Oldie)
  - SelectedIllustration_Container (일러스트 + 곡명 + 아티스트)
  - StartButton (▶ GAME START → IntroManager.OnStartGame)
  - FadeOverlay (페이드인 효과)
- `IntroManager` — songs, buttons, fadeOverlay, bgmSource 연결됨
- Directional Light 파란빛 분위기 조정

---

## ❌ 미완료 작업 (Unity 재시작 후 실행 필요)

### 우선순위 높음

#### 1. IntroFinalSetup 실행 ⚠️ Unity 재시작 필요
Unity Editor에서 아래 방법 중 하나로 실행:
```
방법A: Console에서 실행
IntroFinalSetup.Execute();

방법B: Edit > Execute Script (Coplay MCP)
Assets/Scripts/Editor/IntroFinalSetup.cs → Execute()
```
**이 스크립트가 하는 일:**
- `GraphicRaycaster` → `TrackedDeviceGraphicRaycaster` 교체 (VR 컨트롤러로 UI 클릭 가능하게)
- `SongButton_0.onClick` → `IntroManager.SelectSong0()` persistent listener 연결
- `SkyDome360`에 느린 Y축 회전 추가

#### 2. BuildSettingsSetup 실행
```
Assets/Scripts/Editor/BuildSettingsSetup.cs → Execute()
```
**빌드 순서: Intro(index 0) → Game(index 1)**

#### 3. 씬 저장 경로 확인
- 이전 세션에서 save_scene이 `Assets/Intro.unity`로 잘못 저장될 수 있음
- Unity에서 Ctrl+S로 `Assets/Scenes/Intro.unity` 수동 저장 권장

### 우선순위 낮음 (선택사항)

#### 4. 곡 일러스트 Sprite 할당
- Inspector에서 `IntroManager` → `songs[0].illustration` 에 Sprite 드래그
- 현재는 단색 플레이스홀더

#### 5. 추가 분위기 효과
- 파티클 시스템 추가 (별, 빛 입자 등)
- 시작 버튼 호버 애니메이션

---

## 씬 구조 참고

```
Intro Scene
├── Directional Light (파란빛, intensity 0.4)
├── XR Interaction Manager
├── EventSystem
├── XR Origin Hands (XR Rig)        ← VR 컨트롤러/손
├── SkyDome360                       ← 360° 비디오 구체
├── VideoPlayer360                   ← motion.mp4 재생
├── BGM                              ← 배경음
├── IntroPanel (World Space Canvas)  ← UI 패널
│   ├── Background
│   ├── Title "VR BEAT SABER"
│   ├── SubTitle
│   ├── Divider
│   ├── SongList
│   │   └── SongButton_0
│   ├── SelectedIllustration_Container
│   │   ├── SongIllustration
│   │   ├── SongTitleLabel
│   │   └── ArtistLabel
│   ├── StartButton ──→ IntroManager.OnStartGame()
│   └── FadeOverlay (CanvasGroup)
├── IntroManager                     ← 메인 컨트롤러
└── Video (기존, 사용 여부 확인 필요)
```

---

## 빠른 체크리스트

- [ ] Unity 열고 Intro 씬 확인
- [ ] `IntroFinalSetup.Execute()` 실행
- [ ] `BuildSettingsSetup.Execute()` 실행  
- [ ] Ctrl+S 씬 저장
- [ ] 플레이 모드에서 VR 컨트롤러로 버튼 클릭 테스트
- [ ] StartButton 클릭 시 Game 씬 전환 확인
- [ ] (선택) 일러스트 Sprite 할당

---

*작업 중단 시간: 2026-05-06 17:16*  
*작업자: Claude Sonnet 4.6*
