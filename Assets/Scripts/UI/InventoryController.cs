using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : ControllerBase
{
    private Item[] itemsArray;

    public Item[] Items => itemsArray;

    public event Action<int, int> OnItemAdded;
    public event Action<int, int> OnItemRemoved;

    public const int MAX_SLOTS = 40;

    // TODO: save/load inventory from json.
    
    public override void Init(GameController gameController)
    {
        base.Init(gameController);

        itemsArray = new Item[MAX_SLOTS];
    }

    public int AddItem(ItemConfigBase config, int amount)
    {
        // First iteration goes through slots which hold items of same type and fills them up.
        for (int i = 0; i < itemsArray.Length; i++)
        {
            if (itemsArray[i] != null &&
                itemsArray[i].ItemConfig.ItemKey == config.ItemKey &&
                itemsArray[i].ItemStack < config.MaxStack)
            {
                int availableStack = config.MaxStack - itemsArray[i].ItemStack;
                int stackToAdd = availableStack < amount ? availableStack : amount;
                
                itemsArray[i].IncreaseStack(stackToAdd);
                amount -= stackToAdd;
                
                OnItemAdded?.Invoke(i, stackToAdd);
                
                if (amount == 0) break;
            }
        }
        
        // Second iteration adds any left overs to empty slots.
        if (amount > 0)
        {
            for (int i = 0; i < itemsArray.Length; i++)
            {
                if (itemsArray[i] == null)
                {
                    var stackToAdd = Mathf.Min(amount, config.MaxStack);
                    var addedItem = new Item(config, stackToAdd, i);

                    itemsArray[i] = addedItem;

                    OnItemAdded?.Invoke(i, stackToAdd);
                    
                    amount -= stackToAdd;
                }

                if (amount == 0) break;
            }
        }

        return amount;
    }

    public void MoveItem(Item itemToMove, int targetSlot)
    {
        
    }

    public void DropItem()
    {
        
    }
}
