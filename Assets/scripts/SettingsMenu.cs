using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider musicVolume;
    public Slider soundEffectsVolume;
    public Toggle muteSound;

    void Start()
    {
        musicVolume.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        soundEffectsVolume.value = PlayerPrefs.GetFloat("SoundEffectsVolume", 1f);
        muteSound.isOn = PlayerPrefs.GetInt("MuteSound", 0) == 1;
        AudioListener.volume = musicVolume.value;
        AudioListener.pause = muteSound.isOn;
    }

    public void SetMute(bool isMuted)
    {
        PlayerPrefs.SetInt("MuteSound", isMuted ? 1 : 0);
        AudioListener.pause = isMuted;
    }

    public void Back()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
    }
}