### 🛠️ Sprint 1: 최소 기능 구현 (11/13 ~ 11/19)

#### [패키지 1] 핵심 매니저
* - [x] `InputManager` (A-3.a)를 구현합니다.
* - [x] `GetMovementInput()` (WASD) 및 `IsPausePressed()` (ESC) 함수를 정의합니다.
* - [x] `GameManager` (A-2.a)를 싱글톤으로 구현합니다.
* - [x] `GameState` 열거형 (Playing, Paused, GameOver)을 정의합니다.
* - [x] `Update()`에서 게임 시간을 `gameTime` 변수로 추적합니다.

#### [패키지 2] 플레이어 생존
* - [x] `PlayerManager` (B-1.a) 및 `PlayerStats` (B-1.a) 클래스를 생성합니다.
* - [x] `InputManager`의 입력을 받아 `Rigidbody.MovePosition`으로 캐릭터를 이동시킵니다.
* - [x] `TakeDamage(float amount)` 및 `Die()` 함수를 구현합니다.
* - [x] **(중요)** `public event Action<float, float> OnHpChanged` 이벤트를 정의하고, HP 변경 시 호출합니다.

#### [패키지 3] 몬스터 생존
* - [x] `Monster` (C-2.a) 추상 클래스를 정의합니다. (HP, `TakeDamage`, `Die` 포함)
* - [x] `NormalMonster` (C-2.b) 클래스를 구현합니다. ( `Monster` 상속)
* - [x] `Update()`에서 `PlayerManager`를 찾아(`FindObjectOfType` 또는 `Tag`) 해당 방향으로 이동시킵니다.
* - [x] `SpawnManager` (C-1.a)를 구현하여, `NormalMonster` 프리팹을 5초마다 화면 밖 랜덤 위치에 생성합니다.

#### [패키지 4] UI 프리팹 + HUD 연결
* - [x] **(UI 프리팹):** SDS 6.7~6.17 기반의 **모든** 인게임 UI (HUD, 일시정지, 보상, 결과창) 프리팹을 제작합니다.
* - [x] **(로직 연결):** `HUDManager` (D-1.a)를 구현합니다.
* - [x] `PlayerManager`의 `OnHpChanged` 이벤트를 구독하여 HP Slider의 `value`를 업데이트합니다.
* - [x] `GameManager`의 `gameTime`을 참조하여 타이머 Text (D-1.b)를 업데이트합니다.

#### [패키지 5] UI 프리팹 + 설정 연결
* - [x] **(UI 프리팹):** SDS 6.1~6.6 기반의 **모든** 메타 UI (타이틀, 로비, 설정, 강화, 도감) 프리팹을 제작합니다.
* - [x] **(로직 연결):** `AudioManager` (A-4.a) (싱글톤)를 구현합니다. (`PlayBGM`, `PlaySFX` 함수 포함)
* - [x] `SettingManager` (A-5.a) 로직을 구현합니다.
* - [x] `MainMenuPanelManager` (E-2.c)에서 (제작된) 설정 프리팹의 Slider와 `AudioManager`의 볼륨을 연결합니다.

---

### 🔄 Sprint 2: 핵심 프로그레션 루프 (11/20 ~ 11/26)

#### [패키지 1] 게임 흐름
* - [x] `GameManager` (A-2.c)에 `StartGame()` (인게임 씬 로드), `GoToMainMenu()` (메인 씬 로드) 함수를 `SceneManager.LoadScene`을 사용하여 구현합니다.

#### [패키지 2] 성장/공격
* - [x] `PlayerManager`에 `GainExp(int amount)` (B-1.b) 함수를 구현합니다.
* - [x] `currentExp >= maxExp`일 때 `LevelUp()` 함수를 호출하고, `OnPlayerLeveledUp` 이벤트를 발생시킵니다.
* - [x] `GainGold(int amount)` (B-1.c) 함수를 구현합니다.
* - [x] **(핵심)** `Equipment` (B-3.b) 추상 클래스 및 기본 장비 1종(예: 자동 발사 투사체)을 구현합니다. `EquipmentManager`가 이를 관리합니다.

#### [패키지 3] 몬스터 상호작용
* - [x] `Monster` (C-2.a)의 `Die()` 함수를 수정합니다.
* - [x] 사망 시 `PlayerManager.GainExp()` 및 `PlayerManager.GainGold()`를 호출하도록 합니다.
* - [x] 몬스터가 `PlayerManager`의 장비(B-3.b) 공격에 피격당할 수 있도록 `TakeDamage` 로직을 완성합니다.

