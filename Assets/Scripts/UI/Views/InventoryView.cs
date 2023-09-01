using UI;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    [Header("Window Settings")]
    [SerializeField] protected bool withBlur;
    
    [Header("References")]
    [SerializeField] private ContainerWindow inventoryWindow;
    [SerializeField] private ContainerWindow externalContainerWindow;

    private bool isOpen;

    private ItemContainer externalContainer;
    private ItemContainer inventoryContainer;
    private PlayerSpawner playerSpawner;
    private CameraController cameraController;
    private UIController uiController;
    private InventoryController inventoryController;
    private ItemsController itemsController;

    public bool IsOpen => isOpen;
    
    public void Init(UIController uiController, 
        ItemContainer inventoryContainer, 
        CameraController cameraController, 
        PlayerSpawner playerSpawner, 
        InventoryController inventoryController,
        ItemsController itemsController)
    {
        this.inventoryContainer = inventoryContainer;
        this.uiController = uiController;
        this.playerSpawner = playerSpawner;
        this.cameraController = cameraController;
        this.inventoryController = inventoryController;
        this.itemsController = itemsController;
    }
    
    public void Open(ItemContainer externalContainer = null)
    {
        isOpen = true;
        
        inventoryWindow.Open(inventoryContainer);

        this.externalContainer = externalContainer;
        if (externalContainer != null)
            externalContainerWindow.Open(externalContainer);

        playerSpawner.Player.SetInputState(false);
        cameraController.SetCameraMovementState(false, true);
        
        if (withBlur)
            uiController.SetBlurState(true);
        
        gameObject.SetActive(true);
    }

    public void Close()
    {
        isOpen = false;
        
        inventoryWindow.Close();
        
        if (externalContainerWindow != null)
            externalContainerWindow.Close();
        
        playerSpawner.Player.SetInputState(true);
        cameraController.SetCameraMovementState(true);
        
        uiController.SetBlurState(false);
        
        gameObject.SetActive(false);
    }
    
    private void Update()
    {
        if (!isOpen) return;
        
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Y))
        {
            var item = itemsController.CreateItem("wood", 1);
            inventoryController.InventoryContainer.AddItem(item);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            var item = itemsController.CreateItem("wood", 10);
            inventoryController.InventoryContainer.AddItem(item);
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            var item = itemsController.CreateItem("metal", 1);
            inventoryController.InventoryContainer.AddItem(item);
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            var item = itemsController.CreateItem("metal", 10);
            inventoryController.InventoryContainer.AddItem(item);
        }
#endif
    }
}
