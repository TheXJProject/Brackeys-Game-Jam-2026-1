using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsleepCameraControl : MonoBehaviour
{
    [SerializeField] string LayerToToggle = "Walls";
    [SerializeField] Camera cam;

    private void OnEnable()
    {
        AsleepLucidControl.onLucidToggled += ToggleLayersToShow;
    }

    private void OnDisable()
    {
        AsleepLucidControl.onLucidToggled -= ToggleLayersToShow;
    }

    private void ToggleLayersToShow(bool isLucid)
    {
        if (isLucid)
        {
            cam.cullingMask &= ~(1 << LayerMask.NameToLayer(LayerToToggle));
        }
        else
        {
            cam.cullingMask |= (1 << LayerMask.NameToLayer(LayerToToggle));
        }
    }
}
