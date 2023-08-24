using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditorInternal.VersionControl;
using UnityEngine;

public class ItemsPooler : ControllerBase
{
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
    
    // Items are spawned in clusters.
    // When dropping item, check for closest cluster, if too far away, create new cluster
    [SerializeField] private ItemDrop itemDropPrefab;
    [SerializeField] private LayerMask groundLayerMask;

    private List<ItemCluster> itemClusters;

    public override void Init(GameController gameController)
    {
        base.Init(gameController);

        // TODO: save cluster data to map.
        itemClusters = new List<ItemCluster>();
    }

    // TODO: add scatter bool.
    public void DropItem(ItemInfo itemInfo, Vector3 dropOrigin)
    {
        var closestCluster = GetClosestCluster(dropOrigin);
        if (closestCluster == null)
        {
            closestCluster = new ItemCluster();
            
            itemClusters.Add(closestCluster);
        }
        
        var itemDrop = CreateItemDrop(closestCluster.ClusterId, itemInfo, dropOrigin);
        closestCluster.AddItem(itemDrop);
    }

    private ItemDrop CreateItemDrop(string clusterId, ItemInfo itemInfo, Vector3 dropOrigin)
    {
        var dropPosition = GetDropPosition(dropOrigin);
        var newItemDrop = Instantiate(itemDropPrefab, transform);
        newItemDrop.Init(clusterId, itemInfo);
        
        newItemDrop.transform.position = dropPosition;
        newItemDrop.gameObject.SetActive(true);

        return newItemDrop;
    }

    private Vector3 GetDropPosition(Vector3 startPosition)
    {
        if (Physics.Raycast(startPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
        {
            return hit.point;
        }

        return startPosition;
    }
    
    private ItemCluster GetClosestCluster(Vector3 dropOrigin)
    {
        ItemCluster closestCluster = null;
        float closestClusterDist = 0f;
        
        foreach (var cluster in itemClusters)      
        {
            var distance = Vector3.Distance(dropOrigin, cluster.ClusterOrigin);
            if (closestCluster == null || closestClusterDist > distance)
            {
                closestCluster = cluster;
                closestClusterDist = distance;
            }
        }

        return closestCluster;
    }
}
