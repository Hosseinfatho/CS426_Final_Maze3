using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController1 : MonoBehaviour
{
        public float rotationSpeed = 50f; // Speed of rotation in degrees per second

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 rotation = new Vector3(verticalInput, horizontalInput, 0f) * rotationSpeed * Time.deltaTime;
        // Apply rotation
        transform.Rotate(rotation);

    }
}
