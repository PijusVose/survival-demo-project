using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventoryWindow : WindowBase, IPointerEnterHandler, IPointerExitHandler
{
    // Public fields
    
    [SerializeField] private MaterialConfig firstMaterialConfig;
    [SerializeField] private MaterialConfig secondMaterialConfig;
    
    // TODO: add separate ItemCell for draggable cell.
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private RectTransform dragCell;
    [SerializeField] private Image dragCellIcon;
    [SerializeField] private TextMeshProUGUI dragCellStackLabel;
    [SerializeField] private GameObject dragCellHealthBar;
    [SerializeField] private RectTransform dragCellGreenBar;

    // Private fields
    
    private PlayerSpawner playerSpawner;
    private CameraController cameraController;
    private InventoryController inventoryController;

    private Item draggedItem;
    private int mouseOverCellId = -1;
    private bool isDraggingItem;
    private bool isMouseInInventory;

    private ItemCell[] itemCells;
    
    public override void Init(UIController uiController, GameController gameController)
    {
        base.Init(uiController, gameController);
        
        playerSpawner = gameController.GetController<PlayerSpawner>();
        cameraController = gameController.GetController<CameraController>();
        inventoryController = gameController.GetController<InventoryController>();
        
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        inventoryController.OnItemAdded += Inventory_OnItemChanged;
        inventoryController.OnItemChanged += Inventory_OnItemChanged;
        inventoryController.OnItemRemoved += Inventory_OnItemChanged;
    }

    private void OnDestroy()
    {
        if (inventoryController == null) return;
        
        inventoryController.OnItemAdded -= Inventory_OnItemChanged;
        inventoryController.OnItemChanged -= Inventory_OnItemChanged;
        inventoryController.OnItemRemoved -= Inventory_OnItemChanged;
    }

    public override void Open()
    {
        LoadInventory();
        
        base.Open();
        
        playerSpawner.Player.SetInputState(false);
        cameraController.SetCameraMovementState(false, true);
    }

    public override void Close()
    {
        base.Close();
        
        if (isDraggingItem)
            CancelDragging();
        
        playerSpawner.Player.SetInputState(true);
        cameraController.SetCameraMovementState(true);
    }

    private void Update()
    {
        if (!isOpen) return;
        
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Y))
        {
            inventoryController.AddItem(firstMaterialConfig, 1);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            inventoryController.AddItem(firstMaterialConfig, 10);
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            inventoryController.AddItem(secondMaterialConfig, 1);
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            inventoryController.AddItem(secondMaterialConfig, 10);
        }
#endif
        
        // TODO: right click takes half of item.
        if (Input.GetMouseButtonDown(0) && mouseOverCellId != -1 && !isDraggingItem)
        {
            StartDragging();
        }
        else if (Input.GetMouseButtonUp(0) && isDraggingItem)
        {
            StopDragging();
        }

        DoDragging();
    }

    // 1. Get all item cells/slots
    // 2. Initialize them and set their indexes
    // Cell indexes start from 0
    // 3. Get all items in inventory
    // 4. Each item has cell index assigned, so find the cell with that index and add item to that cell.
    private void LoadInventory()
    {
        // TODO: use get component from cells parent.
        itemCells = GetComponentsInChildren<ItemCell>(includeInactive: true);

        if (itemCells.Length != InventoryController.MAX_SLOTS)
            Debug.LogWarning("Inventory window slots don't match actual inventory slots!");

        for (int i = 0; i < itemCells.Length; i++)
        {
            itemCells[i].Init(this, i);

            var item = inventoryController.GetItemInSlot(i);
            if (item != null)
            {
                itemCells[i].UpdateSlotItem(item);
            }
        }
    }

    private void Inventory_OnItemChanged(Item item)
    {
        if (!isOpen) return;
        
        if (item.SlotId >= 0 && item.SlotId < itemCells.Length)
        {
            itemCells[item.SlotId].UpdateSlotItem(item);
        }
        else
        {
            Debug.LogWarning($"Slot of ID {item.SlotId} does not exist, could not remove item.");
        }
    }

    private void DoDragging()
    {
        if (isDraggingItem)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(uiCanvas.transform as RectTransform, 
                Input.mousePosition, 
                uiCanvas.worldCamera, 
                out var canvasMousePosition);

            dragCell.position = uiCanvas.transform.TransformPoint(canvasMousePosition);
        }
    }

    private void StartDragging()
    {
        if (mouseOverCellId != -1)
        {
            draggedItem = inventoryController.GetItemInSlot(mouseOverCellId);
            if (draggedItem != null)
            {
                itemCells[draggedItem.SlotId].UpdateSlotItem(null);
            
                dragCellIcon.sprite = draggedItem.ItemConfig.ItemIcon;
                dragCellStackLabel.text = draggedItem.ItemStack.ToString();
            
                dragCellIcon.gameObject.SetActive(true);
                dragCellStackLabel.gameObject.SetActive(true);
                dragCell.gameObject.SetActive(true);
            
                // TODO: show stack/healthbar depending on item type.

                isDraggingItem = true;
            }
        }
    }

    private void StopDragging()
    {
        isDraggingItem = false;

        if (mouseOverCellId != -1 && draggedItem != null)
        {
            inventoryController.AddItemToSlot(draggedItem, mouseOverCellId);
        }
        else
        {
            // TODO: Drop item on the ground if mouse not above window.
            CancelDragging(); // For now, just cancel dragging.
        }
        
        dragCell.gameObject.SetActive(false);
        dragCellIcon.gameObject.SetActive(false);
        dragCellStackLabel.gameObject.SetActive(false);

        draggedItem = null;
    }

    private void CancelDragging()
    {
        isDraggingItem = false;
        
        itemCells[draggedItem.SlotId].UpdateSlotItem(draggedItem);
        
        dragCell.gameObject.SetActive(false);
        dragCellIcon.gameObject.SetActive(false);
        dragCellStackLabel.gameObject.SetActive(false);

        draggedItem = null;
    }
    
    public void SetMouseOverCell(int cellId)
    {
        // TODO: some issue where item gets stuck in cell.
        if (isMouseInInventory && cellId == -1)
            return;
        
        mouseOverCellId = cellId;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseInInventory = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseInInventory = false;
    }
}
