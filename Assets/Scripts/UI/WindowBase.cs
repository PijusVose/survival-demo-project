using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WindowBase : MonoBehaviour
{
    [SerializeField] protected bool withBlur;
    
    protected UIController uiController;
    protected bool isOpen;

    public bool IsOpen => isOpen;

    public void Init(UIController uiController)
    {
        this.uiController = uiController;
    }
    
    public void Open()
    {
        if (withBlur)
            uiController.SetBlurState(true);

        gameObject.SetActive(true);
        
        isOpen = true;
    }

    public void Close()
    {
        if (withBlur)
            uiController.SetBlurState(false);
        
        gameObject.SetActive(false);
        
        isOpen = false;
    }
}
