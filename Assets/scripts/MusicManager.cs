using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        AudioListener.pause = PlayerPrefs.GetInt("MuteSound", 0) == 1;
    }
}