using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InteractablePrompt : MonoBehaviour
{
    private enum PromptType
    {
        NONE,
        OPEN_DOOR,
        INTERACT
    }

    [SerializeField] private IInteractable interactable;
    [SerializeField] private Transform promptTarget;
    [SerializeField] private CanvasGroup promptCanvasGroup;
    [SerializeField] private PromptType promptType;
    [SerializeField] private Camera mainCamera;
    
    private const KeyCode INTERACT_KEYCODE = KeyCode.E;
    
    private void Start()
    {
        if (promptCanvasGroup == null)
            promptCanvasGroup = GetComponent<CanvasGroup>();
        
        if (mainCamera == null)
            mainCamera = Camera.main;

        DOVirtual.DelayedCall(5f, ShowPrompt);
    }

    private void LateUpdate()
    {
        if (promptTarget == null) return;

        transform.position = promptTarget.position;
        transform.rotation = mainCamera.transform.rotation;
    }

    public void OnInteract()
    {
        // TODO: after interaction, open door or something.
        if (interactable == null) return;
    }

    public void ShowPrompt()
    {
        transform.localScale = Vector3.zero;
        
        gameObject.SetActive(true);

        transform.DOKill();
        transform.DOScale(Vector3.one, 0.5f);
    }

    public void HidePrompt()
    {
        transform.DOKill();
        transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
