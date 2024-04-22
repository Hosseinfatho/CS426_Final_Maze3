using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShaderController : MonoBehaviour
{
    public float changeInterval = 5f; // Time interval for changing materials
    public Material reflectiveMaterial; // Material with the reflective shader
    public Material toonMaterial; // Material with the toon shader

    private Renderer enemyRenderer; // Reference to the enemy's renderer
    private Material originalMaterial; // Original material of the enemy
    private bool isToonMaterialActive = false; // Flag to track which material is currently active
    private float timeSinceLastChange = 0f; // Time elapsed since the last material change

    void Start()
    {
        // Get the renderer component attached to the enemy
        enemyRenderer = GetComponent<Renderer>();

        // Store the original material
        originalMaterial = enemyRenderer.material;

        // Start with the reflective material
        enemyRenderer.material = reflectiveMaterial;
    }

    void Update()
    {
        // Increment time since last change
        timeSinceLastChange += Time.deltaTime;

        // Check if it's time to change material
        if (timeSinceLastChange >= changeInterval)
        {
            // Toggle between reflective and toon materials
            if (isToonMaterialActive)
            {
                enemyRenderer.material = reflectiveMaterial;
                isToonMaterialActive = false;
            }
            else
            {
                enemyRenderer.material = toonMaterial;
                isToonMaterialActive = true;
            }

            // Reset time since last change
            timeSinceLastChange = 0f;
        }
    }
}

