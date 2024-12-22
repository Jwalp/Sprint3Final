using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform playerTransform;
    public GameObject interactionHintUI;
    public float interactionRange = 3.0f;
    private AudioSource audioSource;
    public AudioClip doorSound;
    private PlayerController playerController;

    void Start()
    {
        playerController = playerTransform.GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        GameObject nearestDoor = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject door in doors)
        {
            float distance = Vector3.Distance(playerTransform.position, door.transform.position);
            if (distance < nearestDistance && distance <= interactionRange)
            {
                nearestDoor = door;
                nearestDistance = distance;
            }
        }

        if (nearestDoor != null && nearestDistance <= interactionRange && playerController.key > 0)
        {
            interactionHintUI.SetActive(true);

            if (playerController.key > 0 && Input.GetKeyDown(KeyCode.E))
            {
                if (doorSound != null)
                {
                    audioSource.PlayOneShot(doorSound);
                }

                Destroy(nearestDoor);

                playerController.key--;
                playerController.SetCountText();
            }
        }
        else
        {
            interactionHintUI.SetActive(false);
        }
    }
}
