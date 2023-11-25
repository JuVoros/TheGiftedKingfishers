using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class setVolume : MonoBehaviour
{
   public AudioMixer mixer;

    public void SetLevel(float sliderVal)
    {
        if (CompareTag("Music"))
            mixer.SetFloat("Music Volume",Mathf.Log10(sliderVal) * 20);
        else
            mixer.SetFloat("SFX Volume", Mathf.Log10(sliderVal) * 20);


    }
}
