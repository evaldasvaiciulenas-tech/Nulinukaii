using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider musicSlider;
    public Slider soundEffectsSlider;
    public Toggle muteToggle;

    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        soundEffectsSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        muteToggle.isOn = PlayerPrefs.GetInt("MuteSound", 0) == 1;
        AudioListener.pause = muteToggle.isOn;
        AudioListener.volume = musicSlider.value;
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        AudioListener.volume = muteToggle.isOn ? 0f : value;
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    public void SetMute(bool muted)
    {
        PlayerPrefs.SetInt("MuteSound", muted ? 1 : 0);
        AudioListener.pause = muted;
        PlayerPrefs.Save();
    }
}