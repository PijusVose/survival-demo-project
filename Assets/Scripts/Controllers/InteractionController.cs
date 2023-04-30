using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class InteractionController : MonoBehaviour
{
    [SerializeField] private SphereCollider interactionCollider;
    [SerializeField] private Camera playerCamera;
    
    [SerializeField] private float interactionRadius;

    private PromptsManager promptsManager;
    private List<IInteractable> interactables;
    private IInteractable currentInteractObject;

    private void Start()
    {
        interactables = new List<IInteractable>();

        SetupCollider();
        
        promptsManager = PromptsManager.Instance;
    }

    private void Update()
    {
        CheckForBestInteractable();
    }

    private void SetupCollider()
    {
        interactionCollider = GetComponent<SphereCollider>();
        interactionCollider.radius = interactionRadius;
    }

    private void CheckForBestInteractable()
    {
        if (interactables == null || interactables.Count == 0) return;

        MonoBehaviour chosenInteractable = null;
        foreach (var interactable in interactables)
        {
            if (interactable is MonoBehaviour interactableBehaviour)
            {
                var currDotProduct = GetDotProductFromCamera(interactableBehaviour.transform.position);
                if (currDotProduct <= 0) continue;
                
                if (chosenInteractable == null)
                {
                    chosenInteractable = interactableBehaviour;

                    continue;
                }
                
                var lastDotProduct = GetDotProductFromCamera(chosenInteractable.transform.position);
                if (currDotProduct > lastDotProduct)
                    chosenInteractable = interactableBehaviour;
            }
        }

        if (currentInteractObject == null)
        {
            if (chosenInteractable != null)
                promptsManager.ShowInteractPrompt(chosenInteractable.transform);
        }
        else 
        {
            if (chosenInteractable == null)
            {
                promptsManager.HideInteractPrompt();
            }
            else if (chosenInteractable != (MonoBehaviour)currentInteractObject)
            {
                promptsManager.ShowInteractPrompt(chosenInteractable.transform);
            }
        }
        
        currentInteractObject = chosenInteractable != null ? chosenInteractable as IInteractable : null;
    }

    private float GetDotProductFromCamera(Vector3 objectPosition)
    {
        var dirVector = (objectPosition - playerCamera.transform.position).normalized;
        
        return Vector3.Dot(playerCamera.transform.forward, dirVector);
    }

    private void OnTriggerEnter(Collider other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null && !interactables.Contains(interactable))
        {
            interactables.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (interactables == null || interactables.Count == 0) return;
        
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null && interactables.Contains(interactable))
        {
            if (interactable == currentInteractObject)
            {
                promptsManager.HideInteractPrompt();
            }
            
            interactables.Remove(interactable);
        }
    }
}
