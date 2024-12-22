using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public Collider attackHitbox;
    public float attackDamage = 10f;
    public float waitDuration = 0f;  // Delay before starting the attack, to try to time with the annimation
    public float animationDuration = 1f;  // Duration of the attack animation
    private bool isAttacking = false; // Cooldown for attacking
    private PlayerController playerController;


    void Start()
    {
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }
    }

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
