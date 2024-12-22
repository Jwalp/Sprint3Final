using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform teleportTarget;
    private GameObject player;
    private BGSongChange music;

    private void Start()
    {
        music = FindObjectOfType<BGSongChange>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;

            player.SetActive(false);
            player.transform.position = teleportTarget.position;
            player.SetActive(true);
            music.IncrementTeleportCount();
        }
    }
}
