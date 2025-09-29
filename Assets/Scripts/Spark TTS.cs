using UnityEngine;
using SparkTTS;
using SparkTTS.Utils;
using System.Threading.Tasks;

public class TTSExample : MonoBehaviour
{
    private CharacterVoice characterVoice;
    private AudioSource audioSource;

    async void Start()
    {
        // Initialize the TTS system (optional - sets log levels and optimal memory usage mode)
        CharacterVoiceFactory.Initialize(LogLevel.WARNING, true); // Set OptimalMemoryUsage to true for IOS
        
        // Get reference to AudioSource
        audioSource = GetComponent<AudioSource>();
        
        // Get the singleton instance of the factory
        var voiceFactory = CharacterVoiceFactory.Instance;
        
        // Create a styled voice (gender: male/female, pitch: very_low/low/moderate/high/very_high, speed: very_low/low/moderate/high/very_high)
        characterVoice = await voiceFactory.CreateFromStyleAsync(
            gender: "female",
            pitch: "moderate", 
            speed: "moderate",
            referenceText: "Hello, I am a character voice sample."
        );
        
        // Generate and play speech
        if (characterVoice != null)
        {
            await GenerateAndPlaySpeech("Hello, welcome to my game! I'm an on-device TTS voice.");
        }
    }
    
    public async Task GenerateAndPlaySpeech(string text)
    {
        if (characterVoice == null) return;
        
        AudioClip generatedClip = await characterVoice.GenerateSpeechAsync(text);
        
        if (generatedClip != null && audioSource != null)
        {
            audioSource.clip = generatedClip;
            audioSource.Play();
        }
    }
    
    private void OnDestroy()
    {
        // Clean up resources
        characterVoice?.Dispose();
        // Note: Don't dispose the factory instance as it's a singleton
    }
}