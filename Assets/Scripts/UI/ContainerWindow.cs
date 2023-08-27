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
    private InventoryController inventoryController;
    private PlayerSpawner playerSpawner;
    private ItemsController itemsController;

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

        inventoryController = GameController.Instance.GetController<InventoryController>();
        itemsController = GameController.Instance.GetController<ItemsController>();
        playerSpawner = GameController.Instance.GetController<PlayerSpawner>();
        
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

        if (itemSlots.Count < container.SlotsAmount)
        {
            var slotsToAdd = container.SlotsAmount - itemSlots.Count;
            for (int i = 0; i < slotsToAdd; i++)
            {
                var slot = Instantiate(itemSlotPrefab, slotsGrid);
                
                itemSlots.Add(slot);
            }
        }

        for (int i = 0; i < container.SlotsAmount; i++)
        {
            var slot = itemSlots[i];
            
            slot.Init(this, i);
        }
        
        var rows = Mathf.CeilToInt(container.SlotsAmount / (float)SLOT_COLUMN_COUNT);
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
        
        if (item.ContainerInfo.SlotId >= 0 && item.ContainerInfo.SlotId < itemSlots.Count)
        {
            var slot = itemSlots.FirstOrDefault(x => x.SlotId == item.ContainerInfo.SlotId);
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
            var itemToDrag = container.GetItemInSlot(mouseOverSlot.SlotId);
            if (itemToDrag != null)
            {
                if (isHalf)
                {
                    var newDraggedItem = container.SplitItem(itemToDrag);
                    dragItemSlot.EnableDragSlot(newDraggedItem, mouseOverSlot.SlotId);
                }
                else
                {
                    dragItemSlot.EnableDragSlot(itemToDrag, mouseOverSlot.SlotId);
                    
                    var slot = GetSlotOfId(mouseOverSlot.SlotId);
                    slot.UpdateSlotItem(null);
                }

                isDragging = true;
            }
        }
    }

    private void StopDragging()
    {
        isDragging = false;

        if (mouseOverSlot != null && dragItemSlot.draggedItem != null)
        {
            var leftover = container.AddItemToSlot(dragItemSlot.draggedItem, mouseOverSlot.SlotId);
            if (leftover != 0)
                container.AddItemToSlot(dragItemSlot.draggedItem, dragItemSlot.startSlotId);
        }
        else
        {
            if (!isMouseOverWindow)
            {
                itemsController.DropItem(dragItemSlot.draggedItem, GetContainerPosition());
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
        
        if (dragItemSlot.draggedItem != null)
            container.AddItemToSlot(dragItemSlot.draggedItem, dragItemSlot.startSlotId);
        
        dragItemSlot.DisableDragSlot();
    }

    private ItemSlot GetSlotOfId(int slotId)
    {
        return itemSlots.FirstOrDefault(x => x.SlotId == slotId);
    }
    
    private Vector3 GetContainerPosition()
    {
        if (inventoryController.IsInventoryContainer(container.ContainerId))
            return playerSpawner.Player.transform.position;
        
        return container.transform.position;
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
