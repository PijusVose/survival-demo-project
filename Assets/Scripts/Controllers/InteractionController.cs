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

    [SerializeField] private float minDotProduct;
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
        CheckForClosestInteractable();
    }

    private void SetupCollider()
    {
        interactionCollider = GetComponent<SphereCollider>();
        interactionCollider.radius = interactionRadius;
    }

    private void CheckForClosestInteractable()
    {
        if (interactables == null || interactables.Count == 0) return;

        IInteractable targetInteractable = null;
        foreach (var interactable in interactables)
        {
            CheckForTargetInteractable(interactable, ref targetInteractable);
        }

        if (currentInteractObject == null)
        {
            if (targetInteractable != null)
                promptsManager.ShowInteractPrompt(targetInteractable);
        }
        else 
        {
            if (targetInteractable == null)
            {
                promptsManager.HideInteractPrompt();
            }
            else if (targetInteractable != currentInteractObject)
            {
                promptsManager.ShowInteractPrompt(targetInteractable);
            }
        }

        currentInteractObject = targetInteractable;
    }

    private void CheckForTargetInteractable(IInteractable interactable, ref IInteractable targetInteractable)
    {
        if (interactable is MonoBehaviour interactableAsMono)
        {
            var currDotProduct = GetDotProductBetweenCamera(interactableAsMono.transform.position);
            if (currDotProduct <= minDotProduct) return;
                
            if (targetInteractable == null)
            {
                targetInteractable = interactable;

                return;
            }

            var targetPosition = ((MonoBehaviour)targetInteractable).transform.position;
            var lastDotProduct = GetDotProductBetweenCamera(targetPosition);
            if (currDotProduct > lastDotProduct)
                targetInteractable = interactable;
        }
    }

    private float GetDotProductBetweenCamera(Vector3 startPosition)
    {
        var dirFromPlayer = (startPosition - transform.position).normalized;
        
        return Vector3.Dot(playerCamera.transform.forward, dirFromPlayer);
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
            interactables.Remove(interactable);
            
            if (interactable == currentInteractObject)
            {
                promptsManager.HideInteractPrompt();

                currentInteractObject = null;
            }
        }
    }
}
