using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterCopy : MonoBehaviour
{
    public Transform teleportTarget;
    private GameObject player;
    public bool hasTeleported = false;
    private bool musicChanged = false;
    public bool hasTeleportedTwice = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;

            player.SetActive(false);
            if (musicChanged)
                hasTeleportedTwice = true;
            else
                hasTeleported = true;
            player.transform.position = teleportTarget.position;
            player.SetActive(true);

        }
    }

    public void MusicChanged() {
        hasTeleported = false;
        hasTeleportedTwice = false;
        musicChanged = true;
    }
}
