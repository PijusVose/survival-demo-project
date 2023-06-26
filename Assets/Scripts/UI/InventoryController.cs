using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : ControllerBase
{
    private List<Item> inventoryItems;

    public List<Item> Items => inventoryItems;

    public event Action<Item> OnItemAdded;
    public event Action<Item> OnItemRemoved;
    public event Action<Item> OnItemUpdated;
    
    public override void Init(GameController gameController)
    {
        base.Init(gameController);
    }

    public void AddItem(ItemConfigBase config, int count)
    {
        var item = new Item(config);
        item.

        OnItemAdded?.Invoke();
    }

    public void DropItem()
    {
        
    }
}
