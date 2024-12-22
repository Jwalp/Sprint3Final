using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int damageDealt = 10;

    private Animator animator;

    private bool isAttacking = false; // For spamming purposes
    private bool isSecondAttackTriggered = false;
    public bool canPlaySound = true;
    private bool canPlaySecondSound = true;

    // Reference to the required weapon/tool object
    public GameObject sword;
    public Collider swordHitbox;

    private AudioSource audioSource;
    public AudioClip attack1Sound;
    public AudioClip attack2Sound;

    private PlayerController playerController;
    private Grab grab;
    bool hasGrab;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerController = GetComponent<PlayerController>();
        grab = GetComponent<Grab>();
        hasGrab = grab.hasGrab;
    }

    void Update()
    {
        HandleAttackInput();
    }

    private void HandleAttackInput()
    {
        if (sword == null || !sword.activeInHierarchy)
        {
            return;
        }


        if (isSecondAttackTriggered)
        {
            return;
        }

        if (playerController.isDead) {
            return;
        }

        hasGrab = grab.hasGrab;
        if (Input.GetMouseButtonDown(0) && !hasGrab) //Has grab is there so player can't attack while holding something
        {
            if (!isAttacking)
            {
                animator.SetTrigger("Attack1");
                if (canPlaySound)
                {
                    audioSource.PlayOneShot(attack1Sound);
                    canPlaySound = false;
                }
                swordHitbox.enabled = true;
                isAttacking = true;

                StartCoroutine(AttackCooldown(0.667f)); //Animation length
            }
            else if (!isSecondAttackTriggered) //There was two annimations so I wanted to utilize both
            {
                animator.SetTrigger("Attack2");
                if (canPlaySecondSound)
                {
                    audioSource.PlayOneShot(attack2Sound);
                    canPlaySound = false;
                    canPlaySecondSound = false;
                }
                swordHitbox.enabled = true;
                isSecondAttackTriggered = true;
                StartCoroutine(SecondAttackCooldown(1.333f));
            }
        }
    }

    
    private IEnumerator AttackCooldown(float annimationDuration)
    {
        yield return new WaitForSeconds(annimationDuration);
        isAttacking = false;
        canPlaySound = true;
        if (!isSecondAttackTriggered) swordHitbox.enabled = false;
    }

    private IEnumerator SecondAttackCooldown(float annimationDuration)
    {
        yield return new WaitForSeconds(annimationDuration);
        swordHitbox.enabled = false;
        isSecondAttackTriggered = false;
        canPlaySecondSound = true;
    }

    void OnTriggerEnter(Collider other)
    {        
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyController>().TakeDamage(damageDealt);
        }
        else if (other.gameObject.CompareTag("Boss"))
            {
                other.gameObject.GetComponent<BossController>().TakeDamage(damageDealt);
            }
    }

    public void IncreaseDamage(float amount)
    {
        damageDealt += (int)amount;
    }
}