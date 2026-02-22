using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PC_STATE
{
    OFF,
    SLIDE1,
    SLIDE2
}

public class AwakePCInteractedScript : MonoBehaviour
{
    [SerializeField] private PC_STATE state = PC_STATE.OFF;
    [SerializeField] private GameObject Slide1;
    [SerializeField] private GameObject Slide2;
    [SerializeField] private AwakePlayerControl PlayerControl;
    [SerializeField] private AwakeInteract InteractControl;

    public static Action<PC_STATE> onComputerInteraction;


    private void TurnMeOn()
    {
        Slide1.SetActive(true);
        Slide2.SetActive(false);
        PlayerControl.StopMovement();
    }

    private void GoSecondSlide()
    {
        Slide1.SetActive(false);
        Slide2.SetActive(true);
    }

    private void TurnMeOff()
    {
        Slide1.SetActive(false);
        Slide2.SetActive(false);
        PlayerControl.AllowMovement();
    }

    public void PCInteraction()
    {
        if (state == PC_STATE.OFF)
        {
            TurnMeOn();
            state = PC_STATE.SLIDE1;
            InteractControl.promptTextShown = "Scroll Down [E]";
        }
        else if (state == PC_STATE.SLIDE1)
        {
            GoSecondSlide();
            state = PC_STATE.SLIDE2;
            InteractControl.promptTextShown = "Exit Computer [E]";
        }
        else if (state == PC_STATE.SLIDE2)
        {
            TurnMeOff();
            state = PC_STATE.OFF;
            InteractControl.promptTextShown = "Check Computer [E]";
        }

        onComputerInteraction?.Invoke(state);
    }
   
}
