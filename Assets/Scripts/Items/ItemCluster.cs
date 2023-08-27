using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemCluster
{
    private string clusterId;
    private Vector3 clusterOrigin;
    private List<ItemDrop> itemDrops;

    public string ClusterId => clusterId;
    public Vector3 ClusterOrigin => clusterOrigin;
    public List<ItemDrop> ItemDrops => itemDrops;

    // TODO: max cluster items?

    public ItemCluster()
    {
        clusterId = Guid.NewGuid().ToString();
        itemDrops = new List<ItemDrop>();
    }

    public void AddItem(ItemDrop itemDrop)
    {
        if (itemDrops.Count == 0)
            clusterOrigin = itemDrop.transform.position;
            
        itemDrops.Add(itemDrop);
    }
}
