using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// 개별 업그레이드 슬롯 UI를 표현합니다.
/// 버튼(정확히는 버튼 역할을 하는) 오브젝트에 직접 추가하여 사용합니다.
/// UpgradeData SO를 연결해야 합니다.
/// 좌클릭: 구매, 우클릭: 환불
/// </summary>
public class UpgradeSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region Serialized Fields
    [Header("Dependencies")]
    [SerializeField] private UpgradeData upgradeData;
    
    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 1f); // 어두운 회색
    #endregion

    #region Private Fields
    private UpgradeData _data;
    private UpgradeManager _upgradeManager;
    private TooltipController _tooltipController;
    private Image _buttonImage;
    private TMP_Text[] _texts;
    #endregion

    #region Unity LifeCycle
    private void Awake()
    {
        // 같은 GameObject의 Image 컴포넌트 찾기
        _buttonImage = GetComponent<Image>();
        
        // 자식의 모든 Text 찾기
        _texts = GetComponentsInChildren<TMP_Text>();
        
        // 디버그 로그
        Debug.Log($"UpgradeSlotUI ({gameObject.name}): Image={_buttonImage != null}, Texts={_texts?.Length ?? 0}개 발견");
        
        if (_texts != null && _texts.Length > 0)
        {
            foreach (var text in _texts)
            {
                Debug.Log($"  - {text.gameObject.name}: {text.text}");
            }
        }
    }

    private void Start()
    {
        // UpgradeManager 자동 찾기 (싱글톤)
        _upgradeManager = UpgradeManager.Instance;
        if (_upgradeManager == null)
        {
            Debug.LogError($"UpgradeSlotUI ({gameObject.name}): UpgradeManager를 찾을 수 없습니다!");
        }
        
        // TooltipManager 자동 찾기 (씬에서 검색)
        _tooltipController = FindAnyObjectByType<TooltipController>();
        if (_tooltipController == null)
        {
            Debug.LogWarning($"UpgradeSlotUI ({gameObject.name}): TooltipManager를 찾을 수 없습니다. 툴팁이 표시되지 않습니다.");
        }
        
        // 자동 초기화
        if (upgradeData != null && _upgradeManager != null)
        {
            Initialize(upgradeData, _upgradeManager);
            
            // UpgradeManager 이벤트 구독
            _upgradeManager.OnGoldChanged += OnGoldChanged;
            _upgradeManager.OnUpgradeChanged += OnUpgradeChanged;
        }
        else
        {
            Debug.LogError($"UpgradeSlotUI ({gameObject.name}): UpgradeData 또는 UpgradeManager가 할당되지 않았습니다!");
        }
    }

    private void OnDestroy()
    {
        if (_upgradeManager != null)
        {
            _upgradeManager.OnGoldChanged -= OnGoldChanged;
            _upgradeManager.OnUpgradeChanged -= OnUpgradeChanged;
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// 슬롯을 초기화합니다.
    /// </summary>
    public void Initialize(UpgradeData data, UpgradeManager manager)
    {
        _data = data;
        _upgradeManager = manager;
        
        Debug.Log($"UpgradeSlotUI: {data.UpgradeName} 초기화 완료");
        
        UpdateDisplay();
    }

    /// <summary>
    /// 슬롯의 표시 내용을 업데이트합니다.
    /// </summary>
    public void UpdateDisplay()
    {
        if (_data == null || _upgradeManager == null) return;

        int currentLevel = _upgradeManager.GetUpgradeLevel(_data.UpgradeType);
        
        // 레벨에 따라 색상만 변경 (텍스트는 툴팁에서 표시)
        Color targetColor = currentLevel > 0 ? normalColor : lockedColor;
        
        if (_buttonImage != null)
        {
            _buttonImage.color = targetColor;
        }
        
        // 모든 텍스트 색상 변경
        if (_texts != null)
        {
            foreach (var text in _texts)
            {
                if (text != null)
                {
                    text.color = targetColor;
                }
            }
        }
    }

    /// <summary>
    /// 마우스 클릭 이벤트 처리 (좌클릭/우클릭 구분)
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_upgradeManager == null || _data == null) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 좌클릭: 구매
            OnPurchaseClicked();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 우클릭: 환불
            OnRefundClicked();
        }
    }

    /// <summary>
    /// 마우스가 슬롯 위에 올라갔을 때 툴팁 표시
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipController != null && _data != null && _upgradeManager != null)
        {
            int currentLevel = _upgradeManager.GetUpgradeLevel(_data.UpgradeType);
            int cost = _data.GetCostForLevel(currentLevel);
            
            _tooltipController.ShowUpgradeTooltip(_data, currentLevel, cost);
        }
    }

    /// <summary>
    /// 마우스가 슬롯에서 벗어났을 때 툴팁 숨김
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipController != null)
        {
            _tooltipController.HideTooltip();
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// 구매 버튼 클릭 시 호출됩니다.
    /// </summary>
    private void OnPurchaseClicked()
    {
        bool success = _upgradeManager.Purchase(_data);
        
        if (success)
        {
            UpdateDisplay();
            RefreshTooltip(); // 툴팁 갱신
        }
    }

    /// <summary>
    /// 환불 버튼 클릭 시 호출됩니다.
    /// </summary>
    private void OnRefundClicked()
    {
        bool success = _upgradeManager.Refund(_data);
        
        if (success)
        {
            UpdateDisplay();
            RefreshTooltip(); // 툴팁 갱신
        }
    }

    /// <summary>
    /// 툴팁을 갱신합니다 (마우스가 슬롯 위에 있을 때).
    /// </summary>
    private void RefreshTooltip()
    {
        // 툴팁이 활성화되어 있으면 (마우스가 슬롯 위에 있으면) 갱신
        if (_tooltipController != null && _data != null && _upgradeManager != null)
        {
            int currentLevel = _upgradeManager.GetUpgradeLevel(_data.UpgradeType);
            int cost = _data.GetCostForLevel(currentLevel);
            
            _tooltipController.ShowUpgradeTooltip(_data, currentLevel, cost);
        }
    }

    /// <summary>
    /// 골드가 변경되었을 때 호출됩니다.
    /// </summary>
    private void OnGoldChanged(int newGold)
    {
        UpdateDisplay();
    }

    /// <summary>
    /// 업그레이드가 변경되었을 때 호출됩니다.
    /// </summary>
    private void OnUpgradeChanged(UpgradeType type, int newLevel)
    {
        // 이 슬롯의 업그레이드 타입과 일치하면 업데이트
        if (_data != null && _data.UpgradeType == type)
        {
            UpdateDisplay();
        }
    }
    #endregion
}
