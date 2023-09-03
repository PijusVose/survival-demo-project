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
    public List<Item> Items => containerItems;
    
    public void Init(int amountOfSlots)
    {
        containerId = Guid.NewGuid().ToString();
        slotsAmount = amountOfSlots;
    }

    public bool IsContainerFull() => containerItems.Count >= slotsAmount && containerItems.All(x => x.IsFullStack());

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

    public void RemoveItemFromContainer(Item item, int amount, ItemDrop itemDrop)
    {
        if (item.ItemStack == amount)
        {
            OnItemRemoved?.Invoke(item);
            
            item.MoveToDrop(itemDrop.DropId, itemDrop.transform.position);

            containerItems.Remove(item);
        }
        else
        {
            var itemCopy = new Item(item.ItemConfig, amount);
            itemCopy.MoveToDrop(itemDrop.DropId, itemDrop.transform.position);
            
            item.DecreaseStack(amount);
            
            OnItemChanged?.Invoke(item);
        }
    }

    // Move X amount of item to this slot
    // Check if slot has item
    // - Is item in slot the same item?
    // - Is item in slot of same type?
    // - Is item in slot of different type?
    // --- 
    
    public void MoveItemToSlot(Item itemToMove, int slotId, int stack)
    {
        var itemInSlot = GetItemInSlot(slotId);
        if (itemInSlot != null)
        {
            if (itemInSlot.ItemId == itemToMove.ItemId)
            {
                OnItemChanged?.Invoke(itemToMove);
            }
            else if (itemInSlot.ItemConfig.ItemKey == itemToMove.ItemConfig.ItemKey)
            {
                if (itemInSlot.IsFullStack())
                {
                    OnItemChanged?.Invoke(itemToMove);
                }
                else
                {
                    var leftover = itemInSlot.IncreaseStack(stack);
                    var stackToRemove = stack - leftover;
                    
                    RemoveItem(itemToMove, stackToRemove);
                    
                    OnItemChanged?.Invoke(itemToMove);
                    OnItemChanged?.Invoke(itemInSlot);
                }
            }
            else if (itemToMove.ItemStack == stack)
            {
                var firstSlotId = itemToMove.ContainerInfo.SlotId;
                var secondSlotId = itemInSlot.ContainerInfo.SlotId;
                
                itemInSlot.MoveToContainer(containerId, firstSlotId);
                itemToMove.MoveToContainer(containerId, secondSlotId);

                OnItemChanged?.Invoke(itemInSlot);
                OnItemChanged?.Invoke(itemToMove);
            }
            else
            {
                OnItemChanged?.Invoke(itemToMove);
            }
        }
        else
        {
            if (itemToMove.ItemStack != stack)
            {
                var itemCopy = new Item(itemToMove.ItemConfig, stack);
                itemCopy.MoveToContainer(containerId, slotId);
                
                containerItems.Add(itemCopy);

                itemToMove.DecreaseStack(stack);
                
                OnItemChanged?.Invoke(itemToMove);
                OnItemChanged?.Invoke(itemCopy);
            }
            else
            {
                itemToMove.MoveToContainer(containerId, slotId);
                
                OnItemChanged?.Invoke(itemToMove);
            }
        }
    }

    public int AddItem(Item item)
    {
        for (int i = 0; i < slotsAmount; i++)
        {
            if (IsContainerFull()) return item.ItemStack;
            
            var existingItemWithSpace = FindItemOfTypeWithSpace(item.ItemConfig);
            if (existingItemWithSpace != null)
            {
                var leftover = existingItemWithSpace.IncreaseStack(item.ItemStack);

                item.SetStack(leftover);
                
                OnItemChanged?.Invoke(existingItemWithSpace);

                if (leftover == 0)
                    return 0;
            }
            else
            {
                var slotId = GetAvailableSlotId();
                if (slotId == -1) break;
                
                item.MoveToContainer(containerId, slotId);

                containerItems.Add(item);
                
                OnItemAdded?.Invoke(item);

                return 0;
            }
        }

        return item.ItemStack;
    }

    private Item FindItemOfTypeWithSpace(ItemConfigBase itemConfig)
    {
        return containerItems.FirstOrDefault(x => x.ItemConfig == itemConfig && !x.IsFullStack());
    }

    private Item CopyItem(Item itemToCopy, int stack) => new Item(itemToCopy.ItemConfig, stack);

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
