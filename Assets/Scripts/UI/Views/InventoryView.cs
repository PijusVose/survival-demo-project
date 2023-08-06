using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    [Header("Window Settings")]
    [SerializeField] protected bool withBlur;
    
    [Header("References")]
    [SerializeField] private ContainerWindow inventoryWindow;
    [SerializeField] private ContainerWindow externalContainerWindow;

    [Header("DEBUG")]
    [SerializeField] private MaterialConfig firstMaterialConfig;
    [SerializeField] private MaterialConfig secondMaterialConfig;
    
    private bool isOpen;

    private ItemContainer externalContainer;
    private ItemContainer inventoryContainer;
    private PlayerSpawner playerSpawner;
    private CameraController cameraController;
    private UIController uiController;
    private InventoryController inventoryController;

    public bool IsOpen => isOpen;
    
    public void Init(UIController uiController, ItemContainer inventoryContainer, CameraController cameraController, PlayerSpawner playerSpawner, InventoryController inventoryController)
    {
        this.inventoryContainer = inventoryContainer;
        this.uiController = uiController;
        this.playerSpawner = playerSpawner;
        this.cameraController = cameraController;
        this.inventoryController = inventoryController;
    }
    
    public void Open(ItemContainer externalContainer = null)
    {
        isOpen = true;
        
        this.externalContainer = externalContainer;
        
        inventoryWindow.Init(inventoryContainer);
        inventoryWindow.Open();

        if (externalContainer != null)
        {
            externalContainerWindow.Init(externalContainer);
            externalContainerWindow.Open();
        }
        
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
            inventoryController.InventoryContainer.AddItem(firstMaterialConfig, 1);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            inventoryController.InventoryContainer.AddItem(firstMaterialConfig, 10);
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            inventoryController.InventoryContainer.AddItem(secondMaterialConfig, 1);
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            inventoryController.InventoryContainer.AddItem(secondMaterialConfig, 10);
        }
#endif
    }
}
