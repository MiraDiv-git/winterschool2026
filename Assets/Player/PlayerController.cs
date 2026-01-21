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

    private bool isControlLocked = false; // locker for wall jumps
    private Animator animator; // Animation state machine
    private Rigidbody2D rb;
    

    private InputAction moveA; // Axis for movement
    private InputAction jumpA; // Button for jump

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        moveA = inputActions.FindAction("Player_Keyboard/Move"); // using input action asset as json file, as
        jumpA = inputActions.FindAction("Player_Keyboard/Jump"); // it's more comfy 4 me
    }

    void Update()
    {
        ApplyMovement();
        ApplyJumping();
        ApplyWallJump();
    }

    void ApplyMovement()
    {
        float moveInput = moveA.ReadValue<float>(); // reading input as axis (-1 for A, 1 for D and 0 for nothing)

        if (isControlLocked)
        {
            if (Mathf.Abs(moveInput) < 0.01f) // If input goes to 0, then button was released after jump
            {
                isControlLocked = false; // unlocking the control
            }
            else
            {
                // Saves the inertia from jump and locks the override of horizontal speed
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
                return;
            }
        }

        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y); // Basic movement

        if (moveInput != 0) // Actions while player is moving
        {
            animator.SetBool("isMoving", true);

            if (Mathf.Sign(moveInput) != Mathf.Sign(transform.localScale.x)) // turns -1.0f for left or 1.0f for right directions
            {
                Vector3 scale = transform.localScale; // Saves player's x scale
                scale.x = Mathf.Abs(scale.x) * Mathf.Sign(moveInput); // sets -x for left and +x for right directions
                transform.localScale = scale;
            }
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
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
        if (jumpA.triggered && IsWall())
        {
            
            float jumpDir = GetWallDirection() * -1; // Sets the jump direction to opposite side from jump wall

            rb.linearVelocity = Vector2.zero; // Resets the velocity to apply force overriding the movement
            rb.AddForce(new Vector2(jumpDir * wallJumpForce, jumpHeight), ForceMode2D.Impulse); // makes wall jump

            isControlLocked = true;
    }
    }

    bool IsGrounded()
    {
        if (groundCheck == null) return false;
        
        // Creates a physics circle that checks the collider in it's area from ground layer.
        // Radius increases the size of this circle
        // If it 'senses' the collider - we're on the ground. If not - we're not
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    bool IsWall() // Works mostly the same as IsGrounded
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
            // If the physics circle senses the collider of jump wall, returns the direction from what wall it touched
            if (Physics2D.OverlapCircle(check.position, wallCheckRadius, groundLayer))
            {
                return check.position.x > transform.position.x ? 1 : -1;
            }
        }
        return 0;
    }

    private void OnDrawGizmos() // Just draws physics circles through the gizmos so we can see the area of them
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