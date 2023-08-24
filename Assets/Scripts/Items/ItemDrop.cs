using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int itemStack;
    [SerializeField] private string itemId;
    [SerializeField] private string clusterId;
    [SerializeField] private ItemConfigBase itemConfig;

    public int ItemStack => itemStack;
    public string ItemId => itemId;
    public ItemConfigBase ItemConfig => itemConfig;
    
    // TODO: reference to who dropped item.

    public void Init(string clusterId, ItemInfo itemInfo)
    {
        this.clusterId = clusterId;
        itemId = Guid.NewGuid().ToString();
        itemConfig = itemInfo.itemConfig;
        itemStack = itemInfo.itemStack;
        
        SetupVisuals();
    }

    private void SetupVisuals()
    {
        // TODO: get item info from database, assign model.
        // TODO: rotate item randomly along Y axis.
    }

    public void PickupDrop()
    {
        
    }
}
