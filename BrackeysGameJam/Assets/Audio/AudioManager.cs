using UnityEngine;
using System;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    // ==== For Debugging ====
    [SerializeField] bool Msg = false;

    // Inspector Adjustable Values:
    [Header("Prevent error messages if audio not in use")][SerializeField] bool removeWarningMsgs;

    // Initialise In Inspector:
    [Header("(Reduce Non-priority sounds to prevent clipping)\n")]
    [Range(0, 1)] [SerializeField] float nonPriorityVolume = 1;
    [Header("---- Audio Clips ----\n")]
    public Sound[] musicSounds;
    public Sound[] sfxSounds;
    public Sound[] sfxLoopedSounds;
    [Header("---- Audio Sources ----\n")]
    public SoundSource[] musicSourceList;
    public SoundSource[] sfxSourceList;
    public SoundSource[] sfxLoopingSourceList;

    // Runtime Variables:
    public static AudioManager instance;

    private void Awake()
    {
        // If we haven't already initialised an instance of the Audio manager
        if (instance == null)
        {
            // Make this instance a singleton
            DontDestroyOnLoad(gameObject);
            instance = this;

            // (Yu Gui Oh fusion solution! We make sure that both the Audio Manager
            // and Mixer Manager are present.)
            MixerFXManager mixerFXManger = GetComponent<MixerFXManager>();
            if (mixerFXManger == null)
            {
                if (!removeWarningMsgs) Debug.LogWarning("MixerFXManager component is missing!");
                if (!removeWarningMsgs) Debug.LogWarning("Please add component before running!");
            }
            else
            {
                mixerFXManger.CreateSingleton();
            }

            // From here, sets up audio sources.
            // Set up SFX audio sources:
            // For each sfx source
            foreach (SoundSource sfxSource in sfxSourceList)
            {
                // For each sfx source reset the name and set it to not looping as default
                sfxSource.soundName = SoundSource.defaultName;
                sfxSource.audioSource.loop = false;

                // For each sfx source
                foreach (SoundSource j in sfxSourceList)
                {
                    // We check for duplicate usages
                    if (sfxSource == j)
                    {
                        continue;
                    }
                    else if (sfxSource.audioSource == j.audioSource)
                    {
                        if (!removeWarningMsgs) Debug.LogWarning("Error, SFX source " + sfxSource.audioSource.name + " duplicate found!");
                    }
                }
            }

            // Set up Music audio sources:
            // For each music source
            foreach (SoundSource musicSource in musicSourceList)
            {
                // For each music source reset the name and set it to looping as default
                musicSource.soundName = SoundSource.defaultName;
                musicSource.audioSource.loop = true;

                // For each music 
                foreach (SoundSource j in musicSourceList)
                {
                    // Check for duplicates
                    if (musicSource == j)
                    {
                        continue;
                    }
                    else if (musicSource.audioSource == j.audioSource)
                    {
                        if (!removeWarningMsgs) Debug.LogWarning("Error, music source " + musicSource.audioSource.name + " duplicate found!");
                    }
                }
            }

            // Set up looping SFX audio sources:
            // For each music source
            foreach (SoundSource loopingSFXSource in sfxLoopingSourceList)
            {
                // For each music source reset the name and set it to looping as default
                loopingSFXSource.soundName = SoundSource.defaultName;
                loopingSFXSource.audioSource.loop = true;

                // For each music 
                foreach (SoundSource j in sfxLoopingSourceList)
                {
                    // Check for duplicates
                    if (loopingSFXSource == j)
                    {
                        continue;
                    }
                    else if (loopingSFXSource.audioSource == j.audioSource)
                    {
                        if (!removeWarningMsgs) Debug.LogWarning("Error, music source " + loopingSFXSource.audioSource.name + " duplicate found!");
                    }
                }
            }

            // Once set up the audio manager we can initialise the mixer
            MixerFXManager.instance.InitialiseMixer();
        }
        else
        {
            // Destroy this gameobject if another audio manager already exists (singleton)
            Destroy(gameObject);
        }
    }

    public void PlayMusic(string name, double dspTimeTillPlay)
    {
        if (CheatLogic.cheatTool.musicAndSoundForceOff) return;

        // Find the sound passed in from our list of sounds
        Sound sound = Array.Find(musicSounds, x => x.name == name);

        // Find a music source that's currently free
        SoundSource source = Array.Find(musicSourceList, y => y.soundIsSelected == false);

        // Throw error if we haven't found either the sound or a free music source
        if (sound == null)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, music sound " + name + " not found!");
        }
        else if (source == null)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, no music source available!");
        }
        else if (dspTimeTillPlay < 0.05f)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Warning, May fail to play track if time is less than 0.05!");
        }
        else
        {
            // Transfer volume, pitch, panning and the clip itself to the audio source
            source.audioSource.volume = sound.volume;
            source.audioSource.pitch = sound.pitch;
            source.audioSource.panStereo = sound.panning;
            source.audioSource.clip = sound.clip;

            // The music source is now in use
            source.soundIsSelected = true;
            source.soundName = sound.name;

            // Use PlayScheduled to play the track
            source.audioSource.PlayScheduled(dspTimeTillPlay);

            if (Msg) Debug.Log("Music Played: " + source.soundName);
            if (Msg) Debug.Log("Name Check: " + sound.clip.name);
            if (Msg) Debug.Log("Source Used: " + source.audioSource.name);
        }
    }

    public void StopMusic(string name, double? dspStopValue = null)
    {
        if (CheatLogic.cheatTool.musicAndSoundForceOff) return;

        // Find the sound passed in from our list of sounds
        Sound sound = Array.Find(musicSounds, x => x.name == name);
        
        // Find the music source that's currently playing that sound
        SoundSource source = Array.Find(musicSourceList, y => y.soundName == name);

        // Throw error if we haven't found either the sound or any source that's playing the sound
        if (sound == null)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, music sound " + name + " not found!");
        }
        else if (source == null)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, no music source found playing " + name + "!");
        }
        else
        {
            // Depending on passed value
            if (!dspStopValue.HasValue)
            {
                // Stop the track straight away
                source.audioSource.Stop();

                // The source is now not in use
                source.soundName = SoundSource.defaultName;
                source.soundIsSelected = false;
            }
            else
            {
                // Otherwise schedule the stop
                source.audioSource.SetScheduledEndTime(dspStopValue.Value);

                // Wait until the source has stopped playing before showing the manager it is available
                StartCoroutine(WaitForAudioToStop(source));
            }
        }
    }

    public void StopAllMusic()
    {
        if (CheatLogic.cheatTool.musicAndSoundForceOff) return;

        bool playingCheck = false;

        // For each music source we have
        foreach (SoundSource source in musicSourceList)
        {
            // Check if the source was playing
            playingCheck |= source.soundIsSelected;

            // Stops the track
            source.audioSource.Stop();

            // The source is now not in use
            source.soundName = SoundSource.defaultName;
            source.soundIsSelected = false;
        }

        // Throw error if no source was playing anything
        if (!playingCheck)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, no music was playing!");
        }
    }

    public void PlaySFX(string name, bool prioritySound = false, float? volumeTemp = null, bool randPitch = false, float? pitchTemp = null)
    {
        if (CheatLogic.cheatTool.musicAndSoundForceOff) return;

        // Find the sound passed in from our list of sounds
        Sound sound = Array.Find(sfxSounds, x => x.name == name);

        // Find a music source that's currently free
        SoundSource source = Array.Find(sfxSourceList, y => y.audioSource.isPlaying == false);

        // If a manual volume has been passed, Check it's within ranges
        if ((volumeTemp != null) && (volumeTemp > 1 || volumeTemp < 0))
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, volumeTemp, " + volumeTemp + ", is outside of range!");
            return;
        }

        // Throw error if the sound can't be found or if no sources exist
        if (sound == null)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, sfx sound " + name + " not found!");
        }
        else if (sfxSourceList == null || sfxSourceList.Length == 0)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, no sfx source exist!");
        }
        else
        {
            // If we couldn't find a free source and the sound is not a priority
            if ((source == null) && !prioritySound)
            {
                float tempVolume = (volumeTemp ?? sound.volume) * nonPriorityVolume;
                int randomIdx = UnityEngine.Random.Range(0, sfxSourceList.Length);

                // Find a random sfx source and play a oneshot with some volume reducion
                sfxSourceList[randomIdx].audioSource.PlayOneShot(sound.clip, tempVolume);

                if (Msg) Debug.Log("Played One Shot.");
                if (Msg) Debug.Log("Clip Name: " + sound.clip.name);
                if (Msg) Debug.Log("Source Used: " + sfxSourceList[randomIdx].audioSource.name);
                if (Msg) Debug.Log("Non Priority Final Volume: " + tempVolume);
            }
            else
            {
                // Otherwise, if we couldn't find a source and the sound should be a priority
                if ((source == null) && prioritySound)
                {
                    float shortest = float.MaxValue;
                    SoundSource tempSource = null;

                    // For each sfx source
                    foreach (SoundSource soundSource in sfxSourceList)
                    {
                        // If the clip isn't null (and it should never be if all sources are in use, but can never be too careful)
                        // Find if it is the shortest playing clip
                        if ((soundSource.audioSource.clip != null) && (soundSource.audioSource.clip.length < shortest))
                        {
                            shortest = soundSource.audioSource.clip.length;
                            tempSource = soundSource;
                        }
                    }

                    // We will use the source which is currently playing the shortest sound
                    source = tempSource;

                    if (Msg) Debug.Log("Priority used.");
                }

                float tempPitch = sound.pitch;

                // If we are setting the pitch
                if (pitchTemp.HasValue)
                {
                    tempPitch = pitchTemp.Value;
                }
                // Otherwise, if we want a random pitch
                else if (randPitch)
                {
                    // Get a random pitch from the range given
                    float minValue = MathF.Min(sound.randomPitchRange.x, sound.randomPitchRange.y);
                    float maxValue = MathF.Max(sound.randomPitchRange.x, sound.randomPitchRange.y);

                    // Get a random pitch to use
                    tempPitch = UnityEngine.Random.Range(minValue, maxValue);
                }

                // Transfer volume, pitch, panning and the clip itself to the audio source
                source.audioSource.volume = volumeTemp ?? sound.volume;
                source.audioSource.pitch = tempPitch;
                source.audioSource.panStereo = sound.panning;
                source.audioSource.clip = sound.clip;

                // The music source is now in use
                source.soundName = sound.name;

                // Player the sound (Don't need to use scheduleplay here)
                source.audioSource.Play();

                if (Msg) Debug.Log("Music Played: " + source.soundName);
                if (Msg) Debug.Log("Name Check: " + sound.clip.name);
                if (Msg) Debug.Log("Source Used: " + source.audioSource.name);
                if (Msg && (volumeTemp != null)) Debug.Log("Manual Volume: " + volumeTemp);
            }
        }
    }

    public void PlayLoopingSFX(string name, double? timeTillPlay = null, float? volumeTemp = null, bool randPitch = false, float? pitchTemp = null)
    {
        if (CheatLogic.cheatTool.musicAndSoundForceOff) return;

        // Find the sound passed in from our list of sounds
        Sound sound = Array.Find(sfxLoopedSounds, x => x.name == name);

        // Find a looping source that's currently free
        SoundSource source = Array.Find(sfxLoopingSourceList, y => y.soundIsSelected == false);

        // Throw error if we haven't found either the sound or a free music source
        if (sound == null)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, music sound " + name + " not found!");
        }
        else if (source == null)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, no music source available!");
        }
        else if (timeTillPlay < 0.05f && timeTillPlay.HasValue)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Warning, May fail to play track if time is less than 0.05!");
        }
        else
        {
            float tempPitch = sound.pitch;

            // If we are setting the pitch
            if (pitchTemp.HasValue)
            {
                tempPitch = pitchTemp.Value;
            }
            // Otherwise, if we want a random pitch
            else if (randPitch)
            {
                // Get a random pitch from the range given
                float minValue = MathF.Min(sound.randomPitchRange.x, sound.randomPitchRange.y);
                float maxValue = MathF.Max(sound.randomPitchRange.x, sound.randomPitchRange.y);

                // Get a random pitch to use
                tempPitch = UnityEngine.Random.Range(minValue, maxValue);
            }

            // Transfer volume, pitch, panning and the clip itself to the audio source
            source.audioSource.volume = volumeTemp ?? sound.volume;
            source.audioSource.pitch = tempPitch;
            source.audioSource.panStereo = sound.panning;
            source.audioSource.clip = sound.clip;

            // The music source is now in use
            source.soundIsSelected = true;
            source.soundName = sound.name;

            // If we want to set a time when the sound starts
            if (timeTillPlay.HasValue)
            {
                // Use PlayScheduled to play the track
                source.audioSource.PlayScheduled(timeTillPlay.Value);
            }
            else
            {
                // Otherwise, play the track outright
                source.audioSource.Play();
            }

            if (Msg) Debug.Log("Music Played: " + source.soundName);
            if (Msg) Debug.Log("Name Check: " + sound.clip.name);
            if (Msg) Debug.Log("Source Used: " + source.audioSource.name);
        }
    }

    public void StopLoopingSFX(string name, double? timeTillStop = null)
    {
        if (CheatLogic.cheatTool.musicAndSoundForceOff) return;

        // Find the sound passed in from our list of sounds
        Sound sound = Array.Find(sfxLoopedSounds, x => x.name == name);

        // Find the looping source that's currently playing that sound
        SoundSource source = Array.Find(sfxLoopingSourceList, y => y.soundName == name);

        // Throw error if we haven't found either the sound or any source that's playing the sound
        if (sound == null)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, music sound " + name + " not found!");
        }
        else if (source == null)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Error, no music source found playing " + name + "!");
        }
        else
        {
            // Depending on passed value
            if (!timeTillStop.HasValue)
            {
                // Stop the track straight away
                source.audioSource.Stop();

                // The source is now not in use
                source.soundName = SoundSource.defaultName;
                source.soundIsSelected = false;
            }
            else
            {
                // Otherwise schedule the stop
                source.audioSource.SetScheduledEndTime(timeTillStop.Value);

                // Wait until the source has stopped playing before showing the manager it is available
                StartCoroutine(WaitForAudioToStop(source));
            }
        }
    }

    public void StopAllSFX()
    {
        if (CheatLogic.cheatTool.musicAndSoundForceOff) return;

        bool playingCheck = true;

        // For each sfx source there is
        foreach (SoundSource source in sfxSourceList)
        {
            // Check if the source has ever been useds
            if (source.soundName != SoundSource.defaultName)
            {
                playingCheck = false;
            }

            // Stop the audiosource from playing
            source.audioSource.Stop();

            // The source is now not in use
            source.soundName = SoundSource.defaultName;
        }

        // For each looping sfx source there is
        foreach (SoundSource source in sfxLoopingSourceList)
        {
            // Check if the source has ever been useds
            if (source.soundName != SoundSource.defaultName)
            {
                playingCheck = false;
            }

            // Stop the audiosource from playing
            source.audioSource.Stop();

            // The source is now not in use
            source.soundName = SoundSource.defaultName;
            source.soundIsSelected = false;
        }

        // Throw error if no source has been used since game load or the last stopSFX() call
        if (playingCheck)
        {
            if (!removeWarningMsgs) Debug.LogWarning("Caution, no SFX have been played since start or last 'StopSFX()' call!");
        }
    }

    IEnumerator WaitForAudioToStop (SoundSource source)
    {
        // Wait for when the audio source stops playing 
        while (source.audioSource.isPlaying)
        {
            yield return null;
        }

        // Allow the source to be used again
        source.soundName = SoundSource.defaultName;
        source.soundIsSelected = false;
    }

    // Help

    // The AudioManager functions affect the basic elements of the sounds.
    // They change only the most fundamental aspects such as, whether a sound
    // is playing, stopped or it's base volume.

    //Functions:
    //  Functions for music sources.
    //    - PlayMusic(string name, double timeTillPlay)
    //                                              Plays sound with 'name' on loop
    //    - StopMusic(string name, double timeTillStop)
    //                                              Stops sound with 'name'
    //    - StopAllMusic()                          Stops all looping tracks
    //
    //  Functions for SFX sources.
    //    - PlaySFX(string name, bool prioritySound = false, float? volumeTemp = null, bool randPitch = false, float? pitchTemp = null)
    //                                              Plays sound with 'name' once at 'volumeTemp'
    //                                              (if 'volumeTemp' is null the sound will play
    //                                              at default volume set in inspecter)
    //    - PlayLoopingSFX(string name, float? volumeTemp = null, bool randPitch = false, float? pitchTemp = null)
    //                                              Plays sound with 'name' looping at 'volumeTemp'
    //                                              (if 'volumeTemp' is null the sound will play
    //                                              at default volume set in inspecter)
    //    - StoppLoopingSFX(string name, double timeTillStop)
    //                                              Stops named looping SFX
    //    - StopAllSFX()                            Stops all SFX and clears the names in the SFX sources
    //  Additional Functions.
    //    - WaitForAudioToStop (SoundSource source) Resets the source after the sound has stopped playing
}
