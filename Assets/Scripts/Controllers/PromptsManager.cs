using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PromptsManager : ControllerBase
{
    [SerializeField] private InteractablePrompt interactionPromptPrefab;
    
    private List<IPrompt> prompts = new();
    
    // TODO: Show/hide when inventory is enabled.

    public void ShowInteractPrompt(IInteractable interactable)
    {
        var interactionPrompt = GetInteractionPrompt();
        if (interactionPrompt != null)
        {
            if (interactable is MonoBehaviour interactableAsMono)
            {
                interactionPrompt.ShowPrompt(interactable, interactableAsMono.transform, interactable.PromptOffset);
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

    public bool IsPromptShown<T>() where T : IPrompt
    {
        return prompts.Any(x => x.GetType() == typeof(T) && x.IsShown);
    }

    private InteractablePrompt GetInteractionPrompt()
    {
        var prompt = prompts.FirstOrDefault(x => x.GetType() == typeof(InteractablePrompt)) as InteractablePrompt;
        if (prompt == null)
        {
            prompt = Instantiate(interactionPromptPrefab, transform);
            prompt.Init(Camera.main);
            
            prompts.Add(prompt);
        }

        return prompt;
    }
}
