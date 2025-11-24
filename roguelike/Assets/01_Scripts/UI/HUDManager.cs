using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    #region Serialized Fields
    [Header("Dependencies")]
    [SerializeField] private PlayerManager playerManager; // 인스펙터에서 연결

    [Header("UI Elements (S1)")]
    [SerializeField] private Slider hpSlider; // (D-1.a)
    [SerializeField] private Slider expSlider; // (D-1.a)
    [SerializeField] private TextMeshProUGUI timerText; // (D-1.b)

    [Header("UI Elements (S3)")]
    [SerializeField] private TextMeshProUGUI goldText; // (D-1.b)
    [SerializeField] private TextMeshProUGUI killCountText; // (D-1.b)
    [SerializeField] private GameObject bossHpBarPanel; // (D-1.d)
    [SerializeField] private GameObject questInfoPanel; // (D-1.d)
    
    // (S3, D-1.c) 장비/아이템 슬롯 UI 참조
    // [SerializeField] private Image[] equipmentSlots;
    // [SerializeField] private Image[] itemSlots;
    #endregion

    #region Unity LifeCycle
    private void Start()
    {
        // 안전 장치
        if (playerManager == null)
        {
            Debug.LogError("HUDManager: PlayerManager가 인스펙터에 할당되지 않았습니다!");
            return;
        }

        // --- 이벤트 구독 ---

        // [Sprint 1]
        // GameManager(싱글톤)의 시간 이벤트 구독
        GameManager.Instance.OnTimeChanged += UpdateTimerText;
        // PlayerManager(SerializeField)의 HP 이벤트 구독
        playerManager.OnHpChanged += UpdateHpBar;
        
        // [Sprint 2 & 3]
        // PlayerManager의 EXP, Gold, Kill 이벤트 구독
        playerManager.OnExpChanged += UpdateExpBar;
        playerManager.OnGoldChanged += UpdateGoldText;
        playerManager.OnKillCountChanged += UpdateKillCountText;

        // (S3) [패키지 3]의 이벤트도 구독해야 함
        // SpawnManager.OnBossSpawned += ShowBossHpBar;
        // QuestManager.OnQuestStarted += ToggleQuestInfo;

        InitHUD();
    }

    private void OnDestroy()
    {
        // --- (컨벤션 1-3) 이벤트 구독 해제 (메모리 누수 방지) ---
        
        // GameManager가 null이 아닌지 확인 (게임 종료 시 Instance가 먼저 파괴될 수 있음)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTimeChanged -= UpdateTimerText;
        }

        if (playerManager != null)
        {
            playerManager.OnHpChanged -= UpdateHpBar;
            playerManager.OnExpChanged -= UpdateExpBar;
            playerManager.OnGoldChanged -= UpdateGoldText;
            playerManager.OnKillCountChanged -= UpdateKillCountText;
        }
        
        // (S3) [패키지 3] 이벤트 구독 해제
        // SpawnManager.OnBossSpawned -= ShowBossHpBar;
        // QuestManager.OnQuestStarted -= ToggleQuestInfo;
    }
    #endregion

    #region Private Methods (Event Handlers)
    // (컨벤션 1-3)
    // Update()가 아닌, 이벤트가 "방송"될 때만 호출되는 함수들입니다.

    /// <summary>
    /// (S1, D-1.a) PlayerManager.OnHpChanged 이벤트가 호출
    /// </summary>
    private void UpdateHpBar(float currentHp, float maxHp)
    {
        if (hpSlider != null)
            hpSlider.value = currentHp / maxHp;
    }

    /// <summary>
    /// (S2, D-1.a) PlayerManager.OnExpChanged 이벤트가 호출
    /// </summary>
    private void UpdateExpBar(float currentExp, float maxExp)
    {
        if (expSlider != null)
            expSlider.value = currentExp / maxExp;
    }

    /// <summary>
    /// (S1, D-1.b) GameManager.OnTimeChanged 이벤트가 호출
    /// </summary>
    private void UpdateTimerText(float time)
    {
        if (!timerText) return;
        
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
    
    /// <summary>
    /// (S3, D-1.b) PlayerManager.OnGoldChanged 이벤트가 호출
    /// </summary>
    private void UpdateGoldText(int amount)
    {
        if (goldText != null)
            goldText.text = amount.ToString();
    }
    
    /// <summary>
    /// (S3, D-1.b) PlayerManager.OnKillCountChanged 이벤트가 호출
    /// </summary>
    private void UpdateKillCountText(int amount)
    {
        if (killCountText != null)
            killCountText.text = amount.ToString();
    }

    // (S3, D-1.d)
    // private void ShowBossHpBar(...) { ... }
    // private void ToggleQuestInfo(...) { ... }

    /// <summary>
    /// HUD 초기화
    /// </summary>
    public void InitHUD()
    {
        // HP
        UpdateHpBar(playerManager.CurrentHp, playerManager.CurrentHp);

        // EXP
        UpdateExpBar(playerManager.CurrentExp, 1);

        // Timer
        UpdateTimerText(0);

        // Gold
        UpdateGoldText(playerManager.Gold);

        // Kill Count
        UpdateKillCountText(0);

        // Boss HP Bar 숨김
        if (bossHpBarPanel != null)
            bossHpBarPanel.SetActive(false);

        // Quest 패널 숨김
        if (questInfoPanel != null)
            questInfoPanel.SetActive(false);

    }
    #endregion
}