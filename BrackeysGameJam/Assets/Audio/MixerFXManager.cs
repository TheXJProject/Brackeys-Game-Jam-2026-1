using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Audio;
using System.Collections.Generic;

public class MixerFXManager : MonoBehaviour
{
    // ==== For Debugging ====
    [SerializeField] bool Msg = false;

    // Inspector Adjustable Values:
    [Header("Prevent error messages if audio not in use")] [SerializeField] bool removeWarningMsgs;

    // Initialise In Inspector:
    public AudioMixer audioMixer;
    [Header("---- Mixer Groups ----\n")]
    [Header("_For Player_")]
    public PlayerGroups playerGroups;
    [Header("_Core Groups_")]
    public MainGroups mainGroups;
    [Header("_Groups Info_")]
    public MixerGroupsInfo[] groupInfo;

    // Runtime Variables:
    public static MixerFXManager instance;
    private readonly Dictionary<(MixerGroupsInfo, EX_PARA), Coroutine> activeFades = new();
    string exParaMasterPlayer;
    string exParaMusicPlayer;
    string exParaSfxPlayer;

    public void CreateSingleton()
    {
        // If we haven't already initialised an instance of the Audio manager
        if (instance == null)
        {
            // Make this instance a singleton
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            // Destroy this
            Destroy(gameObject);
        }
    }

