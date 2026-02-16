using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AwakePlayerControl : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2;

    [SerializeField] Transform playerTransform;
    [SerializeField] Rigidbody2D playerRB;

    InputController playerControls;
    InputAction move;
    Vector2 moveDirection;

    private void Awake()
    {
        playerControls = new InputController();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
    }

    private void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        playerRB.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }
}
