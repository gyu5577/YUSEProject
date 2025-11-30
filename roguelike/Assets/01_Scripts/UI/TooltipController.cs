using UnityEngine;
using TMPro;

/// <summary>
/// 툴팁 표시 및 위치 관리를 담당하는 매니저입니다.
/// </summary>
public class TooltipController : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private RectTransform tooltipRect;
    [SerializeField] private TMP_Text contentText;
    [SerializeField] private Vector2 offset = new Vector2(150, 10);
    #endregion

    #region Unity LifeCycle

    private void Update()
    {
        // 툴팁이 활성화되어 있으면 마우스 위치를 따라감
        if (tooltipRect != null && tooltipRect.gameObject.activeSelf)
        {
            UpdateTooltipPosition();
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// 툴팁을 표시합니다.
    /// </summary>
    public void ShowTooltip(string text)
    {
        if (tooltipRect == null || contentText == null) return;
        
        tooltipRect.gameObject.SetActive(true);
        contentText.text = text;
        UpdateTooltipPosition();
    }

    /// <summary>
    /// 업그레이드 정보를 툴팁으로 표시합니다.
    /// </summary>
    public void ShowUpgradeTooltip(UpgradeData data, int currentLevel, int cost)
    {
        if (tooltipRect == null || contentText == null || data == null) return;
        
        bool isMaxLevel = currentLevel >= data.MaxLevel;
        
        // 툴팁 텍스트 구성
        string nameLevel = isMaxLevel 
            ? $"{data.UpgradeName} (MAX)" 
            : $"{data.UpgradeName} ({currentLevel}/{data.MaxLevel})";
        
        string costText = isMaxLevel 
            ? "MAX" 
            : $"비용: {cost}";
        
        // 최종 툴팁 텍스트
        string tooltipText = $"{nameLevel}\n\n{data.Description}\n\n{costText}";
        
        tooltipRect.gameObject.SetActive(true);
        contentText.text = tooltipText;
        UpdateTooltipPosition();
    }

    /// <summary>
    /// 툴팁을 숨깁니다.
    /// </summary>
    public void HideTooltip()
    {
        if (tooltipRect == null) return;
        
        tooltipRect.gameObject.SetActive(false);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// 툴팁 위치를 마우스 커서에 맞게 업데이트합니다.
    /// </summary>
    private void UpdateTooltipPosition()
    {
        Vector2 mousePos = Input.mousePosition;

        float width = tooltipRect.rect.width;
        float height = tooltipRect.rect.height;

        float finalX = mousePos.x + offset.x;
        float finalY = mousePos.y + offset.y;

        // 화면 오른쪽을 벗어나면 화면 오른쪽 끝에 붙이기
        if (finalX + width > Screen.width)
        {
            finalX = Screen.width - width;
        }

        // 화면 아래쪽을 벗어나면 위로 (아마 그럴 일 없겠지만 보험용)
        if (finalY - height < 0)
        {
            finalY = mousePos.y + height - offset.y;
        }

        tooltipRect.position = new Vector2(finalX, finalY);
    }
    #endregion
}
