using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public class AnimatorController : MonoBehaviour
    {
        public Animator animator;

        public void UpdateSpeed(float speed)
        {
            animator.SetFloat("Speed", speed);
        }

        public void TriggerAttack()
        {
            animator.SetTrigger("Attack");
        }

        public void SetIsJumping(bool isJumping)
        {
            animator.SetBool("IsJumping", isJumping);
        }
    }

}
