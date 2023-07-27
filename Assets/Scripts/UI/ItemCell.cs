using JetBrains.Annotations;
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
    public Item StoredItem => storedItem;
    
    public void Init(InventoryWindow inventoryWindow, int cellId)
    {
        ResetCell();
        
        this.inventoryWindow = inventoryWindow;
        this.cellId = cellId;
    }

    public void UpdateSlotItem(Item item)
    {
        // TODO: refactor this, use Update, Add and Remove functions.
        if (item == null)
        {
            ResetCell();

            storedItem = null;

            return;
        }
        
        if (storedItem == null || storedItem.ItemId != item.ItemId)
        {
            // TODO: specific handling for non-material items like armor, weapons etc.
            // TODO: Update health bars and so on.
            
            itemIcon.sprite = item.ItemConfig.ItemIcon;
            itemIcon.gameObject.SetActive(true);
        
            itemStackLabel.gameObject.SetActive(true);
            itemStackLabel.text = item.ItemStack.ToString();

            storedItem = item;
        }
        else
        {
            if (item.ItemStack == 0)
            {
                ResetCell();

                storedItem = null;
            }
            else
            {
                itemStackLabel.text = item.ItemStack.ToString();
            }
        }
    }
    
    private void ResetCell()
    {
        itemIcon.sprite = null;
        itemIcon.gameObject.SetActive(false);
        itemStackLabel.gameObject.SetActive(false);
        itemHealthBar.SetActive(false);
        greenHealthBar.localScale = Vector3.one;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryWindow.SetMouseOverCell(cellId);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryWindow.SetMouseOverCell(-1);
    }
}
