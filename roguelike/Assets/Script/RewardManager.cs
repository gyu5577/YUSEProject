using System;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    #region Events
    // (컨벤션 1-3) GameManager가 구독할 이벤트
    // (SDS 3.2.1 HandleRewardFinished)
    public event Action OnRewardProcessFinished;
    #endregion

    #region Serialized Fields
    // (Sprint 2에서 PlayerManager 등 다른 의존성을 이곳에 연결)
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private InGamePanelManager inGamePanelManager;

    #endregion

    #region Private Fields
    // (Sprint 2에서 보상 목록, 새로고침 비용 등을 이곳에 정의)
    private int rerollCount = 0;
    private int rerollPrice = 100; // 테스트용 임시 리롤 비용
    #endregion

    #region Unity LifeCycle
    private void Start()
    {
        if (playerManager == null)
            Debug.LogError("RewardManager: PlayerManager가 인스펙터에 연결되지 않았습니다!");

    }
    #endregion

    #region Public Methods
    /// <summary>
    /// (S2, B-4) GameManager가 레벨업을 감지하면 호출
    /// 보상 3개 생성
    /// </summary>
    public void GenerateRewards()
    {
        Debug.Log("RewardManager: reroll -> GenerateRewards() 호출됨");

        // (Sprint 2) 이곳에 3개의 보상을 생성하는 로직 구현
        
        
        
    }

    /// <summary>
    /// 보상 선택 시 호출
    /// </summary>
    public void OnRewardSelected(Equipment data) 
    {

        // 선택된 장비 착용
        playerManager.AddEquipment(data);

        OnRewardProcessFinished?.Invoke();
    }

    /// <summary>
    /// 리롤: 새로운 보상 3개 다시 생성
    /// 리롤 가격만큼 골드 차감
    /// </summary>
    public void OnRerollPressed() 
    {
        if (playerManager.Gold < rerollPrice)
        {
            Debug.Log("RewardManager: 골드 부족 -> 리롤 불가");
            return;
        }

        // 골드 차감
        playerManager.GainGold(-rerollPrice);
        rerollCount++;

        // 보상 다시 생성
        GenerateRewards();
    }

    /// <summary>
    /// 보상 스킵
    /// </summary>
    public void OnSkipPressed() 
    {

        // 보상 없이 종료
        OnRewardProcessFinished?.Invoke();  
    }
    #endregion

    #region Private Methods
    // ...
    #endregion
}