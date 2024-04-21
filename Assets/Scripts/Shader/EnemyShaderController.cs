using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShaderController : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    public float activationRange = 5f; // Range within which the player can activate the shader change
    public Shader reflectionShader; // Reflection shader for the enemy
    public Shader toonShader; // Toon shader for the enemy

    private Renderer enemyRenderer; // Reference to the enemy's renderer
    private Shader originalShader; // Original shader of the enemy

    void Start()
    {
        // Find the player GameObject by tag
        player = GameObject.FindGameObjectWithTag("Player");

        // Get the renderer component attached to the enemy
        enemyRenderer = GetComponent<Renderer>();

        // Store the original shader
        originalShader = enemyRenderer.material.shader;
    }

    void Update()
    {
        // Check if the player is nearby
        if (Vector3.Distance(player.transform.position, transform.position) <= activationRange)
        {
            // Change the shader to toon shader
            enemyRenderer.material.shader = toonShader;
        }
        else
        {
            // Revert to the original shader
            enemyRenderer.material.shader = originalShader;
        }
    }
}
