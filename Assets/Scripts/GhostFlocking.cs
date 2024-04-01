using UnityEngine;

public class GhostFlocking : MonoBehaviour
{
    public float moveSpeed = 2.0f; // Speed of the ghost
    public float rotationSpeed = 1.0f; // Speed of rotation

    void Update()
    {
        // Move the ghost forward
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // Apply flocking behavior (You'll need to implement this)
        ApplyFlocking();
    }

    void ApplyFlocking()
    {
        // Implement flocking algorithm here
        // This method should calculate and apply the appropriate forces for cohesion, separation, and alignment
        // You may need references to neighboring ghosts to calculate these forces
    }
}