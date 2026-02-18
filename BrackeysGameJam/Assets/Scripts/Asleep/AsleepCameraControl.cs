using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsleepCameraControl : MonoBehaviour
{
    [SerializeField] List<string> layersHiddenOnLucid = new() { "Walls" };
    [SerializeField] List<string> layersShownOnLucid = new() { "Enemies" };
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
        foreach (string layer in layersHiddenOnLucid)
        {
            if (isLucid)
            {
                cam.cullingMask &= ~(1 << LayerMask.NameToLayer(layer));
            }
            else
            {
                cam.cullingMask |= (1 << LayerMask.NameToLayer(layer));
            }
        }

        foreach (string layer in layersShownOnLucid)
        {
            if (!isLucid)
            {
                cam.cullingMask &= ~(1 << LayerMask.NameToLayer(layer));
            }
            else
            {
                cam.cullingMask |= (1 << LayerMask.NameToLayer(layer));
            }
        }
    }
}
