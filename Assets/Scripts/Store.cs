using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    public float interactionRange = 3.0f;    // Distance at which the store can be interacted with
    public GameObject interactionHintUI;     // UI to show the "Press E to Open Store" hint
    public GameObject storeCanvas;           // The store canvas that will open when interacted with    
    public Transform playerTransform;        // Reference to the player's transform (assigned in Inspector)
    public TextMeshProUGUI feedbackText;     // Text to show feedback (e.g., "Not enough money")
    public TextMeshProUGUI coinCountText;    // Text to show the player's current coin count

    private PlayerController playerController; // Reference to the PlayerController to get money
    public PlayerAttack playerAttack;
    private bool isStoreOpen = false;          // Track if the store is currently open
    private AudioSource audioSource; // AudioSource for door sound
    public AudioClip oofSound;
    public AudioClip buySound;

    // Item prices (set according to your game logic)
    public int healthPrice = 5;
    public int jumpPrice = 2;
    public int sprintPrice = 2;
    public int speedPrice = 3;
    public int defensePrice = 3;
    public int damagePrice = 5;
    public TextMeshProUGUI healthPriceText;
    public TextMeshProUGUI jumpPriceText;
    public TextMeshProUGUI sprintPriceText;
    public TextMeshProUGUI speedPriceText;
    public TextMeshProUGUI defensePriceText;
    public TextMeshProUGUI damagePriceText;

    void Start()
    {
        playerController = playerTransform.GetComponent<PlayerController>();
        playerAttack = playerTransform.GetComponent<PlayerAttack>();
        audioSource = GetComponent<AudioSource>();
        UpdateCoinCount();
    }

    void Update()
    {
        // Find all store objects in the scene
        GameObject[] stores = GameObject.FindGameObjectsWithTag("Store");
        GameObject nearestStore = null;
        float nearestDistance = Mathf.Infinity;

        // Find the nearest store
        foreach (GameObject store in stores)
        {
            float distance = Vector3.Distance(playerTransform.position, store.transform.position);
            if (distance < nearestDistance && distance <= interactionRange)
            {
                nearestStore = store;
                nearestDistance = distance;
            }
        }

        if (nearestStore != null && nearestDistance <= interactionRange)
        {
            interactionHintUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (isStoreOpen)
                {
                    CloseStore();
                }
                else
                {
                    OpenStore();
                }
            }
        }
        else
        {
            interactionHintUI.SetActive(false);
            if (isStoreOpen)
            {
                CloseStore();
            }
        }
    }

    void OpenStore()
    {
        storeCanvas.SetActive(true);
        interactionHintUI.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UpdateCoinCount();
        UpdatePriceText();

        isStoreOpen = true;
    }


    void UpdatePriceText()
    {
        healthPriceText.text = "Price: " + healthPrice.ToString();
        jumpPriceText.text = "Price: " + jumpPrice.ToString();
        sprintPriceText.text = "Price: " + sprintPrice.ToString();
        speedPriceText.text = "Price: " + speedPrice.ToString();
        defensePriceText.text = "Price: " + defensePrice.ToString();
        damagePriceText.text = "Price: " + damagePrice.ToString();
    }

    void CloseStore()
    {
        feedbackText.text = "Press E to interact";
        storeCanvas.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isStoreOpen = false;
    }

    void UpdateCoinCount()
    {
        if (coinCountText != null)
        {
            coinCountText.text = "Current Coin Count: " + playerController.money.ToString();
        }
    }

    public bool CanAffordItem(float itemPrice)
    {
        if (playerController != null && playerController.money >= itemPrice)
        {
            return true;
        }
        else { 
        audioSource.PlayOneShot(oofSound);
        return false;
        }
    }

    public bool MakePurchase(int itemPrice, string itemName)
    {
        if (CanAffordItem(itemPrice))
        {
            audioSource.PlayOneShot(buySound);
            playerController.money = playerController.money - itemPrice;
            playerController.SetCountText();
            feedbackText.text = itemName + " purchased successfully!";
            UpdateCoinCount();
            return true;
        }
        else
        {
            feedbackText.text = "Not enough money to purchase " + itemName;
            return false;
        }
    }

    // Methods for each item button
    public void BuyHealth()
    {
        if (!MakePurchase(healthPrice, "Health Boost")) return;
        playerController.IncreaseHealth(10);
        healthPrice *= 2;
        UpdatePriceText();
    }

    public void BuyJump()
    {
        if (!MakePurchase(jumpPrice, "Jump Boost")) return;
        playerController.IncreaseJump(1);
        jumpPrice *= 2;
        UpdatePriceText();
    }

    public void BuySprint()
    {
        if (!MakePurchase(sprintPrice, "Sprint Boost")) return;
        playerController.IncreaseSprint(2);
        sprintPrice *= 2;
        UpdatePriceText();
    }

    public void BuySpeed()
    {
        if (!MakePurchase(speedPrice, "Speed Boost")) return;
        playerController.IncreaseSpeed(0.75f);
        speedPrice *= 2;
        UpdatePriceText();
    }

    public void BuyDefense()
    {
        if (!MakePurchase(defensePrice, "Defense Boost")) return;
        playerController.IncreaseDefense(2);
        defensePrice *= 2;
        UpdatePriceText();
    }

    public void BuyDamage()
    {
        if (!MakePurchase(damagePrice, "Damage Boost")) return;
        playerAttack.IncreaseDamage(5);
        damagePrice *= 2;
        UpdatePriceText();
    }
}
