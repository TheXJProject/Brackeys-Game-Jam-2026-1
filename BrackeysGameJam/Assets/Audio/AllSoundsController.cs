using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllSoundsController : MonoBehaviour
{
    const float newEnemy = -1;
    const float maxWalkFreq = 3f;
    const float minWalkFreq = 0.001f;

    public static AllSoundsController instance;

    public SceneName startScene;
    public AudioClip buttonPress;
    public AudioClip doorLocked;
    public AudioClip doorOpens;
    public AudioClip spottedClip;
    public AudioClip pickUpKey;
    public AudioClip unlockDoor;
    public AudioClip catchDreamon;
    [SerializeField] string[] whispers;
    [SerializeField][Range(minWalkFreq, maxWalkFreq)] float footStepFrequencyBedroom;
    [SerializeField][Range(minWalkFreq, maxWalkFreq)] float footStepFrequencyDream;
    [SerializeField] float randomWhisperFrequencyBedroom;
    [SerializeField] float randomWhisperFrequencyDream;
    [SerializeField] float minWhisperTime;
    [SerializeField] float timeBetweenBeingSpotted;
    [SerializeField] double musicStartTime = 0.5f;
    [SerializeField] float fadeInTime = 1;
    [SerializeField] float fadeOutTime = 1;
    [SerializeField] float quickFadeOutTime = 1;
    [SerializeField] float heartbeatTimeUntilFadeOut = 1;
    SceneName currentScene;
    bool walking = false;
    float timeWalking = 0f;
    float timeWhispers = 0f;
    private Dictionary<int, float> dreamonSpottedTimes = new();
    float heartBeatTime = 0f;
    bool playingHeartBeat = false;

    private void OnEnable()
    {
        TransitionManager.onLoadingNextScene += NewScene;
        TransitionManager.onBeginFadeOut += FadeOut;
        AsleepEnemy.onPlayerSeen += EnemySeenPlayer;
        AwakePlayerControl.onPlayer2DStartedMoving += StartWalking;
        AwakePlayerControl.onPlayer2DStoppedMoving += StopWalking;
        AsleepPlayerControl.onPlayer3DStartedMoving += StartWalking;
        AsleepPlayerControl.onPlayer3DStoppedMoving += StopWalking;
        StartGame2.beginPressed += StartScreenEnterGame;
        AsleepLucidControl.onLucidToggled += LucidMode;
        AwakeEndAnimThenNextThing.onLossFadeScreenStarted += StartDeathSequence;
        AwakeEndAnimThenNextThing.onWinFadeScreenStarted += WinFadeFirstStep;
        AwakeEndAnimThenNextThing.onLossScreenShown += FadeToDeathScreen;
        AwakeEndAnimThenNextThing.onWinScreenShown += FadeToWinScreen;
        AsleepLucidControl.onLucidStarted += EnterExitLucid;
        AsleepLucidControl.onLucidEnded += EnterExitLucid;
        AsleepTrap.onEnemyTrapped += EnemyTrappedSounds;
    }

    private void OnDisable()
    {
        TransitionManager.onLoadingNextScene -= NewScene;
        TransitionManager.onBeginFadeOut -= FadeOut;
        AsleepEnemy.onPlayerSeen -= EnemySeenPlayer;
        AwakePlayerControl.onPlayer2DStartedMoving -= StartWalking;
        AwakePlayerControl.onPlayer2DStoppedMoving -= StopWalking;
        AsleepPlayerControl.onPlayer3DStartedMoving -= StartWalking;
        AsleepPlayerControl.onPlayer3DStoppedMoving -= StopWalking;
        StartGame2.beginPressed -= StartScreenEnterGame;
        AsleepLucidControl.onLucidToggled -= LucidMode;
        AwakeEndAnimThenNextThing.onLossFadeScreenStarted -= StartDeathSequence;
        AwakeEndAnimThenNextThing.onWinFadeScreenStarted -= WinFadeFirstStep;
        AwakeEndAnimThenNextThing.onLossScreenShown -= FadeToDeathScreen;
        AwakeEndAnimThenNextThing.onWinScreenShown -= FadeToWinScreen;
        AsleepLucidControl.onLucidStarted += EnterExitLucid;
        AsleepLucidControl.onLucidEnded += EnterExitLucid;
        AsleepTrap.onEnemyTrapped -= EnemyTrappedSounds;
    }

    private void Awake()
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

        // WALKING
        if (timeWalking < maxWalkFreq)
        {
            timeWalking += Time.deltaTime;
        }

        // HEARTBEAT
        if (heartBeatTime > 0)
        {
            heartBeatTime -= Time.deltaTime;
        }
        else if (playingHeartBeat)
        {
            MixerFXManager.instance.SetLoopingSFXParam("RacingHeartbeat", EX_PARA.VOLUME, heartbeatTimeUntilFadeOut, 0f);
            playingHeartBeat = false;
        }
    }

    void NewScene(SceneName name)
    {
        currentScene = name;
        Debug.LogWarning("Now in: " + name);

        // If we need to mute sounds before we go into the new scene
        switch (currentScene)
        {
            case SceneName.WON:
            case SceneName.LOST:
            case SceneName.MAZE1:
            case SceneName.MAZE2:
            case SceneName.MAZE3:
            case SceneName.MAZE4:
            case SceneName.MAZE5:
                FullResetToNothing();
                break;
            case SceneName.AWAKEPARALYZED1:
            case SceneName.AWAKEPARALYZED2:
            case SceneName.AWAKEPARALYZED3:
            case SceneName.AWAKEPARALYZED4:
            case SceneName.AWAKEPARALYZED5:
            case SceneName.AWAKEBEGINNING:
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

        // No PitchShift
        MixerFXManager.instance.ForceSetParam(GROUP_OPTIONS.SFX_OVERALL, EX_PARA.PITCH_SHIFT);
        MixerFXManager.instance.ForceSetParam(GROUP_OPTIONS.MUSIC_OVERALL, EX_PARA.PITCH_SHIFT);

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
                PlayButCheck("ElectricHum", 0.6f);
                PlayButCheck("WindOutside");
                break;

            case SceneName.AWAKEPARALYZED4:
            case SceneName.AWAKEPARALYZED3:
                PlayButCheck("ScratchingNails");
                goto case SceneName.AWAKEPARALYZED1;
            case SceneName.AWAKEPARALYZED5:
            case SceneName.AWAKEPARALYZED2:
            case SceneName.AWAKEPARALYZED1:
                PlayButCheck("ElectricHum");
                PlayButCheck("FloorCreaking");
                PlayButCheck("WindOutside");
                break;

            case SceneName.MAZE5:
            case SceneName.MAZE4:
            case SceneName.MAZE3:
                // play general whispers
                PlayButCheck("GeneralWhispers");
                goto case SceneName.MAZE1;
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
                PlayButCheck("WindOutside");
                break;

            default:
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
        Debug.Log("Fadein" + currentScene);
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

            case SceneName.AWAKEPARALYZED4:
            case SceneName.AWAKEPARALYZED3:
                MixerFXManager.instance.SetLoopingSFXParam("ScratchingNails", EX_PARA.VOLUME, fadeInTime);
                goto case SceneName.AWAKEPARALYZED1;
            case SceneName.AWAKEPARALYZED5:
            case SceneName.AWAKEPARALYZED2:
            case SceneName.AWAKEPARALYZED1:
                MixerFXManager.instance.SetLoopingSFXParam("ElectricHum", EX_PARA.VOLUME, fadeInTime);
                MixerFXManager.instance.SetLoopingSFXParam("FloorCreaking", EX_PARA.VOLUME, fadeInTime);
                MixerFXManager.instance.SetLoopingSFXParam("WindOutside", EX_PARA.VOLUME, fadeInTime);

                MixerFXManager.instance.SetMusicParam("BChoir", EX_PARA.VOLUME, fadeInTime);
                MixerFXManager.instance.SetMusicParam("BDeepChords", EX_PARA.VOLUME, fadeInTime);
                MixerFXManager.instance.SetMusicParam("BPianoSFX", EX_PARA.VOLUME, fadeInTime);
                MixerFXManager.instance.SetMusicParam("BMusicBox", EX_PARA.VOLUME, fadeInTime);
                break;

            case SceneName.MAZE1:
                MixerFXManager.instance.SetMusicParam("MChords", EX_PARA.VOLUME, fadeInTime);

                MixerFXManager.instance.SetLoopingSFXParam("Dripping", EX_PARA.VOLUME, fadeInTime);
                break;

            case SceneName.MAZE2:
                MixerFXManager.instance.SetMusicParam("MChords", EX_PARA.VOLUME, fadeInTime);
                MixerFXManager.instance.SetMusicParam("MPianoSFX", EX_PARA.VOLUME, fadeInTime);

                MixerFXManager.instance.SetLoopingSFXParam("Dripping", EX_PARA.VOLUME, fadeInTime);
                break;

            case SceneName.MAZE3:
                MixerFXManager.instance.SetMusicParam("MChords", EX_PARA.VOLUME, fadeInTime);
                MixerFXManager.instance.SetMusicParam("MPianoSFX", EX_PARA.VOLUME, fadeInTime);

                MixerFXManager.instance.SetLoopingSFXParam("GeneralWhispers", EX_PARA.VOLUME, fadeInTime, 0.2f);
                MixerFXManager.instance.SetLoopingSFXParam("Dripping", EX_PARA.VOLUME, fadeInTime);
                break;

            case SceneName.MAZE4:
                MixerFXManager.instance.SetMusicParam("MChords", EX_PARA.VOLUME, fadeInTime);
                MixerFXManager.instance.SetMusicParam("MPianoSFX", EX_PARA.VOLUME, fadeInTime);
                MixerFXManager.instance.SetMusicParam("MMusicBoxAndGong", EX_PARA.VOLUME, fadeInTime);

                MixerFXManager.instance.SetLoopingSFXParam("GeneralWhispers", EX_PARA.VOLUME, fadeInTime, 0.3f);
                MixerFXManager.instance.SetLoopingSFXParam("Dripping", EX_PARA.VOLUME, fadeInTime);
                break;

            case SceneName.MAZE5:
                MixerFXManager.instance.SetMusicParam("MPianoSFX", EX_PARA.VOLUME, fadeInTime);
                MixerFXManager.instance.SetMusicParam("MMusicBoxAndGong", EX_PARA.VOLUME, fadeInTime);

                MixerFXManager.instance.SetLoopingSFXParam("GeneralWhispers", EX_PARA.VOLUME, fadeInTime, 0.4f);
                MixerFXManager.instance.SetLoopingSFXParam("Dripping", EX_PARA.VOLUME, fadeInTime);
                break;

            case SceneName.LOST:
                // Don't fade in anything initially on lost
                // Play the loss SFX music
                AudioManager.instance.PlaySFX("DeathMusic", true);
                break;

            case SceneName.WON:
                MixerFXManager.instance.SetMusicParam("WinMusic", EX_PARA.VOLUME, fadeInTime);

                MixerFXManager.instance.SetLoopingSFXParam("GeneralWhispers", EX_PARA.VOLUME, fadeInTime, 0.3f);
                MixerFXManager.instance.SetLoopingSFXParam("ElectricHum", EX_PARA.VOLUME, fadeInTime, 0.3f);
                MixerFXManager.instance.SetLoopingSFXParam("WindOutside", EX_PARA.VOLUME, fadeInTime, 0.3f);
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
            case SceneName.AWAKEPARALYZED1:
            case SceneName.AWAKEPARALYZED2:
            case SceneName.AWAKEPARALYZED3:
            case SceneName.AWAKEPARALYZED4:
            case SceneName.AWAKEPARALYZED5:
                MixerFXManager.instance.SetMusicOverallParam(EX_PARA.VOLUME, fadeOutTime, 0);
                MixerFXManager.instance.SetSfxOverallParam(EX_PARA.VOLUME, fadeOutTime, 0);
                break;

            case SceneName.MAZE5:
            case SceneName.MAZE4:
            case SceneName.MAZE3:
            case SceneName.MAZE2:
            case SceneName.MAZE1:
                MixerFXManager.instance.SetMusicOverallParam(EX_PARA.VOLUME, quickFadeOutTime, 0);
                MixerFXManager.instance.SetSfxOverallParam(EX_PARA.VOLUME, quickFadeOutTime, 0);
                break;

            case SceneName.LOST:
                MixerFXManager.instance.SetMusicParam("BDeepChords", EX_PARA.VOLUME, fadeOutTime);
                MixerFXManager.instance.SetMusicParam("BPianoSFX", EX_PARA.VOLUME, fadeOutTime);
                break;
            case SceneName.WON:
                MixerFXManager.instance.SetMusicParam("WinMusic", EX_PARA.VOLUME, fadeOutTime);
                break;
            default:
                Debug.LogWarning("Error, couldn't find scene!");
                break;
        }
    }

    // ++++++++ Unique functionality +++++++++

    void StartScreenEnterGame(float time)
    {
        // Remove music box
        MixerFXManager.instance.SetMusicParam("BMusicBox", EX_PARA.VOLUME, time, 0f);
        MixerFXManager.instance.SetMusicParam("BChoir", EX_PARA.VOLUME, time, 0.5f);

        // Remove general whispers
        MixerFXManager.instance.SetLoopingSFXParam("GeneralWhispers", EX_PARA.VOLUME, time, 0f);

        // Add room ambience
        MixerFXManager.instance.SetLoopingSFXParam("ElectricHum", EX_PARA.VOLUME, time);
        MixerFXManager.instance.SetLoopingSFXParam("WindOutside", EX_PARA.VOLUME, time);
    }

    void FadeToDeathScreen()
    {
        MixerFXManager.instance.SetMusicParam("BDeepChords", EX_PARA.VOLUME, fadeInTime);
        MixerFXManager.instance.SetMusicParam("BPianoSFX", EX_PARA.VOLUME, fadeInTime);

        MixerFXManager.instance.SetLoopingSFXParam("GeneralWhispers", EX_PARA.VOLUME, fadeInTime);
    }

    void FadeToWinScreen()
    {
        MixerFXManager.instance.SetLoopingSFXParam("GeneralWhispers", EX_PARA.VOLUME, fadeInTime, 0f);
    }

    void StartDeathSequence()
    {
        Debug.Log("LossFadeFirst");
        // TODO: this

        // Wait set amount of time

        // Play scream SFX
    }

    void WinFadeFirstStep()
    {
        Debug.Log("WinFadeFirst");
        MixerFXManager.instance.SetLoopingSFXParam("ElectricHum", EX_PARA.VOLUME, fadeInTime, 0f);
        MixerFXManager.instance.SetLoopingSFXParam("WindOutside", EX_PARA.VOLUME, fadeInTime, 0f);
    }

    void StartWalking()
    {
        walking = true;

        if (currentScene == SceneName.AWAKEBEGINNING)
        {
            if (timeWalking > footStepFrequencyBedroom / 4f)
            {
                timeWalking = 0;
                AudioManager.instance.PlaySFX("SingleFootstepLight", false, null, true);
            }
        }
        else
        {
            if (timeWalking > footStepFrequencyDream / 4f)
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
            AudioManager.instance.PlaySFX(whispers[index], false, 0.3f);
        }
    }

    void EnemySeenPlayer(int enemy, AudioSource source)
    {
        Debug.Log("Enemyspots" + enemy);
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
            Debug.Log("Enemy screams" + enemy + spottedClip);
            dreamonSpottedTimes[enemy] = Time.time;
            source.Play();
            if (!playingHeartBeat)
            {
                MixerFXManager.instance.SetLoopingSFXParam("RacingHeartbeat", EX_PARA.VOLUME, fadeInTime);
                heartBeatTime += heartbeatTimeUntilFadeOut;
                playingHeartBeat = true;
            }
        }
    }

    void LucidMode(bool inLucid)
    {
        if (inLucid)
        {
            MixerFXManager.instance.SetMusicOverallParam(EX_PARA.PITCH_SHIFT, 0.2f, 0f);
            MixerFXManager.instance.SetSfxOverallParam(EX_PARA.PITCH_SHIFT, 0.2f, 0f);
        }
        else
        {
            MixerFXManager.instance.SetMusicOverallParam(EX_PARA.PITCH_SHIFT, 0.2f);
            MixerFXManager.instance.SetSfxOverallParam(EX_PARA.PITCH_SHIFT, 0.2f);
        }
    }

    void EnterExitLucid()
    {
        AudioManager.instance.PlaySFX("SingleHeartbeat");
    }

    void EnemyTrappedSounds(GameObject dreamon)
    {
        // Find the audiosources for rumbling
        AudioSource source = Array.Find(dreamon.GetComponents<AudioSource>(), source => source.clip != null && source.clip.name == "Rumbling");
        AudioSource source2 = Array.Find(dreamon.GetComponents<AudioSource>(), source => source.clip != null && source.clip.name == "Rasping");

        if ((source != null) && (source2 != null))
        {
            source.Stop();
            source2.Stop();
            source2.clip = catchDreamon;
            source2.loop = false;
            source2.Play();  
        }
        else
        {
            Debug.LogWarning("Error, couldn't find both sources!");
        }
    }
}