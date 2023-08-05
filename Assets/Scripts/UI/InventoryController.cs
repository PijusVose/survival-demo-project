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

    public void AddItemToSlot(ItemInfo itemInfo, int slotId)
    {
        var itemInSlot = GetItemInSlot(slotId);
        if (itemInSlot != null)
        {
            if (itemInSlot.ItemConfig.ItemKey == itemInfo.itemConfig.ItemKey)
            {
                if (itemInSlot.IsFullStack())
                {
                    AddItemToSlot(itemInfo, itemInfo.originalSlotId);
                }
                else
                {
                    itemInfo.itemStack = itemInSlot.IncreaseStack(itemInfo.itemStack);
                
                    OnItemChanged?.Invoke(itemInSlot);

                    if (itemInfo.itemStack > 0)
                    {
                        var originalItem = GetItemInSlot(itemInfo.originalSlotId);
                        if (originalItem != null)
                        {
                            originalItem.IncreaseStack(itemInfo.itemStack);
                        
                            OnItemChanged?.Invoke(originalItem);
                        }
                        else
                        {
                            AddItem(itemInfo.itemConfig, itemInfo.itemStack);
                        }
                    }
                }
            }
            else
            {
                if (itemInfo.originalSlotId != -1)
                {
                    var originalItem = GetItemInSlot(itemInfo.originalSlotId);
                    if (originalItem != null)
                    {
                        originalItem.IncreaseStack(itemInfo.itemStack);
                        
                        OnItemChanged?.Invoke(originalItem);
                    }
                    else
                    {
                        itemInSlot.SlotId = itemInfo.originalSlotId;
                    
                        OnItemChanged?.Invoke(itemInSlot);

                        var item = new Item(itemInfo.itemConfig, itemInfo.itemStack, slotId);
                        itemsHolder.Add(item);
                    
                        OnItemAdded?.Invoke(item);
                    }
                }
                else
                {
                    AddItem(itemInfo.itemConfig, itemInfo.itemStack);
                }
            }
        }
        else
        {
            var item = new Item(itemInfo.itemConfig, itemInfo.itemStack, slotId);
            itemsHolder.Add(item);
                    
            OnItemAdded?.Invoke(item);
        }
    }

    public void RemoveItem(Item item, int amount)
    {
        var leftover = item.DecreaseStack(amount);
        
        OnItemRemoved?.Invoke(item);
        
        if (leftover == 0)
        {
            item.SlotId = -1;
            
            itemsHolder.Remove(item);
        }
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

    public Item SplitItem(Item item)
    {
        var originalStack = item.ItemStack / 2;
        var copiedStack = item.ItemStack - originalStack;

        RemoveItem(item, copiedStack);

        var copiedItem = CopyItem(item, copiedStack);

        return copiedItem;
    }

    private Item CopyItem(Item item, int amount)
    {
        var copiedItem =  new Item(item.ItemConfig, amount, item.SlotId);
        itemsHolder.Add(copiedItem);

        return copiedItem;
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
