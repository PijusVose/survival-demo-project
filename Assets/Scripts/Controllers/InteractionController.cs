using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class InteractionController : MonoBehaviour, IPlayerPlugin
{
    [SerializeField] private SphereCollider interactionCollider;

    [SerializeField] private float minDotProduct;
    [SerializeField] private float interactionRadius;

    private ItemsController itemsController;
    private IPlayerController playerController;
    private CameraController cameraController;
    private PromptsManager promptsManager;
    
    private List<IInteractable> interactables = new();
    private IInteractable currentInteractObject;
    
    // Interaction target vector is always camera.forward.
    // SphereCollider also checks for closests items, then compares dot products of all of them
    // Closest target to interaction target vector shows interaction prompt
    // Play hide/show animation of interaction UI prompt

    private bool isInitialized;
    
    public void Init(IPlayerController playerController)
    {
        this.playerController = playerController;

        SetupCollider();
        
        promptsManager = playerController.GameController.GetController<PromptsManager>();
        cameraController = playerController.GameController.GetController<CameraController>();
        itemsController = this.playerController.GameController.GetController<ItemsController>();

        SubscribeToEvents();
        
        isInitialized = true;
    }

    private void SubscribeToEvents()
    {
        itemsController.OnItemPickedUp += RemoveInteractable;
    }

    private void OnDestroy()
    {
        if (itemsController == null) return;

        itemsController.OnItemPickedUp -= RemoveInteractable;
    }

    private void Update()
    {
        if (!isInitialized) return;
        
        CheckForClosestInteractable();
        CheckForInteractKeyCode();
    }

    private void SetupCollider()
    {
        interactionCollider = GetComponent<SphereCollider>();
        interactionCollider.radius = interactionRadius;
    }

    private void CheckForInteractKeyCode()
    {
        if (currentInteractObject == null) return;

        if (Input.GetKeyDown(currentInteractObject.InteractKey))
        {
            currentInteractObject.OnInteract();
        }
    }

    private void CheckForClosestInteractable()
    {
        if (interactables == null || interactables.Count == 0)
        {
            if (promptsManager.IsPromptShown<InteractablePrompt>())
                promptsManager.HideInteractPrompt();
            
            return;
        }

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
            if (!interactableAsMono.gameObject.activeInHierarchy) return;
            
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

    private void RemoveInteractable(IInteractable interactable)
    {
        if (currentInteractObject == interactable)
            currentInteractObject = null;
        
        interactables.Remove(interactable);
    }

    private float GetDotProductBetweenCamera(Vector3 startPosition)
    {
        var dirFromPlayer = (startPosition - transform.position).normalized;
        
        return Vector3.Dot(cameraController.PlayerCamera.transform.forward, dirFromPlayer);
    }

    private void OnTriggerEnter(Collider other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null && !interactables.Contains(interactable) && other.gameObject.activeInHierarchy)
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
