using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PromptsManager : MonoBehaviour
{
    [SerializeField] private GameObject worldObjectsParent;

    private List<IInteractable> interactables;
    private List<IPrompt> prompts;

    private void Start()
    {
        if (worldObjectsParent != null)
            interactables = worldObjectsParent.GetComponentsInChildren<IInteractable>().ToList();
    }
    
    
    // TODO: on update, hide or show prompts depending on distance.
}
