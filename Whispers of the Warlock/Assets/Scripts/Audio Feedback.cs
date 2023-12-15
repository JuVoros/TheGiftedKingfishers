using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFeedback : MonoBehaviour
{
  


    public static void Effect(AudioSource source, AudioClip clip)
    {
        source.PlayOneShot(clip);

    }



}
