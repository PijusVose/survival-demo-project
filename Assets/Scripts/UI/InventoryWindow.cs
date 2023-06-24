using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryWindow : WindowBase
{
    private PlayerSpawner playerSpawner;
    private CameraController cameraController;
    
    public override void Init(UIController uiController, GameController gameController)
    {
        base.Init(uiController, gameController);

        playerSpawner = gameController.GetController<PlayerSpawner>();
        cameraController = gameController.GetController<CameraController>();
    }

    public override void Open()
    {
        base.Open();
        
        playerSpawner.Player.SetInputState(false);
        cameraController.SetCameraMovementState(false, true);
    }

    public override void Close()
    {
        base.Close();
        
        playerSpawner.Player.SetInputState(true);
        cameraController.SetCameraMovementState(true);
    }
}
