using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILIGHT : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    public Light lampLight; // Reference to the light component of the lamp
    public float activationRange = 5f; // Range within which the player can activate the lamp
    public float flickerIntensity = 0.5f; // Intensity of the flicker effect
    public float flickerSpeed = 1f; // Speed of the flicker effect

    private float baseIntensity; // Base intensity of the lamp light

    private void Start()
    {
        // Store the base intensity of the lamp light
        baseIntensity = lampLight.intensity;

        // Start the flickering coroutine
        StartCoroutine(Flicker());
    }

    private IEnumerator Flicker()
    {
        while (true)
        {
            // Check if the player is nearby
            if (Vector3.Distance(player.transform.position, transform.position) <= activationRange)
            {
                // Randomly adjust the intensity of the lamp light
                float flicker = Random.Range(-flickerIntensity, flickerIntensity);
                lampLight.intensity = baseIntensity + flicker;
            }
            else
            {
                // Reset the intensity to the base value if the player is not nearby
                lampLight.intensity = baseIntensity;
            }

            // Wait for a short duration
            yield return new WaitForSeconds(flickerSpeed);
        }
    }
}