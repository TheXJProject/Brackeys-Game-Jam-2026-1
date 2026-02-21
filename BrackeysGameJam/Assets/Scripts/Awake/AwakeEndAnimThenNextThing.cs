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
    [SerializeField] SpriteRenderer playAgain;
    [SerializeField] GameState gameState;

    [SerializeField] Color colorPlayAgain;

    private void Awake()
    {
        colorPlayAgain = playAgain.color;
        Color colour = colorPlayAgain;
        colour.a = 0f;
        playAgain.color = colour;
    }

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
        {
            onWinFadeScreenStarted?.Invoke();

            float timer = 0;
            Color color = colorPlayAgain;
            while (timer < endScreenFade)
            {
                timer += Time.deltaTime;
                color.a = timer / endScreenFade;
                playAgain.color = color;
                yield return null;
            }

            color.a = 1;
            playAgain.color = color;

            if (nextToUnlock != null)
                nextToUnlock.SetActive(true);

            onWinScreenShown?.Invoke();
        }
        else if (gameState == GameState.LOSS)
        {
            onLossFadeScreenStarted?.Invoke();

            float timer = 0;
            Color color = playAgain.color;
            while (timer < endScreenFade)
            {
                timer += Time.deltaTime;
                color.a = timer / endScreenFade;
                playAgain.color = color;
                yield return null;
            }

            color.a = 1;
            playAgain.color = color;

            if (nextToUnlock != null)
                nextToUnlock.SetActive(true);

            onLossScreenShown?.Invoke();
        }
        else
        {
            Debug.LogWarning("Error, incorrect end scene type set!");
        }
    }
}
