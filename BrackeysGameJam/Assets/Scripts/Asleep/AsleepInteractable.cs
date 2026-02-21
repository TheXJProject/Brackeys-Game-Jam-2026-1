using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Button
{
    public int ButtonID;
}

public class AsleepInteractable : MonoBehaviour
{
    public delegate void OnPuzzlePieceAdded();
    public static OnPuzzlePieceAdded onPuzzlePieceAdded;

    public static event Action onLevelCollectablePickedUp;
    public static event Action onPuzzleSolved;
    public static event Action<int> onButtonPressed;
    public static event Action<AudioSource> onButtonPressedAudio;
    public static event Action<AudioSource> onLockedDoorTriedAudio;
    public static event Action<AudioSource> onDoorOpenedAudio;
    public static event Action onKeyCollectedAudio;

    public enum InteractableType
    {
        COLLECTABLE,
        BUTTON,
        OPENABLE,
        LOCKEDOPENABLE
    }

    [SerializeField] Button buttonInfo;
    private int numberOfPuzzlesToSolve = 0;
    private int numberOfPuzzlesSolved = 0;

    public InteractableType interactType = InteractableType.COLLECTABLE;
    private readonly string[] interactActionName = { "Pick up", "Press", "Open", "LOCKED" };


    private void OnEnable()
    {
        AsleepInteractable.onLevelCollectablePickedUp += Unlock;
        AsleepButtonManager.onButtonSequenceSolved += Unlock;
        AsleepInteractable.onPuzzlePieceAdded += AddToPuzzlesToUnlock;
        AsleepTrapManager.onAllTrapsActivated += Unlock;
    }

    private void OnDisable()
    {
        AsleepInteractable.onLevelCollectablePickedUp -= Unlock;
        AsleepButtonManager.onButtonSequenceSolved -= Unlock;
        AsleepInteractable.onPuzzlePieceAdded -= AddToPuzzlesToUnlock;
        AsleepTrapManager.onAllTrapsActivated -= Unlock;
    }

    private void Start()
    {
        if (interactType == InteractableType.COLLECTABLE) AsleepInteractable.onPuzzlePieceAdded?.Invoke();

        // Yeah thats right, look at my unreasonably long if-statement and weep
        if (interactType == InteractableType.LOCKEDOPENABLE && TransitionManager.instance.wokeUpThisGame && TransitionManager.instance.IsThisFirstLevel()) 
            interactType = InteractableType.OPENABLE;
    }

    public void Interact()
    {
        switch (interactType)
        {
            case InteractableType.COLLECTABLE:
                Collect();
                break;
            case InteractableType.BUTTON:
                Press();
                break;
            case InteractableType.OPENABLE:
                Open();
                break;
            case InteractableType.LOCKEDOPENABLE:
                LockedDoorTried();
                break;
            default:
                break;
        }
    }

    public string GetInteractText()
    {
        return interactActionName[(int)interactType];
    }

    private void Press()
    {
        onButtonPressed?.Invoke(buttonInfo.ButtonID);
        onButtonPressedAudio?.Invoke(GetComponent<AudioSource>());
    }

    private void Collect()
    {
        onKeyCollectedAudio?.Invoke();
        onLevelCollectablePickedUp?.Invoke();
        gameObject.SetActive(false);
    }

    private void AddToPuzzlesToUnlock()
    {
        ++numberOfPuzzlesToSolve;
    }

    private void LockedDoorTried()
    {
        onLockedDoorTriedAudio?.Invoke(GetComponent<AudioSource>());
    }

    private void Unlock()
    {
        if (interactType == InteractableType.LOCKEDOPENABLE)
        {
            onPuzzleSolved?.Invoke();
            if (numberOfPuzzlesToSolve == ++numberOfPuzzlesSolved)
                interactType = InteractableType.OPENABLE;
        }
    }

    private void Open()
    {
        onDoorOpenedAudio?.Invoke(GetComponent<AudioSource>());
        if (TransitionManager.instance.IsThisLastLevel())
            TransitionManager.instance.LoadVictoryLevel();
        else
            TransitionManager.instance.LoadNextSleepLevel();
    }
}
