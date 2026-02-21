using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ButtonSpawnInfo
{
    public int startNodeIndex;
    public int id;
    public Material material;
}

public class AsleepButtonManager : MonoBehaviour
{
    public GameObject buttonGameObject;
    public List<ButtonSpawnInfo> buttonsToSpawn;

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

    public void SpawnButtons(Maze maze)
    {
        foreach (var button in buttonsToSpawn)
        {
            int row = button.startNodeIndex / maze.size;
            int col = button.startNodeIndex % maze.size;

            GameObject newButton = Instantiate(buttonGameObject,
                new Vector3(col * maze.scale.x, 0, -row * maze.scale.z), Quaternion.identity, transform);

            AsleepInteractable interactableScript = newButton.GetComponent<AsleepInteractable>();

            interactableScript.buttonInfo.ButtonID = button.id;
            interactableScript.buttonInfo.ButtonMeshRenderer.material = button.material;
        }
    }
}
