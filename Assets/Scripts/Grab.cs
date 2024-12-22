using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    public float grabDistance = 5f;
    private GameObject grabbedObject = null;
    public Camera playerCamera;
    public float verticalOffset = 1f;
    public float verticalSensitivity = 0.1f;
    public float maxVerticalOffset = 2f;
    public float minVerticalOffset = 0f;

    private float currentVerticalOffset = 0f;
    private Vector3 holdOffset;

    // Reference to player controller values
    private PlayerController playerController;
    private float rotationX;
    private bool canMove;
    private float lookXLimit;
    public bool hasGrab = false;

    // Smoothing values
    public float smoothSpeed = 10f;
    private Vector3 currentVelocity;

    private AudioSource audioSource;
    public AudioClip grabSound;
    public AudioClip putSound;
    public AudioClip tossSound;


    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            rotationX = playerController.rotationX;
            lookXLimit = playerController.lookXLimit;
            canMove = playerController.canMove;
        }
        audioSource = GetComponent<AudioSource>();

    }

    void Update()
    {

        
        if (Input.GetMouseButtonDown(1) && !playerController.isDead)
        {
            TryGrabObject();
        }

        if (grabbedObject != null && !playerController.isDead)
        {
            float verticalAngle = Mathf.Clamp(rotationX / lookXLimit, -1f, 1f);
            currentVerticalOffset = Mathf.Lerp(minVerticalOffset, maxVerticalOffset, (verticalAngle + 1f) * 0.5f);

            MoveGrabbedObject();
        }

        if ((Input.GetMouseButtonUp(1) && grabbedObject != null) || playerController.isDead)
        {
            ReleaseObject();
        }
    }

    void TryGrabObject()
    {
        if (playerController.isDead) return;

        RaycastHit hit;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out hit, grabDistance))
        {
            if (hit.collider.CompareTag("Pickup"))
            {
                audioSource.PlayOneShot(grabSound);
                grabbedObject = hit.collider.gameObject;
                

                Rigidbody rigidbody = grabbedObject.GetComponent<Rigidbody>();
                
                rigidbody.isKinematic = true;

                Destroy(rigidbody);
                Bounds objectBounds = grabbedObject.GetComponent<Collider>().bounds;
                Destroy(grabbedObject.GetComponent<BoxCollider>());
                float objectSize = objectBounds.size.z;
                holdOffset = new Vector3(0f, 0.75f, 0.5f + objectSize);

                currentVerticalOffset = 0f;
                hasGrab = true;
            }
        }
    }

    void MoveGrabbedObject()
    {
        if (grabbedObject != null)
        {
            Vector3 targetPosition = transform.position +
                                   transform.right * holdOffset.x +
                                   transform.up * (holdOffset.y + currentVerticalOffset) +
                                   transform.forward * holdOffset.z;

            grabbedObject.transform.position = Vector3.SmoothDamp(
                grabbedObject.transform.position,
                targetPosition,
                ref currentVelocity,
                1f / smoothSpeed
            );

            grabbedObject.transform.rotation = Quaternion.Lerp(
                grabbedObject.transform.rotation,
                transform.rotation,
                Time.deltaTime * smoothSpeed
            );
        }
    }

    void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            grabbedObject.AddComponent<Rigidbody>();
            grabbedObject.AddComponent<BoxCollider>();
            Rigidbody rigidbody = grabbedObject.GetComponent<Rigidbody>(); 
            rigidbody.isKinematic = false;
            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
            bool velocity = Input.GetKey(KeyCode.LeftShift);
            if (velocity)
            {
                audioSource.PlayOneShot(tossSound);
                rb.velocity = currentVelocity;
            }
            else {
                Wait();
                audioSource.PlayOneShot(putSound);
            }
            grabbedObject = null;
            hasGrab = false;
        }
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.7f);
    }

    public void ForceRelease()
    {
        ReleaseObject();
    }
}
