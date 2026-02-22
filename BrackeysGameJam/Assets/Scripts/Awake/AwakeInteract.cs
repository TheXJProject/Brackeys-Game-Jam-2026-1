using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum InteractWith2D
{
    BED,
    COMPUTER
}

public class AwakeInteract : MonoBehaviour
{
    public UnityEvent onInteractedWith;
    public static event Action<InteractWith2D> onInteractedWithIn2D;

    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] public string promptTextShown = "Interact [E]";
    [SerializeField] private InteractWith2D interactObject = InteractWith2D.BED;
    private bool withinRangeForPrompt = false;
    private bool disableInteraction;

    InputController playerControls;
    InputAction interact;

    private void Awake()
    {
        playerControls = new InputController();
    }

    private void OnEnable()
    {
        interact = playerControls.Player.Interact;
        interact.Enable();
        interact.started += Interact;
    }

    private void OnDisable()
    {
        interact.Disable();
        interact.started -= Interact;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if ( collision.tag == "Player" && !disableInteraction)
        {
            withinRangeForPrompt = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !disableInteraction)
        {
            withinRangeForPrompt = false;
        }
    }

    private void Update()
    {
        if (withinRangeForPrompt)
            promptText.text = promptTextShown;
        else
            promptText.text = "";
    }

    private void Interact(InputAction.CallbackContext context)
    {
        if (withinRangeForPrompt)
        {
            onInteractedWith?.Invoke();
            if (interactObject == InteractWith2D.BED) 
            { 
                onInteractedWithIn2D?.Invoke(interactObject);
                disableInteraction = true;
                withinRangeForPrompt = false;
            }
        }
    }
}
