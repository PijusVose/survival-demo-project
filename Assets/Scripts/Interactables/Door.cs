using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform doorPivot;
    [SerializeField] private Transform doorHandlePivot;

    private bool isOpen;
    
    public void OnInteract()
    {
        var goalRotation = new Vector3(0f ,isOpen ? 0f : 90f ,0f);
        isOpen = !isOpen;

        doorPivot.DOKill();
        doorPivot.DOLocalRotate(goalRotation, 1f);
    }

    public void ShowInteractPrompt()
    {
        
    }

    public void HideInteractPrompt()
    {
        
    }
}
