using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Hossein Prepare this code for changing shader between toon and reflection by closeness of player to enemy
/// </summary>

public class DynamicShaderSwitch : MonoBehaviour
{
    public Transform player; // Reference to the player transform
    public Transform Robot; // Reference to the Robot transform
    public float switchDistance = 5f; // Distance threshold for switching shaders
    public Shader toonShader; // Toon shader
    public Shader reflectiveShader; // Reflective shader

    private Renderer RobotRenderer;

    void Start()
    {
        // Get the renderer component of the Robot
        RobotRenderer = Robot.GetComponent<Renderer>();

        // Ensure both shaders are assigned
        if (toonShader == null || reflectiveShader == null)
        {
            Debug.LogError("One or both shaders are not assigned!");
            return;
        }

        // Set the initial shader to the toon shader
        RobotRenderer.material.shader = toonShader;
    }

    void Update()
    {
        if (player == null || Robot == null)
            return;

        // Calculate the distance between the player and the Robot
        float distance = Vector3.Distance(player.position, Robot.position);

        // Switch shaders based on distance
        if (distance < switchDistance)
        {
            // Interpolate between the two shaders based on distance
            float t = distance / switchDistance;
            RobotRenderer.material.shader = InterpolateShaders(toonShader, reflectiveShader, t);
        }
        else
        {
            // If distance is greater than switchDistance, use the toon shader
            RobotRenderer.material.shader = toonShader;
        }
    }

    Shader InterpolateShaders(Shader shaderA, Shader shaderB, float t)
    {
        // Simple shader interpolation
        return t < 0.5f ? shaderA : shaderB;
    }
}
