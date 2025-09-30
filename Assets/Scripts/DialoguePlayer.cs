using UnityEngine;
using Neocortex.Data;
using Neocortex;
using TMPro;
using System.Collections;

public class DialoguePlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [Header("Neocortex Audio Components")]
    [SerializeField] private NeocortexSmartAgent agent;
    [Header("Ollama Text Components")]
    [SerializeField] private OllamaModelDropdown modelDropdown;
    [SerializeField, TextArea(2, 10)] private string systemPrompt;

    [Header("UI")]
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Dialogue Settings")]
    [Tooltip("How long the dialogue UI stays visible before starting to fade out")]
    [SerializeField] private float dialogueDisplayDuration = 3f;
    [Tooltip("How long the fade out animation takes")]
    [SerializeField] private float fadeOutDuration = 1f;

    private OllamaRequest request;


    void Start()
    {
        request = new OllamaRequest();

        request.OnChatResponseReceived += OnChatResponseReceived;
        request.ModelName = modelDropdown.options[0].text;

        request.AddSystemMessage(systemPrompt);

        agent.OnAudioResponseReceived.AddListener(OnAudioResponseReceived);
    }
    private void OnChatResponseReceived(ChatResponse response)
    {
        Debug.Log(response.message);
        dialogueText.text = response.message;
        ShowDialogueUI();
        // agent.TextToAudio(response.message);
    }

    private void OnAudioResponseReceived(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
    public void PlayDialogue(string dialogue)
    {
        request.Send(dialogue);
    }

    private void ShowDialogueUI()
    {
        // Stop any existing fade coroutine
        StopAllCoroutines();

        // Show the UI immediately
        dialogueUI.SetActive(true);

        // Set text alpha to full
        if (dialogueText != null)
        {
            Color textColor = dialogueText.color;
            textColor.a = 1f;
            dialogueText.color = textColor;
        }

        // Start the fade out coroutine
        StartCoroutine(FadeOutDialogueUI());
    }

    private IEnumerator FadeOutDialogueUI()
    {
        // Wait for the display duration
        yield return new WaitForSeconds(dialogueDisplayDuration);

        // Fade out smoothly using text color alpha
        if (dialogueText != null)
        {
            Color startColor = dialogueText.color;
            float elapsedTime = 0f;

            while (elapsedTime < fadeOutDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);

                Color newColor = startColor;
                newColor.a = alpha;
                dialogueText.color = newColor;

                yield return null;
            }

            // Ensure alpha is exactly 0
            Color finalColor = startColor;
            finalColor.a = 0f;
            dialogueText.color = finalColor;
        }

        // Deactivate the UI
        dialogueUI.SetActive(false);
    }
}
