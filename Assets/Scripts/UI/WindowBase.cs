using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WindowBase : MonoBehaviour
{
    [SerializeField] protected bool withBlur;

    protected GameController gameController;
    protected UIController uiController;
    protected bool isOpen;

    public bool IsOpen => isOpen;

    public virtual void Init(UIController uiController, GameController gameController)
    {
        this.uiController = uiController;
        this.gameController = gameController;
    }
    
    public virtual void Open()
    {
        if (withBlur)
            uiController.SetBlurState(true);

        gameObject.SetActive(true);
        
        isOpen = true;
    }

    public virtual void Close()
    {
        if (withBlur)
            uiController.SetBlurState(false);
        
        gameObject.SetActive(false);
        
        isOpen = false;
    }
}
