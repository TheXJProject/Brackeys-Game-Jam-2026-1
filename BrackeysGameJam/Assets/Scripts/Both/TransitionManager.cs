using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public static event Action onSuccessPinchStartWakeUp;
    public static event Action<SceneName> onLoadingNextScene;
    public static event Action onBeginFadeOut;

    [Header("Order the indicies for awake scenes in order of play")]
    [SerializeField] List<int> orderedAwakeScenesToLoad;

    [Header("Order the indicies for asleep scenes in order of play")]
    [SerializeField] List<int> orderedAsleepScenesToLoad;
    private int currentLevelSceneIndex = 0;

    public bool gameStarted = false;
    public float time = 0;
    public float time2 = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public bool IsThisLastLevel()
    {
        if (currentLevelSceneIndex == orderedAsleepScenesToLoad.Count - 1)
            return true;
        else 
            return false;
    }

    public void WakeUpFromPinch()
    {
        TransitionAnimControl.onBlinkMiddle += LoadCurrentAwakeScene;
        TransitionAnimControl.instance.StartBlinkTransition();
        onSuccessPinchStartWakeUp?.Invoke();
        onBeginFadeOut?.Invoke();
    }
    public void FallAsleep()
    {
        TransitionAnimControl.onBlinkMiddle += LoadCurrentAsleepScene;
        TransitionAnimControl.instance.StartBlinkTransition();
        onBeginFadeOut?.Invoke();
    }

    public void LoadNextSleepLevel()
    {
        TransitionAnimControl.onBlinkMiddle += LoadNextAsleepScene;
        TransitionAnimControl.instance.StartBlinkTransition();
        onBeginFadeOut?.Invoke();
    }
    public void LoadVictoryLevel()
    {
        TransitionAnimControl.onBlinkMiddle += LoadWinningScene;
        TransitionAnimControl.instance.StartBlinkTransition();
        onBeginFadeOut?.Invoke();
    }
    public void LoadDeathLevel()
    {
        TransitionAnimControl.onBlinkMiddle += LoadLosingScene;
        TransitionAnimControl.instance.StartBlinkTransition();
        onBeginFadeOut?.Invoke();
    }
    public void LoadSceneRestartGame()
    {
        TransitionAnimControl.onBlinkMiddle += LoadOpeningScene;
        TransitionAnimControl.instance.StartBlinkTransition();
        onBeginFadeOut?.Invoke();
    }

    private void LoadCurrentAwakeScene()
    {
        TransitionAnimControl.onBlinkMiddle -= LoadCurrentAwakeScene;
        SceneManager.LoadScene(orderedAwakeScenesToLoad[currentLevelSceneIndex]);
        SendStartedAwakeScene(currentLevelSceneIndex);
        ToggleMouseOn();
    }

    private void LoadCurrentAsleepScene()
    {
        TransitionAnimControl.onBlinkMiddle -= LoadCurrentAsleepScene;
        GameManager.instance.awakeState = GameManager.AwakeState.FROMWAKEUP;
        SceneManager.LoadScene(orderedAsleepScenesToLoad[currentLevelSceneIndex]);
        SendStartedAsleepScene(currentLevelSceneIndex);
        ToggleMouseOff();
    }
    private void LoadNextAsleepScene()
    {
        TransitionAnimControl.onBlinkMiddle -= LoadNextAsleepScene;
        SceneManager.LoadScene(orderedAsleepScenesToLoad[++currentLevelSceneIndex]);
        SendStartedAsleepScene(currentLevelSceneIndex);
        ToggleMouseOff();
    }

    private void LoadOpeningScene()
    {
        TransitionAnimControl.onBlinkMiddle -= LoadOpeningScene;
        SceneManager.LoadScene((int)SceneName.AWAKEBEGINNING);
        SendStartedScene(SceneName.AWAKEBEGINNING);
        ToggleMouseOn();
    }

    private void LoadLosingScene()
    {
        TransitionAnimControl.onBlinkMiddle -= LoadLosingScene;
        SceneManager.LoadScene((int)SceneName.LOST);
        SendStartedScene(SceneName.LOST);
        ToggleMouseOn();
    }

    private void LoadWinningScene()
    {
        TransitionAnimControl.onBlinkMiddle -= LoadWinningScene;
        SceneManager.LoadScene((int)SceneName.WON);
        SendStartedScene(SceneName.WON);
        ToggleMouseOn();
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

    private void SendStartedScene(SceneName sceneName)
    {
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
