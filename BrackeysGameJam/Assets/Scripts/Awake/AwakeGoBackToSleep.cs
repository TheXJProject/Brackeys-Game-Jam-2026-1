using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AwakeGoBackToSleep : MonoBehaviour
{
    public static Action onWakeUp;

    InputController playerControls;
    InputAction goBackToSleep;

    private void Awake()
    {
        playerControls = new InputController();
    }

    private void Start()
    {
        onWakeUp?.Invoke();
    }

    private void OnEnable()
    {
        goBackToSleep = playerControls.Player.ToggleSleep;
        goBackToSleep.Enable();
        goBackToSleep.started += GoToSleep;
    }

    private void OnDisable()
    {
        goBackToSleep.Disable();
        goBackToSleep.started -= GoToSleep;
    }

    //AlexIsAPoopyHead
    private void GoToSleep(InputAction.CallbackContext context)
    {
        TransitionManager.instance.FallAsleep();
    }
}
