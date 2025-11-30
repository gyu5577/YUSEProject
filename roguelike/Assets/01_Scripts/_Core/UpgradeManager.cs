using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 업그레이드 시스템의 핵심 로직을 담당하는 매니저입니다.
/// UI 표시는 UpgradeUI가 담당합니다.
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    #region Singleton
    public static UpgradeManager Instance { get; private set; }
    #endregion

    #region Events
    public event Action<int> OnGoldChanged;
    public event Action<UpgradeType, int> OnUpgradeChanged;
    #endregion

    #region Properties
    public int CurrentGold => _currentGold;
    public List<UpgradeData> AvailableUpgrades => availableUpgrades;
    #endregion

    #region Serialized Fields
    [FormerlySerializedAs("_availableUpgrades")]
    [Header("업그레이드 데이터")]
    [SerializeField] private List<UpgradeData> availableUpgrades = new List<UpgradeData>();
    #endregion

    #region Private Fields
    private int _currentGold;
    private Dictionary<UpgradeType, int> _upgradeLevels = new Dictionary<UpgradeType, int>();
    #endregion

    #region Unity LifeCycle
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        LoadData();
        
        // 테스트용: 초기 골드가 0이면 1000 골드 지급
        if (_currentGold == 0)
        {
            _currentGold = 1000;
            SaveManager.SaveGold(_currentGold);
            SaveManager.Save();
            OnGoldChanged?.Invoke(_currentGold);
            Debug.Log("UpgradeManager: 테스트용 1000 골드 지급");
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// 특정 업그레이드의 현재 레벨을 반환합니다.
    /// </summary>
    public int GetUpgradeLevel(UpgradeType type)
    {
        if (_upgradeLevels.ContainsKey(type))
        {
            return _upgradeLevels[type];
        }
        return 0;
    }

    /// <summary>
    /// 특정 업그레이드 타입의 총 보너스 값을 반환합니다.
    /// </summary>
    public float GetStatBonus(UpgradeType type)
    {
        foreach (var upgradeData in availableUpgrades)
        {
            if (upgradeData.UpgradeType == type)
            {
                int level = GetUpgradeLevel(type);
                return upgradeData.GetTotalBonus(level);
            }
        }
        return 0f;
    }

    /// <summary>
    /// 업그레이드를 구매합니다.
    /// </summary>
    public bool Purchase(UpgradeData data)
    {
        if (data == null)
        {
            Debug.LogWarning("UpgradeManager: UpgradeData가 null입니다.");
            return false;
        }

        int currentLevel = GetUpgradeLevel(data.UpgradeType);
        
        // 최대 레벨 체크
        if (currentLevel >= data.MaxLevel)
        {
            Debug.Log($"UpgradeManager: {data.UpgradeName}은(는) 이미 최대 레벨입니다.");
            return false;
        }

        int cost = data.GetCostForLevel(currentLevel);
        
        // 골드 체크
        if (_currentGold < cost)
        {
            Debug.Log("UpgradeManager: 골드가 부족합니다.");
            return false;
        }

        // 구매 처리
        _currentGold -= cost;
        _upgradeLevels[data.UpgradeType] = currentLevel + 1;
        
        // 저장
        SaveManager.SaveGold(_currentGold);
        SaveManager.SaveUpgradeLevel(data.UpgradeType, currentLevel + 1);
        SaveManager.Save();
        
        // 이벤트 발생
        OnGoldChanged?.Invoke(_currentGold);
        OnUpgradeChanged?.Invoke(data.UpgradeType, currentLevel + 1);
        
        Debug.Log($"UpgradeManager: {data.UpgradeName} 레벨 {currentLevel + 1} 구매 완료 (비용: {cost})");
        
        return true;
    }
    
    /// <summary>
    /// 업그레이드를 환불합니다.
    /// </summary>
    public bool Refund(UpgradeData data)
    {
        if (data == null)
        {
            Debug.LogWarning("UpgradeManager: UpgradeData가 null입니다.");
            return false;
        }

        int currentLevel = GetUpgradeLevel(data.UpgradeType);
        
        // 레벨이 0이면 환불 불가
        if (currentLevel <= 0)
        {
            Debug.Log($"UpgradeManager: {data.UpgradeName}은(는) 레벨이 0이므로 환불할 수 없습니다.");
            return false;
        }

        // 이전 레벨의 비용 계산
        int refundAmount = data.GetCostForLevel(currentLevel - 1);
        
        // 환불 처리
        _currentGold += refundAmount;
        _upgradeLevels[data.UpgradeType] = currentLevel - 1;
        
        // 저장
        SaveManager.SaveGold(_currentGold);
        SaveManager.SaveUpgradeLevel(data.UpgradeType, currentLevel - 1);
        SaveManager.Save();
        
        // 이벤트 발생
        OnGoldChanged?.Invoke(_currentGold);
        OnUpgradeChanged?.Invoke(data.UpgradeType, currentLevel - 1);
        
        Debug.Log($"UpgradeManager: {data.UpgradeName} 레벨 {currentLevel - 1} 환불 완료 (환불: {refundAmount})");
        
        return true;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// 저장된 데이터를 불러옵니다.
    /// </summary>
    private void LoadData()
    {
        // 골드 불러오기
        _currentGold = SaveManager.LoadGold();
        
        // 모든 업그레이드 레벨 불러오기
        _upgradeLevels.Clear();
        Debug.Log($"UpgradeManager: Available Upgrades Count = {availableUpgrades.Count}");
        
        foreach (var upgradeData in availableUpgrades)
        {
            int level = SaveManager.LoadUpgradeLevel(upgradeData.UpgradeType);
            _upgradeLevels[upgradeData.UpgradeType] = level;
            Debug.Log($"업그레이드 불러옴: {upgradeData.UpgradeName} (Type: {upgradeData.UpgradeType}) = Level {level}");
        }
        
        // 이벤트 발생 (UI 초기화용)
        OnGoldChanged?.Invoke(_currentGold);
        
        Debug.Log($"UpgradeManager: 데이터 로드 완료 (골드: {_currentGold})");
    }
    #endregion
}
