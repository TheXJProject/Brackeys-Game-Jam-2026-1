using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionAnimControl : MonoBehaviour
{
    public static event Action onBlinkMiddle;
    public static event Action onBlinkFinished;

    public static TransitionAnimControl instance;

    private bool inTransition = false;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // TODO Something that listens out for transitions of things that are happening and knows what to do

    public void StartBlinkTransition()
    {
        if (inTransition)
        {
            //TODO finish quicker somehow
            return;
        }
        inTransition = true;
        // Do Coroutine

        StartCoroutine(BlinkTransition());

        onBlinkMiddle?.Invoke();

        onBlinkFinished?.Invoke();
    }

    private IEnumerator BlinkTransition()
    {
        inTransition = false;
        //TODO: BLINK TRANSITION
        //TODO: Something that interrupts the blink transition
        yield return null;
    }
}
