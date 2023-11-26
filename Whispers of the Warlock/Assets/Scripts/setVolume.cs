using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class setVolume : MonoBehaviour
{
   public AudioMixer mixer;

    public void SetLevel(float sliderVal)
    {
        
        if (CompareTag("Music"))
        {
            mixer.SetFloat("Music Volume", Mathf.Log10(sliderVal) * 20);
            PlayerPrefs.SetFloat("Music value", sliderVal);
        }
        else if (CompareTag("SFX"))
        {
            mixer.SetFloat("SFX Volume", Mathf.Log10(sliderVal) * 20);
            PlayerPrefs.SetFloat("SFX value", sliderVal);

        }
        else if (CompareTag("Menu Music"))
        {
            mixer.SetFloat("Menu Music Volume", Mathf.Log10(sliderVal) * 20);
            PlayerPrefs.SetFloat("Menu Music value", sliderVal);

        }
        else if (CompareTag("Menu SFX"))
        {
            mixer.SetFloat("Menu SFX Volume", Mathf.Log10(sliderVal) * 20);
            PlayerPrefs.SetFloat("Menu SFX value", sliderVal);

        }
    }
}
