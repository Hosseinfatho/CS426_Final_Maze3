using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LampController : MonoBehaviour
{
    public GameObject lampLight; // Reference to the light component of the lamp
    public float activationRange = 5f; // Range within which the player can activate the lamp

    private bool isPlayerNearby = false;

    private void Update()
    {
        // Check if the player is nearby and press the activation button (e.g., 'E')
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            // Toggle the lamp light
            lampLight.SetActive(!lampLight.activeSelf);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering collider is the player
        if (other.CompareTag("Player"))
        {
            // Player is nearby
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the exiting collider is the player
        if (other.CompareTag("Player"))
        {
            // Player is no longer nearby
            isPlayerNearby = false;
        }
    }
}