using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContainerWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Prefabs")] 
    [SerializeField] protected ItemSlot itemSlotPrefab;
    
    [Header("References")] 
    [SerializeField] private DragItemSlot dragItemSlot;
    [SerializeField] private Transform slotsGrid;
    [SerializeField] private GridLayoutGroup slotsGridLayoutGroup;
    [SerializeField] private RectTransform backgroundRectTransform;

    private ItemContainer container;
    private ItemsPooler itemsPooler;
    
    private List<ItemSlot> itemSlots = new List<ItemSlot>();
    private ItemSlot mouseOverSlot;
    private bool isDragging;
    private bool isOpen;
    private bool isMouseOverWindow;

    private const int SLOT_SIZE = 100;
    private const int SLOT_SPACING = 10;
    private const int GRID_SPACING = 10;
    private const int SLOT_COLUMN_COUNT = 10;
    private const int TOP_BAR_HEIGHT = 40;

    public void Init(ItemContainer container)
    {
        this.container = container;
        itemsPooler = GameController.Instance.GetController<ItemsPooler>();
        
        dragItemSlot.Init();
        
        SubscribeToEvents();
    }
    
    private void SubscribeToEvents()
    {
        container.OnItemAdded += Container_OnItemChanged;
        container.OnItemChanged += Container_OnItemChanged;
        container.OnItemRemoved += Container_OnItemChanged;
    }

    private void OnDestroy()
    {
        if (container == null) return;
        
        container.OnItemAdded -= Container_OnItemChanged;
        container.OnItemChanged -= Container_OnItemChanged;
        container.OnItemRemoved -= Container_OnItemChanged;
    }
    
    public void Open()
    {
        SetupGrid();
        LoadContainerItems();
        
        gameObject.SetActive(true);
        
        isOpen = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        
        CancelDragging();
        
        isOpen = false;
    }

    private void Update()
    {
        if (!isOpen) return;
        
        CheckForInput();
    }

    private void CheckForInput()
    {
        if (Input.GetMouseButtonDown(0) && mouseOverSlot != null && !isDragging)
        {
            StartDragging();
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            StopDragging();
        }
        else if (Input.GetMouseButtonDown(1) && mouseOverSlot != null && !isDragging)
        {
            StartDragging(true);
        }
        else if (Input.GetMouseButtonUp(1) && isDragging)
        {
            StopDragging();
        }
    }

    private void SetupGrid()
    {
        slotsGridLayoutGroup.spacing = new Vector2(SLOT_SPACING, SLOT_SPACING);
        slotsGridLayoutGroup.cellSize = new Vector2(SLOT_SIZE, SLOT_SIZE);
        slotsGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        slotsGridLayoutGroup.constraintCount = SLOT_COLUMN_COUNT;

        if (itemSlots.Count < container.AmountOfSlots)
        {
            var slotsToAdd = container.AmountOfSlots - itemSlots.Count;
            for (int i = 0; i < slotsToAdd; i++)
            {
                var slot = Instantiate(itemSlotPrefab, slotsGrid);
                
                itemSlots.Add(slot);
            }
        }

        for (int i = 0; i < container.AmountOfSlots; i++)
        {
            var slot = itemSlots[i];
            
            slot.Init(this, i);
        }
        
        var rows = Mathf.CeilToInt(container.AmountOfSlots / (float)SLOT_COLUMN_COUNT);
        var bgHeight = rows * SLOT_SIZE + (rows - 1) * SLOT_SPACING + GRID_SPACING * 2 + TOP_BAR_HEIGHT;
        var bgWidth = SLOT_COLUMN_COUNT * SLOT_SIZE + (SLOT_COLUMN_COUNT - 1) * SLOT_SPACING + GRID_SPACING * 2;

        backgroundRectTransform.sizeDelta = new Vector2(bgWidth, bgHeight);
    }

    private void LoadContainerItems()
    {
        // TODO: this.
    }
    
    private void Container_OnItemChanged(Item item)
    {
        if (!isOpen) return;
        
        if (item.SlotId >= 0 && item.SlotId < itemSlots.Count)
        {
            var slot = itemSlots.FirstOrDefault(x => x.SlotId == item.SlotId);
            if (slot != null)
            {
                slot.UpdateSlotItem(item);
            }
        }
    }
    
    private void StartDragging(bool isHalf = false)
    {
        if (mouseOverSlot != null)
        {
            var itemInSlot = container.GetItemInSlot(mouseOverSlot.SlotId);
            if (itemInSlot != null)
            {
                var dragStack = isHalf ? itemInSlot.ItemStack / 2 : itemInSlot.ItemStack;
                dragStack = Mathf.Max(1, dragStack);
                
                var dragItemInfo = new ItemInfo(itemInSlot.ItemConfig, dragStack, itemInSlot.SlotId);

                dragItemSlot.EnableDragSlot(dragItemInfo);
                
                container.RemoveItem(itemInSlot, dragStack);
                
                isDragging = true;
            }
        }
    }

    private void StopDragging()
    {
        isDragging = false;

        if (mouseOverSlot != null && dragItemSlot.DraggedItemInfo != null)
        {
            container.AddItemToSlot(dragItemSlot.DraggedItemInfo, mouseOverSlot.SlotId);
        }
        else
        {
            if (!isMouseOverWindow)
            {
                itemsPooler.DropItem(dragItemSlot.DraggedItemInfo, container.transform.position);
            }
            else
            {
                CancelDragging();
            }
        }
        
        dragItemSlot.DisableDragSlot();
    }

    private void CancelDragging()
    {
        isDragging = false;
        
        if (dragItemSlot.DraggedItemInfo != null)
            container.AddItemToSlot(dragItemSlot.DraggedItemInfo, dragItemSlot.DraggedItemInfo.originalSlotId);
        
        dragItemSlot.DisableDragSlot();
    }

    public void SetMouseOverSlot(ItemSlot slot)
    {
        if (isMouseOverWindow && slot == null)
            return;
            
        mouseOverSlot = slot;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOverWindow = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOverWindow = false;

        mouseOverSlot = null;
    }
}
