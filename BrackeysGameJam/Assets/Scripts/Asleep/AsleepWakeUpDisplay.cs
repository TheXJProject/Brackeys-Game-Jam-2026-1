using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AsleepWakeUpDisplay : MonoBehaviour
{
    public static event Action onFinishWakeUpVisual;

    [SerializeField] private float timeToWakeUp = 2.0f;
    [SerializeField] private Slider displayChargeTimer;
    [SerializeField] private GameObject wakeUpDisplay;
    private float currentCharge = 0;
    private bool chargingUp = false;


    private void OnEnable()
    {
        currentCharge = 0;
        displayChargeTimer.value = currentCharge / timeToWakeUp;
        AsleepWakeUpControl.onStartWakeUpSequence += BeginWakingUp;
        AsleepWakeUpControl.onCancelWakeUpSequence += StopWakingUp;
    }

    private void OnDisable()
    {
        AsleepWakeUpControl.onStartWakeUpSequence -= BeginWakingUp;
        AsleepWakeUpControl.onCancelWakeUpSequence -= StopWakingUp;

        StopWakingUp(); // Because being disabled means that lucid mode was turned on
    }

    private void Update()
    {
        if (chargingUp)
        {
            currentCharge += Time.deltaTime;
            if (currentCharge >= timeToWakeUp)
            {
                onFinishWakeUpVisual?.Invoke();
                chargingUp = false;
            }

            if (displayChargeTimer == null)
                Debug.LogWarning("Expected a slider for the wake up display bar");
            else
                displayChargeTimer.value = currentCharge / timeToWakeUp;
        }
    }

    private void BeginWakingUp()
    {
        //wakeUpDisplay.SetActive(true);
        chargingUp = true;
    }

    private void StopWakingUp()
    {
        //wakeUpDisplay.SetActive(false);
        chargingUp = false;
        currentCharge = 0;
        displayChargeTimer.value = currentCharge / timeToWakeUp;
    }
}
