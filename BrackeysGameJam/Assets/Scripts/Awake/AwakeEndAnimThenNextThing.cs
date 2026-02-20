using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AwakeEndAnimThenNextThing : MonoBehaviour
{
    [SerializeField] private GameObject nextToUnlock;
    [SerializeField] private int endScreenFade = 5;
    [SerializeField] Image blackFade;

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
        yield return null;
    }
}
