using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField] private List<AudioClip> tracks = new List<AudioClip>();
    [SerializeField] public bool next = false;
    [SerializeField] private int playing = 0;
    IEnumerator cor;
    AudioSource player;
    private void Start() {
        player = GetComponent<AudioSource>();
        cor = SetTrack();
        StartCoroutine(cor);
    }

    private IEnumerator SetTrack()
    {
        while (playing >= 0) 
        {
            next = false;
            player.Stop();           
            player.clip = tracks[playing];
            player.Play();
            NextTrack();
            while (player.isPlaying && !next) {
                yield return new WaitForFixedUpdate();
            }
        }
    }

    private void NextTrack()
    {
        if (playing >= tracks.Count-1) playing = 0;
        else playing++;
    }
    public void StopPlaying()
    {
        playing = -1;
    }
}
