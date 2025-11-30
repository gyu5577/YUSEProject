/// <summary>
/// 업그레이드 타입을 정의하는 Enum입니다.
/// SaveManager와 UpgradeManager에서 공용으로 사용됩니다.
/// </summary>
public enum UpgradeType
{
    Hp,                 // 체력
    Speed,              // 이동 속도
    AttackDamageMult,       // 공격력
    AttackSpeedMult,        // 공격 속도
    CooldownMult,           // 쿨타임
    MagnetRange,        // 자석 범위
    CritChance,         // 치명타 확률
    CritDamageMult,         // 치명타 피해
    ExpMult,      // 경험치 획득량
    GoldMult,     // 골드 획득량
    DamageReductionMult    // 피해 감소율
}
