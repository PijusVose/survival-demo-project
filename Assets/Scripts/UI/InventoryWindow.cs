using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryWindow : WindowBase
{
    [SerializeField] private MaterialConfig materialConfig;
    
    private PlayerSpawner playerSpawner;
    private CameraController cameraController;
    private InventoryController inventoryController;

    private ItemCell currentMouseOnCell;

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
        
        // DEBUG STUFF.
        if (Input.GetKeyDown(KeyCode.Y))
        {
            inventoryController.AddItem(materialConfig, 1);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            inventoryController.AddItem(materialConfig, 10);
        }
    }

    // 1. Get all item cells/slots
    // 2. Initialize them and set their indexes
    // Cell indexes start from 0
    // 3. Get all items in inventory
    // 4. Each item has cell index assigned, so find the cell with that index and add item to that cell.
    private void LoadInventory()
    {
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

    public void Inventory_OnItemAdded(int slotId, int amountAdded)
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
    
    public void Inventory_OnItemRemoved(int slotId, int amountRemoved)
    {
        if (!isOpen) return;
        
        // TODO: remove item from slot.
    }
    
    private ItemCell GetCellByIndex(int index)
    {
        return itemCells.FirstOrDefault(x => x.CellIndex == index);
    }

    // If mouse leaves cell, don't set to null?
    // Make IPointerEnter/Exit for background as well?
    // So if released between cells, item goes to last entered cell or something.
    // Only if outside background drop item on ground.
    public void SetMouseOverCell(ItemCell cell)
    {
        if (currentMouseOnCell != null)
        {
            // TODO: What to do in this situation? Maybe just force set mouse over cell?
        }
        else
        {
            currentMouseOnCell = cell;
        }
    }
}
