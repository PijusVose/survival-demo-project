using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class UIController : ControllerBase
{
    [SerializeField] private GameObject blurBackground;

    private List<WindowBase> windows;

    private GameController gameController;
    
    public override void Init(GameController gameController)
    {
        this.gameController = gameController;
    }

    private void Awake()
    {
        InitWindows();
    }

    private void Update()
    {
        CheckForInput();
    }

    public void SetBlurState(bool state)
    {
        blurBackground.SetActive(state);
    }

    private void CheckForInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            var inventoryWindow = GetWindowOfType<InventoryWindow>();
            if (inventoryWindow != null)
            {
                if (inventoryWindow.IsOpen)
                {
                    inventoryWindow.Close();
                }
                else
                {
                    inventoryWindow.Open();
                }
            }
            else
            {
                Debug.LogWarning("InventoryWindow was not opened/close. Could not retrieve InventoryWindow.");
            }
        }
    }

    private void GetWindows()
    {
        windows = GetComponentsInChildren<WindowBase>(includeInactive: true).ToList();
    }

    public T GetWindowOfType<T>()
    {
        return windows.OfType<T>().FirstOrDefault();
    }

    private void InitWindows()
    {
        GetWindows();

        foreach (var window in windows)       
        {
            window.Init(this);
        }
    }
}
