using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemStackLabel;
    [SerializeField] private GameObject itemHealthBar;
    [SerializeField] private RectTransform greenHealthBar;
    
    [SerializeField] private Item storedItem;
    
    private ContainerWindow containerWindow;
    private int slotId;

    public int SlotId => slotId;
    public Item StoredItem => storedItem;
    
    public void Init(ContainerWindow containerWindow, int slotId)
    {
        ResetCell();
        
        this.containerWindow = containerWindow;
        this.slotId = slotId;
    }

    public void UpdateSlotItem(Item item)
    {
        if (item == null || item.ItemStack == 0)
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
            itemStackLabel.text = item.ItemStack.ToString();
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
        containerWindow.SetMouseOverSlot(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        containerWindow.SetMouseOverSlot(null);
    }
}
