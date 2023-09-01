using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ContainerWindow : MonoBehaviour
    {
        private enum DragType
        {
            None,
            LeftClick,
            RightClick
        }
    
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

        private List<ItemSlot> itemSlots = new();
        private ItemSlot mouseOverSlot;
        private DragType dragType;
        private bool isOpen;
        private bool isMouseOverWindow;
        private bool isInitialized;

        private const int SLOT_SIZE = 100;
        private const int SLOT_SPACING = 10;
        private const int GRID_SPACING = 10;
        private const int SLOT_COLUMN_COUNT = 10;
        private const int TOP_BAR_HEIGHT = 40;

        private void Init()
        {
            if (isInitialized) return;
        
            inventoryController = GameController.Instance.GetController<InventoryController>();
            itemsController = GameController.Instance.GetController<ItemsController>();
            playerSpawner = GameController.Instance.GetController<PlayerSpawner>();
        
            dragItemSlot.Init();
        
            SubscribeToEvents();
        
            SetupGrid();

            isInitialized = true;
        }
    
        private void SubscribeToEvents()
        {
            container.OnItemAdded += Container_OnItemChanged;
            container.OnItemChanged += Container_OnItemChanged;
            container.OnItemRemoved += Container_OnItemChanged;
        }

        private void UnsubscribeFromEvents()
        {
            if (container == null) return;
        
            container.OnItemAdded -= Container_OnItemChanged;
            container.OnItemChanged -= Container_OnItemChanged;
            container.OnItemRemoved -= Container_OnItemChanged;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
    
        public void Open(ItemContainer container)
        {
            this.container = container;
        
            Init();
            LoadContainerItems();
        
            gameObject.SetActive(true);
        
            isOpen = true;
        }

        public void Close()
        {
            gameObject.SetActive(false);
        
            CancelDragging();

            container = null;
            isOpen = false;
        }

        private void Update()
        {
            if (!isOpen) return;
        
            isMouseOverWindow = Helper.IsMouseOverUIElement(gameObject);
        
            CheckForInput();
        }

        private void CheckForInput()
        {
            if (Input.GetMouseButtonDown(0) && mouseOverSlot != null && dragType == DragType.None)
            {
                StartDragging(DragType.LeftClick);
            }
            else if (Input.GetMouseButtonUp(0) && dragType == DragType.LeftClick)
            {
                StopDragging();
            }
            else if (Input.GetMouseButtonDown(1) && mouseOverSlot != null && dragType == DragType.None)
            {
                StartDragging(DragType.RightClick);
            }
            else if (Input.GetMouseButtonUp(1) && dragType == DragType.RightClick)
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
            if (container == null) return;
        
            foreach (var item in container.Items)
            {
                var slot = itemSlots.FirstOrDefault(s => s.SlotId == item.ContainerInfo.SlotId);
                if (slot != null)
                {
                    slot.SetSlotItem(item);
                }
            }
        }
    
        private void Container_OnItemChanged(Item item)
        {
            if (!isOpen) return;
            
            if (item.ContainerInfo.SlotId >= 0 && item.ContainerInfo.SlotId < itemSlots.Count)
            {
                var slot = itemSlots.FirstOrDefault(x => x.SlotId == item.ContainerInfo.SlotId);
                if (slot != null)
                {
                    slot.SetSlotItem(item);
                }
            }
        }
    
        private void StartDragging(DragType startDragType)
        {
            if (mouseOverSlot != null)
            {
                var itemToDrag = container.GetItemInSlot(mouseOverSlot.SlotId);
                if (itemToDrag != null)
                {
                    var isHalfDrag = startDragType == DragType.RightClick && itemToDrag.ItemStack > 1;
                    var dragStack = isHalfDrag ? itemToDrag.ItemStack / 2 : itemToDrag.ItemStack;

                    dragItemSlot.EnableDragSlot(itemToDrag, mouseOverSlot.SlotId, dragStack);
                
                    var slot = GetSlotOfId(mouseOverSlot.SlotId);
                    if (isHalfDrag)
                    {
                        slot.SetSlotStack(itemToDrag.ItemStack - dragStack);
                    }
                    else
                    {
                        slot.SetSlotItem(null);
                    }

                    dragType = startDragType;
                }
            }
        }

        private void StopDragging()
        {
            dragType = DragType.None;

            if (isMouseOverWindow)
            {
                if (mouseOverSlot != null && dragItemSlot.DraggedItem != null)
                {
                    container.MoveItemToSlot(dragItemSlot.DraggedItem, mouseOverSlot.SlotId, dragItemSlot.ItemStack);
                }
                else
                {
                    CancelDragging();
                }
            }
            else
            {
                DropItemFromContainer(dragItemSlot.DraggedItem);
            }

            dragItemSlot.DisableDragSlot();
        }

        private void CancelDragging()
        {
            if (dragType != DragType.None)
                container.MoveItemToSlot(dragItemSlot.DraggedItem, dragItemSlot.StartSlotId, dragItemSlot.ItemStack);
        
            dragType = DragType.None;
        
            dragItemSlot.DisableDragSlot();
        }

        private void DropItemFromContainer(Item item)
        {
            container.RemoveItem(item, item.ItemStack);
        
            itemsController.DropItem(item, GetContainerPosition());
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
            mouseOverSlot = slot;
        }
    }
}
