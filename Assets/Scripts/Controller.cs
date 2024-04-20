using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    [SerializeField] GameObject playerModel;
    [SerializeField] GameObject cameraContainer;
    [SerializeField] float playerSpeed;
    [SerializeField] float mouseSensitivity;

    // To be removed later:
    [SerializeField] TMP_Text debugTextField;
    [SerializeField] Button resetButton;
    [SerializeField] Button testButton;

    CharacterController controller; //this will be dynamically added to the PlayerContainer
    Vector3 movementVector;
    Vector3 lastGroundPosition; // last position when we were touching ground
    GameObject mainCamera;
    GameObject cameraTopPlane;
    Animator playerAnimator;
    AudioSource jumpSound;

    float playerGravity = -2f;
    float lerpTime = 0;
    float smoothing = 0.25f;

    private void resetPos()
    {
        controller.enabled = false;
        gameObject.transform.position = new Vector3(-8, 10.5f, 1);
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        controller.enabled = true;
    }

    private void testButtonPressed()
    {
        fixPlayerRotation();
    }

    GameObject FindChildWithTag(GameObject parent, string tag)
    {
        GameObject child = null;
        foreach (Transform transform in parent.transform)
        {
            if (transform.CompareTag(tag))
            {
                child = transform.gameObject;
                break;
            }
        }
        return child;
    }


    private void Start()
    {
        controller = gameObject.AddComponent<CharacterController>();
        controller.radius = 1;
        controller.height = 1;
        resetButton.onClick.AddListener(resetPos);
        testButton.onClick.AddListener(testButtonPressed);
        mainCamera = FindChildWithTag(cameraContainer, "MainCamera");
        cameraTopPlane = FindChildWithTag(cameraContainer, "CameraTopPlane");
        playerAnimator = GetComponentInChildren<Animator>();
        jumpSound = GetComponent<AudioSource>();
    }

    /*
        This makes a line from player position (transform.position), with direction of the vector (vector), length of that line, layerMask
        Returns true if this new line intersects with layerMask
        So to detect the "ground", cube must be made of layer called "Ground"
    */
    private bool checkGround(Vector3 vector, float distance = 10.0f, string layerName = "Ground")
    {
        return Physics.Raycast(gameObject.transform.position, vector, distance, 1 << LayerMask.NameToLayer(layerName));
    }

    private Vector3[] rotationVectors = { new Vector3(90, 0, 0), new Vector3(-90, 0, 0), new Vector3(0, 90, 0), new Vector3(0, -90, 0), new Vector3(0, 0, 90), new Vector3(0, 0, -90) };
    private void fixPlayerOrientation()
    {
        var currentRotation = gameObject.transform.rotation.eulerAngles;

        foreach (Vector3 vector in rotationVectors)
        {
            var rotatedAngle = Quaternion.Euler(currentRotation + vector);

            if (checkGround(rotatedAngle * Vector3.down))
            {
                gameObject.transform.rotation = rotatedAngle;
                return;
            }
        }
    }

    private bool cameraInSight()
    {
        RaycastHit hit;

        if (Physics.Raycast(mainCamera.transform.position, gameObject.transform.position - mainCamera.transform.position, out hit, 100))
        {
            return hit.collider.tag == "Player";
        }

        return false;
    }

    // This is still broken
    private void fixPlayerRotation()
    {
        if (!cameraInSight()) return;

        Quaternion lookRot = Quaternion.LookRotation((cameraTopPlane.transform.position - gameObject.transform.position).normalized, getPlayerUpVector());
        gameObject.transform.rotation = Quaternion.Euler(_roundVector(lookRot.eulerAngles));
    }




    // Rounds the given number to the closest right angle value (so 44* is rounded to 0, and 46* rounded to 90)
    private float _round(float value)
    {
        float sign = 1;
        if (value < 0) sign = -1;

        float mult = 0;

        while (Math.Abs(value) >= 45)
        {
            mult++;
            value -= 90 * sign;
        }

        return 90 * mult * sign;
    }

    private Vector3 _roundVector(Vector3 vect)
    {
        vect.Set(_round(vect.x), _round(vect.y), _round(vect.z));
        return vect;
    }

    // 
    private Vector3 _normalizedMaxVector(Vector3 vect)
    {
        float max = vect.x;

        if (Math.Abs(vect.y) > Math.Abs(max)) max = vect.y;
        if (Math.Abs(vect.z) > Math.Abs(max)) max = vect.z;

        if (vect.x != max) vect.x = 0;
        if (vect.y != max) vect.y = 0;
        if (vect.z != max) vect.z = 0;

        return vect.normalized;
    }

    private Vector3 getPlayerUpVector()
    {
        return _normalizedMaxVector(gameObject.transform.position);
    }


    void Update()
    {
        debugTextField.text = "";

        // Vector that points to the ground according to the current player's rotation (not actual ground vector)
        Vector3 groundVector = gameObject.transform.rotation * Vector3.down;

        // On mouse left click / hold, rotate the camera around the cube
        if (Input.GetMouseButton(0))
        {
            cameraContainer.transform.Rotate(Input.GetAxis("Mouse Y") * -mouseSensitivity, Input.GetAxis("Mouse X") * mouseSensitivity, 0);
        }

        // On mouse right click, center the camera on the player
        if (Input.GetMouseButtonDown(1))
        {
            cameraContainer.transform.position = groundVector * -1;
            cameraContainer.transform.rotation = gameObject.transform.rotation;
            cameraContainer.transform.LookAt(Vector3.zero);
        }

        /*
            Allow for player input only when character is "touching" the ground.
            This also sets how far above the ground character is hovering.
        */
        playerAnimator.ResetTrigger("Walking");
        if (checkGround(groundVector, 0.5f))
        {
            lastGroundPosition = gameObject.transform.position;
            movementVector.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            movementVector = Vector3.ClampMagnitude(movementVector, 1);


            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                playerAnimator.SetTrigger("Walking");
            }

            if (movementVector == Vector3.zero)
            {
                lerpTime = 0;
            }
        }
        else
        {
            if (!checkGround(groundVector) && Vector3.Distance(lastGroundPosition, gameObject.transform.position) > 1)
            {
                fixPlayerOrientation();
                jumpSound.Play();
                //fixPlayerRotation();
            }

            // this moves the player down towards the face - gravity
            movementVector.y += playerGravity * Time.deltaTime;
        }

        /*
            Mouse wheel for camera zooming in / out
        */
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        if (mouseWheel != 0)
        {
            Vector3 position = mainCamera.transform.localPosition;
            position.z += mouseWheel * 15;

            if (position.z <= -20 && position.z >= -50)
            {
                mainCamera.transform.localPosition = position;
            }
        }

        controller.Move(gameObject.transform.rotation * movementVector * Time.deltaTime * playerSpeed);

        Vector3 lookDirection = movementVector.normalized;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            playerModel.transform.localRotation = Quaternion.Lerp(playerModel.transform.localRotation, Quaternion.LookRotation(lookDirection.normalized), Mathf.Clamp01(lerpTime * (1 - smoothing)));
        }
        lerpTime += Time.deltaTime;

    }
}
