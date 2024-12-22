using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGSongChange : MonoBehaviour
{
    public AudioClip startSong;
    public AudioClip newSong;
    public AudioClip newerSong;

    private AudioSource audioSource;

    private int teleportCount = 0; // Tracks the total number of teleports

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = startSong;
        audioSource.Play();
    }

    public void IncrementTeleportCount()
    {
        teleportCount++;
        ChangeMusic();
    }

    private void ChangeMusic()
    {
        if (teleportCount == 1)
        {
            PlaySong(newSong);
        }
        else if (teleportCount == 2)
        {
            PlaySong(newerSong);
        }
        else if (teleportCount >= 3) //Occurs when the boss is defeated
        {
            PlaySong(startSong);
        }
    }

    private void PlaySong(AudioClip clip)
    {
        if (audioSource.clip != clip)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
