using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemStackLabel;
    [SerializeField] private GameObject itemHealthBar;
    [SerializeField] private RectTransform greenHealthBar;
    
    [SerializeField] private Item storedItem;
    
    private InventoryWindow inventoryWindow;
    private int cellId;

    public int CellIndex => cellId;
    
    public void Init(InventoryWindow inventoryWindow, int cellId)
    {
        this.inventoryWindow = inventoryWindow;
        this.cellId = cellId;
    }
    
    public void PlaceItem()
    {
        
    }

    public void TakeItem()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryWindow.SetMouseOverCell(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryWindow.SetMouseOverCell(null);
    }
}
