using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider soundEffectsSlider;

    void Start()
    {
        float music = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        musicSlider.onValueChanged.RemoveAllListeners();
        soundEffectsSlider.onValueChanged.RemoveAllListeners();

        musicSlider.value = music;
        soundEffectsSlider.value = sfx;

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        soundEffectsSlider.onValueChanged.AddListener(SetSFXVolume);

        SetMusicVolume(music);
        SetSFXVolume(sfx);
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.Save();
    }
}