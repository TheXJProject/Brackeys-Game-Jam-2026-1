using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
    
public enum GameState
{
    WON,
    LOSS,
    OTHER
}

public class AwakeEndAnimThenNextThing : MonoBehaviour
{
    public static event Action onWinFadeScreenStarted;
    public static event Action onWinScreenShown;
    public static event Action onLossFadeScreenStarted;
    public static event Action onLossScreenShown;

    [SerializeField] private GameObject nextToUnlock;
    [SerializeField] private int endScreenFade = 5;
    [SerializeField] Image blackFade;
    [SerializeField] GameState gameState;

    [SerializeField] 

    private void Start()
    {
        // TODO: DO screen animation
        StartCoroutine(WaitForASec());
    }

    private IEnumerator WaitForASec()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(ShowEndScreen());
    }

    public IEnumerator ShowEndScreen()
    {
        if (gameState == GameState.WON)
            onWinFadeScreenStarted?.Invoke();
        else if (gameState == GameState.LOSS)
            onLossFadeScreenStarted?.Invoke();

        float timer = 0;
        Color color = blackFade.color;
        while ( timer < endScreenFade)
        {
            timer += Time.deltaTime;
            color.a = timer / endScreenFade;
            blackFade.color = color;
            yield return null;
        }

        color.a = 1;
        blackFade.color = color;

        if (nextToUnlock != null)
            nextToUnlock.SetActive(true);

        if (gameState == GameState.WON)
            onWinScreenShown?.Invoke();
        else if (gameState == GameState.LOSS)
            onLossScreenShown?.Invoke();
        yield return null;
    }
}
