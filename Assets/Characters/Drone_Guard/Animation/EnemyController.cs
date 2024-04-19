using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyController : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    public Animator animator; // Reference to the Animator component of the enemy
    public float attackRange = 5f; // Range at which the player triggers the attack animation

    private bool isPlayerInRange = false;

    void Update()
    {
        // Calculate distance between player and enemy
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        // Check if player is within attack range
        if (distanceToPlayer <= attackRange)
        {
            // Player is in attack range
            isPlayerInRange = true;
            // Trigger attack animation
            animator.SetBool("isAttacking", true);
        }
        else
        {
            // Player is not in attack range
            isPlayerInRange = false;
            // Set back to idle animation
            animator.SetBool("isAttacking", false);
        }
    }

    // You can use OnTriggerEnter to trigger the attack animation when the player enters the enemy's trigger collider
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            // Player entered the trigger collider of the enemy
            animator.SetBool("isAttacking", true);
        }
    }

    // You can use OnTriggerExit to reset the animation when the player exits the enemy's trigger collider
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            // Player exited the trigger collider of the enemy
            animator.SetBool("isAttacking", false);
        }
    }
}

