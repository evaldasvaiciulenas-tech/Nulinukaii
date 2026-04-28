using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }
    public void PlayMusic()
    {
        Debug.Log("PlayMusic - audioSource: " + audioSource + ", isPlaying: " + (audioSource != null ? audioSource.isPlaying.ToString() : "null"));
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
}