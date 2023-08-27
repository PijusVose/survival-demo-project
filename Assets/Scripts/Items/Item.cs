using System;
using System.Diagnostics.Contracts;
using UnityEngine;

[Serializable]
public class Item
{
    public class ItemContainerInfo
    {
        public int SlotId { get; set; }
        public string ContainerId { get; set; }
    }

    public class ItemDropInfo
    {
        public string DropId { get; set; }
        public Vector3 DropPosition { get; set; }
    }

    private int itemStack;
    private ItemContainerInfo containerInfo;
    private ItemDropInfo dropInfo;

    private readonly string itemId;
    private readonly ItemConfigBase itemConfig;

    public Item(ItemConfigBase config, int stack)
    {
        itemId = Guid.NewGuid().ToString();
        itemConfig = config;
        itemStack = stack;
    }

    public ItemConfigBase ItemConfig => itemConfig;
    public string ItemId => itemId;
    public int ItemStack => itemStack;

    public ItemContainerInfo ContainerInfo => containerInfo;
    public ItemDropInfo DropInfo => dropInfo;
    public bool IsFullStack() => itemStack == itemConfig.MaxStack;

    public void MoveToContainer(string containerId, int slotId)
    {
        if (containerInfo == null)
            containerInfo = new ItemContainerInfo();

        containerInfo.SlotId = slotId;
        containerInfo.ContainerId = containerId;

        dropInfo = null;
    }

    public void MoveToDrop(string dropId, Vector3 dropPosition)
    {
        if (dropInfo == null)
            dropInfo = new ItemDropInfo();

        dropInfo.DropId = dropId;
        dropInfo.DropPosition = dropPosition;
        
        containerInfo = null;
    }

    public int IncreaseStack(int amount)
    {
        var carry = 0;

        if (itemStack + amount > itemConfig.MaxStack)
            carry = itemStack + amount - itemConfig.MaxStack;

        itemStack = Mathf.Clamp(itemStack + amount, 0, itemConfig.MaxStack);
        
        return carry;
    }

    public int DecreaseStack(int amount)
    {
        var carry = itemStack - amount;

        if (carry < 0)
            carry = Mathf.Abs(itemStack - amount);

        itemStack = Mathf.Max(itemStack - amount, 0);
        
        return carry;
    }

    public void SetStack(int amount)
    {
        itemStack = amount;
    }
}
