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


    //private void Update()
    //{
    //    switch(audioIndex)
    //    {
    //        case 0:
    //            source.PlayOneShot(startClip);
    //            break;
    //        case 1:
    //            source.PlayOneShot(priestClip);
    //            break;
    //        case 2:
    //            source.PlayOneShot(treeClip);
    //            break;
    //        case 3:
    //            source.PlayOneShot(wizardClip);
    //            break;
    //        case 4:
    //            source.PlayOneShot(endClip);
    //            break;
    //        default:
    //            source.PlayOneShot(defaultClip);
    //            break;

    //    }
    //}

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

