using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPrompt
{
    void Init(Camera mainCamera);
    bool IsShown { get; }
    void ShowPrompt(IInteractable interactable, Transform target, Vector3 offset);
    void HidePrompt();
}
