using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AsleepWAKEUP : MonoBehaviour
{
    [SerializeField] float timeTillTextAppears = 3;
    [SerializeField] float timeForTextToFadeIn = 1;
    [SerializeField] Color initialColour = Color.black;
    [SerializeField] Color finalColour = Color.white;
    [SerializeField] TextMeshProUGUI text;

    private void Start()
    {
        text.enabled = false;
        if(!TransitionManager.instance.wakeUpTextShownThisGame)
            StartCoroutine(RememberToWAKEUP());
    }

    IEnumerator RememberToWAKEUP()
    {
        TransitionManager.instance.wakeUpTextShownThisGame = true;
        yield return new WaitForSeconds(timeTillTextAppears);
        text.enabled = true;

        float timer = 0;
        Color color = initialColour;
        while (timer < timeForTextToFadeIn)
        {
            timer += Time.deltaTime;
            color = initialColour * (1 - (timer / timeForTextToFadeIn)) + finalColour * (timer / timeForTextToFadeIn);
            color.a = timer / timeForTextToFadeIn;
            text.color = color;
            yield return null;
        }

        color = finalColour;
        color.a = 1;
        text.color = color;
        yield return null;
    }
}
