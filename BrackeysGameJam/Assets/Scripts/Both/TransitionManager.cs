using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] private string awakeSceneName = "Main_AwakeScene";

    public static TransitionManager instance;

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
        TransitionAnimControl.onBlinkMiddle += LoadAwakeScene;
        TransitionAnimControl.instance.StartBlinkTransition();
    }

    private void LoadAwakeScene()
    {
        TransitionAnimControl.onBlinkMiddle -= LoadAwakeScene;
        SceneManager.LoadScene(awakeSceneName);
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }
}
