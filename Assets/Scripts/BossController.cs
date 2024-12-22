using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public GameObject player;

    // Stats
    public float chaseSpeed = 5f;
    public float detectionRange = 20f;
    public float attackRange = 2f;
    public float maxHealth = 100f;
    private float currentHealth;

    // Loot
    public GameObject lootPrefab;
    public int lootDropCount = 3;
    public float lootDropRadius = 2f;

    // Animator and other necessary things
    private Animator animator;
    private bool isChasing = false;
    private bool isDead = false;

    // Attack variables, combined for Boss
    public Collider attackHitbox;
    public float attackDamage = 30f;
    public float waitDuration = 1f;
    public float animationDuration = 1.75f;
    private bool isAttacking = false;
    private PlayerController playerController;

    // Rage Mode Variables
    public float rageHealthThreshold1 = 66f;
    public float rageHealthThreshold2 = 33f;
    public float rageDuration = 7f;
    private bool isInRage = false;
    private bool isInSecondRage = false;
    private bool inAnnimation = false;
    private bool hasRaged = false;
    private bool hasSecondRaged = false;
    private float originalSpeed;
    private float originalAttackCooldown;
    private float originalDamage;
    private float originalDetectRange;
    public GameObject shieldBubble;


    //Audio
    private AudioSource audioSource;
    public AudioClip madSound;
    public AudioClip attackSound;
    public AudioClip breakSound;
    private BGSongChange music;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        playerController = player.GetComponent<PlayerController>();
        originalSpeed = chaseSpeed;
        originalAttackCooldown = animationDuration;
        originalDamage = attackDamage;
        originalDetectRange = detectionRange;

        audioSource = GetComponent<AudioSource>();
        music = FindObjectOfType<BGSongChange>();
        shieldBubble.SetActive(false);
    }

    void Update()
    {
        if (isDead || inAnnimation) return;

        if (currentHealth <= maxHealth * rageHealthThreshold1 / 100f && !isInRage && !hasRaged)
        {
            hasRaged = true;
            EnterRageMode(rageHealthThreshold1);
        }
        else if (currentHealth <= maxHealth * rageHealthThreshold2 / 100f && !isInSecondRage && !hasSecondRaged)
        {
            hasSecondRaged = true;
            EnterRageMode(rageHealthThreshold2);
        }

        AggressiveBehavior();
    }

    private void AggressiveBehavior()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        RaycastHit hit;

        bool hasLineOfSight = Physics.Raycast(transform.position, directionToPlayer, out hit) && (hit.collider.tag == "Ground" || hit.collider.tag == "Player");

        if (distanceToPlayer <= attackRange && hasLineOfSight && !playerController.isDead)
        {
            animator.SetBool("IsMoving", false);
            TriggerAttack(player);
        }
        else if (distanceToPlayer <= detectionRange && hasLineOfSight && !playerController.isDead)
        {
            ChasePlayer();
        }
        else
        {
            isChasing = false;
            animator.SetBool("IsMoving", false);
        }
    }

    private void ChasePlayer()
    {
        Vector3 targetPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, chaseSpeed * Time.deltaTime);
        FaceTarget(targetPosition);
        animator.SetBool("IsMoving", true);
    }

    private void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        if (isInRage || isInSecondRage)
        {
            damage *= 0.2f;
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        audioSource.PlayOneShot(breakSound);
        StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        for (int i = 0; i < lootDropCount; i++)
        {
            if (lootPrefab != null)
            {
                Vector3 randomPosition = transform.position + new Vector3(Random.Range(-lootDropRadius, lootDropRadius), 1.5f, Random.Range(-lootDropRadius, lootDropRadius));
                Instantiate(lootPrefab, randomPosition, Quaternion.identity);
            }
        }

        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
        music.IncrementTeleportCount();
    }

    // Rage Mode makes Bosses stats more powerful for a short amount of time
    private void EnterRageMode(float rageHealthThreshold)
    {
        inAnnimation = true;
        if (rageHealthThreshold == rageHealthThreshold1)
        {
            isInRage = true;
            animator.SetTrigger("Mad");
            chaseSpeed *= 2f;
            animationDuration /= 2f;
            attackDamage *= 2f;
            detectionRange *= 2f;

            if (shieldBubble != null)
            {
                shieldBubble.SetActive(true);
            }

            StartCoroutine(RageAnimation());
        }
        else if (rageHealthThreshold == rageHealthThreshold2)
        {
            isInSecondRage = true;
            animator.SetTrigger("Mad");
            chaseSpeed *= 3f;
            animationDuration /= 3f;
            attackDamage *= 3f;
            detectionRange *= 3f;

            if (shieldBubble != null)
            {
                shieldBubble.SetActive(true);
            }

            StartCoroutine(RageAnimation());
        }
    }

    private IEnumerator RageAnimation()
    {
        yield return new WaitForSeconds(1f);
        audioSource.PlayOneShot(madSound);
        yield return new WaitForSeconds(1.667f);
        inAnnimation = false;
        StartCoroutine(RageDuration());
    }

    private IEnumerator RageDuration()
    {
        yield return new WaitForSeconds(rageDuration);

        ExitRageMode();
    }

    private void ExitRageMode()
    {
        isInRage = false;
        isInSecondRage = false;

        // Reset speed and attack cooldown
        chaseSpeed = originalSpeed;
        animationDuration = originalAttackCooldown;
        attackDamage = originalDamage;
        detectionRange = originalDetectRange;

        if (shieldBubble != null)
        {
            shieldBubble.SetActive(false);
        }
    }

    // From Enemy Attack
    public void TriggerAttack(GameObject player)
    {
        playerController = player.GetComponent<PlayerController>();
        if (isAttacking) return;

        if (playerController.isDead) return;
        StartCoroutine(PerformAttack(player));
    }

    private IEnumerator PerformAttack(GameObject player)
    {
        isAttacking = true;

        if (attackHitbox != null)
        {
            attackHitbox.enabled = true;
        }

        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        audioSource.PlayOneShot(attackSound);

        yield return new WaitForSeconds(waitDuration);

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeDamage(attackDamage);
        }

        yield return StartCoroutine(ResetAttackHitbox());

        isAttacking = false;
    }

    private IEnumerator ResetAttackHitbox()
    {
        yield return new WaitForSeconds(animationDuration);

        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }
    }
}
