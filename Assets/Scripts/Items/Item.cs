using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    private int itemStack;
    
    private readonly string itemId;
    private readonly ItemConfigBase itemConfig;

    public Item(ItemConfigBase config, int stack)
    {
        itemId = System.Guid.NewGuid().ToString();
        
        itemConfig = config;
        itemStack = stack;
    }
    
    public ItemConfigBase ItemConfig => itemConfig;
    public string ItemId => itemId;
    public int ItemStack => itemStack;

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
        var carry = 0;

        if (itemStack - amount < 0)
            carry = Mathf.Abs(itemStack - amount);

        itemStack = Mathf.Max(itemStack - amount, 0);
        
        return carry;
    }
}
