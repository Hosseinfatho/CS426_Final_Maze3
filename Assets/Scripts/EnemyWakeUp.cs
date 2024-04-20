using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWakeUp : MonoBehaviour
{
    // Start is called before the first frame updateusing UnityEngine;


    public GameObject player; // Reference to the player GameObject
    public GameObject enemy; // Reference to the enemy GameObject
    public Light spotlight; // Reference to the spotlight component of the enemy
    public AudioSource audioSource; // Reference to the audio source component of the enemy

    public float detectionRange = 10f; // Range at which the player triggers the spotlight and audio

    // private bool isPlayerClose = false;

    void Update()
    {
        // Calculate distance between player and enemy
        float distanceToPlayer = Vector3.Distance(player.transform.position, enemy.transform.position);

        // Check if player is within detection range
        if (distanceToPlayer <= detectionRange)
        {
            // Player is close to enemy
            // isPlayerClose = true;
            // Turn on spotlight
            spotlight.enabled = true;
            // Change spotlight color randomly
            spotlight.color = new Color(Random.value, Random.value, Random.value);
            // Play audio
            audioSource.Play();
        }
        else
        {
            // Player is not close to enemy
            // isPlayerClose = false;
            // Turn off spotlight
            spotlight.enabled = false;
            // Stop audio
            audioSource.Stop();
        }
    }
}

