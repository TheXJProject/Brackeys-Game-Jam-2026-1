using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ToggleShaders : MonoBehaviour
{
    [SerializeField] Volume volume;
    Vignette vignette;
    FilmGrain grain;
    ChromaticAberration chromaticAberration;
    WhiteBalance whiteBalance;
    MotionBlur motiionBlur;
    DepthOfField depthOfField;
    SplitToning splitToning;

    bool setupProperly = true;

    private void Awake()
    {
        if (!volume.profile.TryGet<Vignette>(out vignette))
            setupProperly = false;
        if (!volume.profile.TryGet<FilmGrain>(out grain))
            setupProperly = false;
        if (!volume.profile.TryGet<ChromaticAberration>(out chromaticAberration))
            setupProperly = false;
        if (!volume.profile.TryGet<WhiteBalance>(out whiteBalance))
            setupProperly = false;
        if (!volume.profile.TryGet<MotionBlur>(out motiionBlur))
            setupProperly = false;
        if (!volume.profile.TryGet<DepthOfField>(out depthOfField))
            setupProperly = false;
        if (!volume.profile.TryGet<SplitToning>(out splitToning))
            setupProperly = false;
    }

    private void OnEnable()
    {
        AsleepLucidControl.onLucidToggled += ToggleShadersFromLucid;
    }

    private void OnDisable()
    {
        AsleepLucidControl.onLucidToggled -= ToggleShadersFromLucid;    
    }

    private void ToggleShadersFromLucid(bool isLucid)
    {
        if (!setupProperly)
        {
            Debug.LogWarning("Not setup volume for shader toggle on lucid");
            return;
        }
        vignette.active = !isLucid;
        grain.active = !isLucid;
        chromaticAberration.active = isLucid;
        whiteBalance.active = isLucid;
        motiionBlur.active = !isLucid;
        splitToning.active = !isLucid;

        depthOfField.active = true;
    }
}
