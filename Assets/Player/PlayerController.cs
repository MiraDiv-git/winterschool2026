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
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Wall Jump Check")]
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private Transform[] wallJumpCheck;
    [SerializeField] private float wallCheckRadius = 0.2f;

    private int wallJumpCount = 1;
    private bool isControlLocked = false;

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
        ApplyWallJump();
    }

    void ApplyMovement()
    {
        float moveInput = moveA.ReadValue<float>();

        if (isControlLocked)
        {
            if (Mathf.Abs(moveInput) < 0.01f)
            {
                isControlLocked = false;
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
                return;
            }
        }

        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }

    void ApplyJumping()
    {
        if (jumpA.triggered && IsGrounded())
        {
            rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        }
    }

    void ApplyWallJump()
    {
        if (IsWall()) wallJumpCount = 1;

        if (jumpA.triggered && IsWall() && wallJumpCount > 0)
        {
            wallJumpCount = 0;
            
            float jumpDir = GetWallDirection() * -1; 

            rb.linearVelocity = Vector2.zero; 
            rb.AddForce(new Vector2(jumpDir * wallJumpForce, jumpHeight), ForceMode2D.Impulse);

            isControlLocked = true;
    }
    }

    bool IsGrounded()
    {
        if (groundCheck == null) return false;

        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    bool IsWall()
    {
        if (wallJumpCheck == null) return false;

        foreach (Transform check in wallJumpCheck)
    {
        if (Physics2D.OverlapCircle(check.position, wallCheckRadius, groundLayer))
        {
            return true;
        }
    }
        return false;
    }

    float GetWallDirection()
{
    foreach (Transform check in wallJumpCheck)
    {
        if (Physics2D.OverlapCircle(check.position, wallCheckRadius, groundLayer))
        {
            return check.position.x > transform.position.x ? 1 : -1;
        }
    }
    return 0;
}

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckRadius);
        }

        if (wallJumpCheck != null)
        {
            Gizmos.color = Color.yellow;
            foreach (Transform check in wallJumpCheck)
            {
                Gizmos.DrawWireSphere(check.position, wallCheckRadius);
                Gizmos.DrawLine(check.position, check.position + Vector3.down * wallCheckRadius);
            }
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
