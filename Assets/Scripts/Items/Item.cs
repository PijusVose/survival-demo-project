using System;
using UnityEngine;

[Serializable]
public class Item
{
    private int itemStack;
    private int slotId;
    
    private readonly string itemId;
    private readonly ItemConfigBase itemConfig;

    public Item(ItemConfigBase config, int stack, int slotId)
    {
        itemId = Guid.NewGuid().ToString();
        
        itemConfig = config;
        itemStack = stack;
        this.slotId = slotId;
    }
    
    public ItemConfigBase ItemConfig => itemConfig;
    public string ItemId => itemId;
    public int ItemStack => itemStack;

    public int SlotId
    {
        get => slotId;
        set => slotId = value;
    }
    
    public int AvailableSpace => itemConfig.MaxStack - itemStack;

    public bool IsFullStack() => itemStack == itemConfig.MaxStack;
    
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
}
