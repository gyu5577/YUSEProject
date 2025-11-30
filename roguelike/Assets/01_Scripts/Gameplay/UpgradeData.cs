using UnityEngine;

/// <summary>
/// 업그레이드 데이터를 정의하는 ScriptableObject입니다.
/// </summary>
[CreateAssetMenu(fileName = "New Upgrade", menuName = "Scriptable Objects/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    #region Serialized Fields
    [Header("기본 정보")]
    [SerializeField] private string upgradeName;
    [TextArea]
    [SerializeField] private string description;
    
    [Header("업그레이드 설정")]
    [SerializeField] private UpgradeType upgradeType;
    [SerializeField] private int baseCost = 10;
    [SerializeField] private int maxLevel = 5;
    [SerializeField] private float valuePerLevel = 5f;
    #endregion

    #region Properties
    public string UpgradeName => upgradeName;
    public string Description => description;
    public UpgradeType UpgradeType => upgradeType;
    public int BaseCost => baseCost;
    public int MaxLevel => maxLevel;
    public float ValuePerLevel => valuePerLevel;
    #endregion

    #region Public Methods
    /// <summary>
    /// 특정 레벨에서의 비용을 계산합니다.
    /// </summary>
    public int GetCostForLevel(int currentLevel)
    {
        if (currentLevel >= maxLevel)
        {
            return 0; // 최대 레벨 도달 시 구매 불가
        }
        
        // 레벨이 오를수록 비용이 증가 (예: 10, 15, 22, 33, 49)
        return Mathf.RoundToInt(baseCost * Mathf.Pow(1.5f, currentLevel));
    }

    /// <summary>
    /// 특정 레벨에서의 총 스탯 보너스를 계산합니다.
    /// </summary>
    public float GetTotalBonus(int currentLevel)
    {
        return valuePerLevel * currentLevel;
    }
    #endregion
}
