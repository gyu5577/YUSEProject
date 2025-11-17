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
    #endregion

    #region Private Fields
    // (Sprint 2에서 보상 목록, 새로고침 비용 등을 이곳에 정의)
    #endregion

    #region Unity LifeCycle
    private void Start()
    {
        // Sprint 2에서 PlayerManager의 레벨 업 이벤트를 구독해야 함
        // PlayerManager.Instance.OnPlayerLeveledUp += ...
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// (S2, B-4) GameManager가 호출할 더미 함수
    /// </summary>
    public void GenerateRewards()
    {
        Debug.Log("RewardManager: GenerateRewards() 호출됨 (더미)");
        
        // (Sprint 2) 이곳에 3개의 보상을 생성하는 로직 구현
        
        // (참고) 보상 선택(OnRewardSelected) 후
        // GameManager에 완료를 알리기 위해 아래 이벤트를 호출해야 함
        // OnRewardProcessFinished?.Invoke();
    }
    
    // (Sprint 2에서 OnRerollPressed, OnSkipPressed 등 구현)
    #endregion

    #region Private Methods
    // ...
    #endregion
}