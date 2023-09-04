using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemsController : ControllerBase
{
   [SerializeField] private ItemDatabase itemDatabase;
   [SerializeField] private ItemsPooler itemsPooler;
   [SerializeField] private LayerMask groundLayerMask;

   private List<ItemCluster> itemClusters = new();

   private PlayerSpawner playerSpawner;
   private InteractionController interactionController;
   private InventoryController inventoryController;

   private const float SCATTER_WEIGHT = 0.5f;
   private const float MAX_CLUSTER_RADIUS = 5f;

   public event Action<ItemDrop> OnItemPickedUp;

   protected override void AwakeController()
   {
       inventoryController = gameController.GetController<InventoryController>();
   }

   // GetItemConfigById(string itemId)
   // Control clusters of items.

   public Item CreateItem(string itemKey, int stack)
   {
       var itemConfig = itemDatabase.itemConfigs.FirstOrDefault(x => x.ItemKey == itemKey);
       if (itemConfig != null)
           return new Item(itemConfig, stack);

       Debug.LogWarning($"Item of key [{itemKey}] does not exist in the database.");
           
       return null;
   }
   
   public void DropItem(ItemContainer container, Item item, int amount, Vector3 dropOrigin, bool scatter = true)
   {
       var closestCluster = GetClosestCluster(dropOrigin);
       if (closestCluster == null)
       {
           closestCluster = new ItemCluster();
           
           itemClusters.Add(closestCluster);
       }

       if (scatter)
       {
           var xOffset = Random.Range(-SCATTER_WEIGHT,SCATTER_WEIGHT);
           var zOffset = Random.Range(-SCATTER_WEIGHT, SCATTER_WEIGHT);
           dropOrigin += new Vector3(xOffset, 0f, zOffset);
       }

       var itemDrop = GenerateItemDrop(closestCluster, item, amount, dropOrigin);
       closestCluster.AddItem(itemDrop);
       
       container.RemoveItemFromContainer(item, amount, itemDrop);
   }

   private ItemDrop GenerateItemDrop(ItemCluster itemCluster, Item item, int amount, Vector3 dropOrigin)
   {
       var dropPosition = GetDropPosition(dropOrigin);
       var itemDrop = itemsPooler.GetItemDrop(item);
       
       if (item.ItemStack != amount)
       {
           var copyOfItem = new Item(item.ItemConfig, amount);
           
           itemDrop.Setup(itemCluster.ClusterId, copyOfItem, dropPosition);
       }
       else
       {
           itemDrop.Setup(itemCluster.ClusterId, item, dropPosition);
       }

       return itemDrop;
   }
   
   private Vector3 GetDropPosition(Vector3 startPosition)
   {
       startPosition += new Vector3(0f, 0.5f, 0f);
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
           if (distance > MAX_CLUSTER_RADIUS) continue;

           if (closestCluster == null || closestClusterDist > distance)
           {
               closestCluster = cluster;
               closestClusterDist = distance;
           }
       }

       return closestCluster;
   }

   public void PickupItem(ItemDrop itemDrop)
   {
       var leftover = inventoryController.InventoryContainer.AddItem(itemDrop.Item);
       if (leftover == 0)
       {
           var cluster = GetClusterOfItem(itemDrop);
           if (cluster != null)
           {
               cluster.RemoveItem(itemDrop);
               
               itemsPooler.ReturnItemDropToPool(itemDrop);
           }
           else
           {
               Debug.LogWarning($"Item of ID {itemDrop.Item.ItemId} does not belong to any cluster!");
           }
       }

       OnItemPickedUp?.Invoke(itemDrop);
   }

   private ItemCluster GetClusterOfItem(ItemDrop itemDrop) => itemClusters.FirstOrDefault(x => x.ItemDrops.Exists(y => y == itemDrop));

#if UNITY_EDITOR
   private void OnDrawGizmos()
   {
       if (itemClusters == null) return;
       
       foreach (var cluster in itemClusters)
       {
           var center = cluster.GetClusterCenter();
           foreach (var itemDrop in cluster.ItemDrops)
           {
               Gizmos.DrawLine(center, itemDrop.transform.position);
           }
       }
   }
#endif
    
}
