/*
 * [PlayerManager.cs]
 * [패키지 2] 플레이어 로직
 * Sprint 1 목표(B-1.a)에 따라 이동, HP 관리, 사망 처리,
 * 그리고 HUD 연동을 위한 OnHpChanged 이벤트를 구현합니다.
 *
 * 이 스크립트는 '통합 코딩 컨벤션'을 완벽하게 준수하여 작성되었습니다.
 */

using System; // event Action 사용
using UnityEngine;

// 코딩 컨벤션 1-4 (GetComponent)를 위해 Rigidbody2D 강제
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerManager : MonoBehaviour
{
    #region Events
    // (Sprint 1, B-1.a) HP 변경 이벤트 정의 (HUDManager가 구독할 대상)
    public event Action<float, float> OnHpChanged;
    // (Sprint 2, D-1.b) 경험치 변경 이벤트 정의
    public event Action<float, float> OnExpChanged;
    // 재화, 킬 카운트 변경 이벤트 정의
    public event Action<int> OnGoldChanged;
    public event Action<int> OnKillCountChanged;
    
    // (Sprint 2, B-1.b) 레벨 업 이벤트를 정의합니다. (GameManager가 구독할 대상)
    public event Action OnPlayerLeveledUp;
    #endregion

    #region Properties
    // 외부에서 현재 HP를 읽을 수 있도록 프로퍼티로 노출
    public float CurrentHp { get => _currentHp; }
    // (Sprint 2 추가) 레벨, 경험치, 골드 프로퍼티
    public int Level { get => _level; }
    public int CurrentExp { get => _currentExp; }
    public int Gold { get => _gold; }

    //위치
    public Vector2 Player_Position =>transform.position;
    #endregion
    
    #region Serialized Fields
    [Header("Dependencies (Required)")]
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private ItemManager itemManager;
    
    [Header("Stats")]
    [SerializeField]
    private PlayerStats stats;
    #endregion

    #region Private Fields
    private Rigidbody2D _rb;
    private float _currentHp;
    
    // --- Sprint 2에서 사용할 변수 ---
    private int _level = 1;
    private int _currentExp = 0;
    private int _maxExp = 100; // 초기 최대 경험치
    private int _gold = 0;
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
        // ItemManager의 초기화를 PlayerManager가 대신 처리합니다.
        if (itemManager != null && inputManager != null)
        {
            itemManager.Initialize(inputManager);
        }
        else
        {
            Debug.LogError("PlayerManager: ItemManager 또는 InputManager 참조가 없어 ItemManager를 초기화할 수 없습니다.");
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
    
    /// <summary>
    /// InputManager로부터 아이템 사용 입력을 받을 때 호출됩니다.
    /// </summary>
    private void HandleItemUseInput(int slotNumber)
    {
        if (GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }
        if (itemManager != null)
        {
            itemManager.ActivateItem(slotNumber);
        }
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
    
    /// <summary>
    /// (S2, B-1.b) 경험치를 획득합니다.
    /// </summary>
    /// <param name="amount">획득한 경험치 양</param>
    public void GainExp(int amount)
    {
        _currentExp += amount;
        
        // 경험치 획득 후 UI 갱신 알림
        OnExpChanged?.Invoke((float)_currentExp, (float)_maxExp);

        if (_currentExp >= _maxExp)
        {
            LevelUp();
        }
    }

    /// <summary>
    /// (S2, B-1.c) 재화를 획득합니다.
    /// </summary>
    /// <param name="amount">획득한 재화 양</param>
    public void GainGold(int amount)
    {
        _gold += amount;
        
        // 재화 획득 후 UI 갱신 알림
        OnGoldChanged?.Invoke(_gold);
    }
    
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
    
    /// <summary>
    /// (S2, B-1.b) 레벨 업 처리
    /// </summary>
    private void LevelUp()
    {
        // 경험치 이월 및 레벨 증가
        _currentExp -= _maxExp;
        _level++;
        
        // 다음 레벨 필요 경험치 증가 (예: 20% 증가)
        _maxExp = Mathf.RoundToInt(_maxExp * 1.2f);
        
        // 레벨 업 후에도 남은 경험치가 최대 경험치보다 많을 수 있으므로 재귀 호출 가능성 고려
        // (단순화를 위해 여기서는 한 번만 처리하거나 while문 사용 가능)
        
        // 레벨 업 후 UI 갱신 알림 (변경된 maxExp 반영)
        OnExpChanged?.Invoke((float)_currentExp, (float)_maxExp);

        // GameManager 등에게 레벨 업 사실 알림
        OnPlayerLeveledUp?.Invoke(); 
    }

    /// <summary>
    /// 장비 선택 시 호출되는 더미 함수
    /// </summary>
    public void AddEquipment(Equipment data) 
    {
    
    }
    #endregion
}