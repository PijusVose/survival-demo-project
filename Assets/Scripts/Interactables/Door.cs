using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Transform doorPivot;
    [SerializeField] private Transform doorHandlePivot;
    [SerializeField] private Transform interactionBillboard;
    [SerializeField] private float interactionDuration = 1f;

    private bool isOpen;
    private bool isPromptShown;

    public void OnInteract()
    {
        var goalRotation = new Vector3(0f ,isOpen ? 0f : 90f ,0f);
        isOpen = !isOpen;

        doorPivot.DOKill();
        doorPivot.DOLocalRotate(goalRotation, interactionDuration);
    }

    private void LateUpdate()
    {
        HandleInteractPrompt();
    }

    public void ShowInteractPrompt()
    {
        isPromptShown = true;
        interactionBillboard.gameObject.SetActive(true);
        interactionBillboard.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public void HideInteractPrompt()
    {
        interactionBillboard.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
        interactionBillboard.gameObject.SetActive(false);
        isPromptShown = false;
    }

    public void HandleInteractPrompt()
    {
        interactionBillboard.rotation = cameraController.GetCameraRotation();
    }
}
