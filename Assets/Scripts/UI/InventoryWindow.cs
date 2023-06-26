using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryWindow : WindowBase
{
    private PlayerSpawner playerSpawner;
    private CameraController cameraController;
    private InventoryController inventoryController;

    private ItemCell currentMouseOnCell;
    
    public override void Init(UIController uiController, GameController gameController)
    {
        base.Init(uiController, gameController);
        
        playerSpawner = gameController.GetController<PlayerSpawner>();
        cameraController = gameController.GetController<CameraController>();
        inventoryController = gameController.GetController<InventoryController>();
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

    private void LoadInventory()
    {
        foreach (var itemData in inventoryController.Items)
        {
            
        }
    }

    private ItemCell GetCellByIndex(int index)
    {
        
    }

    // If mouse leaves cell, don't set to null?
    // Make IPointerEnter/Exit for background as well?
    // So if released between cells, item goes to last entered cell or something.
    // Only if outside background drop item on ground.
    public void SetMouseOverCell(ItemCell cell)
    {
        if (currentMouseOnCell != null)
        {
            // Handle this case.
        }
        else
        {
            currentMouseOnCell = cell;
        }
    }
}
