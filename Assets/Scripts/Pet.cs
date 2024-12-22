using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    public Transform player;
    public float followDistance = 5f;
    public float speed = 3f;
    public float heightOffset = 3f;
    public float rotationSpeed = 5f;
    public float sideOffset = 2f;
    public float detectionRadius = 5f; // Range to detect items
    private string[] detectableTags = { "Key", "Sword", "Sheild" }; // List of things the bird would hint for the user
    public AudioClip soundClip;
    private AudioSource audioSource;
    private bool canPlaySound = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        FollowPlayer();
        HandleSound();
    }

    private void FollowPlayer()
    {
        if (player != null)
        {
            Vector3 targetPosition = player.position + Vector3.up * heightOffset + Vector3.right * sideOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
            Vector3 directionToPlayer = new Vector3(player.position.x - transform.position.x, 0, player.position.z - transform.position.z);
            if (directionToPlayer.magnitude > 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    private void HandleSound()
    {
        if (IsNearDetectableItem() && canPlaySound)
        {
            audioSource.PlayOneShot(soundClip);
            StartCoroutine(SoundCooldown());
        }
    }

    private bool IsNearDetectableItem()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var collider in hitColliders)
        {
            foreach (string tag in detectableTags)
            {
                if (collider.CompareTag(tag))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private IEnumerator SoundCooldown()
    {
        canPlaySound = false;
        yield return new WaitForSeconds(8f);
        canPlaySound = true;
    }

    public void MovePet(Vector3 direction)
    {
        Vector3 move = direction * speed * Time.deltaTime;
        transform.position += move;
    }
}
