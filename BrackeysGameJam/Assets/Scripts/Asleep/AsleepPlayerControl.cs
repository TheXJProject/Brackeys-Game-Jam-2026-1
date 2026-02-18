using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class AsleepPlayerControl : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2;
    [SerializeField] float mouseSensitivity = 0.1f;
    [SerializeField] float detectionRange = 5f;

    [SerializeField] Transform playerTransform;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Rigidbody playerRB;
    [SerializeField] AsleepLucidControl lucidControl;
    [SerializeField] AsleepWakeUpControl wakeUpControl;
    [SerializeField] TextMeshProUGUI interactText;

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
    bool stopMovement = false;
    bool lookingAtInteractable = false;
    AsleepInteractable curInteractable;

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
        AsleepLucidControl.onLucidToggled += ToggleStopMoving;
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
        AsleepLucidControl.onLucidToggled -= ToggleStopMoving;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (UnityEngine.Cursor.visible)
            {
                UnityEngine.Cursor.visible = false;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                UnityEngine.Cursor.visible = true;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
            }
        }


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
        if (lookingAtInteractable)
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
                cameraTransform.localRotation = Quaternion.Euler(-90,0,0);
            else
                cameraTransform.localRotation = Quaternion.Euler(90, 0, 0);
        }

        bool hit = Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit lineOfSightRay, detectionRange);
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
        if (stopMovement)
        {
            moveVector = Vector3.zero;
            return;
        }
        lookDirection = playerTransform.forward;
        Vector2 inputValue = move.ReadValue<Vector2>();
        Vector3 moveScaler = new Vector3(inputValue.x, 0, inputValue.y);
        Quaternion moveRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        moveVector = moveRotation * moveScaler;
    }

    private void ToggleStopMoving(bool isLucid) => stopMovement = isLucid;
}
