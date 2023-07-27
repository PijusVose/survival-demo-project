using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryController : ControllerBase
{
    private List<Item> itemsHolder;

    public List<Item> Items => itemsHolder;

    public event Action<Item> OnItemAdded;
    public event Action<Item> OnItemRemoved;
    public event Action<Item> OnItemChanged;

    public const int MAX_SLOTS = 40;

    // TODO: save/load inventory from json.
    
    public override void Init(GameController gameController)
    {
        base.Init(gameController);

        itemsHolder = new List<Item>();
    }

    public int AddItem(ItemConfigBase config, int amount)
    {
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            var existingItemWithSpace = FindItemOfTypeWithSpace(config);
            if (existingItemWithSpace != null)
            {
                amount = existingItemWithSpace.IncreaseStack(amount);
                
                OnItemChanged?.Invoke(existingItemWithSpace);
            }
            else
            {
                if (itemsHolder.Count == MAX_SLOTS)
                    break;

                var addedCount = Mathf.Clamp(amount, 0, config.MaxStack);
                var freeSlotId = GetFreeSlotId();
                var item = new Item(config, addedCount, freeSlotId);

                amount -= addedCount;
                
                // TODO: EVEN BETTER IDEA, CREATE SLOT CLASS YOU DUMBASS.
                
                itemsHolder.Add(item);
                
                OnItemAdded?.Invoke(item);
            }
            
            if (amount == 0)
                break;
        }

        return amount;
    }

    private Item FindItemOfTypeWithSpace(ItemConfigBase itemConfig)
    {
        return itemsHolder.FirstOrDefault(x => x.ItemConfig == itemConfig && !x.IsFullStack());
    }

    public int GetFreeSlotId()
    {
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            if (itemsHolder.All(x => x.SlotId != i))
                return i;
        }

        return -1;
    }

    public Item GetItemInSlot(int slotId) =>
        itemsHolder.FirstOrDefault(x => x.SlotId == slotId);

    public void SwitchItems(Item firstItem, Item secondItem)
    {
        var firstSlotId = firstItem.SlotId;
        var secondSlotId = secondItem.SlotId;

        firstItem.SlotId = secondSlotId;
        secondItem.SlotId = firstSlotId;
        
        // Maybe call OnItemMoved?
    }

    public int MoveItem(Item itemToMove, int targetSlot)
    {
        var itemInSlot = GetItemInSlot(targetSlot);
        var overflow = 0;
        
        if (itemInSlot == null)
        {
            itemToMove.SlotId = targetSlot;
        
            OnItemChanged?.Invoke(itemToMove);
        }
        else if (itemInSlot.ItemConfig.ItemKey == itemToMove.ItemConfig.ItemKey)
        {
            overflow = itemInSlot.IncreaseStack(itemToMove.ItemStack);
            
            OnItemChanged?.Invoke(itemToMove);
        }
        else
        {
            Debug.Log($"Could not move item to slot {targetSlot}. Slot already occupied");
        }

        return overflow;
    }

    public void DropItem()
    {
        
    }
}
