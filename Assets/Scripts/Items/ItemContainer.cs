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
    
    public int AddItemToSlot(Item itemToAdd, int slotId)
    {
        var itemInSlot = GetItemInSlot(slotId);
        if (itemInSlot != null)
        {
            if (itemInSlot.ItemId == itemToAdd.ItemId)
            {
                OnItemChanged?.Invoke(itemToAdd);
            }
            else if (itemInSlot.ItemConfig.ItemKey == itemToAdd.ItemConfig.ItemKey)
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

    public void AddItem(Item item)
    {
        for (int i = 0; i < slotsAmount; i++)
        {
            if (IsContainerFull()) break;
            
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
                var slotId = GetAvailableSlotId();
                if (slotId == -1) break;
                
                item.MoveToContainer(containerId, slotId);

                containerItems.Add(item);
                
                OnItemAdded?.Invoke(item);

                break;
            }
        }
        
        // TODO: what happens if item doesn't fit anymore in inventory? Just drop it on the ground?
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
