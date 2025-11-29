using System;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private LootDataBase lootDataBase;
    [SerializeField] private InventoryManager inventoryManager;

    // 일단 rewardPanel UI는 rewardManager에서 업데이트. 나중에 위치 변경 가능
    [Header("RewardPanel UI")]
    [SerializeField] private TextMeshProUGUI rerollCostText;
    [SerializeField] private TextMeshProUGUI rerollCountText;
    [SerializeField] private TextMeshProUGUI skipExpRatio;
    #endregion

    #region Private Fields
    private int _maxRerollCount = 2; // 테스트용 임시 최대 리롤 횟수
    private int _rerollCount = 0;
    private int _baseRerollPrice = 100; // 테스트용 임시 리롤 비용
    private int _rerollPrice = 0;
    private float _skipExpRatio = 0.2f; // 테스트용 임시 경험치 보상 비율
    #endregion

    #region Unity LifeCycle
    private void Start()
    {
        if (playerManager == null)
            Debug.LogError("RewardManager: PlayerManager가 인스펙터에 연결되지 않았습니다!");

        _rerollPrice = _baseRerollPrice;
        _rerollCount = _maxRerollCount;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// (S2, B-4) GameManager가 레벨업을 감지하면 호출
    /// 보상 3개 생성
    /// </summary>
public void GenerateRewards()
{
    // 일단 rewardPanel UI는 rewardPanel UI는rewardManager에서 업데이트. 나중에 위치 변경 가능
    UpdateRerollCost(_rerollPrice);
    UpdateRerollCount(_rerollCount);
    UpdateSkipExpRatio(_skipExpRatio);


    Debug.Log("RewardManager: GenerateRewards() 호출됨");
/*
    HashSet<UnityEngine.Object> rewards = new HashSet<UnityEngine.Object>(3);

    // 무한 루프 방지용 안전 장치 (보유 아이템이 없는데 뽑으려 할 때 등 대비)
    int safetyCount = 0; 

    while (rewards.Count < 3 && safetyCount < 100)
    {
        safetyCount++;
        UnityEngine.Object select = null;

        // (장비,아이템 풀 구분) 10개 중 랜덤으로 뽑기
        int flag = UnityEngine.Random.Range(0, 10);

        // 20% 확률로 소모품(Item) 등장
        if (flag >= 8)
        {
            if (lootDataBase.itemPool.Count > 0)
            {
                 select = lootDataBase.itemPool[UnityEngine.Random.Range(0, lootDataBase.itemPool.Count)];
            }
        }
        else
        {
            // (장비 구분) 3개 중 랜덤으로 뽑기
            int flag2 = UnityEngine.Random.Range(0, 3);

            // 40% 확률로 [현재 보유한 장비]에서 등장 (업그레이드)
            // 수정 포인트: 보유한 장비가 있을 때만 이 로직을 타야 함
            if (flag < 4 && (inventoryManager.Passives.Count > 0 || inventoryManager.Weapons.Count > 0))
            {
                // 1/3 확률로 패시브 등장 (패시브가 있어야 함)
                if (flag2 == 0 && inventoryManager.Passives.Count > 0)
                {
                    // 수정됨: passivePool 대신 Passives 프로퍼티 사용
                    // 컴포넌트(.Passives[i])에서 데이터(.PassiveData)를 꺼냄
                    select = inventoryManager.Passives[UnityEngine.Random.Range(0, inventoryManager.Passives.Count)].PassiveData;
                }
                // 2/3 확률로 무기 등장 (무기가 있어야 함)
                else if (inventoryManager.Weapons.Count > 0)
                {
                    // 수정됨: weaponPool 대신 Weapons 프로퍼티 사용
                    // 컴포넌트(.Weapons[i])에서 데이터(.WeaponData)를 꺼냄
                    select = inventoryManager.Weapons[UnityEngine.Random.Range(0, inventoryManager.Weapons.Count)].WeaponData;
                }
                // (예외 처리) 위 조건에 안 걸리면 전체 풀에서 뽑도록 유도하거나 루프 다시 돔
            }
            // 60% 확률 (혹은 보유 장비가 없을 때) -> 전체 풀에서 등장 (신규 획득)
            else
            {
                // 1/3 확률로 패시브 등장
                if (flag2 == 0)
                {
                    if (lootDataBase.passivePool.Count > 0)
                        select = lootDataBase.passivePool[UnityEngine.Random.Range(0, lootDataBase.passivePool.Count)];
                }
                // 2/3 확률로 무기 등장
                else
                {
                    if (lootDataBase.weaponPool.Count > 0)
                        select = lootDataBase.weaponPool[UnityEngine.Random.Range(0, lootDataBase.weaponPool.Count)];
                }
            }
        }

        if (select != null)
        {
            rewards.Add(select);
        }
    }        

        // UI 표시는 InGamePanelManager에서 하는 것 고려
        /*
        for (int i = 0; i < rewards.Count; i++)
        {
            // UI 교체
            rewardSlots[i].icon = rewards[i].Icon;
            rewardSlots[i].nameText.text = rewards[i].EquipmentName;

            // 기존 리스너 제거 후 새로 등록
            rewardSlots[i].selectButton.onClick.RemoveAllListeners();
            rewardSlots[i].selectButton.onClick.AddListener(() =>
            {
                OnRewardSelected(rewards[i]);
            });
        }
        */

        
    }

    /// <summary>
    /// 보상 선택 시 호출
    /// </summary>
    public void OnRewardSelected(EquipmentData data) 
    {

        // 선택된 장비 착용
        playerManager.AddEquipment(data);

        // 리롤 횟수 초기화
        _rerollCount = _maxRerollCount;
        // 리롤 비용 초기화
        _rerollPrice = _baseRerollPrice;

        OnRewardProcessFinished?.Invoke();
    }

    /// <summary>
    /// 리롤: 새로운 보상 3개 다시 생성
    /// 리롤 가격만큼 골드 차감
    /// </summary>
    public void OnRerollPressed() 
    {
        if (playerManager.Gold < _rerollPrice)
        {
            Debug.Log("RewardManager: 골드 부족 -> 리롤 불가");
            return;
        }

        if (_rerollCount <= 0)
        {
            Debug.Log("RewardManager: 리롤 횟수 부족 -> 리롤 불가");
            return;
        }

        // 골드 차감
        playerManager.GainGold(-_rerollPrice);

        // 리롤 횟수 감소
        _rerollCount--;
        // 리롤 비용 증가
        _rerollPrice = _rerollPrice * 2;

        // 보상 다시 생성
        GenerateRewards();
    }

    /// <summary>
    /// 보상 스킵
    /// </summary>
    public void OnSkipPressed() 
    {
        // 경험치 보상 후 종료
        playerManager.GainExp((int)(playerManager.MaxExp * _skipExpRatio)); // 테스트용 임시 경험치 보상

        // 리롤 횟수 초기화
        _rerollCount = _maxRerollCount;
        // 리롤 비용 초기화
        _rerollPrice = _baseRerollPrice;

        OnRewardProcessFinished?.Invoke();
    }
    #endregion

    #region Private Methods
    // 일단 rewardPanel UI는 rewardPanel UI는rewardManager에서 업데이트. 나중에 위치 변경 가능

    /// <summary>
    /// 리롤 비용 업데이트 함수
    /// 리롤 버튼 누를 때 호출
    /// </summary>
    private void UpdateRerollCost(int amount)
    {
        if (rerollCostText != null)
            rerollCostText.text = amount.ToString();

        // 리롤 비용 지불 불가능 시 빨간색으로 표시
        if (_rerollPrice > playerManager.Gold)
        {
            rerollCostText.color = Color.red;
        }
        else
        {
            rerollCostText.color = Color.white;
        }
    }

    /// <summary>
    /// 리롤 횟수 업데이트 함수
    /// 리롤 버튼 누를 때 호출
    /// </summary>
    private void UpdateRerollCount(int amount)
    {
        if (rerollCountText != null)
            rerollCountText.text = amount.ToString();

        // 리롤 횟수 부족 시 빨간색으로 표시
        if (_rerollCount > 0)
        {
            rerollCountText.color = Color.white;
        }
        else
        {
            rerollCountText.color = Color.red;
        }
    }

    private void UpdateSkipExpRatio(float amount)
    {
        if (skipExpRatio != null)
            skipExpRatio.text = $"{amount*100}%";
    }
    #endregion
}