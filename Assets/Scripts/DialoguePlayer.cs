using UnityEngine;
using Neocortex.Data;
using Neocortex;

public class DialoguePlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [Header("Neocortex Audio Components")]
    [SerializeField] private NeocortexSmartAgent agent;
    [Header("Ollama Text Components")]
    [SerializeField] private OllamaModelDropdown modelDropdown;
    [SerializeField, TextArea(2, 10)] private string systemPrompt;

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
}
