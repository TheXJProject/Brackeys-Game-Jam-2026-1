using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class AsleepPlayerControl : MonoBehaviour
{
    public static event Action onPlayer3DStartedMoving;
    public static event Action onPlayer3DStoppedMoving;

    [SerializeField] float moveSpeed = 2;
    [SerializeField] float mouseSensitivity = 0.1f;
    [SerializeField] float detectionRange = 5f;

    [SerializeField] Transform playerTransform;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Rigidbody playerRB;
    [SerializeField] AsleepLucidControl lucidControl;
    [SerializeField] AsleepWakeUpControl wakeUpControl;
    [SerializeField] TextMeshProUGUI interactText;

    public static event Action onPlayerKilled;

    // Input control
    InputController playerControls;
    InputAction move;
    InputAction look;
    InputAction lucid;
    InputAction wakeUp;
    InputAction interact;

    // Movement and look control
    Vector3 moveVector;
    Vector3 lookDirection = Vector3.forward;
    bool checkIsLucid = false;
    bool lookingAtInteractable = false;
    AsleepInteractable curInteractable;
    bool sentStartedMoving = false;

    private void MoveToMazeStart(Maze maze)
    {
        playerTransform.position = maze.startNodePosition;
    }

    private void Awake()
    {
        playerControls = new InputController();
        interactText.text = "";
    }

    private void OnEnable()
    {
        look = playerControls.Player.Look;
        look.Enable();
        move = playerControls.Player.Move;
        move.Enable();

        lucid = playerControls.Player.Visor;
        lucid.Enable();
        lucid.started += StartLucidFromInput;
        lucid.canceled += EndLucidFromInput;

        wakeUp = playerControls.Player.ToggleSleep;
        wakeUp.Enable();
        wakeUp.started += StartHeldWakeUp;
        wakeUp.canceled += EndHeldWakeUp;

        interact = playerControls.Player.Interact;
        interact.Enable();
        interact.started += Interact;

        // Non Input events
        AsleepLucidControl.onLucidToggled += ToggleIsLucidCheck;
        MazeGenerator.onMazeGenerated += MoveToMazeStart;
    }

    private void OnDisable()
    {
        look.Disable();
        move.Disable();
        lucid.Disable();
        wakeUp.Disable();
        interact.Disable();

        lucid.started -= StartLucidFromInput;
        lucid.canceled -= EndLucidFromInput;
        wakeUp.started -= StartHeldWakeUp;
        wakeUp.canceled -= EndHeldWakeUp;
        interact.started -= Interact;

        // Non input events
        AsleepLucidControl.onLucidToggled -= ToggleIsLucidCheck;
        MazeGenerator.onMazeGenerated -= MoveToMazeStart;
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(1))
        //{
        //    if (UnityEngine.Cursor.visible)
        //    {
        //        UnityEngine.Cursor.visible = false;
        //        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        //    }
        //    else
        //    {
        //        UnityEngine.Cursor.visible = true;
        //        UnityEngine.Cursor.lockState = CursorLockMode.None;
        //    }
        //}


        DetermineLookDirection();
        DetermineMoveDirection();
    }

    private void FixedUpdate()
    {
        playerRB.velocity = new Vector3(moveVector.x * moveSpeed, 0, moveVector.z * moveSpeed);
    }

    private void StartLucidFromInput(InputAction.CallbackContext context)
    {
        lucidControl.TransitionBeginLucid();
    }

    private void EndLucidFromInput(InputAction.CallbackContext context)
    {
        lucidControl.TransitionEndLucid();
    }

    private void StartHeldWakeUp(InputAction.CallbackContext context)
    {
        wakeUpControl.AttemptWakeUp();
    }

    private void EndHeldWakeUp(InputAction.CallbackContext context)
    {
        wakeUpControl.CancelWakeUp();
    }

    private void Interact(InputAction.CallbackContext context)
    {
        if (lookingAtInteractable && !checkIsLucid)
        {
            curInteractable.Interact();
        }
    }

    private void DetermineLookDirection()
    {
        Vector2 inputValue = look.ReadValue<Vector2>();

        // Affect Left-Right Rotation
        Quaternion turnRot = Quaternion.Euler(0, inputValue.x * mouseSensitivity, 0);
        playerTransform.forward = turnRot * playerTransform.forward;

        // Affect up-down rotation
        Quaternion nodRot = Quaternion.Euler(inputValue.y * -mouseSensitivity, 0, 0);
        cameraTransform.localRotation = nodRot * cameraTransform.localRotation;

        if (Vector3.Angle(cameraTransform.forward, playerTransform.forward) > 90)
        {
            if (Vector3.Dot(Vector3.up, cameraTransform.forward) >= 0)
                cameraTransform.localRotation = Quaternion.Euler(-90, 0, 0);
            else
                cameraTransform.localRotation = Quaternion.Euler(90, 0, 0);
        }

        bool hit = Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit lineOfSightRay,
            detectionRange);
        if (hit && lineOfSightRay.transform.gameObject.tag == "Interactable")
        {
            GameObject obj = lineOfSightRay.transform.gameObject;
            AsleepInteractable interactable = obj.GetComponent<AsleepInteractable>();
            if (interactable)
            {
                interactText.text = interactable.GetInteractText();
                curInteractable = interactable;
                lookingAtInteractable = true;
            }
            else
                Debug.LogWarning("Object marked interactable has not interactable script");
        }
        else
        {
            interactText.text = "";
            curInteractable = null;
            lookingAtInteractable = false;
        }
    }

    private void DetermineMoveDirection()
    {

        if (checkIsLucid)
        {
            if (sentStartedMoving && playerRB.velocity.magnitude < 0.0001f)
            {
                sentStartedMoving = false;
                onPlayer3DStoppedMoving?.Invoke();
            }
            moveVector = Vector3.zero;
            return;
        }
        lookDirection = playerTransform.forward;
        Vector2 inputValue = move.ReadValue<Vector2>();
        Vector3 moveScaler = new Vector3(inputValue.x, 0, inputValue.y);
        Quaternion moveRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        moveVector = moveRotation * moveScaler;

        playerRB.velocity = new Vector3(moveVector.x * moveSpeed, 0, moveVector.z * moveSpeed);
        if (!sentStartedMoving && playerRB.velocity.magnitude > 0.0001f)
        {
            sentStartedMoving = true;
            onPlayer3DStartedMoving?.Invoke();
        }
        else if (sentStartedMoving && playerRB.velocity.magnitude < 0.0001f)
        {
            sentStartedMoving = false;
            onPlayer3DStoppedMoving?.Invoke();
        }
    }

    private void ToggleIsLucidCheck(bool isLucid) => checkIsLucid = isLucid;

    public static void killPlayer()
    {
        onPlayerKilled?.Invoke();

        if (CheatLogic.cheatTool != null)
        {
            if (CheatLogic.cheatTool.cannotDie)
            {
                print("deaed");
            }
            else
            {
                TransitionManager.instance.LoadDeathLevel();
            }
        }
        else
        {
            print("deaed (CheatLogic not present)");
        }
    }
}
