using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

//SOURCE FOR MOVEMENT SPECIFICALLY: https://www.youtube.com/watch?v=qQLvcS9FxnY&ab_channel=AllThingsGameDev
public class PlayerController : MonoBehaviour
{
    public Camera playerCamera;


    //Store Editables
    public float maxHealth = 1f;
    public float walkSpeed = 6f;
    public float jumpPower = 5f;
    public float runSpeed = 0f;
    public float defense = 0f;
    //Attack Damage Seperately


    //Moving things and stats
    public float gravity = 9.8f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    Vector3 moveDirection = Vector3.zero;
    public float rotationX = 0;
    public bool canMove = true;
    private float currentHealth;
    public bool isDead = false;
    private bool canDamage = true;

    //Animator, controller and invetory
    private Animator animator;
    private CharacterController characterController;
    public TextMeshProUGUI countText;
    public int money = 0;
    public int key = 0;
    private bool hasTreasure;
    private bool hasHatchet;
    private bool hasSheild;
    public GameObject sword;
    public GameObject sheild;

    //Death Things
    public GameObject deathCanvas;
    public TextMeshProUGUI respawnText;
    public float respawnTime = 7f; //Do at least 1 second cause of Revive Annimation 
    public GameObject winCanvas;

    //Audio
    private AudioSource audioSource;
    public AudioClip coinSound;
    public AudioClip keySound;
    public AudioClip weaponSound;
    public AudioClip deathSound;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        money = 0;
        key = 0;
        hasTreasure = false;
        hasHatchet = false;
        hasSheild = false;
        SetCountText();

    }

    public void SetCountText()
    {
        string display = "Health: " + currentHealth.ToString() + "\n";

        display += "Coins: " + money.ToString() + "\n";

        if (key > 0)
            display += "Keys: " + key.ToString() + "\n";
        if (hasHatchet)
            display += "Sword\n";
//        if (hasSheild)
//            display += "Sheild\n";
        if (hasTreasure)
            display += "Crown\n";

        countText.text = display;
    }


    void Update()
    {
        if (isDead) return;

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (walkSpeed + (isRunning ? runSpeed : 0)) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (walkSpeed + (isRunning ? runSpeed : 0)) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            gravity = 9.8f;
            moveDirection.y = jumpPower;

        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            if (isRunning)
            {
                gravity = 13.067f;
            }
            else
            {
                gravity = 9.8f;
            }
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        float speed = new Vector2(curSpeedX, curSpeedY).magnitude; 

        speed = (float)(speed / 6.0) ;

        // Update the Animator speed parameter, I don't think it works though
        animator.SetFloat("Speed", speed);


        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            //other.gameObject.SetActive(false);
            Destroy(other.gameObject);
            audioSource.PlayOneShot(coinSound);
            money++;
            SetCountText();
        }
        if (other.gameObject.CompareTag("Key"))
        {
            //other.gameObject.SetActive(false);
            Destroy(other.gameObject);
            audioSource.PlayOneShot(keySound);
            key++;
            SetCountText();
        }
        if (other.gameObject.CompareTag("Health"))
        {
            //other.gameObject.SetActive(false);
            Destroy(other.gameObject);
            audioSource.PlayOneShot(coinSound);
            currentHealth = Mathf.Min(currentHealth+Mathf.RoundToInt(maxHealth/10), maxHealth);
            SetCountText();
        }
        if (other.gameObject.CompareTag("Sword"))
        {
            //other.gameObject.SetActive(false);
            Destroy(other.gameObject);
            audioSource.PlayOneShot(weaponSound);
            hasHatchet = true;
            sword.SetActive(true);
            SetCountText();
        }
        if (other.gameObject.CompareTag("Sheild"))
        {
            //other.gameObject.SetActive(false);
            Destroy(other.gameObject);
            audioSource.PlayOneShot(weaponSound);
            hasSheild = true;
            sheild.SetActive(true);
            IncreaseHealth(99);
            SetCountText();
        }

        if (other.gameObject.CompareTag("Treasure"))
        {
            other.gameObject.SetActive(false);
            audioSource.PlayOneShot(coinSound);
            hasTreasure = true;
            winCanvas.SetActive(true);
            SetCountText();
        }

    }

    public void TakeDamage(float damage)
    {
        if (isDead || !canDamage) return;

        currentHealth = currentHealth - Mathf.Max(damage - defense, 0);
        SetCountText();
        if (currentHealth <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        if (isDead) return;

        isDead = true;
        canDamage = false;

        audioSource.PlayOneShot(deathSound);

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        GetComponent<PlayerController>().enabled = false;
        
        deathCanvas.SetActive(true);
        countText.text = "";
        StartCoroutine(RevivePlayer());
    }

    private IEnumerator RevivePlayer()
    {
        float timer = respawnTime;

        while (timer > 2)
        {
            respawnText.text = $"Respawning in: {Mathf.Ceil(timer)}";
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        if (animator != null)
        {
            animator.SetTrigger("Revive");
        }

        while (timer > 0)
        {
            respawnText.text = $"Respawning in: {Mathf.Ceil(timer)}";
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        currentHealth = maxHealth;
        SetCountText();

        // Re-enable movement and other actions
        
        GetComponent<PlayerController>().enabled = true;
        isDead = false;
        deathCanvas.SetActive(false);

        if (!hasHatchet || !hasSheild)
            yield return new WaitForSeconds(3f);
        else
            yield return new WaitForSeconds(1f);

        canDamage = true;
    }

    //Store methods
    public void IncreaseHealth(float amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        SetCountText();
    }

    public void IncreaseJump(float amount)
    {
        jumpPower += amount;
    }

    public void IncreaseSprint(float amount)
    {
        runSpeed += amount;
    }

    public void IncreaseSpeed(float amount)
    {
        walkSpeed += amount;
    }

    public void IncreaseDefense(float amount)
    {
        defense += amount;
    }

}
