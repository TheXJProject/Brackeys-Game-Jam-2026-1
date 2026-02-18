using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsleepButtonManager : MonoBehaviour
{
    public static event Action onButtonSequenceSolved;

    [SerializeField] private List<int> buttonOrder;
    int curIndex = 0;

    private void Start()
    {
        AsleepInteractable.onPuzzlePieceAdded?.Invoke();
    }

    private void OnEnable()
    {
        AsleepInteractable.onButtonPressed += CheckAgainstSequence;
    }

    private void OnDisable()
    {
        AsleepInteractable.onButtonPressed -= CheckAgainstSequence;
    }

    private void CheckAgainstSequence(int buttonID)
    {
        if (buttonID == buttonOrder[curIndex])
        {
            if (++curIndex == buttonOrder.Count) onButtonSequenceSolved?.Invoke();
        }
        else
        {
            curIndex = 0;
            if (buttonID == buttonOrder[curIndex])
            {
                if (++curIndex == buttonOrder.Count) onButtonSequenceSolved?.Invoke();
            }
        }
    }
}
