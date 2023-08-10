using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField] private string containerId;
    [SerializeField] private string containerName;
    [SerializeField] private int amountOfSlots;
    
    private List<Item> containerItems = new List<Item>();
    
    public event Action<Item> OnItemAdded;
    public event Action<Item> OnItemRemoved;
    public event Action<Item> OnItemChanged;

    public int AmountOfSlots => amountOfSlots;
    public List<Item> GetItems() => containerItems;
    
    public void Init(int amountOfSlots)
    {
        containerId = Guid.NewGuid().ToString();
        
        this.amountOfSlots = amountOfSlots;
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
                            AddItemToSlot(itemInfo, itemInfo.originalSlotId);
                        }
                    }
                }
            }
            else
            {
                var originalSlotItem = GetItemInSlot(itemInfo.originalSlotId);
                if (originalSlotItem != null)
                {
                    originalSlotItem.IncreaseStack(itemInfo.itemStack);
                    
                    OnItemChanged?.Invoke(originalSlotItem);
                }
                else
                {
                    itemInSlot.SlotId = itemInfo.originalSlotId;
                    
                    OnItemChanged?.Invoke(itemInSlot);

                    var item = new Item(itemInfo.itemConfig, itemInfo.itemStack, slotId);
                    containerItems.Add(item);
                    
                    OnItemAdded?.Invoke(item);
                }
            }
        }
        else
        {
            var item = new Item(itemInfo.itemConfig, itemInfo.itemStack, slotId);
            containerItems.Add(item);
                    
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
            
            containerItems.Remove(item);
        }
    }

    public int AddItem(ItemConfigBase config, int amount)
    {
        var leftover = amount;
        for (int i = 0; i < amountOfSlots; i++)
        {
            var existingItemWithSpace = FindItemOfTypeWithSpace(config);
            if (existingItemWithSpace != null)
            {
                leftover = existingItemWithSpace.IncreaseStack(leftover);
                
                OnItemChanged?.Invoke(existingItemWithSpace);
            }
            else
            {
                if (containerItems.Count == amountOfSlots)
                    break;

                var addedCount = Mathf.Clamp(leftover, 0, config.MaxStack);
                var freeSlotId = GetFreeSlotId();
                var item = new Item(config, addedCount, freeSlotId);

                leftover -= addedCount;

                containerItems.Add(item);
                
                OnItemAdded?.Invoke(item);
            }
            
            if (leftover == 0)
                break;
        }

        return amount;
    }

    private Item FindItemOfTypeWithSpace(ItemConfigBase itemConfig)
    {
        return containerItems.FirstOrDefault(x => x.ItemConfig == itemConfig && !x.IsFullStack());
    }

    public int GetFreeSlotId()
    {
        for (int i = 0; i < amountOfSlots; i++)
        {
            if (containerItems.All(x => x.SlotId != i))
                return i;
        }

        return -1;
    }
    
    private Item CopyItem(Item item, int amount)
    {
        var copiedItem =  new Item(item.ItemConfig, amount, item.SlotId);
        containerItems.Add(copiedItem);

        return copiedItem;
    }

    public Item GetItemInSlot(int slotId) =>
        containerItems.FirstOrDefault(x => x.SlotId == slotId);
    
    public void DropItem()
    {
        
    }

    public void DropAllItems()
    {
        
    }
}