    public void InitialiseMixer()
    {
        // (Yu Gui Oh fusion solution! We make sure that both the Audio Manager
        // and Mixer Manager are present.)
        if (GetComponent<AudioManager>() == null)
        {
            if (!removeWarningMsgs) Debug.LogWarning("AudioManager component is missing!");
            if (!removeWarningMsgs) Debug.LogWarning("Please add component before running!");
        }

        // Error check player groups
        if (playerGroups.masterPlayer == null)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, player group masterPlayer not filled in!");
            if (!removeWarningMsgs) Debug.LogWarning("Please add component before running! Cheers :)");
        }
        else if (!Array.Exists(groupInfo, gr => gr.group == playerGroups.masterPlayer))
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, main group " + playerGroups.masterPlayer.name + " not in the main group collection!");
        }
        else
        {
            // Check it doesn't exist in core groups
            foreach (GROUP_OPTIONS option in System.Enum.GetValues(typeof(GROUP_OPTIONS)))
            {
                if (Array.Exists(mainGroups.GroupOptionToArray(option), gr => gr == playerGroups.masterPlayer))
                {
                    if (!removeWarningMsgs) Debug.LogWarning("Error, player group " + playerGroups.masterPlayer + " in the core group collection!");
                }
            }
        }
        if (playerGroups.musicPlayer == null)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, player group musicPlayer not filled in!");
            if (!removeWarningMsgs) Debug.LogWarning("Please add component before running! Cheers :)");
        }
        else if (!Array.Exists(groupInfo, gr => gr.group == playerGroups.musicPlayer))
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, main group " + playerGroups.musicPlayer.name + " not in the main group collection!");
        }
        else
        {
            // Check it doesn't exist in core groups
            foreach (GROUP_OPTIONS option in System.Enum.GetValues(typeof(GROUP_OPTIONS)))
            {
                if (Array.Exists(mainGroups.GroupOptionToArray(option), gr => gr == playerGroups.musicPlayer))
                {
                    if (!removeWarningMsgs) Debug.LogWarning("Error, player group " + playerGroups.musicPlayer + " in the core group collection!");
                }
            }
        }
        if (playerGroups.sfxPlayer == null)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, player group sfxPlayer not filled in!");
            if (!removeWarningMsgs) Debug.LogWarning("Please add component before running! Cheers :)");
        }
        else if (!Array.Exists(groupInfo, gr => gr.group == playerGroups.sfxPlayer))
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, main group " + playerGroups.sfxPlayer.name + " not in the main group collection!");
        }
        else
        {
            // Check it doesn't exist in core groups
            foreach (GROUP_OPTIONS option in System.Enum.GetValues(typeof(GROUP_OPTIONS)))
            {
                if (Array.Exists(mainGroups.GroupOptionToArray(option), gr => gr == playerGroups.sfxPlayer))
                {
                    if (!removeWarningMsgs) Debug.LogWarning("Error, player group " + playerGroups.sfxPlayer + " in the core group collection!");
                }
            }
        }

        // For each main group collection
        foreach (GROUP_OPTIONS option in System.Enum.GetValues(typeof(GROUP_OPTIONS)))
        {
            // Make sure at least one group is assigned
            if (mainGroups.GroupOptionToArray(option).Length == 0)
            {
                if (!removeWarningMsgs) Debug.LogWarning("Error, main group " + option + " not filled in!");
                if (!removeWarningMsgs) Debug.LogWarning("Please add component before running! Cheers :)");
            }
            else
            {
                // For each individual group in a collection
                foreach (AudioMixerGroup g in mainGroups.GroupOptionToArray(option))
                {
                    // Check it exsits in somewhere in the set of all groups
                    if (!Array.Exists(groupInfo, gr => gr.group == g))
                    {
                        if (!removeWarningMsgs) Debug.LogWarning("Error, main group " + g.name + " not in the main group collection!");
                    }
                }
            }
        }

        // Check main group array filled in
        if (groupInfo.Length == 0)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, main group array empty!");
        }

        // From here, sets up audio channels.
        // Check if correct group info has been filled in and if it is valid.
        // For each group construct
        foreach (MixerGroupsInfo g in groupInfo)
        {
            // If not assigned a group from the mixer
            if (g.group == null)
            {
                // Throw error
                if (!removeWarningMsgs) Debug.LogWarning("Error, group reference missing in 'Mixer Groups'!");

                g.name = MixerGroupsInfo.errorName;
                g.parameters.volume = MixerGroupExpoParameters.errorName;
                g.parameters.lowPassEQ = MixerGroupExpoParameters.errorName;
                g.parameters.highPassEQ = MixerGroupExpoParameters.errorName;
                g.parameters.pitchShift = MixerGroupExpoParameters.errorName;

                // ===== (EX_PARA SET: Add in extra parameters if added to!) =====
            }
            else
            {
                // Otherwise, apply the groups name automatically
                g.name = g.group.name;

                // Check if parameters have been filled in
                // If empty
                if (g.parameters.volume == "") // ===== Volume =====
                {
                    // Apply the default name (Some parameters won't be used so we don't throw an error)
                    g.parameters.volume = MixerGroupExpoParameters.defaultName;
                }
                // If we have inputted a name for a potential exposed parameter, check it exists and if it does
                // Assign the value, it currently has, to our "start" variable
                else if (!audioMixer.GetFloat(g.parameters.volume, out g.parameters.startVolume))
                {
                    // If we don't find a match throw error
                    if (!removeWarningMsgs) Debug.LogWarning("Error, parameter with name, " + ((g.parameters.volume == "") ? "none" : g.parameters.volume) + ", cannot be found in the mixer!");
                    g.parameters.volume = MixerGroupExpoParameters.errorName;
                }

                // If empty
                if (g.parameters.lowPassEQ == "") // ===== LowPassEQ =====
                {
                    // Apply the default name (Some parameters won't be used so we don't throw an error)
                    g.parameters.lowPassEQ = MixerGroupExpoParameters.defaultName;
                }
                // If we have inputted a name for a potential exposed parameter, check it exists and if it does
                // Assign the value, it currently has, to our "start" variable
                else if (!audioMixer.GetFloat(g.parameters.lowPassEQ, out g.parameters.startLowPassEQ))
                {
                    // If we don't find a match throw error
                    if (!removeWarningMsgs) Debug.LogWarning("Error, parameter with name, " + ((g.parameters.lowPassEQ == "") ? "none" : g.parameters.lowPassEQ) + ", cannot be found in the mixer!");
                    g.parameters.lowPassEQ = MixerGroupExpoParameters.errorName;
                }

                // If empty
                if (g.parameters.highPassEQ == "") // ===== HighPassEQ =====
                {
                    // Apply the default name (Some parameters won't be used so we don't throw an error)
                    g.parameters.highPassEQ = MixerGroupExpoParameters.defaultName;
                }
                // If we have inputted a name for a potential exposed parameter, check it exists and if it does
                // Assign the value, it currently has, to our "start" variable
                else if (!audioMixer.GetFloat(g.parameters.highPassEQ, out g.parameters.startHighPassEQ))
                {
                    // If we don't find a match throw error
                    if (!removeWarningMsgs) Debug.LogWarning("Error, parameter with name, " + ((g.parameters.highPassEQ == "") ? "none" : g.parameters.highPassEQ) + ", cannot be found in the mixer!");
                    g.parameters.highPassEQ = MixerGroupExpoParameters.errorName;
                }

                // If empty
                if (g.parameters.pitchShift == "") // ===== PitchShift =====
                {
                    // Apply the default name (Some parameters won't be used so we don't throw an error)
                    g.parameters.pitchShift = MixerGroupExpoParameters.defaultName;
                }
                // If we have inputted a name for a potential exposed parameter, check it exists and if it does
                // Assign the value, it currently has, to our "start" variable
                else if (!audioMixer.GetFloat(g.parameters.pitchShift, out g.parameters.startPitchShift))
                {
                    // If we don't find a match throw error
                    if (!removeWarningMsgs) Debug.LogWarning("Error, parameter with name, " + ((g.parameters.pitchShift == "") ? "none" : g.parameters.pitchShift) + ", cannot be found in the mixer!");
                    g.parameters.pitchShift = MixerGroupExpoParameters.errorName;
                }

                // ===== (EX_PARA SET: Add in extra parameters if added to!) =====

                // Check if audiosources exist in Audio Manager.
                // For each audio source we have linked to the mixer
                foreach (AudioSource audioSource in g.linkedAudioSources.audioSources)
                {
                    // Find the source in the audio manager, it could be sfx or music
                    SoundSource sourceMusic = Array.Find(AudioManager.instance.musicSourceList, y => y.audioSource == audioSource);
                    SoundSource sourceSFX = Array.Find(AudioManager.instance.sfxSourceList, y => y.audioSource == audioSource);
                    SoundSource sourceLoopingSFX = Array.Find(AudioManager.instance.sfxLoopingSourceList, y => y.audioSource == audioSource);

                    // If both don't exist, throw error
                    if ((sourceMusic == null) && (sourceSFX == null) && (sourceLoopingSFX == null))
                    {
                        if (!removeWarningMsgs) Debug.LogWarning("Error, audiosource, " + ((audioSource == null) ? "none" : audioSource.name) + ", not found in Audio Manager!");
                    }

                    // Check if audiosources are linked correctly.
                    if (audioSource != null)
                    {
                        // If the group, the audiosource is assigned to, is not the same as the name of the group construct
                        // then the audiosource has been assigned to the wrong group. So throw error
                        if (audioSource.outputAudioMixerGroup.name != g.group.name)
                        {
                            if (!removeWarningMsgs) Debug.LogWarning("Error, audiosource, " + ((audioSource == null) ? "none" : audioSource.name) + ", not outputting to this group, " + g.group.name + "!");
                        }
                    }
                }
            }
        }

        // Get the group info of the player group
        MixerGroupsInfo playerGroup = Array.Find(groupInfo, gI => gI.group == playerGroups.masterPlayer);

        // Get the exposed parameter for the player master group
        exParaMasterPlayer = GetExposedParams(playerGroup, EX_PARA.VOLUME).Item1;

        // Get the group info of the player group
        playerGroup = Array.Find(groupInfo, gI => gI.group == playerGroups.musicPlayer);

        // Get the exposed parameter for the player master group
        exParaMusicPlayer = GetExposedParams(playerGroup, EX_PARA.VOLUME).Item1;

        // Get the group info of the player group
        playerGroup = Array.Find(groupInfo, gI => gI.group == playerGroups.sfxPlayer);

        // Get the exposed parameter for the player master group
        exParaSfxPlayer = GetExposedParams(playerGroup, EX_PARA.VOLUME).Item1;

        if (Msg) Debug.Log("Finished Audio Setup.");
    }

    public void SetMusicParam(string name, EX_PARA param, float duration, float? value = null)
    {
        if (CheatLogic.cheatTool.musicAndSoundForceOff) return;

        float targetValue = 0;
        string expoParam = "";

        // We want to set a parameter in a music group that's playing track with "name".
        // We need to know what parameter, how long it will take, and the value we're setting it to.
        // First, we find the group and get it's current value for that parameter.
        // To do that, find the audio source outputting the track
        SoundSource source = Array.Find(AudioManager.instance.musicSourceList, y => y.soundName == name);

        // Error check
        if (source == null)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, can't find a source with audio source playing " + name + ".");
            return;
        }

        // Then we find which group the source is playing to. We check each element of our array of groups.
        // In each element, we check our list of audiosources to see if the list contains the source we're looking for.
        MixerGroupsInfo groupToUse = Array.Find(groupInfo, x => Array.Exists(x.linkedAudioSources.audioSources, z => z == source.audioSource));

        // Error check
        if (groupToUse == null)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, can't find an group with audio source playing " + name + ".");
            return;
        }
        // If the paramater is already fading
        else if (activeFades.TryGetValue((groupToUse, param), out Coroutine exists))
        {
            // Error Check
            if (exists == null)
            {
                if (!removeWarningMsgs) Debug.LogWarning("Error, existing coroutine is null!");
                if (!removeWarningMsgs) Debug.LogWarning("Group " + groupToUse.name);
            }
            else
            {
                // Stop the coroutine and remove it from active fades
                StopCoroutine(exists);
                activeFades.Remove((groupToUse, param));
            }
        }

        // From the group to use, and the param type, get the correct exposed parameter and start value (on game loadup)
        expoParam = GetExposedParams(groupToUse, param).Item1;
        targetValue = GetExposedParams(groupToUse, param).Item2;

        // Get the current value of the exposed parameter
        if (!audioMixer.GetFloat(expoParam, out float currentValue))
        {
            // Throw error if this fails
            if (!removeWarningMsgs) Debug.LogWarning("Error, failed to get current value for " + expoParam);
            return;
        }

        // Convert and clamp (linear values should be between 0 and 1)
        currentValue = ConvertType(param, false, currentValue);
        targetValue = Mathf.Clamp01(value ?? ConvertType(param, false, targetValue));

        // Finally, kick off a coroutine that fades the value
        Coroutine fade = StartCoroutine(Fader(expoParam, (groupToUse, param), duration, currentValue, targetValue));

        // If the fade exists
        if (fade != null)
        {
            // Store the coroutine
            activeFades[(groupToUse, param)] = fade;
        }
    }

    public void SetMusicOverallParam(EX_PARA param, float duration, float? value = null)
    {
        if (CheatLogic.cheatTool.musicAndSoundForceOff) return;

        float targetValue = 0;
        string expoParam = "";

        // For each group in Music overall
        foreach (AudioMixerGroup g in mainGroups.musicOverall)
        {
            // Error check
            if (g == null)
            {
                if (!removeWarningMsgs) Debug.LogWarning("Error, group in musicOverall is null.");
                continue;
            }

            // Then we check each element of our array of groups to find the right infomation
            MixerGroupsInfo groupToUse = Array.Find(groupInfo, y => y.group == g);

            // If the paramater is already fading
            if (activeFades.TryGetValue((groupToUse, param), out Coroutine exists))
            {
                // Error Check
                if (exists == null)
                {
                    if (!removeWarningMsgs) Debug.LogWarning("Error, existing coroutine is null!");
                }
                else
                {
                    // Stop the coroutine and remove it from active fades
                    StopCoroutine(exists);
                    activeFades.Remove((groupToUse, param));
                }
            }

            // From the group to use, and the param type, get the correct exposed parameter and start value (on game loadup)
            expoParam = GetExposedParams(groupToUse, param).Item1;
            targetValue = GetExposedParams(groupToUse, param).Item2;

            // Get the current value of the exposed parameter
            if (!audioMixer.GetFloat(expoParam, out float currentValue))
            {
                // Throw error if this fails
                if (!removeWarningMsgs) Debug.LogWarning("Error, failed to get current value for " + param);
                return;
            }

            // Convert and clamp (linear values should be between 0 and 1)
            currentValue = ConvertType(param, false, currentValue);
            targetValue = Mathf.Clamp01(value ?? ConvertType(param, false, targetValue));

            // Finally, kick off a coroutine that fades the value
            Coroutine fade = StartCoroutine(Fader(expoParam, (groupToUse, param), duration, currentValue, targetValue));

            // If the fade exists
            if (fade != null)
            {
                // Store the coroutine
                activeFades[(groupToUse, param)] = fade;
            }
        }
    }

    public void SetLoopingSFXParam(string name, EX_PARA param, float duration, float? value = null)
    {
        if (CheatLogic.cheatTool.musicAndSoundForceOff) return;

        float targetValue = 0;
        string expoParam = "";

        // We want to set a parameter in a looping SFX group that's playing track with "name".
        // We need to know what parameter, how long it will take, and the value we're setting it to.
        // First, we find the group and get it's current value for that parameter.
        // To do that, find the audio source outputting the track
        SoundSource source = Array.Find(AudioManager.instance.sfxLoopingSourceList, y => y.soundName == name);

        // Error check
        if (source == null)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, can't find a source with audio source playing " + name + ".");
            return;
        }

        // Then we find which group the source is playing to. We check each element of our array of groups.
        // In each element, we check our list of audiosources to see if the list contains the source we're looking for.
        MixerGroupsInfo groupToUse = Array.Find(groupInfo, x => Array.Exists(x.linkedAudioSources.audioSources, z => z == source.audioSource));

        // Error check
        if (groupToUse == null)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, can't find an group with audio source playing " + name + ".");
            return;
        }
        // If the paramater is already fading
        else if (activeFades.TryGetValue((groupToUse, param), out Coroutine exists))
        {
            // Error Check
            if (exists == null)
            {
                if (!removeWarningMsgs) Debug.LogWarning("Error, existing coroutine is null!");
                if (!removeWarningMsgs) Debug.LogWarning("Group " + groupToUse.name);
            }
            else
            {
                // Stop the coroutine and remove it from active fades
                StopCoroutine(exists);
                activeFades.Remove((groupToUse, param));
            }
        }

        // From the group to use, and the param type, get the correct exposed parameter and start value (on game loadup)
        expoParam = GetExposedParams(groupToUse, param).Item1;
        targetValue = GetExposedParams(groupToUse, param).Item2;

        // Get the current value of the exposed parameter
        if (!audioMixer.GetFloat(expoParam, out float currentValue))
        {
            // Throw error if this fails
            if (!removeWarningMsgs) Debug.LogWarning("Error, failed to get current value for " + expoParam);
            return;
        }

        // Convert and clamp (linear values should be between 0 and 1)
        currentValue = ConvertType(param, false, currentValue);
        targetValue = Mathf.Clamp01(value ?? ConvertType(param, false, targetValue));

        // Finally, kick off a coroutine that fades the value
        Coroutine fade = StartCoroutine(Fader(expoParam, (groupToUse, param), duration, currentValue, targetValue));

        // If the fade exists
        if (fade != null)
        {
            // Store the coroutine
            activeFades[(groupToUse, param)] = fade;
        }
    }

    public void SetSfxOverallParam(EX_PARA param, float duration, float? value = null)
    {
        if (CheatLogic.cheatTool.musicAndSoundForceOff) return;

        float targetValue = 0;
        string expoParam = "";

        // For each group in sfx overall
        foreach (AudioMixerGroup g in mainGroups.sfxOverall)
        {
            // Error check
            if (g == null)
            {
                if (!removeWarningMsgs) Debug.LogWarning("Error, group in sfxOverall is null.");
                continue;
            }

            // Then we check each element of our array of groups to find the right infomation
            MixerGroupsInfo groupToUse = Array.Find(groupInfo, y => y.group == g);

            // If the paramater is already fading
            if (activeFades.TryGetValue((groupToUse, param), out Coroutine exists))
            {
                // Error Check
                if (exists == null)
                {
                    if (!removeWarningMsgs) Debug.LogWarning("Error, existing coroutine is null!");
                }
                else
                {
                    // Stop the coroutine and remove it from active fades
                    StopCoroutine(exists);
                    activeFades.Remove((groupToUse, param));
                }
            }

            // From the group to use, and the param type, get the correct exposed parameter and start value (on game loadup)
            expoParam = GetExposedParams(groupToUse, param).Item1;
            targetValue = GetExposedParams(groupToUse, param).Item2;

            // Get the current value of the exposed parameter
            if (!audioMixer.GetFloat(expoParam, out float currentValue))
            {
                // Throw error if this fails
                if (!removeWarningMsgs) Debug.LogWarning("Error, failed to get current value for " + param);
                return;
            }

            // Convert and clamp (linear values should be between 0 and 1)
            currentValue = ConvertType(param, false, currentValue);
            targetValue = Mathf.Clamp01(value ?? ConvertType(param, false, targetValue));

            // Finally, kick off a coroutine that fades the value
            Coroutine fade = StartCoroutine(Fader(expoParam, (groupToUse, param), duration, currentValue, targetValue));

            // If the fade exists
            if (fade != null)
            {
                // Store the coroutine
                activeFades[(groupToUse, param)] = fade;
            }
        }
    }

    public void ForceSetParam(GROUP_OPTIONS collection, EX_PARA param, float? value = null)
    {
        if (CheatLogic.cheatTool.musicAndSoundForceOff) return;

        // We will force one group (or collection of groups) to cancel all coroutines and
        // be set to "value" for one of the exposed parameter types
        foreach (AudioMixerGroup group in mainGroups.GroupOptionToArray(collection))
        {
            // Find the group from our group info array that matches the group from the collection
            MixerGroupsInfo groupToSet = Array.Find(groupInfo, y => y.group.name == group.name);

            // Error check
            if (groupToSet == null)
            {
                if (!removeWarningMsgs) Debug.LogWarning("Error, failed to find group from info array!");
            }
            else
            {
                // If the paramater is fading
                if (activeFades.TryGetValue((groupToSet, param), out Coroutine exists))
                {
                    // Error Check
                    if (exists == null)
                    {
                        if (!removeWarningMsgs) Debug.LogWarning("Error, existing coroutine is null!");
                    }
                    else
                    {
                        // Stop the coroutine and remove it from active fades
                        StopCoroutine(exists);
                        activeFades.Remove((groupToSet, param));
                    }
                }

                // "value" is either the starting value of the parameter if nothing was passed
                float targetValue = GetExposedParams(groupToSet, param).Item2;
                targetValue = Mathf.Clamp01(value ?? ConvertType(param, false, targetValue));

                // Get the exposed parameter too
                string exParam = GetExposedParams(groupToSet, param).Item1;
                
                // Set the value of the exposed parameter to target
                if (!audioMixer.SetFloat(exParam, ConvertType(param, true, targetValue)))
                {
                    // Throw error if this fails
                    if (!removeWarningMsgs) Debug.LogWarning("Error, failed to set target value for " + param);
                }
            }
        }

        if (Msg) Debug.Log("Force set: " + collection);
        if (Msg) Debug.Log("Parameter type: " + param);
        if (Msg) Debug.Log("Target value: " + (value.HasValue ? value : "default"));
    }

    IEnumerator Fader(string exposedParam, (MixerGroupsInfo, EX_PARA) key, float duration, float current, float target)
    {
        if (Msg) Debug.Log("Fading " + exposedParam + ", " + key.Item2 + " over duration " + duration + ".");
        if (Msg) Debug.Log("Current value (linear) is " + current + ".");
        if (Msg) Debug.Log("Target value (linear) is " + target + ".");

        float elapsed = 0f;

        // If the duration is zero, or the current value is the same as the target
        if ((duration <= 0) || Mathf.Approximately(current, target))
        {
            // Set the value of the exposed parameter to target
            if (!audioMixer.SetFloat(exposedParam, ConvertType(key.Item2, true, target)))
            {
                // Throw error if this fails
                if (!removeWarningMsgs) Debug.LogWarning("Error, failed to set target value for " + exposedParam);
            }

            // Remove this coroutine from active routines
            activeFades.Remove(key);

            // We don't need to iterate
            yield break;
        }

        // While 
        while (elapsed < duration)
        {
            // Add time of frame
            elapsed += Time.deltaTime;

            // Find the next value for the exposed parameter
            float completePercentage = Mathf.Clamp01(elapsed / duration);
            float nextValue = Mathf.Lerp(current, target, completePercentage);

            // Set the value of the exposed parameter to target, converting it to the correct type
            if (!audioMixer.SetFloat(exposedParam, ConvertType(key.Item2, true, nextValue)))
            {
                // Throw error if this fails
                if (!removeWarningMsgs) Debug.LogWarning("Error, failed to set target value for " + exposedParam);
            }

            // Repeat each frame
            yield return null;
        }

        // If the elapsed time is greater than or equal to the duration
        if (elapsed >= duration)
        {
            // Set the value of the exposed parameter to target
            if (!audioMixer.SetFloat(exposedParam, ConvertType(key.Item2, true, target)))
            {
                // Throw error if this fails
                if (!removeWarningMsgs) Debug.LogWarning("Error, failed to set target value for " + exposedParam);
            }
        }

        // Remove this coroutine from active routines
        activeFades.Remove(key);
    }

    (string, float) GetExposedParams(MixerGroupsInfo group, EX_PARA type)
    {
        // Attempt to get the exposed parameter and default targetvalue (starting value), determined by the type of parameter
        switch (type)
        {
            case EX_PARA.VOLUME: return (group.parameters.volume, group.parameters.startVolume);
            case EX_PARA.LOW_PASS_EQ: return (group.parameters.lowPassEQ, group.parameters.startLowPassEQ);
            case EX_PARA.HIGH_PASS: return (group.parameters.highPassEQ, group.parameters.startHighPassEQ);
            case EX_PARA.PITCH_SHIFT: return (group.parameters.pitchShift, group.parameters.startPitchShift);

            // ===== (EX_PARA SET: Add in extra parameters if added to!) =====
            default:
                if (!removeWarningMsgs) Debug.LogWarning("Error, errrm.. okay? Somehow incorrect enum passed: " + type);
                return (null, 0f);
        }
    }

    float ConvertType(EX_PARA type, bool toType, float inputValue)
    {
        // Return value depending on type
        switch (type)
        {
            case EX_PARA.VOLUME:
                // If we want to convert to type
                if (toType)
                {
                    // Convert the linear inputted volume to dB which uses a logarithmic scale
                    // If input is less than or equal to 0 change to minimum value (-80dB = Mathf.Log10(0.0001f) * 20)
                    return Mathf.Log10(inputValue <= 0 ? 0.0001f : inputValue) * 20f;
                }
                // Otherwise, we want to convert from type
                else
                {
                    // Convert the volume in dB to use a linear scale
                    // (the inverse of converting to a logarithmic scale)
                    return Mathf.Pow(10f, inputValue / 20f);
                }
            case EX_PARA.LOW_PASS_EQ:
                // If we want to convert to type
                if (toType)
                {
                    return 0f;
                }
                // Otherwise, we want to convert from type
                else
                {
                    return 0f;
                }
            case EX_PARA.HIGH_PASS:
                // If we want to convert to type
                if (toType)
                {
                    // Convert the linear 0-1 scale to a log 10hz to 22000hz scale
                    float logFreq = Mathf.Lerp(Mathf.Log10(10), Mathf.Log10(22000), inputValue);
                    return Mathf.Pow(10f, logFreq);
                }
                // Otherwise, we want to convert from type
                else
                {
                    // Convert the log 10hz to 22000hz scale, to a linear 0-1 scale
                    return Mathf.InverseLerp(Mathf.Log10(10), Mathf.Log10(22000), Mathf.Log10(inputValue));
                }
            case EX_PARA.PITCH_SHIFT:
                // If we want to convert to type
                if (toType)
                {
                    // Convert the linear 0-1 to a range of 0.5 to 2.0
                    float newValue = Mathf.Lerp(0.5f, 2.0f, inputValue);
                    return newValue;
                }
                // Otherwise, we want to convert from type
                else
                {
                    // Convert the range of 0.5 to 2.0 to a linear 0-1
                    float newValue = (inputValue - 0.5f) / (1.50f);
                    return newValue;
                }

            // ===== (EX_PARA SET: Add in extra parameters if added to!) =====
            default:
                return 0f;
        }
    }

    void SetVolumeLevel(string exParam, float level)
    {
        if (CheatLogic.cheatTool.musicAndSoundForceOff) return;

        // Clamp the desired level between 0.0001 and 1
        level = ConvertType(EX_PARA.VOLUME, true, Mathf.Clamp(level, 0.0001f, 1f));

        // Set the volume on the mixer
        if (!audioMixer.SetFloat(exParam, level))
        {
            // Throw error if this fails
            if (!removeWarningMsgs) Debug.LogWarning("Error, failed to set target value for " + exParam);
        }
    }

    public void SetPlayerMaster(float level) => SetVolumeLevel(exParaMasterPlayer, level);
    public void SetPlayerMusic(float level) => SetVolumeLevel(exParaMusicPlayer, level);
    public void SetPlayerSfx(float level) => SetVolumeLevel(exParaSfxPlayer, level); 

    // The MixerFXManager functions affect the various channels in the audio mixer.
    // They change the pre-master volume and can apply FX to the channels.

    //Functions:
    //  Functions for development.
    //    - InitialiseMixer()                       Called once by the audio manager for setup
    //    - SetMusicParam(string name, EX_PARA param, float duration, float? value = null)
    //                                              The function takes a name of the music track, type of effect to change,
    //                                              duration of change, and a target value for the effect parameter. It 
    //                                              will alter the effect for the track that is in use
    //    - SetMusicOverallParam(EX_PARA param, float duration, float? value = null)
    //                                              Same as SetMusic Param but for the music overall
    //    - SetLoopingSFXParam(string name, EX_PARA param, float duration, float? value = null)
    //                                              Same as for music but for looping sfx
    //    - SetSfxOverallParam(EX_PARA param, float duration, float? value = null)
    //                                              Same as SetMusic Param but for the sfx overall
    //    - ForceSetParam(GROUP_OPTIONS collection, EX_PARA param, float? value = null)
    //                                              Will force set a collection memeber/overall group effect without fading
    //    - Fader(string exposedParam, (MixerGroupsInfo, EX_PARA) key, float duration, float current, float target)
    //                                              Start a coroutine for the fading of an effect. The coroutine is assigned
    //                                              a key for identification and has a duration, starting value (current)
    //                                              and target value
    //    - GetExposedParams(MixerGroupsInfo group, EX_PARA type)
    //                                              Gets the name and value of a type of exposed paramter from a group
    //    - ConvertType(EX_PARA type, bool toType, float inputValue)
    //                                              Converts a value of a exposed parameter into a linear form in to 
    //                                              a custom form or vice versa
    //  Functions for player.
    //    - SetVolumeLevel(string exParam, float level)
    //                                              Takes an exposed volume parameter and sets a value to it
    //    - SetPlayerMaster(float level)            Adjusts the master volume
    //    - SetPlayerMusic(float level)             Adjusts the music volume
    //    - SetPlayerSfx(float level)               Adjusts the sfx volume
}
