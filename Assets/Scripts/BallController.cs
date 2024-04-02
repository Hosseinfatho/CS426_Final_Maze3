using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float forceMagnitude = 500f; // Adjust the force magnitude as needed

    private Rigidbody rb; // To store the Rigidbody component

    void Start()
    {
        // Get the Rigidbody component attached to this GameObject
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check for input to determine when to apply the force
        if (Input.GetKeyDown(KeyCode.Space)) // Using spacebar as an example input
        {
            // Apply an upward force
            rb.AddForce(Vector3.up * forceMagnitude);
        }

        // Example to move the ball forward
        if (Input.GetKeyDown(KeyCode.W)) // Using W as an example input
        {
            rb.AddForce(Vector3.forward * forceMagnitude);
        }
    }
}