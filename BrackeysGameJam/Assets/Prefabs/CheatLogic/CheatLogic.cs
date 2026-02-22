using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatLogic : MonoBehaviour
{
    const bool forceOff = true;
    public static CheatLogic cheatTool;

    public bool musicAndSoundForceOff;
    public bool cannotDie;

    private void Awake()
    {
        // If we haven't already initialised an instance of cheat tools
        if (cheatTool == null)
        {
            // Make this instance a singleton
            DontDestroyOnLoad(gameObject);
            cheatTool = this;
        }
        else
        {
            // Otherwise, destroy this object
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!forceOff)
        {
            // If we press the up arrow turn off/on music and sfx
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                musicAndSoundForceOff = !musicAndSoundForceOff;
            }

            // If we press the up arrow turn off/on if the player can die
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                cannotDie = !cannotDie;
            }
        }
    }
}