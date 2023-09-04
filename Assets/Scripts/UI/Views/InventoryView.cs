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
    private GameController gameController;
    private PromptsManager promptsController;
    private PlayerSpawner playerSpawner;
    private CameraController cameraController;
    private UIController uiController;
    private InventoryController inventoryController;
    private ItemsController itemsController;

    public bool IsOpen => isOpen;
    
    public void Init(UIController uiController, GameController gameController)
    {
        this.uiController = uiController;
        this.gameController = gameController;

        playerSpawner = gameController.GetController<PlayerSpawner>();
        cameraController = gameController.GetController<CameraController>();
        inventoryController = gameController.GetController<InventoryController>();
        itemsController = gameController.GetController<ItemsController>();
        promptsController = gameController.GetController<PromptsManager>();

        inventoryContainer = inventoryController.InventoryContainer;
    }
    
    public void Open(ItemContainer externalContainer = null)
    {
        isOpen = true;
        
        promptsController.SetPromptsEnabled(false);
        
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
        
        promptsController.SetPromptsEnabled(true);
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
