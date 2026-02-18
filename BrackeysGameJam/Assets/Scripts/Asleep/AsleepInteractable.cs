using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsleepInteractable : MonoBehaviour
{
    public static event Action<AsleepInteractable> onInteractableCollected;

    public enum InteractableType
    {
        COLLECTABLE,
        USABLE
    }
    private readonly string[] interactActionName = { "Pick up", "Interact" };

    public InteractableType interactType = InteractableType.COLLECTABLE;

    public void Interact()
    {
        switch (interactType)
        {
            case InteractableType.COLLECTABLE:
                Collect();
                break;
            case InteractableType.USABLE:
                break;
            default:
                break;
        }
    }

    public string GetInteractText()
    {
        return interactActionName[(int)interactType];
    }

    public void Collect()
    {
        onInteractableCollected?.Invoke(this);
        gameObject.SetActive(false);
    }
}
