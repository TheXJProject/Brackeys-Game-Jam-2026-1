using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AsleepInteractable : MonoBehaviour
{
    public static event Action onLevelCollectablePickedUp;

    public enum InteractableType
    {
        COLLECTABLE,
        USABLE,
        OPENABLE
    }
    private readonly string[] interactActionName = { "Pick up", "Interact", "Open" };

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
            case InteractableType.OPENABLE:
                print("YOU WIN");
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
        onLevelCollectablePickedUp?.Invoke();
        gameObject.SetActive(false);
    }
}
