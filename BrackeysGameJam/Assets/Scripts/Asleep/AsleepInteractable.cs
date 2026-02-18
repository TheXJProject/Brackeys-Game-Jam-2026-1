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
        OPENABLE,
        LOCKEDOPENABLE
    }
    private readonly string[] interactActionName = { "Pick up", "Interact", "Open", "LOCKED" };

    public InteractableType interactType = InteractableType.COLLECTABLE;

    private void OnEnable()
    {
        AsleepInteractable.onLevelCollectablePickedUp += Unlock;
    }

    private void OnDisable()
    {
        AsleepInteractable.onLevelCollectablePickedUp -= Unlock;
    }

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
                Open();
                break;
            case InteractableType.LOCKEDOPENABLE:
                
                break;
            default:
                break;
        }
    }

    public string GetInteractText()
    {
        return interactActionName[(int)interactType];
    }

    private void Collect()
    {
        onLevelCollectablePickedUp?.Invoke();
        gameObject.SetActive(false);
    }

    private void Unlock()
    {
        if (interactType == InteractableType.LOCKEDOPENABLE)
            interactType = InteractableType.OPENABLE;
    }

    private void Open()
    {
        print("Door opened");
    }
}
