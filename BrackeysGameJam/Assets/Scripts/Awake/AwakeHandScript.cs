using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwakeHandScript : MonoBehaviour
{
    public static event Action onHandWaitingToGrab;

    [SerializeField] float secondsBeforeHandComesIn = 2.0f;
    [SerializeField] float timeForHandToComeIn = 5.0f;
    [SerializeField] float timeForHandToUnravel = 3.5f;
    [SerializeField] float handPauseTime = 3.0f;
    [SerializeField] float handQuickLaunchTime = 0.3f;
    [SerializeField] float fadeOutTime = 0.20f;
    [SerializeField] AwakeEndAnimThenNextThing animNextThing;
    [SerializeField] Animator handAnimator;
    [SerializeField] Transform handTransform;
    [SerializeField] Vector2 startPos;
    [SerializeField] Vector2 StopPos;
    [SerializeField] Vector2 FinalPos;

    private bool handUnravelled = false;
    private bool handShotForward = false;
    float timer = 0;

    public void BeginHandScriptVisuals()
    {
        StartCoroutine(StartHandVisuals());
    }

    IEnumerator StartHandVisuals()
    {
        yield return new WaitForSeconds(secondsBeforeHandComesIn);
        do
        {
            timer += Time.deltaTime;
            handTransform.position = Vector2.Lerp(startPos, StopPos, timer / timeForHandToComeIn);
            if (timer > timeForHandToUnravel && !handUnravelled)
            {
                handUnravelled = true;
                handAnimator.SetTrigger("next");
            }
            yield return null;
        } while (timer < timeForHandToComeIn);
        handTransform.position = StopPos;

        yield return new WaitForSeconds(handPauseTime);
        onHandWaitingToGrab?.Invoke();
        handAnimator.SetTrigger("next2");
        timer = 0;
        do
        {
            timer += Time.deltaTime;
            handTransform.position = Vector2.Lerp(StopPos, FinalPos, timer / handQuickLaunchTime);
            if (timer > fadeOutTime && !handShotForward)
            {
                handShotForward = true;
                animNextThing.ShowEndScreenCoroutine();
            }
            yield return null;
        } while (timer < handQuickLaunchTime);

        handTransform.position = FinalPos;
        yield return null;
    }
}
