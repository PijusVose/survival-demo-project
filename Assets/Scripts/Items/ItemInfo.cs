using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo
{
    public ItemConfigBase itemConfig;
    public int itemStack;
    public int originalSlotId;

    public ItemInfo(ItemConfigBase itemConfig, int itemStack, int originalSlotId = -1)
    {
        this.itemConfig = itemConfig;
        this.itemStack = itemStack;
        this.originalSlotId = originalSlotId;
    }
}
