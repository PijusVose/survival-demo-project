using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DragItemSlot : MonoBehaviour
{
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private Image slotIcon;
    [SerializeField] private TextMeshProUGUI stackLabel;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private RectTransform greenBarTransform;

    private ItemInfo dragItemInfo;

    public ItemInfo DraggedItemInfo => dragItemInfo;
    
    // TODO: Init with UiController and set canvas.

    public void Init()
    {
        var uiController = GameController.Instance.GetController<UIController>();
        uiCanvas = uiController.UICanvas;
    }
    
    public void EnableDragSlot(ItemInfo itemInfo)
    {
        dragItemInfo = itemInfo;
        
        slotIcon.sprite = itemInfo.itemConfig.ItemIcon;
        stackLabel.text = itemInfo.itemStack.ToString();
            
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

        dragItemInfo = null;
    }

    private void Update()
    {
        if (dragItemInfo == null) return;

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
