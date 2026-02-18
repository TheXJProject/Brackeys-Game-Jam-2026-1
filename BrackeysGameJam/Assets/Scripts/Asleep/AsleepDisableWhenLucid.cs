using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsleepDisableWhenLucid : MonoBehaviour
{
    [SerializeField] List<GameObject> objectsToDisable;

    private void OnEnable()
    {
        AsleepLucidControl.onLucidToggled += ToggleObjects;
    }

    private void OnDisable()
    {
        AsleepLucidControl.onLucidToggled -= ToggleObjects;
    }

    private void ToggleObjects(bool isLucid)
    {
        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(!isLucid);
        }
    }
}
