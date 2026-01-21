using UnityEngine;
using UnityEngine.InputSystem;

public enum Modes { Light, Dark }

public class LightDarkSwitch : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject lightLevel;
    [SerializeField] private GameObject darkLevel;
    [SerializeField] private Sprite lightSprite;
    [SerializeField] private Sprite darkSprite;

    [Header("Settings")]
    [SerializeField] private Modes currentMode;
    [SerializeField] private InputActionAsset inputActions;

    private SpriteRenderer playerSpriteRenderer;
    private InputAction switchA;
    private Animator animator;

    private void OnValidate() // To make it work in the Editor
    {
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        UpdateVisuals(currentMode);
    }

    void Awake()
    {
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        switchA = inputActions.FindAction("Player_Keyboard/SwitchMode");
    }

    void OnEnable() => switchA?.Enable();
    void OnDisable() => switchA?.Disable();

    void Update()
    {
        if (switchA.triggered)
        {
            ToggleMode();
        }
    }

    void ToggleMode()
    {
        currentMode = (currentMode == Modes.Light) ? Modes.Dark : Modes.Light;
        UpdateVisuals(currentMode);
    }

    void UpdateVisuals(Modes mode)
    {
        bool isLight = (mode == Modes.Light);
    
        lightLevel.SetActive(isLight);
        darkLevel.SetActive(!isLight);

        playerSpriteRenderer.sprite = isLight ? lightSprite : darkSprite;
        animator.SetBool("isWhite", isLight);
    }
}