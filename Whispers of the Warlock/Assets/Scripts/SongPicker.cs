using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongPicker : MonoBehaviour
{
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip song;
    [SerializeField] Collider colli;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        { 
            AudioLoop.AudioClipSet(song, source);
            colli.enabled = false;
        
        }



    }

}