#### [패키지 4] 보상 루프 완성
* - [x] **(핵심)** `RewardManager` (B-4)를 구현합니다.
* - [x] `PlayerManager.OnPlayerLeveledUp` 이벤트를 구독합니다.
* - [x] 이벤트 수신 시 `GameManager.PauseGame()`을 호출하고, (제작된) 보상 패널(D-2.b)을 활성화합니다.
* - [x] 보상 선택 시 `PlayerManager.AddEquipment()` 또는 `GainExp()`(건너뛰기)를 호출하고 `GameManager.ResumeGame()`을 실행합니다.
* - [x] `InputManager`의 `IsPausePressed` 입력에 `GameManager.PauseGame()` 및 (제작된) 일시정지 패널(D-2.a)을 연결합니다.
* - [x] **(추가)** `RewardManager`에 `OnRerollPressed()` (UC #4) 함수를 구현하고, 재화를 소모(`PlayerManager.SpendGold`)하여 보상을 새로 생성하는 로직을 보상 패널 버튼에 연결합니다.

#### [패키지 5] 메인 메뉴 연결
* - [x] 타이틀 씬(E-1.a)과 로비 씬(E-1.b)을 구성합니다.
* - [x] (제작된) 로비 UI 프리팹의 '시작하기' 버튼 `OnClick` 이벤트에 `GameManager.StartGame()` 함수를 연결합니다.

---

### ⚔️ Sprint 3: 콘텐츠 확장 (11/27 ~ 12/03)

#### [패키지 1] 메타 지원
* - [x] 데이터 저장/로드 (E-3.a) 유틸리티 클래스(예: `SaveManager`)를 구현합니다. (`PlayerPrefs` 또는 `JsonUtility` 사용)
* - [x] `GameManager`에 `GameOver()` (A-2.b) 및 `GameClear()` (A-2.b) 상태 및 로직을 구현합니다.

#### [패키지 2] 콘텐츠: 장비/아이템
* - [x] **(핵심)** n종의 새로운 `Equipment` (B-3.a)를 기획하고 구현합니다. (예: 근접 무기, 방어형 장비 등)
* - [x] `ItemManager` (B-3.c) 및 '체력 물약' 등 소모성 아이템 n종을 구현합니다.
* - [x] `ExperienceOrb` (B-2.a) 프리팹을 만들고, 플레이어에게 자석처럼 끌려오는 기능을 구현합니다.
* - [x] (수정) `Monster.Die()`가 `GainExp` 대신 `ExperienceOrb`를 스폰하도록 변경합니다.

#### [패키지 3] 콘텐츠: 보스/이벤트
* - [ ] `BossMonster` (C-2.c) 클래스(특수 공격 패턴 포함)를 구현합니다.
* - [ ] `SpawnManager` (C-1.b)가 10분이 되면 `BossMonster`를 스폰하도록 수정합니다.
* - [ ] **(핵심)** `QuestManager` (C-3.a) 및 `DefenceQuest` (SDS 6.13) 로직을 1종 기획하고 구현합니다.

#### [패키지 4] 고급 UI 연결
* - [ ] `SpawnManager`의 보스 스폰 이벤트와 `HUDManager.ShowBossHpBar()` (D-1.d)를 연결합니다.
* - [ ] `QuestManager`의 퀘스트 시작/종료 이벤트와 `HUDManager.ToggleQuestInfo()` (D-1.d)를 연결합니다.
* - [x] `GameManager`의 `GameOver`/`GameClear` 상태에 따라 (제작된) 결과창 UI (D-2.c)를 활성화하고, **결과창의 '재시작(UC #12)' 및 '메인으로(UC #13)' 버튼에 `GameManager.RestartGame()` / `GoToMainMenu()` 함수를 연결합니다.**
* - [ ] `HUDManager`가 `EquipmentManager`의 장비 목록을 받아 아이콘을 표시(D-1.c)하도록 구현합니다.
* - [x] `PlayerManager`의 `GainGold` 및 `KillCount` 이벤트를 `HUDManager` (D-1.b)에 연결하여 재화/킬카운트 Text를 업데이트합니다.

#### [패키지 5] 메타 시스템 연결
* - [ ] **(핵심)** `UpgradeManager` (E-2.a)를 구현합니다.
* - [ ] (제작된) 강화 UI 패널 버튼에 `UpgradeStat()` 함수를 연결합니다. (재화 소모 및 `SaveManager` 호출 포함)
* - [x] `CodexManager` (E-2.b)를 구현하고, (제작된) 도감 UI 패널과 연결하여 해금된 정보를 표시합니다.
* - [x] **(추가)** `MainMenuPanelManager`에서 (제작된) 설정 프리팹의 '도감 초기화' 버튼(UC #10)에 `CodexManager.ResetCodex()` 함수를 연결합니다.

---

### ✨ Sprint 4: 최종 폴리싱 및 테스트 (12/04 ~ 12/05)

#### [전체] 통합 및 테스트
* - [ ] **(밸런싱):** 몬스터 스탯, 스폰 속도, 장비 데미지, 레벨 업 요구 경험치, 강화 비용 등 모든 수치를 조정합니다.
* - [ ] **(폴리싱):** `AudioManager`를 통해 모든 BGM, SFX(공격, 피격, 레벨 업, UI 클릭)를 적용합니다.
* - [ ] **(버그 수정):** S1~S3에서 발생한 모든 버그(특히 치명적인 버그)를 수정하고 최종 빌드를 준비합니다.