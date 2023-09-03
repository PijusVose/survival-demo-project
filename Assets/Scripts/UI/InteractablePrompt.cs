using DG.Tweening;
using TMPro;
using UnityEngine;

public class InteractablePrompt : MonoBehaviour, IPrompt
{
    [SerializeField] private TextMeshProUGUI interactionTextLabel;
    [SerializeField] private Transform promptTarget;
    [SerializeField] private Camera promptCamera;
    [SerializeField] private float showDuration = 0.5f;

    public bool IsShown => isShown;
    
    private Vector3 promptOffset;
    private bool isShown;

    private const string INTERACT_TEXT_FORMAT = "Press {0} to {1}";

    public void Init(Camera mainCamera)
    {
        promptCamera = mainCamera;
    }
    
    private void LateUpdate()
    {
        DirectPromptToCamera();
    }

    private void DirectPromptToCamera()
    {
        if (promptTarget == null) return;
        
        transform.position = promptTarget.position + promptOffset;
        transform.rotation = promptCamera.transform.rotation;
    }

    public void ShowPrompt(IInteractable interactable, Transform target, Vector3 offset)
    {
        promptTarget = target;
        promptOffset = offset;

        interactionTextLabel.text = string.Format(INTERACT_TEXT_FORMAT, interactable.InteractKey.ToString(), interactable.InteractActionName);
        
        DirectPromptToCamera();

        if (!isShown)
        {
            transform.localScale = Vector3.zero;
        
            gameObject.SetActive(true);

            transform.DOKill();
            transform.DOScale(Vector3.one, showDuration);
        }
        
        isShown = true;
    }

    public void HidePrompt()
    {
        isShown = false;
        
        transform.DOKill();
        transform.DOScale(Vector3.zero, showDuration).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });

        promptTarget = null;
    }
}
