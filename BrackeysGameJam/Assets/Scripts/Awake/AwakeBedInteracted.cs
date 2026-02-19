using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwakeBedInteracted : MonoBehaviour
{
    public void BedInteraction()
    {
        TransitionManager.instance.FallAsleep();
    }
}
