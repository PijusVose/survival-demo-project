using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWindow
{
    void Open();
    void Close();
    void Init(UIController uiController);
    
    bool IsOpen { get; set; }
}
