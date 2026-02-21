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
    public Maze.WallDirection wallFace;
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
        foreach (var buttonInfo in buttonsToSpawn)
        {
            int row = buttonInfo.startNodeIndex / maze.size;
            int col = buttonInfo.startNodeIndex % maze.size;

            Vector3 offsetVector = maze.getOnWallFaceOffset(buttonInfo.wallFace);
            Vector3 rotationVector = maze.getOnWallFaceRotation(buttonInfo.wallFace);

            GameObject newButton = Instantiate(buttonGameObject,
                new Vector3(col * maze.scale.x, 0, -row * maze.scale.z) +
                offsetVector - Vector3.Scale(offsetVector, new Vector3(0.02f, 0, 0.02f)),
                Quaternion.Euler(rotationVector), transform);

            AsleepInteractable interactableScript = newButton.GetComponent<AsleepInteractable>();

            interactableScript.buttonInfo.ButtonID = buttonInfo.id;
            interactableScript.buttonInfo.ButtonMeshRenderer.material = buttonInfo.material;
        }
    }
}
