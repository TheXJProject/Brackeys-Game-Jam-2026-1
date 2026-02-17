using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsleepLucidControl : MonoBehaviour
{
    public static event Action<bool> onLucidToggled;
    
    
    private bool isLucid = false;

    public void TransitionBeginLucid()
    {
        TransitionAnimControl.onBlinkMiddle += ToggleLucid;
        TransitionAnimControl.instance.StartBlinkTransition();
    }

    public void ToggleLucid()
    {
        isLucid = !isLucid;
        onLucidToggled?.Invoke(isLucid);
        TransitionAnimControl.onBlinkMiddle -= ToggleLucid;
    }

    public void TransitionEndLucid()
    {
        TransitionAnimControl.onBlinkMiddle += LucidOff;
        TransitionAnimControl.instance.StartBlinkTransition();
    }

    private void LucidOff()
    {
        isLucid = false;
        onLucidToggled?.Invoke(isLucid);
        TransitionAnimControl.onBlinkMiddle -= LucidOff;
    }
}
