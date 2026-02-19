using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum AwakeState
    {
        BEGINNING,
        FROMWAKEUP,
        FROMDEATH,
        FROMWIN
    }

    public AwakeState awakeState = AwakeState.BEGINNING;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

}
