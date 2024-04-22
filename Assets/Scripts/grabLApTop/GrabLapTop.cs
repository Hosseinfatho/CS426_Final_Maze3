using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabLapTop : MonoBehaviour
{
private GameObject player;
    private bool isGrabbed = false;

    void Start()
    {
        // Find the player GameObject by tag
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        // Check if the player is close enough to grab the object
        if (Vector3.Distance(transform.position, player.transform.position) < 1.0f)
        {
            // Input handling
            if (Input.GetKeyDown(KeyCode.T))
            {
                // Grab the object
                isGrabbed = true;
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                // Release the object
                isGrabbed = false;
            }
        }

        // Move the object with the player if it's grabbed
        if (isGrabbed)
        {
            transform.position = player.transform.position;
        }
    }
}
