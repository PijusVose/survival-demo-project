using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemsController : ControllerBase
{
   [SerializeField] private ItemDatabase itemDatabase;
   [SerializeField] private ItemsPooler itemsPooler;
   [SerializeField] private LayerMask groundLayerMask;

   private List<ItemCluster> itemClusters;
   
   public override void Init(GameController gameController)
   {
       base.Init(gameController);
       
       itemClusters = new List<ItemCluster>();
   }
   
   // GetItemConfigById(string itemId)
   // DropItemFromContainer(ItemInfo itemInfo)
   // CreateItemDrop
   // Control clusters of items.

   public Item CreateItem(string itemKey, int stack)
   {
       var itemConfig = itemDatabase.itemConfigs.FirstOrDefault(x => x.ItemKey == itemKey);
       if (itemConfig != null)
           return new Item(itemConfig, stack);

       Debug.LogWarning($"Item of key [{itemKey}] does not exist in the database.");
           
       return null;
   }
   
   // TODO: add scatter bool.
   public void DropItem(Item item, Vector3 dropOrigin)
   {
       var closestCluster = GetClosestCluster(dropOrigin);
       if (closestCluster == null)
       {
           closestCluster = new ItemCluster();
           
           itemClusters.Add(closestCluster);
       }

       var dropPosition = GetDropPosition(dropOrigin);
       var itemDrop = itemsPooler.GetItemDrop(item);
       itemDrop.Setup(closestCluster.ClusterId, item, dropPosition);

       closestCluster.AddItem(itemDrop);
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
