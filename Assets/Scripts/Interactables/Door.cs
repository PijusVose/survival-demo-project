using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform doorPivot;
    [SerializeField] private Transform doorHandlePivot;
    
    [SerializeField] private float interactionDuration = 1f;
    [SerializeField] private Vector3 promptOffset;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    
    private bool isOpen;
    private bool isInteracting;

    public Vector3 PromptOffset
    {
        get => promptOffset;
    }

    public KeyCode InteractKey
    {
        get => interactKey;
    }

    public void OnInteract()
    {
        if (isInteracting) return;
        
        var goalRotation = new Vector3(0f ,isOpen ? 0f : 90f ,0f);
        isOpen = !isOpen;

        isInteracting = true;

        doorPivot.DOKill();
        doorPivot.DOLocalRotate(goalRotation, interactionDuration).OnComplete(() =>
        {
            isInteracting = false;
        });
        
        // TODO: animate door handle.
    }
}
