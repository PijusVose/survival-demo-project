using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler
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

    public void SetSlotItem(Item item)
    {
        if (item == null || item.ItemStack == 0)
        {
            storedItem = null;
        }
        else if (storedItem == null || storedItem.ItemId != item.ItemId)
        {
            storedItem = item;
        }

        UpdateSlot();
    }

    private void UpdateSlot()
    {
        if (storedItem != null)
        {
            // TODO: specific handling for non-material items like armor, weapons etc.
            // TODO: Update health bars and so on.
            
            itemIcon.sprite = storedItem.ItemConfig.ItemIcon;
            itemIcon.gameObject.SetActive(true);
            
            itemStackLabel.text = storedItem.ItemStack.ToString();
            itemStackLabel.gameObject.SetActive(true);
        }
        else
        {
            ResetCell();
        }
    }

    public void SetSlotStack(int stack)
    {
        if (storedItem != null)
        {
            itemStackLabel.text = stack.ToString();
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
}
