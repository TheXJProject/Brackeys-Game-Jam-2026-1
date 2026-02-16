using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class AsleepPlayerControl : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2;
    [SerializeField] float mouseSensitivity = 0.1f;

    [SerializeField] Transform playerTransform;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Rigidbody playerRB;

    InputController playerControls;
    InputAction move;
    InputAction look;
    
    Vector3 moveVector;
    Vector3 lookDirection = Vector3.forward;


    private void Awake()
    {
        playerControls = new InputController();
    }

    private void OnEnable()
    {
        look = playerControls.Player.Look;
        look.Enable();
        move = playerControls.Player.Move;
        move.Enable();
    }

    private void OnDisable()
    {
        look.Disable();
        move.Disable();
    }

    private void Update()
    {
        { // Code for looking
            Vector2 inputValue = look.ReadValue<Vector2>();

            // Affect Left-Right Rotation
            Quaternion turnRot = Quaternion.Euler(0, inputValue.x * mouseSensitivity, 0);
            playerTransform.forward = turnRot * playerTransform.forward;
            Quaternion nodRot = Quaternion.Euler(inputValue.y * -mouseSensitivity, 0, 0);
            cameraTransform.forward = nodRot * Vector3.forward;
            //localForward = transform.worldToLocalMatrix.MultiplyVector( transform.forward );

            if (cameraTransform.rotation.eulerAngles.x > 90 || cameraTransform.rotation.eulerAngles.x < -90)
            {
                Debug.Log("Crying about it");
            }
        }

        { // Code for movement
            lookDirection = playerTransform.forward;
            Vector2 inputValue = move.ReadValue<Vector2>();
            Vector3 moveScaler = new Vector3(inputValue.x, 0, inputValue.y);
            Quaternion moveRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            moveVector = moveRotation * moveScaler;
        }
    }

    private void FixedUpdate()
    {
        playerRB.velocity = new Vector3(moveVector.x * moveSpeed, 0, moveVector.z * moveSpeed);
    }
}
