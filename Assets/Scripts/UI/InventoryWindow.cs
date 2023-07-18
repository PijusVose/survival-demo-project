using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : WindowBase
{
    // Public fields
    
    [SerializeField] private MaterialConfig materialConfig;

    // TODO: add separate ItemCell for draggable cell.
    [SerializeField] private GameObject dragCell;
    [SerializeField] private Image dragCellIcon;
    [SerializeField] private TextMeshProUGUI dragCellStackLabel;
    [SerializeField] private GameObject dragCellHealthBar;
    [SerializeField] private RectTransform dragCellGreenBar;
    
    [SerializeField] private int draggedSlotId;
    
    // Private fields
    
    private PlayerSpawner playerSpawner;
    private CameraController cameraController;
    private InventoryController inventoryController;

    private ItemCell currentMouseOnCell;
    private bool isDraggingItem;

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
        inventoryController.OnItemAdded += Inventory_OnItemAdded;
        inventoryController.OnItemRemoved += Inventory_OnItemRemoved;
    }

    private void OnDestroy()
    {
        if (inventoryController == null) return;
        
        inventoryController.OnItemAdded -= Inventory_OnItemAdded;
        inventoryController.OnItemRemoved -= Inventory_OnItemRemoved;
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
        
        playerSpawner.Player.SetInputState(true);
        cameraController.SetCameraMovementState(true);
    }

    private void Update()
    {
        if (!isOpen) return;
        
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Y))
        {
            inventoryController.AddItem(materialConfig, 1);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            inventoryController.AddItem(materialConfig, 10);
        }
#endif

        if (Input.GetMouseButtonDown(0) && currentMouseOnCell != null)
        {
            // TODO: check if cell has item, the
        }
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

            if (i < inventoryController.Items.Length && inventoryController.Items[i] != null)
            {
                itemCells[i].PlaceItem(inventoryController.Items[i]);
            }
        }
    }

    private void Inventory_OnItemAdded(int slotId, int amountAdded)
    {
        if (!isOpen) return;
        
        if (slotId >= 0 && slotId < itemCells.Length)
        {
            var item = inventoryController.Items[slotId];
            
            itemCells[slotId].PlaceItem(item);
        }
        else
        {
            Debug.LogWarning($"Slot of ID {slotId} does not exist, could not add item.");
        }
    }
    
    private void Inventory_OnItemRemoved(int slotId, int amountRemoved)
    {
        if (!isOpen) return;
        
        if (slotId >= 0 && slotId < itemCells.Length)
        {
            var item = inventoryController.Items[slotId];
            
            itemCells[slotId].RemoveItem(item.ItemStack);
        }
        else
        {
            Debug.LogWarning($"Slot of ID {slotId} does not exist, could not remove item.");
        }
    }

    private void StartDragging()
    {
        if (currentMouseOnCell != null)
        {
            draggedSlotId = currentMouseOnCell.CellIndex;
            
            dragCellIcon.sprite = currentMouseOnCell.StoredItem.ItemConfig.ItemIcon;
            dragCellStackLabel.text = currentMouseOnCell.StoredItem.ItemStack.ToString();
            
            // TODO: show stack/healthbar depending on item type.
            
            dragCell.SetActive(true);

            isDraggingItem = true;
        }
    }

    private void StopDragging()
    {
        isDraggingItem = false;
        // If there no UI object under, drop item from inventory.
        // If there is another cell under, replace items.
        // If cell is empty, just place item there.
    }

    // If mouse leaves cell, don't set to null?
    // Make IPointerEnter/Exit for background as well?
    // So if released between cells, item goes to last entered cell or something.
    // Only if outside background drop item on ground.
    public void SetMouseOverCell(ItemCell cell)
    {
        if (currentMouseOnCell != null && currentMouseOnCell != cell)
        {
            // TODO: What to do in this situation? Maybe just force set mouse over cell?
        }
        else
        {
            currentMouseOnCell = cell;
        }
    }
}
