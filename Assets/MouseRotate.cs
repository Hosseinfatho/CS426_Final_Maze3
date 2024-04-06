using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRotate : MonoBehaviour
{
    // Start is called before the first frame update

    
private Vector3 MousePositionViewport = Vector3.zero;
private Quaternion DesiredRotation = new Quaternion();
private float RotationSpeed = 5;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       MousePositionViewport = Camera.main.ScreenToViewportPoint (Input.mousePosition);
    if (MousePositionViewport.x >= 0.6f) {
        DesiredRotation = Quaternion.Euler (0, 90, 0);
    } else if(MousePositionViewport.x < 0.6f && MousePositionViewport.x > 0.4f){
        DesiredRotation = Quaternion.Euler (0, 270, 0);
    }else {
        DesiredRotation = Quaternion.Euler (0, 180, 0);
    }
    transform.rotation = Quaternion.Lerp (transform.rotation, DesiredRotation, Time.deltaTime*RotationSpeed);
  
    }
}

