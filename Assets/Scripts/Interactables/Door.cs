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
        var goalRotation = new Vector3(0f ,isOpen ? 0f : 90f ,0f);
        isOpen = !isOpen;

        doorPivot.DOKill();
        doorPivot.DOLocalRotate(goalRotation, interactionDuration);
        
        // TODO: animate door handle.
    }
}
