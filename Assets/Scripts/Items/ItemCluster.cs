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

    public void RemoveItem(ItemDrop itemDrop)
    {
        itemDrops.Remove(itemDrop);
    }

    public Vector3 GetClusterCenter()
    {
        var totalX = 0f;
        var totalY = 0f;
        var totalZ = 0f;
        
        foreach(var itemDrop in itemDrops)
        {
            var pos = itemDrop.transform.position;
            totalX += pos.x;
            totalY += pos.y;
            totalZ += pos.z;
        }
        
        var centerX = totalX / itemDrops.Count;
        var centerY = totalY / itemDrops.Count;
        var centerZ = totalZ/ itemDrops.Count;

        return new Vector3(centerX, centerY, centerZ);
    }
}
