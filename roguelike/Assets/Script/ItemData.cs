using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Game Data/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Basic Information")]
    [Tooltip("아이템의 이름")]
    public string itemName;

    [Tooltip("아이템의 상세 설명")]
    [TextArea(3, 5)]
    public string description;

    [Tooltip("인벤토리 UI에 표시될 아이콘")]
    public Sprite icon;
}