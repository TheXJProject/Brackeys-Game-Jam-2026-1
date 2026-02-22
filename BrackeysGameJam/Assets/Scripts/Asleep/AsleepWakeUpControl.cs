using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AsleepWakeUpControl : MonoBehaviour
{
    public static event Action onStartWakeUpSequence;
    public static event Action onCancelWakeUpSequence;

    [SerializeField] private float maxRechargeTime = 20.0f;
    [SerializeField] private Slider awakeBar;
    [SerializeField] private Image awakeBarImage;
    [SerializeField] private Color startColour = Color.white;
    [SerializeField] private Color flashColour = Color.red;
    [SerializeField] private float flashTime = 0.8f;
    [SerializeField] private int numberOfFlashes = 3;

    private float wakeUpRecharge = 0;
    private bool canWakeUp = true;
    private Coroutine flashRed;

    private void Awake()
    {
        startColour = awakeBarImage.color;
    }

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
        }
        else
        {
            wakeUpRecharge = maxRechargeTime;
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
            if (flashRed != null)
                StopCoroutine(flashRed);
            flashRed = StartCoroutine(FlashRed());
        }
    }
    IEnumerator FlashRed()
    {

        float timer = 0;
        Color color = startColour;
        float timeOfOneFlash = flashTime / numberOfFlashes;
        while (timer < flashTime)
        {
            timer += Time.deltaTime;
            float delta = (Mathf.Sin(Mathf.PI * (timer / timeOfOneFlash)) + 1) / 2;
            color = startColour * (1 - delta) + flashColour * delta;
            awakeBarImage.color = color;
            yield return null;
        }

        awakeBarImage.color = startColour;
        yield return null;
    }

    public void CancelWakeUp()
    {
        onCancelWakeUpSequence?.Invoke();
    }

    private void LoadAwakeScene()
    {
        TransitionManager.instance.wokeUpThisGame = true;
        TransitionManager.instance.WakeUpFromPinch();
    }
}
