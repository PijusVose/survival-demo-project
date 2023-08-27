using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DragItemSlot : MonoBehaviour
{
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private Image slotIcon;
    [SerializeField] private TextMeshProUGUI stackLabel;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private RectTransform greenBarTransform;

    public int startSlotId;
    public Item draggedItem;
    
    // TODO: Init with UiController and set canvas.

    public void Init()
    {
        var uiController = GameController.Instance.GetController<UIController>();
        uiCanvas = uiController.UICanvas;
    }
    
    public void EnableDragSlot(Item item, int slotId)
    {
        draggedItem = item;
        startSlotId = slotId;
        
        slotIcon.sprite = item.ItemConfig.ItemIcon;
        stackLabel.text = item.ItemStack.ToString();
            
        FollowMouse();
        
        slotIcon.gameObject.SetActive(true);
        stackLabel.gameObject.SetActive(true);
        gameObject.SetActive(true);
        
        // TODO: show stack/healthbar depending on item type.
    }

    public void DisableDragSlot()
    {
        gameObject.SetActive(false);
        slotIcon.gameObject.SetActive(false);
        stackLabel.gameObject.SetActive(false);
        healthBar.SetActive(false);

        draggedItem = null;
    }

    private void Update()
    {
        if (draggedItem == null) return;

        FollowMouse();
    }

    private void FollowMouse()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiCanvas.transform as RectTransform, 
            Input.mousePosition, 
            uiCanvas.worldCamera, 
            out var canvasMousePosition);

        transform.position = uiCanvas.transform.TransformPoint(canvasMousePosition);
    }
}
