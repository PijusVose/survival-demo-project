using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemsPooler : MonoBehaviour
{
    [SerializeField] private ItemDrop itemDropPrefab;

    private List<ItemDrop> itemDrops = new();
    
    // TODO: create my own pooler, where you can get item from pool depending on type.
    private ItemDrop CreateItemDrop()
    {
        var newItemDrop = Instantiate(itemDropPrefab, transform);
        newItemDrop.gameObject.SetActive(true);

        itemDrops.Add(newItemDrop);
        
        return newItemDrop;
    }

    public ItemDrop GetItemDrop(Item item)
    {
        var pooledDrop = itemDrops.FirstOrDefault(x => x.Item.ItemConfig.ItemKey == item.ItemConfig.ItemKey && !x.gameObject.activeInHierarchy);
        if (pooledDrop == null)
            pooledDrop = CreateItemDrop();

        return pooledDrop;
    }

    public void ReturnItemDropToPool(ItemDrop itemDrop)
    {
        itemDrop.gameObject.SetActive(false);
    }
}
