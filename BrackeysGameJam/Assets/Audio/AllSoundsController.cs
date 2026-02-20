using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllSoundsController : MonoBehaviour
{
    const float newEnemy = -1;

    public SceneName startScene;
    public AudioClip buttonPress;
    public AudioClip doorLocked;
    public AudioClip doorOpens;
    public AudioClip spottedClip;
    public AudioClip pickUpKey;
    public AudioClip unlockDoor;
    [SerializeField] string[] whispers;
    [SerializeField] float footStepFrequencyBedroom;
    [SerializeField] float footStepFrequencyDream;
    [SerializeField] float randomWhisperFrequencyBedroom;
    [SerializeField] float randomWhisperFrequencyDream;
    [SerializeField] float minWhisperTime;
    [SerializeField] float timeBetweenBeingSpotted;
    [SerializeField] double musicStartTime = 0.5f;
    [SerializeField] float fadeInTime = 1;
    //[SerializeField] float fadeOutTime = 1;
    SceneName currentScene;
    bool walking = false;
    float timeWalking = 0f;
    float timeWhispers = 0f;
    private Dictionary<int, float> dreamonSpottedTimes = new();

    private void OnEnable()
    {
        TransitionManager.onLoadingNextScene += NewScene;
        TransitionManager.onBeginFadeOut += FadeOut;
        AsleepEnemy.onPlayerSeen += EnemySeenPlayer;
        //+= StartWalking;
        //+= StopWalking;
    }

    private void OnDisable()
    {
        TransitionManager.onLoadingNextScene -= NewScene;
        TransitionManager.onBeginFadeOut -= FadeOut;
        AsleepEnemy.onPlayerSeen -= EnemySeenPlayer;
        //-= StartWalking;
        //-= StopWalking;
    }

    private void Start()
    {
        // Start game with nothing playing
        FullResetToNothing();

        // BedRoom
        AudioManager.instance.PlayMusic("BChoir", musicStartTime);
        AudioManager.instance.PlayMusic("BDeepChords", musicStartTime);
        AudioManager.instance.PlayMusic("BMusicBox", musicStartTime);
        AudioManager.instance.PlayMusic("BPianoSFX", musicStartTime);

        // Main
        AudioManager.instance.PlayMusic("MPianoSFX", musicStartTime);
        AudioManager.instance.PlayMusic("MMusicBoxAndGong", musicStartTime);
        AudioManager.instance.PlayMusic("MChords", musicStartTime);

        // Victory
        AudioManager.instance.PlayMusic("WinMusic", musicStartTime);

        // Enter the start scene
        NewScene(startScene);
    }

    private void Update()
    {
        // WHISPERS
        // If we're in any of the current scenes
        switch (currentScene)
        {
            case SceneName.AWAKEPARALYZED1:
            case SceneName.AWAKEPARALYZED2:
            case SceneName.AWAKEPARALYZED3:
            case SceneName.AWAKEPARALYZED4:
            case SceneName.AWAKEPARALYZED5:
            case SceneName.MAZE1:
            case SceneName.MAZE2:
            case SceneName.MAZE3:
            case SceneName.MAZE4:
            case SceneName.MAZE5:
                // Randomly Play Whispers
                timeWhispers += Time.deltaTime;
                if (timeWhispers > minWhisperTime)
                {
                    PlayRandomWhisper();
                }
                break;

            default:
                // Don't try to play whispers
                break;
        }
    }

    void NewScene(SceneName name)
    {
        currentScene = name;

        // If we need to mute sounds before we go into the new scene
        switch (currentScene)
        {
            case SceneName.AWAKEPARALYZED1:
            case SceneName.AWAKEPARALYZED2:
            case SceneName.AWAKEPARALYZED3:
            case SceneName.AWAKEPARALYZED4:
            case SceneName.AWAKEPARALYZED5:
            case SceneName.LOST:
            case SceneName.WON:
            case SceneName.AWAKEBEGINNING:
                FullResetToNothing();
                break;
            case SceneName.MAZE1:
            case SceneName.MAZE2:
            case SceneName.MAZE3:
            case SceneName.MAZE4:
            case SceneName.MAZE5:
            default:
                break;
        }

        // Kick off ambience
        PlayAmbience();

        // Fade in required tracks
        FadeIn();
    }

    void FullResetToNothing()
    {
        // Set every volume to zero
        MixerFXManager.instance.ForceSetParam(GROUP_OPTIONS.MUSIC_OVERALL, EX_PARA.VOLUME, 0);
        MixerFXManager.instance.ForceSetParam(GROUP_OPTIONS.MUSIC_COLLECTION, EX_PARA.VOLUME, 0);
        MixerFXManager.instance.ForceSetParam(GROUP_OPTIONS.SFX_OVERALL, EX_PARA.VOLUME, 0);
        MixerFXManager.instance.ForceSetParam(GROUP_OPTIONS.LOOPING_SFX, EX_PARA.VOLUME, 0);

        // Stops all SFX
        AudioManager.instance.StopAllSFX();
    }

    void PlayAmbience()
    {
        // Depending what scene we're in play looped SFX
        switch (currentScene)
        {
            case SceneName.AWAKEBEGINNING:
                PlayButCheck("GeneralWhispers");
                PlayButCheck("ElectricHum", 0.2f);
                PlayButCheck("WindOutside", 0.5f);
                break;

            case SceneName.AWAKEPARALYZED4:
            case SceneName.AWAKEPARALYZED3:
                PlayButCheck("Scratching N");
                goto case SceneName.AWAKEPARALYZED3;
            case SceneName.AWAKEPARALYZED5:
            case SceneName.AWAKEPARALYZED2:
            case SceneName.AWAKEPARALYZED1:
                PlayButCheck("ElectricHum");
                PlayButCheck("FloorCreaking");
                PlayButCheck("WindOutside");
                break;

            case SceneName.MAZE4:
            case SceneName.MAZE3:
                // play general whispers
                PlayButCheck("GeneralWhispers");
                goto case SceneName.MAZE5;
            case SceneName.MAZE5:
            case SceneName.MAZE2:
            case SceneName.MAZE1:
                // play ambience
                PlayButCheck("Dripping");
                PlayButCheck("RacingHeartbeat");
                break;

            case SceneName.LOST:
                PlayButCheck("GeneralWhispers");
                break;

            case SceneName.WON:
            default:
                // If we won, don't play any ambience
                break;
        }
    }

    void PlayButCheck(string name, float? volume = null)
    {
        // Find the looping source that's currently playing that sound
        SoundSource source = Array.Find(AudioManager.instance.sfxLoopingSourceList, y => y.soundName == name);

        // Return true if we're already playing the SFX loop
        if (source == null)
        {
            AudioManager.instance.PlayLoopingSFX(name, null, volume);
        }
    }

    void FadeIn()
    {
        MixerFXManager.instance.SetMusicOverallParam(EX_PARA.VOLUME, fadeInTime);
        MixerFXManager.instance.SetSfxOverallParam(EX_PARA.VOLUME, fadeInTime);

        // Different depending what new scene we're in
        switch (currentScene)
        {
            case SceneName.AWAKEBEGINNING:
                MixerFXManager.instance.SetMusicParam("BChoir", EX_PARA.VOLUME, fadeInTime);
                MixerFXManager.instance.SetMusicParam("BMusicBox", EX_PARA.VOLUME, fadeInTime + (float)musicStartTime * 2);

                MixerFXManager.instance.SetLoopingSFXParam("GeneralWhispers", EX_PARA.VOLUME, fadeInTime, 0.3f);
                break;
            case SceneName.AWAKEPARALYZED1:
                break;
            case SceneName.AWAKEPARALYZED2:
                break;
            case SceneName.AWAKEPARALYZED3:
                break;
            case SceneName.AWAKEPARALYZED4:
                break;
            case SceneName.AWAKEPARALYZED5:
                break;
            case SceneName.MAZE1:
                break;
            case SceneName.MAZE2:
                break;
            case SceneName.MAZE3:
                break;
            case SceneName.MAZE4:
                break;
            case SceneName.MAZE5:
                break;
            case SceneName.LOST:
                break;
            case SceneName.WON:
                break;
            default:
                Debug.LogWarning("Error, couldn't find scene!");
                break;
        }
    }

    void FadeOut()
    {
        // Different depending what scene we're currently in
        switch (currentScene)
        {
            case SceneName.AWAKEBEGINNING:
                break;
            case SceneName.AWAKEPARALYZED1:
                break;
            case SceneName.AWAKEPARALYZED2:
                break;
            case SceneName.AWAKEPARALYZED3:
                break;
            case SceneName.AWAKEPARALYZED4:
                break;
            case SceneName.AWAKEPARALYZED5:
                break;
            case SceneName.MAZE1:
                break;
            case SceneName.MAZE2:
                break;
            case SceneName.MAZE3:
                break;
            case SceneName.MAZE4:
                break;
            case SceneName.MAZE5:
                break;
            case SceneName.LOST:
                break;
            case SceneName.WON:
                break;
            default:
                Debug.LogWarning("Error, couldn't find scene!");
                break;
        }
    }

    // ++++++++ Unique functionality +++++++++

    void StartWalking()
    {
        walking = true;

        if (currentScene == SceneName.AWAKEBEGINNING)
        {
            if (timeWalking > footStepFrequencyBedroom / 2)
            {
                timeWalking = 0;
                AudioManager.instance.PlaySFX("SingleFootstepLight", false, null, true);
            }
        }
        else
        {
            if (timeWalking > footStepFrequencyDream / 2)
            {
                timeWalking = 0;
                AudioManager.instance.PlaySFX("SingleFootstep", false, null, true);
            }
        }
        StartCoroutine(Walking());
    }

    void StopWalking()
    {
        walking = false;
    }

    IEnumerator Walking()
    {
        while (walking)
        {
            timeWalking += Time.deltaTime;
            if (currentScene == SceneName.AWAKEBEGINNING)
            {
                if (timeWalking > footStepFrequencyBedroom)
                {
                    timeWalking = 0;
                    AudioManager.instance.PlaySFX("SingleFootstepLight", false, null, true);
                }
            }
            else
            {
                if (timeWalking > footStepFrequencyDream)
                {
                    timeWalking = 0;
                    AudioManager.instance.PlaySFX("SingleFootstep", false, null, true);
                }
            }

            yield return null;
        }
    }

    void PlayRandomWhisper()
    {
        bool playWhisper = false;
        switch (currentScene)
        {
            case SceneName.AWAKEPARALYZED1:
            case SceneName.AWAKEPARALYZED2:
            case SceneName.AWAKEPARALYZED3:
            case SceneName.AWAKEPARALYZED4:
            case SceneName.AWAKEPARALYZED5:
                playWhisper = (UnityEngine.Random.Range(0, randomWhisperFrequencyBedroom) < timeWhispers);
                break;
            case SceneName.MAZE1:
            case SceneName.MAZE2:
            case SceneName.MAZE3:
            case SceneName.MAZE4:
            case SceneName.MAZE5:
                playWhisper = (UnityEngine.Random.Range(0, randomWhisperFrequencyDream) < timeWhispers);
                break;
            default:
                Debug.LogWarning("Errror, shouldn't be able to get here!");
                break;
        }

        // If we want to play a whisper
        if (playWhisper)
        {
            timeWhispers = 0;
            int index = UnityEngine.Random.Range(0, whispers.Length - 1);
            AudioManager.instance.PlaySFX(whispers[index]);
        }
    }

    void EnemySeenPlayer(int enemy, AudioSource source)
    {
        // Add to map if needed
        if (!dreamonSpottedTimes.ContainsKey(enemy))
        {
            dreamonSpottedTimes[enemy] = newEnemy;
            source.clip = spottedClip;
        }

        float timeDifference = Time.time - dreamonSpottedTimes[enemy];
        
        // Check time
        if ((timeDifference > timeBetweenBeingSpotted) || (dreamonSpottedTimes[enemy] == newEnemy))
        {
            dreamonSpottedTimes[enemy] = Time.time;
            source.Play();
        }
    }
}
