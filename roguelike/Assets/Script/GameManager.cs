using System;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필수

// GameState 열거형 정의
public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver,
    GameClear
}

/// <summary>
/// 게임의 전반적인 상태를 관리하는 싱글톤 매니저 클래스입니다.
/// 컨벤션 1-1의 유일한 예외로서 SerializeField 대신 FindAnyObjectByType를 사용합니다.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton
    // 씬을 넘나드는 싱글톤 구현
    public static GameManager Instance { get; private set; }
    #endregion

    #region Events
    // (컨벤션 1-3) UI가 구독할 이벤트
    /// <summary>
    /// (S1, D-1.b) 게임 시간이 변경될 때 HUDManager(타이머)에 알립니다. (현재 시간)
    /// </summary>
    public event Action<float> OnTimeChanged;

    /// <summary>
    /// (S2, D-2.a) 게임 상태(Paused, Playing 등)가 변경될 때 UI에 알립니다.
    /// </summary>
    public event Action<GameState> OnGameStateChanged;
    #endregion

    #region Properties
    // (A-2.a) 외부에서 현재 상태를 읽을 수 있도록 프로퍼티 제공
    public GameState CurrentState { get => _currentState; }
    public float GameTime { get => _gameTime; }
    #endregion

    #region Cached Scene Dependencies
    // (컨벤션 1-1 예외)
    private PlayerManager _playerManager;
    private RewardManager _rewardManager;
    private InGamePanelManager _inGamePanelManager;
    private InputManager _inputManager; 
    #endregion

    #region Private Fields
    private GameState _currentState;
    private float _gameTime;
    
    // (S2, A-2.c) 씬 이름을 상수로 관리 (컨벤션 2 "매직 넘버" 금지)
    private const string MAIN_MENU_SCENE = "MainMenuScene";
    private const string IN_GAME_SCENE = "InGameScene";
    #endregion

    #region Unity LifeCycle
    private void Awake()
    {
        // --- 씬을 넘나드는 싱글톤 구현 ---
        if (Instance == null)
        {
            // 이 GameManager가 최초의 인스턴스임
            Instance = this;

            // 씬이 전환되어도 이 오브젝트를 파괴하지 않음
            DontDestroyOnLoad(gameObject);

            // 씬이 로드될 때마다 OnSceneLoaded 함수를 호출하도록 구독
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            Application.targetFrameRate = 60;
        }
        else
        {
            // 이미 Instance가 존재한다면? (예: 메인 메뉴로 돌아왔을 때)
            // 이 씬에 새로 생긴 GameManager는 파괴함
            Destroy(gameObject);
            return;
        }
        // --- 싱글톤 구현 끝 ---
    }

    private void Start()
    {
        // (S1, A-2.a) 게임 시작 시 초기 상태 설정
        _currentState = GameState.MainMenu;
        _gameTime = 0f;
    }

    private void Update()
    {
        // (S1, A-2.a) 게임 상태가 Playing일 때만 시간 추적
        if (_currentState == GameState.Playing)
        {
            _gameTime += Time.deltaTime;
            // (S1, D-1.b) HUDManager(타이머)에 방송
            OnTimeChanged?.Invoke(_gameTime);
        }
        
    }

    // (중요) 오브젝트 파괴 시 구독한 이벤트를 해제하여 메모리 누수를 방지함
    private void OnDestroy()
    {
        // 싱글톤 인스턴스가 자신일 경우에만 이벤트 구독 해제
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            
            // 인게임 이벤트 구독 해제
            UnsubscribeInGameEvents();
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// (S2, B-4) RewardManager 또는 InputManager가 호출합니다.
    /// </summary>
    public void PauseGame()
    {
        if (_currentState != GameState.Playing) return;

        _currentState = GameState.Paused;
        Time.timeScale = 0f; // (중요) 게임 시간을 멈춤
        OnGameStateChanged?.Invoke(_currentState);
        
        // (SDS 4, Diagram 2) 일시정지 패널 표시
        if (_inGamePanelManager != null)
        {
            _inGamePanelManager.ShowPausePanel(true);
        }
            
    }

    /// <summary>
    /// (S2, B-4) RewardManager 또는 PausePanel이 호출합니다.
    /// </summary>
    public void ResumeGame()
    {
        if (_currentState != GameState.Paused) return;

        _currentState = GameState.Playing;
        Time.timeScale = 1f; // (중요) 게임 시간을 다시 흐르게 함
        OnGameStateChanged?.Invoke(_currentState);

        // (SDS 4, Diagram 2) 모든 패널 닫기
        if(_inGamePanelManager != null)
            _inGamePanelManager.ShowPausePanel(false);
        // (보상 패널은 RewardManager가 직접 닫도록 합니다)
    }
    
    /// <summary>
    /// Pause, Resume 토글 함수
    /// </summary>
    private void HandlePauseInput()
    {
        if (_currentState == GameState.Playing)
        {
            PauseGame();
        }
        else if (_currentState == GameState.Paused)
        {
            ResumeGame();
        }
    }   
    

    /// <summary>
    /// (S3, A-2.b) PlayerManager가 Die()에서 호출합니다.
    /// </summary>
    public void GameOver()
    {
        if (_currentState == GameState.GameOver || _currentState == GameState.GameClear) return;

        _currentState = GameState.GameOver;
        Time.timeScale = 0f; // 게임 정지
        OnGameStateChanged?.Invoke(_currentState);

        // (S3, D-2.c) 게임 오버 패널 표시
        if(_inGamePanelManager != null)
            _inGamePanelManager.ShowGameOverPanel(true);
    }

    /// <summary>
    /// (S3, A-2.b) 보스 사망 시 호출됩니다.
    /// </summary>
    public void GameClear()
    {
        if (_currentState == GameState.GameOver || _currentState == GameState.GameClear) return;

        _currentState = GameState.GameClear;
        Time.timeScale = 0f; // 게임 정지
        OnGameStateChanged?.Invoke(_currentState);
        
        // (S3, D-2.c) 게임 클리어 패널 표시
        if(_inGamePanelManager != null)
            _inGamePanelManager.ShowGameClearPanel(true);
    }

    /// <summary>
    /// (S2, E-1.b) 메인 메뉴에서 '시작하기' 버튼이 호출합니다.
    /// </summary>
    public void StartGame()
    {
        // 씬을 다시 로드하기 전에 TimeScale을 원복
        Time.timeScale = 1f;
        SceneManager.LoadScene(IN_GAME_SCENE); // (S2, A-2.c)
    }

    /// <summary>
    /// (S3, D-2.c) 결과창에서 '메인으로' 버튼이 호출합니다.
    /// </summary>
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MAIN_MENU_SCENE); // (S2, A-2.c)
    }

    /// <summary>
    /// (S3, D-2.c) 결과창에서 '재시작' 버튼이 호출합니다.
    /// </summary>
    public void RestartGame()
    {
        // 현재 씬을 다시 로드 (StartGame과 동일)
        StartGame();
    }
    
    /// <summary>
    /// [병합] 게임 애플리케이션을 종료합니다. (메인메뉴 UI 등에서 사용)
    /// </summary>
    public void Shutdown()
    {
        Application.Quit();
        
        // (참고: 유니티 에디터에서는 Quit()이 동작하지 않을 수 있습니다.)
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// (S2, B-1.b) PlayerManager의 OnPlayerLeveledUp 이벤트를 구독하여 호출됩니다.
    /// </summary>
    private void HandlePlayerLeveledUp()
    {
        // (SDS 4, Diagram 3)
        PauseGame(); // 게임을 멈추고
        
        // (SDS 4, Diagram 3) RewardManager에게 보상 생성을 요청
        if(_rewardManager != null)
            _rewardManager.GenerateRewards();
        
        // (SDS 4, Diagram 3) UI 패널 표시
        if(_inGamePanelManager != null)
            _inGamePanelManager.ShowRewardPanel(true);
    }

    /// <summary>
    /// (S2, B-4) RewardManager가 보상 처리를 완료했을 때 호출됩니다.
    /// (SDS 3.2.1 HandleRewardFinished)
    /// </summary>
    private void HandleRewardFinished()
    {
        // (SDS 4, Diagram 3)
        if(_inGamePanelManager != null)
            _inGamePanelManager.ShowRewardPanel(false);
        
        ResumeGame();
    }

    /// <summary>
    /// 씬이 로드될 때마다 호출되어, 씬 내부의 매니저들을 찾아 연결(구독)합니다.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. 이전 씬의 이벤트 구독 해제 (메모리 누수 방지)
        UnsubscribeInGameEvents();

        if (scene.name == IN_GAME_SCENE)
        {  
            StartCoroutine(InitializeInGameManagers());
        }
        else if (scene.name == MAIN_MENU_SCENE)
        {
            // 1. 메인 메뉴 씬이 로드됨
            _currentState = GameState.Paused;
        }
    }
    
    private System.Collections.IEnumerator InitializeInGameManagers()
    {
        yield return null;
        // 2. 인게임 씬이 로드되었으므로 상태 초기화
        _currentState = GameState.Playing;
        _gameTime = 0f;
            
        // 3. (컨벤션 1-1 예외) 씬 내의 매니저들을 "찾아서" 연결합니다.
        // DontDestroyOnLoad 객체는 인스펙터 참조가 씬 전환 시 끊기므로,
        // 씬 로드 시점에 FindAnyObjectByType(FindObjectOfType은 Obsolete 되었음!)을 사용하는 것이 유일한 방법입니다.
        // 단, 성능에 민감한 게임에서는 이 방법을 권장하지 않습니다.
        _playerManager = FindAnyObjectByType<PlayerManager>();
        _rewardManager = FindAnyObjectByType<RewardManager>();
        _inGamePanelManager = FindAnyObjectByType<InGamePanelManager>();
        _inputManager = FindAnyObjectByType<InputManager>();

        // 4. (안전 장치)
        if (_playerManager == null || _rewardManager == null)
        {
            Debug.LogError("GameManager: InGameScene에서 필수 매니저(Player, Reward, InGamePanel)를 찾을 수 없습니다!");
        }
            
        // 5. 씬 내부 매니저들의 이벤트를 "구독"합니다.
        if(_playerManager != null)
            _playerManager.OnPlayerLeveledUp += HandlePlayerLeveledUp;
            
        if(_rewardManager != null)
            _rewardManager.OnRewardProcessFinished += HandleRewardFinished;

        if (_inputManager != null)
            _inputManager.OnPausePressed += HandlePauseInput;   
    }

    /// <summary>
    /// 씬 전환 시, 이전 씬의 객체들이 가진 이벤트 구독을 안전하게 해제합니다.
    /// </summary>
    private void UnsubscribeInGameEvents()
    {
        // playerManager가 null이 아닌지 (즉, InGameScene이었는지) 확인 후 해제
        if (_playerManager != null)
        {
            _playerManager.OnPlayerLeveledUp -= HandlePlayerLeveledUp;
        }
        if (_rewardManager != null)
        {
            _rewardManager.OnRewardProcessFinished -= HandleRewardFinished;
        }
        if (_inputManager != null)
        {
            _inputManager.OnPausePressed -= HandlePauseInput;
        }

    }
    #endregion
}