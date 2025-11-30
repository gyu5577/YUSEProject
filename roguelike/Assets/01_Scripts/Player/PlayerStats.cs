using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 스탯을 관리합니다.
/// 기본 스탯과 업그레이드 보너스를 분리하여 관리합니다.
/// </summary>
[Serializable]
public class PlayerStats
{
    #region Serialized Fields (Base Stats)
    [Header("기본 스탯")]
    [SerializeField] private float maxHp = 100f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float attackDamageMult = 1f;
    [SerializeField] private float attackSpeedMult = 1f;
    [SerializeField] private float cooldownMult = 1f;
    [SerializeField] private float magnetRange = 2f;
    [SerializeField] private float critChance = 5f;
    [SerializeField] private float critDamageMult = 1.5f;
    [SerializeField] private float expMult = 1f;
    [SerializeField] private float goldMult = 1f;
    [SerializeField] private float damageReductionMult = 0f;
    #endregion

    #region Private Fields (Bonuses)
    private Dictionary<UpgradeType, float> _bonuses = new Dictionary<UpgradeType, float>();
    #endregion

    #region Properties (Final Stats)
    
    public float MaxHp => maxHp + GetBonus(UpgradeType.Hp);
    public float Speed => speed + GetBonus(UpgradeType.Speed);
    public float AttackDamageMult => attackDamageMult + GetBonus(UpgradeType.AttackDamageMult);
    public float AttackSpeedMult => attackSpeedMult + GetBonus(UpgradeType.AttackSpeedMult);
    public float CooldownMult => Mathf.Max(0.1f, cooldownMult - GetBonus(UpgradeType.CooldownMult));
    public float MagnetRange => magnetRange + GetBonus(UpgradeType.MagnetRange);
    public float CritChance => critChance + GetBonus(UpgradeType.CritChance);
    public float CritDamageMult => critDamageMult + GetBonus(UpgradeType.CritDamageMult);
    public float ExpMult => expMult + (GetBonus(UpgradeType.ExpMult));
    public float GoldMult => goldMult + (GetBonus(UpgradeType.GoldMult) / 100f);
    public float DamageReductionMult => damageReductionMult + GetBonus(UpgradeType.DamageReductionMult);
    
    #endregion

    #region Public Methods
    /// <summary>
    /// 특정 타입의 보너스를 설정합니다.
    /// </summary>
    public void SetBonus(UpgradeType type, float value)
    {
        _bonuses[type] = value;
    }

    /// <summary>
    /// 특정 타입의 보너스를 가져옵니다.
    /// </summary>
    public float GetBonus(UpgradeType type)
    {
        if (_bonuses.ContainsKey(type))
        {
            return _bonuses[type];
        }
        return 0f;
    }

    /// <summary>
    /// 모든 보너스를 초기화합니다.
    /// </summary>
    public void ClearBonuses()
    {
        _bonuses.Clear();
    }
    #endregion
}