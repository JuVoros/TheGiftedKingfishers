using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static Unity.VisualScripting.Member;
using UnityEngine.UIElements;

public class onButtonSelect : MonoBehaviour
{
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip clickClip;
    [SerializeField] AudioClip hoverClip;

    public void HoverSound()
    {
        float volume = PlayerPrefs.GetFloat("SFX");
        source.PlayOneShot(hoverClip, volume);

    }
    public void ClickSound()
    {

        float volume = PlayerPrefs.GetFloat("SFX");
        source.PlayOneShot(clickClip, volume);

    }


}
