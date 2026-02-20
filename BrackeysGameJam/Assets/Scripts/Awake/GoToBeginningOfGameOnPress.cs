using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToBeginningOfGameOnPress : MonoBehaviour
{
    public void GoToBeginningOfGame()
    {
        TransitionManager.instance.LoadSceneRestartGame();
    }
}
