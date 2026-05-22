# VRBeatSaber BGM Candidate Notes

다른 환경에서 클론한 뒤에도 후보를 다시 확인할 수 있도록 남기는 BGM 후보 문서이다.
다운로드/임포트 전에는 각 원본 페이지의 최신 라이선스와 사용 조건을 다시 확인한다.

## 적용 기준

- 현재 스테이지 분위기: Retrowave / Synthwave / Neon Grid.
- 리듬게임용이므로 90초 이상이거나 자연스럽게 반복 가능한 loop 음원을 우선 검토한다.
- BPM이 명시되어 있으면 `StageData.bpm`과 `Spawner` 동기화 기준으로 사용한다.
- 음원 파일을 프로젝트에 넣기 전 원본 URL, 라이선스, 작성자 표기 필요 여부를 함께 기록한다.

## 1순위 후보

| 스테이지 | 후보 | 출처 | 확인된 조건 | 적용 메모 |
| --- | --- | --- | --- | --- |
| Retrowave Vapor | Game Synthwave | https://pixabay.com/music/synthwave-game-synthwave-289501/ | Pixabay Content License, 1:52, MP3, Content ID Registered 표시 있음 | Vapor/도시/밤 분위기에 잘 맞음. Content ID Registered 표시가 있으므로 배포 전 실제 플랫폼 정책 재확인 필요. |
| Retrowave Orange | Synthwave House Loop | https://opengameart.org/content/synthwave-house-loop | CC0, 114 BPM, OGG/WAV | Orange 스테이지의 밝은 그리드와 맞는 loop 후보. BPM이 명시되어 있어 노트 동기화가 쉬움. |
| Retrowave VHS | Eyeless (Retrowave) | https://opengameart.org/content/eyeless-retrowave | OpenGameArt 페이지에 CC-BY 4.0 / CC-BY 3.0 / CC0 표기 | VHS/어두운 액션 분위기 후보. 라이선스 표기가 복수라 다운로드 전 원본 파일별 조건 재확인 필요. |

## 추가 탐색 후보

| 후보군 | 출처 | 확인된 조건 | 적용 메모 |
| --- | --- | --- | --- |
| StreamBeats Synthwave catalog | https://streambeats.com/ | DMCA-safe, royalty-free, no attribution required, perpetual license로 안내됨 | 트랙 수가 많아 스테이지별 분위기 비교용으로 좋음. 실제 게임 배포에 사용할 경우 라이선스 페이지 재확인 필요. |
| Nihilore Synthwave | https://www.nihilore.com/synthwave | Creative Commons Attribution 4.0 기반 | 크레딧 표기가 가능하다면 후보로 적합. `README.md` 또는 게임 내 Credits에 작성자/라이선스 표기 필요. |
| Pixabay Synthwave search | https://pixabay.com/music/search/synthwave/ | Pixabay Content License | 빠르게 여러 후보를 비교하기 좋음. 각 곡별 길이와 Content ID 표시 여부 확인 필요. |

## 라이선스 체크리스트

- 상업적/비상업적 사용 가능 여부.
- 저작자 표기 필요 여부와 표기 문구.
- Unity 프로젝트 안에 원본 음원을 포함해 배포해도 되는지.
- Content ID / DMCA / 플랫폼 클레임 가능성.
- 단독 재배포 금지 조건 여부.
- 편집, 루프화, 볼륨 보정, 압축 변환 가능 여부.

## 적용 시 기록할 항목

```text
스테이지:
선택 음원:
원본 URL:
다운로드 날짜:
라이선스:
저작자 표기:
BPM:
길이:
Unity 경로:
비고:
```
