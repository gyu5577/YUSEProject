using System; // event Action 사용
using UnityEngine;

// 코딩 컨벤션 1-4 (GetComponent)를 위해 Rigidbody2D 강제
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerManager : MonoBehaviour
{
    #region Events
    // (Sprint 1, B-1.a) HP 변경 이벤트 정의 (HUDManager가 구독할 대상)
    public event Action<float, float> OnHpChanged;
    
    // (Sprint 2, B-1.b) 레벨 업 이벤트를 정의합니다. (RewardManager가 구독할 대상)
    public event Action OnPlayerLeveledUp;
    #endregion

    #region Properties
    // 외부에서 현재 HP를 읽을 수 있도록 프로퍼티로 노출
    public float CurrentHp { get => _currentHp; }
    #endregion
    
    #region Serialized Fields
    [Header("Dependencies (Required)")]
    [SerializeField]
    private InputManager inputManager;
    
    [Header("Stats")]
    [SerializeField]
    private PlayerStats stats;
    #endregion

    #region Private Fields
    private Rigidbody2D _rb;
    private float _currentHp;
    
    // --- Sprint 2에서 사용할 변수 ---
    // private int _level = 1;
    // private int _currentExp = 0;
    // private int _maxExp = 100;
    // private int _gold = 0;
    #endregion

    #region Unity LifeCycle
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _currentHp = stats.MaxHp;

        // 안전 장치
        if (inputManager == null)
        {
            Debug.LogError("PlayerManager: InputManager가 인스펙터에 할당되지 않았습니다!");
        }
    }

    // 물리 업데이트는 FixedUpdate에서 처리
    private void FixedUpdate()
    {
        // (Sprint 1, B-1.a) InputManager에서 이동 값을 받아 Move 함수 호출
        Vector2 moveInput = new Vector2(
            inputManager.HorizontalInputValue,
            inputManager.VerticalInputValue
        );

        Move(moveInput);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// (B-1.a) 캐릭터의 HP를 감소시킵니다. (Monster가 호출)
    /// </summary>
    /// <param name="amount">받은 데미지 양</param>
    public void TakeDamage(float amount)
    {
        if (_currentHp <= 0) return; // 이미 사망함

        _currentHp -= amount;

        // (Sprint 1, B-1.a) HP가 변경되었음을 모든 구독자(HUDManager 등)에게 "방송"
        OnHpChanged?.Invoke(_currentHp, stats.MaxHp);

        if (_currentHp <= 0)
        {
            _currentHp = 0;
            Die();
        }
    }
    
    // (Sprint 2에서 구현될 함수들...)
    // public void GainExp(int amount) { ... }
    // public void GainGold(int amount) { ... }
    // public void SpendGold(int amount) { ... }
    // public void AddEquipment(Equipment equipment) { ... }
    #endregion

    #region Private Methods
    /// <summary>
    /// (B-1.a) SDS 3.2.2에 정의된 Move 함수 (내부 로직)
    /// </summary>
    private void Move(Vector2 direction)
    {
        // (Sprint 1, B-1.a) Rigidbody.MovePosition 사용
        Vector2 newPosition = _rb.position + direction * (stats.Speed * Time.fixedDeltaTime);
        _rb.MovePosition(newPosition);
    }

    /// <summary>
    /// (B-1.a) SDS 3.2.2에 정의된 Die 함수 (내부 로직)
    /// </summary>
    private void Die()
    {
        // (Sprint 1, B-1.a) 사망 처리
        Debug.Log("Player has died.");

        // (Sprint 3, A-2.b) GameManager에게 사망을 알림
        // (코딩 컨벤션 1-2: GameManager는 싱글톤으로 접근)
        GameManager.Instance.GameOver(); 
        
        // 우선은 플레이어 비활성화
        gameObject.SetActive(false); 
    }
    
    // (Sprint 2에서 구현...)
    // private void LevelUp()
    // {
    //     ...
    //     OnPlayerLeveledUp?.Invoke(); // RewardManager가 구독할 이벤트
    // }
    #endregion
}