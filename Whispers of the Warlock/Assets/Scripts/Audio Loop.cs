using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLoop : MonoBehaviour
{
    [SerializeField] AudioClip startClip;
    [SerializeField] AudioClip priestClip;
    [SerializeField] AudioClip treeClip;
    [SerializeField] AudioClip wizardClip;
    [SerializeField] AudioClip defaultClip;
    [SerializeField] AudioClip endClip;
    [SerializeField] AudioSource source;
    public static void AudioClipSet(AudioClip song, AudioSource source)
    {
        if (source.isPlaying)
        {
            source.Stop();
            AudioClipSet(song, source);
        }
        else
        {
            source.clip = song;
            source.Play();

        }

    }


}

