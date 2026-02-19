using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AllSoundsController : MonoBehaviour
{
    const float musicStartTimeMin = 0.5f;
    const float musicStartTimeMax = 50f;

    [SerializeField] float footStepFrequencyBedroom;
    [SerializeField] float footStepFrequencyDream;
    [SerializeField] float randomWhisperFrequencyBedroom;
    [SerializeField] float randomWhisperFrequencyDream;
    [SerializeField][Range(musicStartTimeMin, musicStartTimeMax)] float musicStartTimeMaxForRandom = 0.5f;
    public SceneName currentScene;
    bool walking = false;
    float timeWalking = 0;
    float timeWhispers = 0;

    private void OnEnable()
    {
        TransitionManager.onLoadingNextScene += NewScene;
        //+= StartWalking;
        //+= StopWalking;
    }

    private void OnDisable()
    {
        TransitionManager.onLoadingNextScene -= NewScene;
        //+= StartWalking;
        //+= StopWalking;
    }

    private void Start()
    {
        double randomStartTime = AudioSettings.dspTime + Random.Range(musicStartTimeMin, musicStartTimeMaxForRandom);

        //  Mute the music collection then play all tracks
        MixerFXManager.instance.ForceSetParam(GROUP_OPTIONS.MUSIC_OVERALL, EX_PARA.VOLUME, 0);

        // BedRoom
        AudioManager.instance.PlayMusic("BChoir", randomStartTime);
        AudioManager.instance.PlayMusic("BDeepChords", randomStartTime);
        AudioManager.instance.PlayMusic("BMusicBox", randomStartTime);
        AudioManager.instance.PlayMusic("BPianoSFX", randomStartTime);

        // Main
        AudioManager.instance.PlayMusic("MPianoSFX", randomStartTime);
        AudioManager.instance.PlayMusic("MMusicBoxAndGong", randomStartTime);
        AudioManager.instance.PlayMusic("MChords", randomStartTime);

        // Victory
        AudioManager.instance.PlayMusic("WinMusic", randomStartTime);

        // Mute all individual Music tracks
        MixerFXManager.instance.ForceSetParam(GROUP_OPTIONS.MUSIC_COLLECTION, EX_PARA.VOLUME, 0);

        // Kick off ambience
        PlayAmbience();
    }

    private void Update()
    {
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
                PlayRandomWhisper();
                break;

            default:
                // Don't try to play whispers
                break;
        }

        timeWhispers += Time.deltaTime;
        if (currentScene == SceneName.AWAKEBEGINNING)
        {
            if (timeWalking > footStepFrequencyBedroom)
            {
                timeWalking = 0;
                //AudioManager.instance.PlaySFX("")
            }
        }
        else
        {
            if (timeWalking > footStepFrequencyDream)
            {
                timeWalking = 0;
            }
        }
    }

    void NewScene(SceneName name)
    {
        currentScene = name;

        // Use PlayAmbience()
        // Use FullReset() with if statements
    }

    void FullReset()
    {
        // Reset all volumes and SFX
    }

    void PlayAmbience()
    {
        //switch (currentScene)
        //{
        //    case SceneNames.AWAKE:
        //        break;
        //    case SceneNames.MAZE1:
        //        break;
        //    case SceneNames.MAZE2:
        //        break;
        //    case SceneNames.MAZE3:
        //        break;
        //    case SceneNames.MAZE4:
        //        break;
        //    case SceneNames.MAZE5:
        //        break;
        //    default:
        //        break;
        //} TODO: this


    }

    void StartWalking()
    {
        walking = true;

        if (currentScene == SceneName.AWAKEBEGINNING)
        {
            if (timeWalking > footStepFrequencyBedroom / 2)
            {
                //AudioManager.instance.PlaySFX("")
            }
        }
        else
        {
            if (timeWalking > footStepFrequencyDream / 2)
            {
                //AudioManager.instance.PlaySFX("")
            }
        }
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
                    //AudioManager.instance.PlaySFX("")
                }
            }
            else
            {
                if (timeWalking > footStepFrequencyDream)
                {
                    timeWalking = 0;
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
                playWhisper = (Random.Range(0, randomWhisperFrequencyBedroom) < timeWhispers);
                break;
            case SceneName.MAZE1:
            case SceneName.MAZE2:
            case SceneName.MAZE3:
            case SceneName.MAZE4:
            case SceneName.MAZE5:
                playWhisper = (Random.Range(0, randomWhisperFrequencyDream) < timeWhispers);
                break;
            default:
                Debug.LogWarning("Errror, shouldn't be able to get here!");
                break;
        }

        // If we want to play a whisper
        if (playWhisper)
        {

        }
        // pick whisper to play
    }
}
