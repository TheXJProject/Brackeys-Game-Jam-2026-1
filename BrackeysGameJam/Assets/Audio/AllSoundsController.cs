using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllSoundsController : MonoBehaviour
{
    const float musicStartTimeMin = 0.5f;
    const float musicStartTimeMax = 50f;
    
    [SerializeField][Range(musicStartTimeMin, musicStartTimeMax)] float musicStartTimeMaxForRandom = 0.5f;
    public SceneNames currentScene;

    private void OnEnable()
    {
        TransitionManager.onLoadingNextScene += NewScene;
    }

    private void OnDisable()
    {
        TransitionManager.onLoadingNextScene -= NewScene;
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

    void NewScene(SceneNames name)
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
}
