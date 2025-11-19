using UnityEngine;

/// <summary>
/// 테스트용 개발자 도구입니다.
/// ` (Backquote) 입력으로 디버그 UI 패널 열기/닫기 가능
/// 기능은 필요에 따라 계속 추가될 수도?
/// </summary>
public class DevTools : MonoBehaviour
{
    #region Serialized Fields
    [Header("Dependencies")]
    [SerializeField] private PlayerManager playerManager;
    
    [Header("UI")]
    [SerializeField] private GameObject debugPanel;
    #endregion

    #region Private Fields
    private bool _isSpeedUp = false;
    private bool _isPanelOpen = false;
    #endregion

    #region Unity LifeCycle
    private void Start()
    {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
        // 릴리즈 빌드에서는 이 스크립트(오브젝트)를 파괴하여 치트 사용 방지
        Destroy(gameObject);
        return;
#endif

        if (playerManager == null)
        {
            // PlayerManager가 없으면 씬에서 찾음 (테스트 편의성 위해 예외적으로 FindAnyObjectByType 허용)
            playerManager = FindAnyObjectByType<PlayerManager>();
            if (playerManager == null)
            {
                Debug.LogWarning("DevTools: PlayerManager를 찾을 수 없습니다.");
            }
        }
        
        // 시작 시 패널 닫기
        if (debugPanel != null)
        {
            debugPanel.SetActive(false);
            _isPanelOpen = false;
        }
    }

    private void Update()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // UI 토글 (Backquote ` 키)
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            ToggleDebugPanel();
        }
#endif
    }
    #endregion

    #region Public Methods (UI 버튼 연결용)
    // UI 버튼의 OnClick 이벤트에 연결할 함수들입니다.

    public void OnBtnHealClicked()
    {
        if (CheckPlayer())
        {
            Debug.Log("[Dev UI] HP Full Restore");
            playerManager.TakeDamage(-10f); // 회복
        }
    }

    public void OnBtnDamageClicked()
    {
        if (CheckPlayer())
        {
            Debug.Log("[Dev UI] Take Damage (10)");
            playerManager.TakeDamage(10f);
        }
    }

    public void OnBtnExpClicked()
    {
        if (CheckPlayer())
        {
            Debug.Log("[Dev UI] Gain Exp (50)");
            playerManager.GainExp(50);
        }
    }

    public void OnBtnGoldClicked()
    {
        if (CheckPlayer())
        {
            Debug.Log("[Dev UI] Gain Gold (100)");
            playerManager.GainGold(100);
        }
    }

    public void OnBtnKillClicked()
    {
        if (CheckPlayer())
        {
            Debug.Log("[Dev UI] Kill Player");
            playerManager.TakeDamage(99999f);
        }
    }

    public void OnBtnSpeedToggleClicked()
    {
        _isSpeedUp = !_isSpeedUp;
        Time.timeScale = _isSpeedUp ? 2.0f : 1.0f;
        Debug.Log($"[Dev UI] TimeScale: {Time.timeScale}");
    }
    #endregion

    #region Private Methods
    private void ToggleDebugPanel()
    {
        if (debugPanel == null) return;

        _isPanelOpen = !_isPanelOpen;
        debugPanel.SetActive(_isPanelOpen);
    }
    
    private bool CheckPlayer()
    {
        if (playerManager == null)
        {
            playerManager = FindAnyObjectByType<PlayerManager>();
        }
        return playerManager != null;
    }
    #endregion
}