using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    [SerializeField] GameObject playerModel;
    [SerializeField] GameObject cameraContainer;
    [SerializeField] float playerSpeed;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float mouseWheelSensitivity;
    [SerializeField] float cameraSnapBackSpeed;

    // To be removed later:
    [SerializeField] TMP_Text debugTextField;
    [SerializeField] Button resetButton;
    [SerializeField] Button testButton;

    CharacterController controller; //this will be dynamically added to the PlayerContainer
    Vector3 movementVector;
    Vector3 lastGroundPosition; // last position when we were touching ground
    GameObject mainCamera;
    Animator playerAnimator;
    AudioSource audioSource;
    bool snapBackCamera = false; //if set to true, camera will center around the player, when done it will be set to false

    // Used to animate the player rotation
    bool rotatePlayer = false;
    Quaternion playerRotationTarget;

    float playerGravity = -2f;
    float lerpTime = 0;
    float smoothing = 0.25f;

    private void resetPos()
    {
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void testButtonPressed()
    {

    }

    private void log(string msg)
    {
        debugTextField.text += msg + "\n";
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
        playerAnimator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
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
        var currentRotation = roundToNearest90(gameObject.transform.rotation.eulerAngles);

        foreach (Vector3 vector in rotationVectors)
        {
            var rotatedAngle = Quaternion.Euler(currentRotation + vector);

            if (checkGround(rotatedAngle * Vector3.down))
            {
                playerRotationTarget = rotatedAngle;
                rotatePlayer = true;
                return;
            }
        }
    }


    /*
        This doesn't work but there's still a chance
    */
    float roundToNearest90(float angle)
    {
        float remainder = angle % 90;

        if (remainder >= 45)
            return Mathf.Ceil(angle / 90) * 90;
        else if (remainder <= -45)
            return Mathf.Floor(angle / 90) * 90;
        else
            return Mathf.Round(angle / 90) * 90;
    }
    Vector3 roundToNearest90(Vector3 vect)
    {
        vect.x = roundToNearest90(vect.x);
        vect.y = roundToNearest90(vect.y);
        vect.z = roundToNearest90(vect.z);

        return vect;
    }
    void fixPlayerOrientation2()
    {
        Quaternion wantedOrientation = gameObject.transform.rotation;
        wantedOrientation.SetLookRotation(Vector3.zero - gameObject.transform.position);
        wantedOrientation *= Quaternion.Euler(-90, 0, 0);

        Vector3 rotation = roundToNearest90(wantedOrientation.eulerAngles);

        wantedOrientation = Quaternion.Euler(rotation);
        rotatePlayer = true;
        playerRotationTarget = wantedOrientation;
    }

    /*
        Quaterion.Euler returns positive values, so for -10* of rotation it will return 350.
        Sometimes that's not what I want, so this function should help to solve that.
        reverseVectorValues reverses that.
        Using those function to clamp the rotation
    */
    Vector3 fixVectorValues(Vector3 eulerAngles)
    {
        eulerAngles.x = (eulerAngles.x > 180) ? eulerAngles.x - 360 : eulerAngles.x;
        eulerAngles.y = (eulerAngles.y > 180) ? eulerAngles.y - 360 : eulerAngles.y;
        eulerAngles.z = (eulerAngles.z > 180) ? eulerAngles.z - 360 : eulerAngles.z;
        return eulerAngles;
    }
    Vector3 reverseVectorValues(Vector3 eulerAngles)
    {
        eulerAngles.x = (eulerAngles.x < 0) ? eulerAngles.x + 360 : eulerAngles.x;
        eulerAngles.y = (eulerAngles.y < 0) ? eulerAngles.y + 360 : eulerAngles.y;
        eulerAngles.z = (eulerAngles.z < 0) ? eulerAngles.z + 360 : eulerAngles.z;
        return eulerAngles;
    }

    void Update()
    {
        debugTextField.text = "";

        // Vector that points to the ground according to the current player's rotation (not actual ground vector)
        Vector3 groundVector = gameObject.transform.rotation * Vector3.down;

        // On mouse left click / hold, rotate the camera around the cube
        if (Input.GetMouseButton(0))
        {
            float rotationAngleLimit = 90f;
            snapBackCamera = false;

            // Calculate the rotation based on mouse input
            float mouseY = Input.GetAxis("Mouse Y") * -mouseSensitivity;
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;

            // Apply rotation to the camera container
            cameraContainer.transform.Rotate(mouseY, mouseX, 0);

            // Check if rotation not outside the limit
            Vector3 currentRotation = fixVectorValues(cameraContainer.transform.localEulerAngles);
            currentRotation.x = Mathf.Clamp(currentRotation.x, 90 - rotationAngleLimit, 90 + rotationAngleLimit);
            cameraContainer.transform.localRotation = Quaternion.Euler(reverseVectorValues(currentRotation));
        }

        // On mouse right click, center the camera on the player
        if (Input.GetMouseButtonDown(1))
        {
            snapBackCamera = true;
        }
        if (snapBackCamera)
        {
            // Using quaternions because of occasional gimbal lock (endless spinning)
            Quaternion currentRotation = cameraContainer.transform.localRotation;
            Quaternion targetRotation = Quaternion.Euler(Vector3.right * 90);

            if (Quaternion.Angle(currentRotation, targetRotation) < 1)
            {
                snapBackCamera = false;
                cameraContainer.transform.localRotation = targetRotation;
            }
            else
            {
                cameraContainer.transform.localRotation = Quaternion.Lerp(currentRotation, targetRotation, Time.deltaTime * cameraSnapBackSpeed);
            }
        }

        /*
            Allow for player input only when character is "touching" the ground.
            This also sets how far above the ground character is hovering.
        */
        playerAnimator.ResetTrigger("Walking");
        if (checkGround(groundVector, 0.5f))
        {
            lastGroundPosition = gameObject.transform.position;

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            movementVector.Set(x, 0, z);

            if (x != 0 || z != 0)
            {
                // camera is rotated by 90* so we need to remove that
                movementVector = cameraContainer.transform.localRotation * Quaternion.Euler(-90, 0, 0) * movementVector;
                movementVector.y = 0;
            }

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
            if (!checkGround(groundVector) && Vector3.Distance(lastGroundPosition, gameObject.transform.position) > 0.5)
            {
                if (Vector3.Distance(lastGroundPosition, gameObject.transform.position) > 3)
                {
                    fixPlayerOrientation2();
                }
                else
                {
                    fixPlayerOrientation();
                }

                audioSource.Play();
            }

            // this moves the player down towards the face - gravity
            movementVector.y += playerGravity * Time.deltaTime;
        }

        /*
            If player doesn't touch ground and requires rotation
        */
        if (rotatePlayer)
        {
            if (Quaternion.Angle(gameObject.transform.rotation, playerRotationTarget) < 3)
            {
                gameObject.transform.rotation = playerRotationTarget;
                rotatePlayer = false;
            }
            else
            {
                gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, playerRotationTarget, Time.deltaTime * 10);
            }
        }


        /*
            Mouse wheel for camera zooming in / out
        */
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        if (mouseWheel != 0)
        {
            Vector3 position = mainCamera.transform.localPosition;
            position.z += mouseWheel * mouseWheelSensitivity;

            if (position.z <= -30 && position.z >= -120)
            {
                mainCamera.transform.localPosition = position;
            }
        }

        controller.Move(gameObject.transform.rotation * movementVector * Time.deltaTime * playerSpeed);

        /*
            Rotates the player character model
        */
        Vector3 lookDirection = movementVector.normalized;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            playerModel.transform.localRotation = Quaternion.Lerp(playerModel.transform.localRotation, Quaternion.LookRotation(lookDirection.normalized), Mathf.Clamp01(lerpTime * (1 - smoothing)));
        }
        lerpTime += Time.deltaTime;
    }
}
