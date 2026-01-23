using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueWindow : MonoBehaviour
{
    [Header("Dialogue Window Settings")]
    [SerializeField] private bool enableSpeaker = true;
    [SerializeField] private string speakerName = "Speaker Name";
    [SerializeField] private Color speakerColor = Color.white;
    [SerializeField] private Sprite[] backgrounds;

    [Header("Effects")]
    [SerializeField] private float approachDelay = 0.0f;
    [SerializeField] private bool fadeIn = true;
    [SerializeField] private bool fadeInBackground = false;
    [SerializeField] private float fadeInDuration = 1.0f;
    [SerializeField] private bool fadeOut = true;
    [SerializeField] private bool fadeOutBackground = false;
    [SerializeField] private float fadeOutDuration = 1.0f;

    [Header("Objects")]
    [SerializeField] private GameObject speakerNameObject;
    [SerializeField] private GameObject dialogueObject;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private InputActionAsset inputActions;

    [Header("Events")]
    [SerializeField] private UnityEvent actionOnEnd;

    [Header("Dialogue")]
    [Tooltip("Characters per second")]
    [SerializeField] private float textSpeed = 20.0f; 
    [SerializeField] private int startLine = 0;
    [TextArea] [SerializeField] private string[] dialogueLines;

    private InputAction nextA;
    private TMP_Text speakerNameText;
    private TMP_Text dialogueTextAsset;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    
    void OnValidate()
    {
        if (inputActions != null)
            nextA = inputActions.FindAction("Dialogue/NextLine");

        if (speakerNameObject != null)
        {
            speakerNameObject.SetActive(enableSpeaker);
            speakerNameText = speakerNameObject.GetComponentInChildren<TMP_Text>();
        }

        if (dialogueObject != null)
            dialogueTextAsset = dialogueObject.GetComponentInChildren<TMP_Text>();

        if (speakerNameText != null)
        {
            speakerNameText.text = speakerName;
            speakerNameText.color = speakerColor;
        }

        if (backgrounds != null && backgrounds.Length > 0 && backgroundImage != null)
        {
            int bgIndex = Mathf.Clamp(startLine, 0, backgrounds.Length - 1);
            
            if (backgrounds[bgIndex] != null)
                backgroundImage.sprite = backgrounds[bgIndex];
        }

        if (dialogueLines != null && startLine < dialogueLines.Length && dialogueTextAsset != null)
        {
            dialogueTextAsset.text = dialogueLines[startLine];
        }
    }

    void Awake()
    {
        OnValidate();
    }

    void Start()
    {   
        if (fadeIn) { StartCoroutine(FadeEffect(true)); }
        else { UpdateDialogueText(); }
    }

    void Update()
    {
        if (nextA != null && nextA.triggered)
        {
            UpdateDialogueText();
        }
    }

    void OnEnable() => nextA?.Enable();
    void OnDisable() => nextA?.Disable();

    void UpdateDialogueText()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueTextAsset.text = dialogueLines[startLine - 1];

            if (backgrounds != null && startLine - 1 < backgrounds.Length && backgrounds[startLine - 1] != null)
                backgroundImage.sprite = backgrounds[startLine - 1];

            isTyping = false;
            return;
        }

        if (startLine < dialogueLines.Length)
        {
            if (backgrounds != null && startLine < backgrounds.Length && backgrounds[startLine] != null)
                backgroundImage.sprite = backgrounds[startLine];
            
            typingCoroutine = StartCoroutine(TypeText(dialogueLines[startLine]));
            startLine++;
        }
        else 
        {
            if (fadeOut)
            {
                StartCoroutine(FadeEffect(false));
                nextA.Disable();
            }
            else
            {
                actionOnEnd?.Invoke();
                gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogueTextAsset.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogueTextAsset.text += letter;
            yield return new WaitForSeconds(1f / Mathf.Max(0.1f, textSpeed)); 
        }

        isTyping = false;
    }

   private IEnumerator FadeEffect(bool isFadeIn)
    {
        CanvasGroup speakerGroup = speakerNameObject.GetComponent<CanvasGroup>();
        CanvasGroup dialogueGroup = dialogueObject.GetComponent<CanvasGroup>();

        float startAlpha = isFadeIn ? 0f : 1f;
        float targetAlpha = isFadeIn ? 1f : 0f;
        float duration = isFadeIn ? fadeInDuration : fadeOutDuration;
        bool processBackground = isFadeIn ? fadeInBackground : fadeOutBackground;

        if (speakerGroup != null) speakerGroup.alpha = startAlpha;
        if (dialogueGroup != null) dialogueGroup.alpha = startAlpha;
        
        if (processBackground && backgroundImage != null)
        {
            Color c = backgroundImage.color;
            backgroundImage.color = new Color(c.r, c.g, c.b, startAlpha);
        }

        if (isFadeIn) yield return new WaitForSeconds(approachDelay);

        if (isFadeIn) UpdateDialogueText();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            
            if (speakerGroup != null) speakerGroup.alpha = currentAlpha;
            if (dialogueGroup != null) dialogueGroup.alpha = currentAlpha;

            if (processBackground && backgroundImage != null)
            {
                Color c = backgroundImage.color;
                backgroundImage.color = new Color(c.r, c.g, c.b, currentAlpha);
            }
            yield return null;
        }

        if (speakerGroup != null) speakerGroup.alpha = targetAlpha;
        if (dialogueGroup != null) dialogueGroup.alpha = targetAlpha;

        if (!isFadeIn)
        {
            actionOnEnd?.Invoke();
            gameObject.SetActive(false);
        }
    }
}