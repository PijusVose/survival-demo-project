using UnityEngine;

public class UIController : ControllerBase
{
    [SerializeField] private GameObject blurBackground;
    [SerializeField] private Canvas uiCanvas;
    
    [Header("Views")] 
    [SerializeField] private InventoryView inventoryView;

    public Canvas UICanvas => uiCanvas;

    private ItemsController itemsController;
    private InventoryController inventoryController;
    private CameraController cameraController;
    private PlayerSpawner playerSpawner;
    
    protected override void AwakeController()
    {
        base.AwakeController();

        inventoryController = gameController.GetController<InventoryController>();
        playerSpawner = gameController.GetController<PlayerSpawner>();
        cameraController = gameController.GetController<CameraController>();
        itemsController = gameController.GetController<ItemsController>();
        
        InitViews();
    }

    private void InitViews()
    {
        if (inventoryView != null)
        {
            inventoryView.Init(this,
                inventoryController.InventoryContainer,
                cameraController,
                playerSpawner,
                inventoryController,
                itemsController);
        }
    }

    private void Update()
    {
        CheckForInput();
    }

    private void CheckForInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventoryView.IsOpen)
            {
                inventoryView.Close();
            }
            else
            {
                inventoryView.Open();
            }
        }
    }
    
    public void SetBlurState(bool state)
    {
        blurBackground.SetActive(state);
    }
}
