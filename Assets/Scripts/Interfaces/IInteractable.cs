using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    Vector3 PromptOffset { get; }
    KeyCode InteractKey { get; }
    void OnInteract();
}
