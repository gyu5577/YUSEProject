using UnityEngine;
using UnityEngine.EventSystems;

// TODO: TooltipManager 클래스로 대체됨, 연결된 프리팹 수정 후 제거 필요
/// <summary>
/// 툴팁을 표시하기 위한 컴포넌트입니다.
/// 개별 UI 요소에 붙어서 마우스 이벤트를 TooltipManager에게 전달합니다.
/// </summary>
public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Serialized Fields
    [SerializeField] private TooltipController tooltipController;
    
    [TextArea]
    [SerializeField] private string description;
    #endregion

    #region Event Handlers
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipController != null)
        {
            tooltipController.ShowTooltip(description);
        }
    }

    public void OnPointerExit(PointerEventData eventData) 
    { 
        if (tooltipController != null)
        {
            tooltipController.HideTooltip();
        }
    }
    #endregion
}
