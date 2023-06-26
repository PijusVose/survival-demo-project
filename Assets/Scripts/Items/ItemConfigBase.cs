using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemConfigBase : ScriptableObject
{
    // TODO: write down rules on how the inventory and item system will work. Then add needed variables.
    [SerializeField] protected string itemDesc;
    [SerializeField] protected string itemKey;
    [SerializeField] protected bool isStackable = true;
    [SerializeField] protected Sprite itemIcon;
    [SerializeField] protected int maxStack = 100;

    public string ItemDescription => itemDesc;
    public string ItemKey => itemKey;
    public bool IsStackable => isStackable;
    public Sprite ItemIcon => itemIcon;
    public int MaxStack => maxStack;
}
