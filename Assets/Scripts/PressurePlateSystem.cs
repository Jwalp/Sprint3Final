using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateSystem : MonoBehaviour
{
    public GameObject[] pressurePlates; 
    public GameObject door;
    private bool doorDestroyed = false;
    private AudioSource audioSource;
    public AudioClip unlock;


    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (doorDestroyed) return;

        bool allPlatesActive = true;

        foreach (var plate in pressurePlates)
        {
            if (!IsObjectOnPlate(plate))
            {
                allPlatesActive = false;
                break;
            }
        }

        if (allPlatesActive)
        {
            Destroy(door);
            audioSource.PlayOneShot(unlock);
            doorDestroyed = true;
        }
    }

    private bool IsObjectOnPlate(GameObject plate)
    {
        Collider[] colliders = Physics.OverlapBox(
            plate.transform.position,
            plate.transform.localScale / 2,
            Quaternion.identity
        );

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Pickup"))
            {
                return true;
            }
        }

        return false;
    }
}
