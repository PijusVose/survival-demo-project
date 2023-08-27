using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField] private string containerId;
    [SerializeField] private string containerName;
    [SerializeField] private int slotsAmount;
    
    private List<Item> containerItems = new();

    public event Action<Item> OnItemAdded;
    public event Action<Item> OnItemRemoved;
    public event Action<Item> OnItemChanged;

    public int SlotsAmount => slotsAmount;
    public string ContainerId => containerId;
    
    public void Init(int amountOfSlots)
    {
        containerId = Guid.NewGuid().ToString();
        slotsAmount = amountOfSlots;
    }
    
    public int AddItemToSlot(Item itemToAdd, int slotId)
    {
        var itemInSlot = GetItemInSlot(slotId);
        if (itemInSlot != null)
        {
            if (itemInSlot.ItemConfig.ItemKey == itemToAdd.ItemConfig.ItemKey)
            {
                if (itemInSlot.IsFullStack())
                {
                    OnItemChanged?.Invoke(itemToAdd);
                }
                else
                {
                    var leftover = itemInSlot.IncreaseStack(itemToAdd.ItemStack);
                    var amountToRemove = itemToAdd.ItemStack - leftover;
                    RemoveItem(itemToAdd, amountToRemove);
                
                    OnItemChanged?.Invoke(itemInSlot);
                }
            }
            else
            {
                var firstSlotId = itemToAdd.ContainerInfo.SlotId;
                var secondSlotId = itemInSlot.ContainerInfo.SlotId;
                
                itemInSlot.MoveToContainer(containerId, firstSlotId);
                itemToAdd.MoveToContainer(containerId, secondSlotId);

                OnItemChanged?.Invoke(itemInSlot);
                OnItemChanged?.Invoke(itemToAdd);

                return 0;
            }
        }
        else
        {
            itemToAdd.MoveToContainer(containerId, slotId);

            OnItemAdded?.Invoke(itemToAdd);

            return 0; 
        }

        return itemToAdd.ItemStack;
    }

    public bool IsContainerFull()
    {
        return containerItems.Count >= slotsAmount && containerItems.All(x => x.IsFullStack());
    }
    
    public void RemoveItem(Item item, int amount)
    {
        item.DecreaseStack(amount);

        if (item.ItemStack == 0)
        {
            OnItemRemoved?.Invoke(item);
            
            containerItems.Remove(item);
        }
        else
        {
            OnItemChanged?.Invoke(item);
        }
    }

    public Item SplitItem(Item item)
    {
        var splitStack = item.ItemStack / 2;
        var splitItem = new Item(item.ItemConfig, splitStack);
        splitItem.MoveToContainer(containerId, -1);

        RemoveItem(item, splitStack);
        
        containerItems.Add(splitItem);

        return splitItem;
    }

    public void AddItem(Item item)
    {
        for (int i = 0; i < slotsAmount; i++)
        {
            var existingItemWithSpace = FindItemOfTypeWithSpace(item.ItemConfig);
            if (existingItemWithSpace != null)
            {
                var leftover = existingItemWithSpace.IncreaseStack(item.ItemStack);

                item.SetStack(leftover);
                
                OnItemChanged?.Invoke(existingItemWithSpace);

                if (leftover == 0)
                    break;
            }
            else
            {
                if (IsContainerFull()) break;

                var slotId = GetAvailableSlotId();
                item.MoveToContainer(containerId, slotId);

                containerItems.Add(item);
                
                OnItemAdded?.Invoke(item);
            }
        }
        
        // TODO: what happens if item doesn't fit anymore in inventory? Just return it?
    }

    private Item FindItemOfTypeWithSpace(ItemConfigBase itemConfig)
    {
        return containerItems.FirstOrDefault(x => x.ItemConfig == itemConfig && !x.IsFullStack());
    }

    private int GetAvailableSlotId()
    {
        for (int i = 0; i < slotsAmount; i++)
        {
            if (containerItems.All(x => x.ContainerInfo.SlotId != i))
                return i;
        }

        return -1;
    }

    public Item GetItemInSlot(int slotId) =>
        containerItems.FirstOrDefault(x => x.ContainerInfo.SlotId == slotId);
    
}
