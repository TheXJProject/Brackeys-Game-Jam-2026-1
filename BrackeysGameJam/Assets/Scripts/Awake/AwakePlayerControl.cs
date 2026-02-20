using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AwakePlayerControl : MonoBehaviour
{
    public static event Action onPlayer2DStartedMoving;
    public static event Action onPlayer2DStoppedMoving;

    [SerializeField] float moveSpeed = 2;

    [SerializeField] Transform playerTransform;
    [SerializeField] Rigidbody2D playerRB;

    InputController playerControls;
    InputAction move;
    Vector2 moveDirection;

    private bool canMove = false;
    private bool sentStartedMoving = false;

    private void Awake()
    {
        playerControls = new InputController();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        StartGame.startGameNow += AllowMovement;
    }

    private void OnDisable()
    {
        move.Disable();
        StartGame.startGameNow -= AllowMovement;
    }

    private void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
    }

    private void AllowMovement() => canMove = true;

    private void FixedUpdate()
    {
        if (canMove)
        {
            playerRB.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
            if (!sentStartedMoving && playerRB.velocity.magnitude > 0.1f)
            {
                sentStartedMoving = true;
                onPlayer2DStartedMoving?.Invoke();
            }
            else if (sentStartedMoving && playerRB.velocity.magnitude < 0.1f)
            {
                sentStartedMoving = false;
                onPlayer2DStoppedMoving?.Invoke();
            }
        }
    }
}
