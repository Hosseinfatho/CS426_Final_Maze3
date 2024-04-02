using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    [SerializeField]
    private GameObject playerContainer;
    [SerializeField]
    private GameObject playerCharacter;
    [SerializeField]
    private GameObject mainCamera;
    [SerializeField]
    private float playerSpeed;
    [SerializeField]
    private float playerGravity;
    [SerializeField]
    private float mouseSensitivity;

    // To be removed later:
    [SerializeField]
    private TMP_Text debugTextField;
    [SerializeField]
    private Button resetButton;
    [SerializeField]
    private Button testButton;

    private CharacterController controller; //this will be dynamically added to the PlayerContainer
    private Vector3 movementVector;
    private Vector3 lastGroundPosition; // last position when we were touching ground


    private void resetPos()
    {
        controller.enabled = false;
        playerContainer.transform.position = new Vector3(0, 6, 0);
        playerContainer.transform.rotation = Quaternion.Euler(0, 0, 0);
        controller.enabled = true;
    }

    private void testButtonPressed()
    {

    }

    private void Start()
    {
        controller = playerContainer.AddComponent<CharacterController>();
        controller.radius = 0.5f;
        controller.height = 1;
        resetButton.onClick.AddListener(resetPos);
        testButton.onClick.AddListener(testButtonPressed);
    }

    /*
        This makes a line from player position (transform.position), with direction of the vector (vector), length of that line, layerMask
        Returns true if this new line intersects with layerMask
        So to detect the "ground", cube must be made of layer called "Ground"
    */
    private bool checkGround(Vector3 vector, float distance = 10.0f)
    {
        return Physics.Raycast(playerContainer.transform.position, vector, distance, 1 << LayerMask.NameToLayer("Ground"));
    }

    /*
        If ground not detected, then player possibly went over the edge,
        so we need to rotate the player so we know where the "new" ground is.

        Assumes that at any point it doesn't require to rotate more than 90* in any direction.
        Rotates only in one axis at the time.

        [If you didn't notice, this function is not very smart]
    */
    private Vector3[] rotationVectors = { new Vector3(90, 0, 0), new Vector3(-90, 0, 0), new Vector3(0, 90, 0), new Vector3(0, -90, 0), new Vector3(0, 0, 90), new Vector3(0, 0, -90) };
    private void fixPlayerRotation()
    {
        var currentRotation = playerContainer.transform.rotation.eulerAngles;

        foreach (Vector3 vector in rotationVectors)
        {
            var rotatedVector = currentRotation + vector;

            if (checkGround(Quaternion.Euler(rotatedVector) * Vector3.down))
            {
                playerContainer.transform.rotation = Quaternion.Euler(rotatedVector);
            }
        }
    }

    void Update()
    {
        // Vector that points to the ground
        Vector3 groundVector = playerContainer.transform.rotation * Vector3.down;

        // On mouse left click / hold, rotate the camera around the cube
        if (Input.GetMouseButton(0))
        {
            mainCamera.transform.Rotate(Input.GetAxis("Mouse Y") * -mouseSensitivity, Input.GetAxis("Mouse X") * mouseSensitivity, 0);
        }
        else
        {

        }

        // On mouse right click, center the camera on the player
        if (Input.GetMouseButtonDown(1))
        {
            mainCamera.transform.position = groundVector * -1;
            mainCamera.transform.LookAt(Vector3.zero);
        }

        /*
            Allow for player input only when character is "touching" the ground.
            This also sets how far above the ground character is hovering.
        */
        if (checkGround(groundVector, 0.5f))
        {
            movementVector.x = Input.GetAxis("Horizontal");
            movementVector.y = 0;
            movementVector.z = Input.GetAxis("Vertical");
            lastGroundPosition = playerContainer.transform.position;
        }
        else
        {
            if (!checkGround(groundVector) && Vector3.Distance(lastGroundPosition, playerContainer.transform.position) > 1)
                fixPlayerRotation();

            // this moves the player down towards the face - gravity
            movementVector.y += playerGravity * Time.deltaTime;
        }

        controller.Move(playerContainer.transform.rotation * movementVector * Time.deltaTime * playerSpeed);

        // Some debug stuff
        debugTextField.text = "";
        debugTextField.text += "Last Ground Distance: " + Vector3.Distance(lastGroundPosition, playerContainer.transform.position).ToString() + "\n";
        debugTextField.text += "Movement vector: " + movementVector.ToString() + "\n";
        debugTextField.text += "Ground vector: " + groundVector.ToString() + "\n";
        debugTextField.text += "Ground vector touching ground: " + checkGround(groundVector).ToString() + '\n';

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        if (horizontal > 0) debugTextField.text += "Pressing D\n";
        else if (horizontal < 0) debugTextField.text += "Pressing A\n";
        if (vertical > 0) debugTextField.text += "Pressing W\n";
        else if (vertical < 0) debugTextField.text += "Pressing S\n";

        // Use this to rotate the character model towards the velocity vector
        if (movementVector != Vector3.zero)
        {
            playerCharacter.transform.forward = playerContainer.transform.rotation * movementVector;
        }
    }
}
