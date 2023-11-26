using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class sliderSaver : MonoBehaviour
{
    public Slider slider;

    public void Awake()
    {
        if (slider.CompareTag("SFX"))
        {
            slider.value = PlayerPrefs.GetFloat("SFX value");
        }
        else if (slider.CompareTag("Music"))
           slider.value = PlayerPrefs.GetFloat("Music value");
        else if (slider.CompareTag("Menu Music"))
            slider.value = PlayerPrefs.GetFloat("Menu Music value");
        else if (slider.CompareTag("Menu SFX"))
            slider.value = PlayerPrefs.GetFloat("Menu SFX value");

    }

    public void OnSliderChange(float newValue)
    {
        if (slider.CompareTag("SFX"))
            PlayerPrefs.SetFloat("SFX value", newValue);
        else if (slider.CompareTag("Music"))
            PlayerPrefs.SetFloat("Music value", newValue);
        else if (slider.CompareTag("Menu Music"))
            PlayerPrefs.SetFloat("Menu Music value", newValue);
        else if (slider.CompareTag("Menu SFX"))
            PlayerPrefs.SetFloat("Menu SFX value", newValue);
    }
}
