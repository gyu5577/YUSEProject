using UnityEngine;

/// <summary>
/// 인게임 오브젝트에 부착되어 아이템의 상태와 로직을 관리
/// </summary>
public class Item : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("이 아이템 인스턴스의 고정 데이터 (이름, 아이콘 등)")]
    [SerializeField]
    private ItemData data;
    
    [Header("Item State")]
    [SerializeField]
    private int durability = 10;
    
    [SerializeField]
    private float currentCooldown = 0f;
    
    [SerializeField]
    private float maxCooldown = 5f; // 아이템 고유 쿨다운 값

    #region Properties
    public int Durability => durability;
    public float CurrentCooldown => currentCooldown;
    #endregion

    /// <summary>
    /// 아이템을 사용
    /// </summary>
    public bool Activate()
    {
        // 1. 쿨다운이 끝났고, 내구도가 남아있는지 확인
        if (currentCooldown <= 0f && durability > 0)
        {
            // 2. 상태 변경
            currentCooldown = maxCooldown; 
            durability--; 
            
            // (TODO) 아이템 효과 발동 로직 호출
            Debug.Log($"아이템 사용 성공! (쿨다운 {maxCooldown}초 시작)");
            
            if (durability <= 0)
            {
                Debug.Log($"아이템 내구도 소진!");
                // (TODO) 인벤토리에서 제거 로직 호출
            }
            return true;
        }
        
        Debug.Log($"아이템 사용 불가: 쿨다운 중 또는 내구도 소진.");
        return false;
    }

    /// <summary>
    /// (ItemManager에서 호출) 쿨다운 시간을 갱신합니다.
    /// </summary>
    public void UpdateCooldown(float deltaTime)
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= deltaTime;
            if (currentCooldown < 0f)
            {
                currentCooldown = 0f;
            }
        }
    }
}