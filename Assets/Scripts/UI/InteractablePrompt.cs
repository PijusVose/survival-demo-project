using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InteractablePrompt : MonoBehaviour, IPrompt
{
    [SerializeField] private Transform promptTarget;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float showDuration = 0.5f;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        DirectPromptToCamera();
    }

    private void DirectPromptToCamera()
    {
        if (promptTarget == null) return;
        
        transform.position = promptTarget.position;
        transform.rotation = mainCamera.transform.rotation;
    }

    public void ShowPrompt(Transform target)
    {
        promptTarget = target;
        
        DirectPromptToCamera();
        
        transform.localScale = Vector3.zero;
        
        gameObject.SetActive(true);

        transform.DOKill();
        transform.DOScale(Vector3.one, showDuration);
    }

    public void HidePrompt()
    {
        transform.DOKill();
        transform.DOScale(Vector3.zero, showDuration).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });

        promptTarget = null;
    }
}
