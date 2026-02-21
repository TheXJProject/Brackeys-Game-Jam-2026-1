using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GoToBeginningOfGameOnPress : MonoBehaviour
{
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

        interact.started += GoToBeginningOfGame;
    }

    private void OnDisable()
    {
        interact.Disable();
        interact.started -= GoToBeginningOfGame;
    }

    public void GoToBeginningOfGame(InputAction.CallbackContext context)
    {
        TransitionManager.instance.LoadSceneRestartGame();
    }
}
