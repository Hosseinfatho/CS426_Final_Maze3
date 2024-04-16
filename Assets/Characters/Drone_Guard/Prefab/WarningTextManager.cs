using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningTextManager : MonoBehaviour
{
    public Transform playerTransform;
    public Transform flockManagerTransform;
    public float warningDistance = 5f;
    public string warningMessage = "Be careful!";
    
    private Text warningText;

    void Start()
    {
        // Ensure that this GameObject has a Text component
        warningText = gameObject.GetComponent<Text>();
        if (warningText == null)
        {
            Debug.LogError("WarningTextManager script requires a Text component on the same GameObject.");
            enabled = false; // Disable the script if Text component is not found
        }
    }

    void Update()
    {
        if (warningText == null)
            return;

        // Calculate the distance between player and flock manager
        float distance = Vector3.Distance(playerTransform.position, flockManagerTransform.position);

        // If the player is close to the flock manager, show the warning text
        if (distance < warningDistance)
        {
            // Enable the warning text
            warningText.enabled = true;

            // Set the warning message
            warningText.text = warningMessage;

            // Position the warning text above the player's head
            warningText.transform.position = playerTransform.position + Vector3.up * 2f;
        }
        else
        {
            // Disable the warning text if the player is not close to the flock manager
            warningText.enabled = false;
        }
    }
}