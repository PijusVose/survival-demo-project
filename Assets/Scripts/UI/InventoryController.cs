using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryController : ControllerBase
{
    [SerializeField] private ItemContainer inventoryContainer;
    
    public ItemContainer InventoryContainer => inventoryContainer;

    private const int MAX_INVENTORY_SPACE = 40;

    protected override void AwakeController()
    {
        base.AwakeController();

        inventoryContainer.Init(MAX_INVENTORY_SPACE);
        
        // TODO: save/load inventory from json.
    }

    public bool IsInventoryContainer(string containerId) => inventoryContainer.ContainerId == containerId;
}
