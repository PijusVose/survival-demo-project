using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PromptsManager : Singleton<PromptsManager>
{
    // TODO: change from singleton to dependency injection.
    [SerializeField] private InteractablePrompt interactionPromptPrefab;
    
    private List<IPrompt> prompts;

    protected override void SingletonStarted()
    {
        prompts = GetComponentsInChildren<IPrompt>(includeInactive: true).ToList();
        
        Debug.Log($"prompts: {prompts.Count}");
    }

    public void ShowInteractPrompt(IInteractable interactable)
    {
        var interactionPrompt = GetInteractionPrompt();
        if (interactionPrompt != null)
        {
            if (interactable is MonoBehaviour interactableAsMono)
            {
                interactionPrompt.ShowPrompt(interactableAsMono.transform, interactable.PromptOffset);
            }
        }
        else
        {
            Debug.LogWarning("Can't show interaction prompt! There are no interaction prompt instances.");
        }
    }

    public void HideInteractPrompt()
    {
        var interactionPrompt = GetInteractionPrompt();
        if (interactionPrompt != null)
        {
            interactionPrompt.HidePrompt();
        }
        else
        {
            Debug.LogWarning("Can't hide interaction prompt! There are no interaction prompt instances.");
        }
    }

    private InteractablePrompt GetInteractionPrompt()
    {
        if (prompts == null || prompts.Count == 0) return null;
        
        // TODO: get prompt which is disabled. If no prompts available, create new instance.
        return prompts.FirstOrDefault(x => x.GetType() == typeof(InteractablePrompt)) as InteractablePrompt;
    }
}
