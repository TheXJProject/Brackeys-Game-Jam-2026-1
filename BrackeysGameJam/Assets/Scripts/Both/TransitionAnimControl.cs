using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionAnimControl : MonoBehaviour
{
    public static TransitionAnimControl instance;

    public static event Action onBlinkMiddle;
    public static event Action onBlinkFinished;

    [Header("Initialise references:")]
    [SerializeField] RectTransform topBlink;
    [SerializeField] RectTransform botBlink;

    [Header("Settings of transition:")]
    [SerializeField] float closingEyesBlinkTime;
    [SerializeField] float openingEyesBlinkTime;
    [SerializeField] float shutEyesBlinkTime;
    [SerializeField] float middleHeight;
    [SerializeField] Vector2 middleAnchorCaps;

    // const 
    const float middleAnchorCap = 0.5f;

    // Runtime variables
    private float startEndHeight;
    private Vector2 topStartEndAnchorCaps;
    private Vector2 botStartEndAnchorCaps;
    private bool inTransition = false;
    private bool reachMidThisTransition = false;
    private bool reverseTransition = false;
    private float percentIn = 0;

    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        startEndHeight = Mathf.Abs(topBlink.anchoredPosition.y);
        topStartEndAnchorCaps = new Vector2(topBlink.anchorMin.y, topBlink.anchorMax.y);
        botStartEndAnchorCaps = new Vector2(botBlink.anchorMin.y, botBlink.anchorMax.y);
    }

    // TODO Something that listens out for transitions of things that are happening and knows what to do

    public void StartBlinkTransition()
    {
        if (inTransition)
        {
            if (reachMidThisTransition)
            {
                reverseTransition = true;
            }
            return;
        }
        inTransition = true;

        StartCoroutine(BlinkTransition());
    }

    private IEnumerator BlinkTransition()
    {
        float elapsedTime = 0;

        if (reverseTransition)
        {
            elapsedTime = closingEyesBlinkTime * (1-percentIn);
            reverseTransition = false;
        }

        // Blink eyes are closing ------------------------------------------------------------------------
        while (elapsedTime < closingEyesBlinkTime)
        {
            topBlink.anchoredPosition = new Vector2(0, Mathf.Lerp(startEndHeight, middleHeight, elapsedTime / closingEyesBlinkTime));
            botBlink.anchoredPosition = topBlink.anchoredPosition * -1;
            Vector2 newTopAnchorCap = Vector2.Lerp(topStartEndAnchorCaps, middleAnchorCaps, elapsedTime / closingEyesBlinkTime);
            topBlink.anchorMin = new Vector2(middleAnchorCap, newTopAnchorCap.x);
            topBlink.anchorMax = new Vector2(middleAnchorCap, newTopAnchorCap.y);
            Vector2 newBotAnchorCap = Vector2.Lerp(botStartEndAnchorCaps, middleAnchorCaps, elapsedTime / closingEyesBlinkTime);
            botBlink.anchorMin = new Vector2(middleAnchorCap, newBotAnchorCap.x);
            botBlink.anchorMax = new Vector2(middleAnchorCap, newBotAnchorCap.y);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0;
        topBlink.anchoredPosition = new Vector2(0, middleHeight);
        botBlink.anchoredPosition = topBlink.anchoredPosition * -1;
        topBlink.anchorMin = new Vector2(middleAnchorCap, middleAnchorCaps.x);
        topBlink.anchorMax = new Vector2(middleAnchorCap, middleAnchorCaps.y);
        botBlink.anchorMin = new Vector2(middleAnchorCap, middleAnchorCaps.x);
        botBlink.anchorMax = new Vector2(middleAnchorCap, middleAnchorCaps.y);
        onBlinkMiddle?.Invoke();
        reachMidThisTransition = true;

        // Blink eyes are closed and waiting ------------------------------------------------------------------------
        while (elapsedTime < shutEyesBlinkTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0;

        // Blink eyes are opening ------------------------------------------------------------------------
        while (elapsedTime < openingEyesBlinkTime)
        {
            if (reverseTransition)
                break;
            topBlink.anchoredPosition = new Vector2(0, Mathf.Lerp(middleHeight, startEndHeight, elapsedTime / openingEyesBlinkTime));
            botBlink.anchoredPosition = topBlink.anchoredPosition * -1;
            Vector2 newTopAnchorCap = Vector2.Lerp(middleAnchorCaps, topStartEndAnchorCaps, elapsedTime / openingEyesBlinkTime);
            topBlink.anchorMin = new Vector2(middleAnchorCap, newTopAnchorCap.x);
            topBlink.anchorMax = new Vector2(middleAnchorCap, newTopAnchorCap.y);
            Vector2 newBotAnchorCap = Vector2.Lerp(middleAnchorCaps, botStartEndAnchorCaps, elapsedTime / openingEyesBlinkTime);
            botBlink.anchorMin = new Vector2(middleAnchorCap, newBotAnchorCap.x);
            botBlink.anchorMax = new Vector2(middleAnchorCap, newBotAnchorCap.y);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (reverseTransition)
        {
            percentIn = elapsedTime / openingEyesBlinkTime;
            reachMidThisTransition = false;
            StartCoroutine(BlinkTransition());
        }
        else
        {
            topBlink.anchoredPosition = new Vector2(0, startEndHeight);
            botBlink.anchoredPosition = topBlink.anchoredPosition * -1;
            topBlink.anchorMin = new Vector2(middleAnchorCap, topStartEndAnchorCaps.x);
            topBlink.anchorMax = new Vector2(middleAnchorCap, topStartEndAnchorCaps.y);
            botBlink.anchorMin = new Vector2(middleAnchorCap, botStartEndAnchorCaps.x);
            botBlink.anchorMax = new Vector2(middleAnchorCap, botStartEndAnchorCaps.y);
            reachMidThisTransition = false;
            inTransition = false;
        }
        onBlinkFinished?.Invoke();
        yield return null;
    }
}
