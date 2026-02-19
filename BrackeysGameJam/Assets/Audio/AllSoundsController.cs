using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AllSoundsController : MonoBehaviour
{
    [SerializeField] string[] whispers;
    [SerializeField] float footStepFrequencyBedroom;
    [SerializeField] float footStepFrequencyDream;
    [SerializeField] float randomWhisperFrequencyBedroom;
    [SerializeField] float randomWhisperFrequencyDream;
    [SerializeField] double musicStartTime = 0.5f;
    public SceneName currentScene;
    bool walking = false;
    float timeWalking = 0f;
    float timeWhispers = 0f;

    private void OnEnable()
    {
        TransitionManager.onLoadingNextScene += NewScene;
        //+= StartWalking;
        //+= StopWalking;
        //+= FadeOut;
    }

    private void OnDisable()
    {
        TransitionManager.onLoadingNextScene -= NewScene;
        //-= StartWalking;
        //-= StopWalking;
        //-= FadeOut;
    }

    private void Start()
    {
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

        // Kick off ambience
        PlayAmbience();
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
                PlayRandomWhisper();
                break;

            default:
                // Don't try to play whispers
                break;
        }
    }

    void NewScene(SceneName name)
    {
        currentScene = name;

        // If we need to mute everything first
        if ()
        {
            FullResetToNothing();
        }

        // Use PlayAmbience()
        // Use FullReset() with if statements
    }

    void FullResetToNothing()
    {
        // Reset all volumes and SFX
        MixerFXManager.instance.ForceSetParam(GROUP_OPTIONS.MUSIC_OVERALL, EX_PARA.VOLUME, 0);
        MixerFXManager.instance.ForceSetParam(GROUP_OPTIONS.MUSIC_COLLECTION, EX_PARA.VOLUME, 0);
        MixerFXManager.instance.ForceSetParam(GROUP_OPTIONS.SFX_OVERALL, EX_PARA.VOLUME, 0);
        MixerFXManager.instance.ForceSetParam(GROUP_OPTIONS.LOOPING_SFX, EX_PARA.VOLUME, 0);

        // Stops all SFX
        AudioManager.instance.StopAllSFX();
    }

    void FadeOut()
    {
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
            timeWhispers = 0;
            int index = Random.Range(0, whispers.Length - 1);
            AudioManager.instance.PlaySFX(whispers[index]);
        }
    }
}
