using System.Collections.Generic;
using UnityEngine;
using System; // Action<int> 사용을 위해 필요

/// <summary>
/// 인게임 씬에서 아이템 사용 및 쿨다운 관리를 담당하는 매니저입니다.
/// </summary>
public class ItemManager : MonoBehaviour
{
    private InputManager _inputManager; 

    #region Item Data Fields
    [Header("Item Slots")]
    [Tooltip("총 3개의 아이템 슬롯 (0, 1, 2)")]
    // List의 초기 사이즈를 3으로 설정하고 null로 채워 둡니다.
    [SerializeField]
    private List<Item> currentItems = new List<Item>(3) { null, null, null };
    #endregion

    #region Unity LifeCycle
    
    private void Start() { } 

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
        {
            // 매 프레임 시간 경과를 모든 아이템에게 통보
            UpdateAllItemCooldowns(Time.deltaTime);
        }
    }

    private void OnDestroy()
    {
        // 씬에서 파괴될 때 이벤트 구독 해제 (메모리 누수 방지)
        if (_inputManager != null)
        {
            _inputManager.GetItemUseInput -= ActivateItem;
            Debug.Log("ItemManager: InputManager 구독 해제 완료.");
        }
    }
    #endregion

    #region Initialization Method (PlayerManager가 호출)
    /// <summary>
    /// PlayerManager로부터 InputManager 참조를 받아 이벤트를 구독합니다.
    /// </summary>
    public void Initialize(InputManager inputManagerReference)
    {
        // 이미 초기화된 경우 중복 호출 방지
        if (_inputManager != null) return; 

        _inputManager = inputManagerReference;
        
        if (_inputManager != null)
        {
            // 이벤트 구독: 키 입력이 들어오면 ActivateItem이 호출됨
            _inputManager.GetItemUseInput += ActivateItem; 
            Debug.Log("ItemManager: InputManager 참조를 통해 구독 완료.");
        }
        else
        {
            Debug.LogError("ItemManager: Initialize 시 InputManager 참조가 null입니다. 연결을 확인하세요!");
        }
    }
    #endregion
    
    #region Public Item Methods
    /// <summary>
    /// 지정된 슬롯의 아이템 사용을 시도합니다.
    /// </summary>
    public void ActivateItem(int slotIndex)
    {
        // 키 입력은 1, 2, 3 이므로 리스트 인덱스 0, 1, 2로 변환합니다.
        int listIndex = slotIndex - 1;

        if (listIndex >= 0 && listIndex < currentItems.Count)
        {
            Item itemToUse = currentItems[listIndex];
            
            if (itemToUse != null)
            {
                itemToUse.Activate(); 
            }
            else
            {
                Debug.Log($"슬롯 {slotIndex}에 장착된 아이템이 없습니다.");
            }
        }
        else
        {
            Debug.LogError($"잘못된 아이템 슬롯 번호입니다: {slotIndex}");
        }
    }

    /// <summary>
    /// 모든 장착 아이템의 쿨다운을 갱신합니다.
    /// </summary>
    public void UpdateAllItemCooldowns(float deltaTime)
    {
        foreach (Item item in currentItems)
        {
            if (item != null)
            {
                item.UpdateCooldown(deltaTime); 
            }
        }
    }
    #endregion
    
    // (TODO) 아이템 획득/장착 로직 (EquipItem, RemoveItem) 등을 여기에 추가할 수 있습니다.
}