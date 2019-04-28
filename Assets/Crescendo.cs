using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crescendo : MonoBehaviour
{
    public AudioClip[] tracks;
    int index = 0;
    AudioSource source;
    Coroutine c;

    void OnEnable()
    {
        source = GetComponent<AudioSource>();
        index = 0;
        source.clip = tracks[index];
        source.Play();
        Debug.Log("play "+index);
        Debug.Log(tracks[index]);
        c = StartCoroutine(Next());
    }

    void OnDisable() {
        if(c != null) {
            StopCoroutine(c);
        }
    }

    IEnumerator Next() {
        while(source.isPlaying) {
            yield return null;
        }
        index = (index + 1) % tracks.Length;
        source.clip = tracks[index];
        source.Play();
        c = StartCoroutine(Next());
    }
}
