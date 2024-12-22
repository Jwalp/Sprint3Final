using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //I wanted to have all of the enmies scripts here so I just made a list for it
    public enum EnemyType { Patrol, Aggressive }
    public EnemyType enemyType;

    public GameObject player;

    //Stats
    public float chaseSpeed = 5f;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float returnSpeed = 3f;
    public float maxHealth = 50f;
    private float currentHealth;

    //Loot
    public GameObject lootPrefab;
    public int lootDropCount = 3;
    public float lootDropRadius = 2f;

    //Animator and aother needed things
    private Animator animator;
    private bool isChasing = false;
    private bool isDead = false;

    //Other Vars
    private Vector3 originalPosition;
    private EnemyAttack enemyAttack;
    private PlayerController playerController;

    void Start()
    {
        currentHealth = maxHealth;
        originalPosition = transform.position;

        animator = GetComponent<Animator>();
        enemyAttack = GetComponent<EnemyAttack>();
        playerController = player.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (isDead) return;

        if (enemyType == EnemyType.Patrol && !isChasing)
        {
            PatrolBehavior();
        }

        if (enemyType == EnemyType.Aggressive || isChasing)
        {
            AggressiveBehavior();
        }
    }

    private void PatrolBehavior()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= detectionRange && !playerController.isDead)
        {
            isChasing = true;
        }
        else if (Vector3.Distance(transform.position, originalPosition) > 0.1f || playerController.isDead)
        {
            animator.SetBool("IsMoving", true);
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, returnSpeed * Time.deltaTime);
            FaceTarget(originalPosition);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
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
            EnemyAttack enemyAttackComponent = GetComponent<EnemyAttack>();
            if (enemyAttackComponent != null)
            {
                enemyAttackComponent.TriggerAttack(player);
            }
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
    }

}