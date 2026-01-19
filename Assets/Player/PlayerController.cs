using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Basic Settings")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    

    private InputAction moveA;
    private InputAction jumpA;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        moveA = inputActions.FindAction("Player_Keyboard/Move");
        jumpA = inputActions.FindAction("Player_Keyboard/Jump");
    }

    void Update()
    {
        ApplyMovement();
        ApplyJumping();
    }

    void ApplyMovement()
    {
        float value = moveA.ReadValue<float>();
        rb.linearVelocity = new Vector2(value * speed, rb.linearVelocity.y);  
    }

    void ApplyJumping()
    {
        if (jumpA.triggered && IsGrounded())
        {
            rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        }
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * checkRadius);
        }
    }

private void OnEnable() 
{
    moveA?.Enable();
    jumpA?.Enable();
}

private void OnDisable() 
{
    moveA?.Disable();
    jumpA?.Disable();
}
}
