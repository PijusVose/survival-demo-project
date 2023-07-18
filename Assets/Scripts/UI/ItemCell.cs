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
        itemIcon.sprite = null;
        itemIcon.gameObject.SetActive(false);
        itemStackLabel.gameObject.SetActive(false);
        itemHealthBar.SetActive(false);
        greenHealthBar.localScale = Vector3.one;
        
        this.inventoryWindow = inventoryWindow;
        this.cellId = cellId;
    }
    
    public void PlaceItem(Item item)
    {
        // TODO: check item type, enable/disable healthbar and stack label.
        
        itemIcon.sprite = item.ItemConfig.ItemIcon;
        itemIcon.gameObject.SetActive(true);
        
        itemStackLabel.gameObject.SetActive(true);
        itemStackLabel.text = item.ItemStack.ToString();
    }

    public void RemoveItem(int amount)
    {
        // TODO: remove from stack or reset cell if item is completely removed.
    }

    public void UpdateItem(Item item)
    {
        // TODO: specific handling for non-material items like armor, weapons etc.
        // TODO: Update health bars and so on.
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
