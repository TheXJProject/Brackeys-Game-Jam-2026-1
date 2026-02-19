using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName // MAKES SURE THIS ENUM MATCHES UP WITH THE ORDERING OF SCENES IN THE SCENE BUILD
{
    AWAKEBEGINNING,
    AWAKEPARALYZED1,
    AWAKEPARALYZED2,
    AWAKEPARALYZED3,
    AWAKEPARALYZED4,
    AWAKEPARALYZED5,
    MAZE1,
    MAZE2,
    MAZE3,
    MAZE4,
    MAZE5,
    LOST,
    WON
}

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance;

    public static event Action<SceneName> onLoadingNextScene;

    [Header("Order the indicies for awake scenes in order of play")]
    [SerializeField] List<int> orderedAwakeScenesToLoad;

    [Header("Order the indicies for asleep scenes in order of play")]
    [SerializeField] List<int> orderedAsleepScenesToLoad;
    private int currentLevelSceneIndex = 0;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void WakeUpFromPinch()
    {
        TransitionAnimControl.onBlinkMiddle += LoadCurrentAwakeScene;
        TransitionAnimControl.instance.StartBlinkTransition();
    }
    public void FallAsleep()
    {
        TransitionAnimControl.onBlinkMiddle += LoadCurrentAsleepScene;
        TransitionAnimControl.instance.StartBlinkTransition();
    }

    public void LoadNextSleepLevel()
    {
        TransitionAnimControl.onBlinkMiddle += LoadNextAsleepScene;
        TransitionAnimControl.instance.StartBlinkTransition();
    }

    private void LoadCurrentAwakeScene()
    {
        TransitionAnimControl.onBlinkMiddle -= LoadCurrentAwakeScene;
        SceneManager.LoadScene(currentLevelSceneIndex);
        SendStartedAwakeScene(currentLevelSceneIndex);
        ToggleMouseOn();
    }

    private void LoadCurrentAsleepScene()
    {
        TransitionAnimControl.onBlinkMiddle -= LoadCurrentAsleepScene;
        GameManager.instance.awakeState = GameManager.AwakeState.FROMWAKEUP;
        SceneManager.LoadScene(currentLevelSceneIndex);
        SendStartedAsleepScene(currentLevelSceneIndex);
        ToggleMouseOff();
    }
    private void LoadNextAsleepScene()
    {
        TransitionAnimControl.onBlinkMiddle -= LoadNextAsleepScene;
        SceneManager.LoadScene(++currentLevelSceneIndex);
        SendStartedAsleepScene(currentLevelSceneIndex);
        ToggleMouseOff();
    }

    private void SendStartedAsleepScene(int indexForAsleepScene)
    {
        SceneName sceneName = (SceneName)orderedAsleepScenesToLoad[indexForAsleepScene];
        onLoadingNextScene?.Invoke(sceneName);
    }

    private void SendStartedAwakeScene(int indexForAwakeScene)
    {
        SceneName sceneName = (SceneName)orderedAwakeScenesToLoad[indexForAwakeScene];
        onLoadingNextScene?.Invoke(sceneName);
    }

    private void ToggleMouseOff()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }
    private void ToggleMouseOn()
    {
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;

    }
}
