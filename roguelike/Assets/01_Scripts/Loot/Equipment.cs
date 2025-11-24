/*
 * [Equipment.cs]
 * [패키지 2] 플레이어 로직 - 장비 시스템
 * 모든 장비가 상속받는 추상 클래스입니다. (SDS 3.2.4)
 * 공통 데이터와 메서드를 정의합니다.
 */

using UnityEngine;

public abstract class Equipment : MonoBehaviour
{
    #region Serialized Fields
    [Header("Basic Info")]
    [SerializeField] protected string equipmentName;
    [SerializeField] [TextArea] protected string description;
    [SerializeField] protected Sprite icon;
    
    [Header("Stats")]
    [SerializeField] protected float baseDamage = 10f;
    [SerializeField] protected float baseCooldown = 2f; // (필요한 경우 사용)
    #endregion

    #region Protected Fields
    
    protected int _level = 1;
    protected float _currentCooldown = 0f;
    protected PlayerManager _player; // 장비 소유자 참조
    #endregion

    #region Public Properties
    public string EquipmentName => equipmentName;
    public int Level => _level;
    #endregion

    #region Unity LifeCycle
    // 장비가 생성될 때 초기화 (PlayerManager가 호출해 줄 수도 있음)
    public virtual void Initialize(PlayerManager player)
    {
        _player = player;
        _currentCooldown = 0f;
        
        // 초기 위치 설정 (플레이어 자식으로 들어갈 경우 로컬 위치 초기화)
        transform.localPosition = Vector3.zero; 
    }

    protected virtual void Update()
    {
        // 쿨다운이 필요한 장비라면 여기서 관리
        if (_currentCooldown > 0f)
        {
            _currentCooldown -= Time.deltaTime;
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// (SDS 3.2.4) 장비 레벨을 증가시킵니다.
    /// </summary>
    public virtual void LevelUp()
    {
        _level++;
        // 예: 데미지 증가, 쿨다운 감소 등 공통 로직
        // baseDamage *= 1.1f; 
        Debug.Log($"{equipmentName} Level Up! Current Level: {_level}");
    }
    
    /// <summary>
    /// (SDS 3.2.4) 쿨다운 갱신 (Manager에서 호출할 수도, Update에서 자체 처리할 수도 있음)
    /// </summary>
    public virtual void UpdateCooldown(float deltaTime)
    {
        _currentCooldown -= deltaTime;
    }
    #endregion

    #region Abstract Methods
    // 자식 클래스마다 공격 방식이 다르므로 추상 메서드로 정의
    // (TestEquip처럼 지속형 장비는 사용 안 할 수도 있지만, 발사형 장비를 위해 정의)
    protected abstract void PerformAttack();
    #endregion
}