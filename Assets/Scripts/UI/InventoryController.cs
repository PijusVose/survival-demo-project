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

    public void AddItemToSlot(Item item, int slotId)
    {
        var itemInSlot = GetItemInSlot(slotId);
        if (itemInSlot != null)
        {
            if (itemInSlot.ItemConfig.ItemKey == item.ItemConfig.ItemKey && !itemInSlot.IsFullStack())
            {
                var leftover = itemInSlot.IncreaseStack(item.ItemStack);
                
                OnItemChanged?.Invoke(itemInSlot);
                
                if (leftover == 0)
                {
                    RemoveItem(item, item.ItemStack);
                }
                else
                {
                    var stackDiff = item.ItemStack - leftover;
                    
                    item.DecreaseStack(stackDiff);
                    
                    OnItemChanged?.Invoke(item);
                }
            }
            else
            {
                SwitchItems(item, itemInSlot);
            }
        }
        else
        {
            item.SlotId = slotId;
        
            OnItemChanged?.Invoke(item);
        }
    }

    public void RemoveItem(Item item, int amount)
    {
        var leftover = item.DecreaseStack(amount);
        if (leftover == 0)
        {
            item.SlotId = -1;
            itemsHolder.Remove(item);
        }

        OnItemRemoved?.Invoke(item);
    }

    public int AddItem(ItemConfigBase config, int amount)
    {
        var leftover = amount;
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            var existingItemWithSpace = FindItemOfTypeWithSpace(config);
            if (existingItemWithSpace != null)
            {
                leftover = existingItemWithSpace.IncreaseStack(leftover);
                
                OnItemChanged?.Invoke(existingItemWithSpace);
            }
            else
            {
                if (itemsHolder.Count == MAX_SLOTS)
                    break;

                var addedCount = Mathf.Clamp(leftover, 0, config.MaxStack);
                var freeSlotId = GetFreeSlotId();
                var item = new Item(config, addedCount, freeSlotId);

                leftover -= addedCount;

                itemsHolder.Add(item);
                
                OnItemAdded?.Invoke(item);
            }
            
            if (leftover == 0)
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

    private void SwitchItems(Item firstItem, Item secondItem)
    {
        var firstSlotId = firstItem.SlotId;
        var secondSlotId = secondItem.SlotId;
        
        firstItem.SlotId = secondSlotId;
        secondItem.SlotId = firstSlotId;
           
        OnItemChanged?.Invoke(firstItem);
        OnItemChanged?.Invoke(secondItem);
    }

    public void DropItem()
    {
        
    }
}
