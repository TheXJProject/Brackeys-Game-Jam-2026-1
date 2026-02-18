using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AsleepWakeUpControl : MonoBehaviour
{
    public static event Action onStartWakeUpSequence;
    public static event Action onCancelWakeUpSequence;

    [SerializeField] private float maxRechargeTime = 20.0f;
    [SerializeField] private Slider awakeBar;

    private float wakeUpRecharge = 0;
    private bool canWakeUp = false;

    private void OnEnable()
    {
        AsleepWakeUpDisplay.onFinishWakeUpVisual += LoadAwakeScene;
    }

    private void OnDisable()
    {
        AsleepWakeUpDisplay.onFinishWakeUpVisual -= LoadAwakeScene;
    }

    private void Update()
    {
        if (wakeUpRecharge < maxRechargeTime)
        {
            wakeUpRecharge += Time.deltaTime;
            canWakeUp = false;
        }
        else
        {
            wakeUpRecharge = maxRechargeTime;
            canWakeUp = true;
        }

        // Update wake up Slider
        if (awakeBar == null)
            Debug.LogWarning("Expected a slider for the wake up bar");
        else
            awakeBar.value = wakeUpRecharge / maxRechargeTime;
    }

    public void AttemptWakeUp()
    {
        if (canWakeUp)
        {
            onStartWakeUpSequence?.Invoke();
        }
        else
        {
            // TODO FAILED WAKE UP - flash the slider red or something
        }
    }

    public void CancelWakeUp()
    {
        onCancelWakeUpSequence?.Invoke();
    }

    private void LoadAwakeScene()
    {
        TransitionManager.instance.WakeUpFromPinch();
    }
}
