using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class setVolume : MonoBehaviour
{
    [SerializeField] AudioClip sampleClip;
    [SerializeField] AudioSource Source;
    [SerializeField] Slider slider;


   
    public void Sample()
    {
        if (!Source.isPlaying)
        {
            float volume = slider.value;
            Source.PlayOneShot(sampleClip, volume);
        }
    }

}
