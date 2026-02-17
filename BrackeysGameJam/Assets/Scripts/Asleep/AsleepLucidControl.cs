using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AsleepLucidControl : MonoBehaviour
{
    public static event Action<bool> onLucidToggled;

    [Header("Initialise references:")]
    [SerializeField] private Slider lucidBar;

    [Header("Lucid Settings")]
    [SerializeField] private float lucidIncreaseRate = 0.5f;
    [SerializeField] private float lucidDecreaseRate = 1.0f;
    [SerializeField] private float maxLucidTime = 5.0f;
    [SerializeField] private float percentNeededToStartLucid = 0.2f;

    public float lucidTimeRemaining;

    private bool canGoLucid = true;
    private bool isLucid = false;
    private bool endLucidGate = true;
    private bool startLucidTriggered = false;


    private void Awake()
    {
        lucidTimeRemaining = maxLucidTime;
    }

    private void Update()
    {
        // Update lucid logic
        if (isLucid)
        {
            lucidTimeRemaining -= Time.deltaTime * lucidDecreaseRate;
            if (lucidTimeRemaining <= 0)
            {
                lucidTimeRemaining = 0;
                canGoLucid = false;
                TransitionEndLucid();
            }
            else if (lucidTimeRemaining < maxLucidTime * percentNeededToStartLucid)
            {
                canGoLucid = false;
            }
        }
        else
        {
            if (lucidTimeRemaining < maxLucidTime)
            {
                lucidTimeRemaining += Time.deltaTime * lucidIncreaseRate;
                if (!canGoLucid && lucidTimeRemaining >= maxLucidTime * percentNeededToStartLucid)
                    canGoLucid = true;
            }
            else
            {
                lucidTimeRemaining = maxLucidTime;
            }
        }

        // Update lucid Slider
        if (lucidBar == null)
            Debug.LogWarning("Expected a slider for the lucid bar");
        else
            lucidBar.value = lucidTimeRemaining / maxLucidTime;
    }
    public void TransitionBeginLucid()
    {
        if (canGoLucid)// && !isLucid)
        {
            startLucidTriggered = true;
            TransitionAnimControl.onBlinkMiddle += lucidOn;
            TransitionAnimControl.instance.StartBlinkTransition();
        }
    }

    public void lucidOn()
    {
        isLucid = true;
        onLucidToggled?.Invoke(isLucid);
        TransitionAnimControl.onBlinkMiddle -= lucidOn;
    }

    public void TransitionEndLucid()
    {
        if (endLucidGate && startLucidTriggered)
        {
            endLucidGate = false;
            TransitionAnimControl.onBlinkMiddle += LucidOff;
            TransitionAnimControl.instance.StartBlinkTransition();
        }
    }

    private void LucidOff()
    {
        startLucidTriggered = false;
        endLucidGate = true;
        isLucid = false;
        onLucidToggled?.Invoke(isLucid);
        TransitionAnimControl.onBlinkMiddle -= LucidOff;
    }
}
