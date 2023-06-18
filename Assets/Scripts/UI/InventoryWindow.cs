using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryWindow : MonoBehaviour, IWindow
{
    private UIController uiController;

    // TODO: change from IWindow to abstract WindowBase.

    public bool IsOpen { get; set; }

    public void Init(UIController uiController)
    {
        this.uiController = uiController;
    }
    
    public void Open()
    {
        uiController.SetBlurState(true);
        
        IsOpen = true;
    }

    public void Close()
    {
        uiController.SetBlurState(false);
        
        IsOpen = false;
    }
}
