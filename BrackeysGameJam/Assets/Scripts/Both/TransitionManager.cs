using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneNames
{
    AWAKE,
    MAZE1,
    MAZE2,
    MAZE3,
    MAZE4,
    MAZE5
}

public class TransitionManager : MonoBehaviour
{
    public static event Action<SceneNames> onLoadingNextScene;

    [SerializeField] private string awakeSceneName = "Main_AwakeScene";

    [Header("Order the indicies for asleep scenes in order of play")]
    [SerializeField] List<int> orderedAsleepScenesToLoad;
    private int currentDreamSceneIndex = 0;

    public static TransitionManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void WakeUpFromPinch() // TODO: LINK UP THESE FUNCTION CALLS TO DIFFERENT SCENES
    {
        onLoadingNextScene?.Invoke(SceneNames.AWAKE);
        TransitionAnimControl.onBlinkMiddle += LoadAwakeScene;
        TransitionAnimControl.instance.StartBlinkTransition();
    }
    public void FallAsleep()
    {
        SceneNames sceneName = (SceneNames)orderedAsleepScenesToLoad[currentDreamSceneIndex];
        onLoadingNextScene?.Invoke(sceneName);
        TransitionAnimControl.onBlinkMiddle += LoadCurrentAsleepScene;
        TransitionAnimControl.instance.StartBlinkTransition();
    }

    public void LoadNextSleepLevel()
    {
        SceneNames sceneName = (SceneNames)orderedAsleepScenesToLoad[currentDreamSceneIndex+1];
        onLoadingNextScene?.Invoke(sceneName);
        TransitionAnimControl.onBlinkMiddle += LoadNextAsleepScene;
        TransitionAnimControl.instance.StartBlinkTransition();
    }

    private void LoadAwakeScene()
    {
        TransitionAnimControl.onBlinkMiddle -= LoadAwakeScene;
        SceneManager.LoadScene(awakeSceneName);
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }

    private void LoadCurrentAsleepScene()
    {
        TransitionAnimControl.onBlinkMiddle -= LoadCurrentAsleepScene;
        GameManager.instance.awakeState = GameManager.AwakeState.FROMWAKEUP;
        SceneManager.LoadScene(currentDreamSceneIndex);
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }
    private void LoadNextAsleepScene()
    {
        TransitionAnimControl.onBlinkMiddle -= LoadNextAsleepScene;
        SceneManager.LoadScene(++currentDreamSceneIndex);
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }
}
