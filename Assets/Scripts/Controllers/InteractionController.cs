using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private Transform interactionOrigin;
    [SerializeField] private float interactionDistance;
    [SerializeField] private float interactionRadius;

    [CanBeNull] private IInteractable currentInteractObject;
    
    private void FixedUpdate()
    {
        var origin = interactionOrigin.position;
        var direction = interactionOrigin.forward;

        if (Physics.SphereCast(origin, interactionRadius, direction, out RaycastHit hitInfo, interactionDistance))
        {
            var interactModule = hitInfo.transform.GetComponentInParent<IInteractable>();
            if (interactModule != null)
            {
                if (interactModule != currentInteractObject)
                    currentInteractObject?.HideInteractPrompt();

                currentInteractObject = interactModule;
                currentInteractObject.ShowInteractPrompt();
            }
            else
            {
                currentInteractObject?.HideInteractPrompt();
                currentInteractObject = null;
            }
        }
        else
        {
            currentInteractObject?.HideInteractPrompt();
            currentInteractObject = null;
        }
    }

    private void Update()
    {
        if (currentInteractObject != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
                currentInteractObject.OnInteract();
        }
    }
}
