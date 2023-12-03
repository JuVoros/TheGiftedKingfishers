using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class soundSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] public Slider musicSlider;
    [SerializeField] public Slider mainMenuSlider;
    [SerializeField] public Slider sfxSlider;


    void Start ()
    {
        
        if (PlayerPrefs.HasKey("musicVolume") || PlayerPrefs.HasKey("mainMusic") || PlayerPrefs.HasKey("SFX"))
            LoadVolume();
        else
        {
            SetMusicVolume();
            SetMainMusicVolume();
            SetSFXVolume();
        }
    }





    public void SetMusicVolume()
    {
        
        float volume = musicSlider.value;
        mixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
        
    }
    public void SetMainMusicVolume()
    {
        float mainVolume = mainMenuSlider.value;
        mixer.SetFloat("Main Menu Music", Mathf.Log10(mainVolume) * 20);
        PlayerPrefs.SetFloat("mainMusic", mainVolume);
        
    }
    public void SetSFXVolume()
    {
        float sfxVolume = sfxSlider.value;
        mixer.SetFloat("SFX", Mathf.Log10(sfxVolume) * 20);
        PlayerPrefs.SetFloat("SFX", sfxVolume);
        
    }
    void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        mainMenuSlider.value = PlayerPrefs.GetFloat("mainMusic");
        sfxSlider.value = PlayerPrefs.GetFloat("SFX");

        SetMusicVolume();
        SetMainMusicVolume();
        SetSFXVolume();

    }

    
}
