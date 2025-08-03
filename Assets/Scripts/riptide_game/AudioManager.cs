using UnityEngine;

// ONLY HAVE ONE OF THESE IN THE SCENE
public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    [SerializeField]
    public AudioClip victoryClip;

    [SerializeField]
    public AudioClip pluckClip;

    [SerializeField]
    public AudioClip loopCounterClip;
    [SerializeField]
    public AudioClip loopBrokenClip;

    public void PlayAudioClip(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Attempted to play a null audio clip.");
            return;
        }

        // Create an AudioSource to play the clip
        GameObject audioObject = new GameObject("AudioSource");
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();

        // Destroy the AudioSource object after the clip finishes playing
        Object.Destroy(audioObject, clip.length);
    }

    public void PlayAudioClipPitched(AudioClip clip, float pitch)
    {
        if (clip == null)
        {
            Debug.LogWarning("Attempted to play a null audio clip with pitch.");
            return;
        }

        // Create an AudioSource to play the clip
        GameObject audioObject = new GameObject("AudioSourcePitched");
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.Play();

        // Destroy the AudioSource object after the clip finishes playing
        Object.Destroy(audioObject, clip.length);
    }

}