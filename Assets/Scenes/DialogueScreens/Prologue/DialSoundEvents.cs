using UnityEngine;
using UnityEngine.Audio;

public class DialSoundEvents : MonoBehaviour
{
    [SerializeField] private DialogueWindow dialogueWindow;
    [SerializeField] private AudioSource part1;
    [SerializeField] private AudioSource part2;
    [SerializeField] private AudioMixer dialogueMixer;

    private float fadeInSpeed = 1.0f;
    private float fadeOutSpeed = 1.0f;

    private int line => dialogueWindow.GetCurrentLine();

    private float timer = 0f; // For debug only!


    void Update()
    {
        PrologDialogueProcess();
    }

    void PrologDialogueProcess()
    {
        switch (line)
        {
            case 0:
                if (!part1.isPlaying) part1.Play();
                break;
            case 4:
                FadeOutMixer();
                break;
            case 5:
                if (part1.isPlaying) part1.Stop();
                if (!part2.isPlaying) part2.Play();
                fadeInSpeed = 0f;
                FadeInMixer();
                break;
        }

        if (dialogueWindow.isLastLine)
        {
            fadeOutSpeed = 6f;
            FadeOutMixer();
        }
    }

    void FadeOutMixer()
    {
        if (dialogueMixer.GetFloat("DialogueVolume", out float currentVolume))
        {
            float newVolume = Mathf.MoveTowards(currentVolume, -80f, (80f / fadeOutSpeed) * Time.deltaTime);
            
            dialogueMixer.SetFloat("DialogueVolume", newVolume);
        }
    }

    void FadeInMixer()
    {
        if (dialogueMixer.GetFloat("DialogueVolume", out float currentVolume))
        {
            float newVolume = Mathf.MoveTowards(currentVolume, 0f, (80f / fadeInSpeed) * Time.deltaTime);
            
            dialogueMixer.SetFloat("DialogueVolume", newVolume);
        }
    }

    void PrintDebugInfo()
    {
        timer += Time.deltaTime;
        if (timer >= 1.0f)
        {
            Debug.Log($"Current line: {dialogueWindow.GetCurrentLine()}");
            timer = 0f;
        }
    }
}
