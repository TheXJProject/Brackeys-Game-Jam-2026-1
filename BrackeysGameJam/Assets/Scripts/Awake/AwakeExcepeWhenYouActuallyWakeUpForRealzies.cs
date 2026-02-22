using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwakeExcepeWhenYouActuallyWakeUpForRealzies : MonoBehaviour
{
    [SerializeField] AwakeEndAnimThenNextThing nextthingController;
    [SerializeField] Animator animator;
    [SerializeField] float timeUntilWakeyWakey = 1;
    [SerializeField] float timeUntilCanPLayAgain = 2;
    public void StartWakingUp()
    {
        StartCoroutine(StartWakeyWakey());
    }

    private IEnumerator StartWakeyWakey()
    {
        yield return new WaitForSeconds(timeUntilWakeyWakey);
        animator.SetTrigger("next");
        yield return new WaitForSeconds(timeUntilCanPLayAgain);
        nextthingController.ShowEndScreenCoroutine();
    }
}
