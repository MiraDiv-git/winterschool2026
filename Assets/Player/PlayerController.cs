using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 5f;
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
        rb.linearVelocity = new Vector3(value * speed, rb.linearVelocity.y, 0);  
    }

    void ApplyJumping()
    {
        if (jumpA.triggered)
        {
            rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        }
    }

    private void OnEnable() => moveA?.Enable();
    private void OnDisable() => moveA?.Disable();
}
