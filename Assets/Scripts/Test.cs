using UnityEngine;
using Neocortex.Data;
using Neocortex;

public class Test : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [Header("Neocortex Audio Components")]
    [SerializeField] private NeocortexSmartAgent agent;
    [Header("Ollama Text Components")]
    [SerializeField] private OllamaModelDropdown modelDropdown;
    [SerializeField, TextArea] private string systemPrompt;

    private OllamaRequest request;


    void Start()
    {
        request = new OllamaRequest();

        request.OnChatResponseReceived += OnChatResponseReceived;
        request.ModelName = modelDropdown.options[0].text;

        request.AddSystemMessage(systemPrompt);

        agent.OnAudioResponseReceived.AddListener(OnAudioResponseReceived);

        request.Send("Headshot");
        Debug.Log("Headshot sent");

    }

    private void OnChatResponseReceived(ChatResponse response)
    {
        Debug.Log(response.message);

        agent.TextToAudio(response.message);
    }

    private void OnAudioResponseReceived(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
