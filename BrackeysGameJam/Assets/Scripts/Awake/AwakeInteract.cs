using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AwakeInteract : MonoBehaviour
{
    public UnityEvent onInteractedWith;

    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private string promptTextShown = "Interact [E]";
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
            disableInteraction = true;
            withinRangeForPrompt = false;
        }
    }
}
