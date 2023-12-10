using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLoop : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;
    [SerializeField] AudioSource source;

    int audioIndex;

    private void Update()
    {
        if (!source.isPlaying)
        {

            if (audioIndex == 0)
            {
                StartCoroutine(waitTime());
                source.PlayOneShot(audioClips[audioIndex]);
                audioIndex = 1;
            }
            else if (audioIndex == 1)
            { 
                StartCoroutine(waitTime());
                source.PlayOneShot(audioClips[audioIndex]);
                audioIndex = 0;
            }
        }
    }IEnumerator waitTime()
    {
        yield return new WaitForSeconds(3f);

    }
}

