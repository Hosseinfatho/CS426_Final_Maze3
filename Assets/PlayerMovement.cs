using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject level;
    [SerializeField]
    private TMP_Text debugTextField;
    [SerializeField]
    private Button resetButton;
    [SerializeField]
    private float playerSpeed; //default 2.0f ?

    private CharacterController controller;
    private Vector3 playerVelocity;

    private void resetPos()
    {
        controller.enabled = false;
        gameObject.transform.position = new Vector3(0, 6, 0);
        gameObject.transform.Rotate(0, 0, 90);
        controller.enabled = true;
    }

    private void Start()
    {
        controller = gameObject.AddComponent<CharacterController>();
        resetButton.onClick.AddListener(resetPos);
    }


    /*
        This makes a line from player position (transform.position), with direction of the vector (vector), length of that line, layerMask
        Returns true if this new line intersects with layerMask
        So to detect the "ground", cube must be made of layer called "Ground"
    */
    private bool checkGround(Vector3 vector)
    {
        return Physics.Raycast(transform.position, vector, 2f, 1 << LayerMask.NameToLayer("Ground"));
    }

    /*
        This returns the vector that points to the ground.
        If no vector points to the ground, it returns the last state.
        0, -1, 0 - is a "normal" state
    */
    private Vector3[] directions = { Vector3.down, Vector3.up, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
    private Vector3 groundVector = Vector3.down;
    private Vector3 getGroundVector()
    {
        foreach (Vector3 vector in directions)
        {
            if (checkGround(vector))
            {
                groundVector = vector;
                return vector;
            }
        }

        return groundVector;
    }

    /*
        This hopefully translates the x, y, z standard input into actual coordinates relatively to the center of the world (at 0,0,0)
        I really don't know how to describe this function better.
    */
    private Vector3 translateMovement(float x, float y, float z)
    {
        Vector3 movement = new Vector3(0, 0, 0);

        if (Equals(groundVector, Vector3.down))
        {
            movement = new Vector3(x, y, z);
        }
        else if (Equals(groundVector, Vector3.up))
        {
            movement = new Vector3(x, -y, z);
        }
        else if (Equals(groundVector, Vector3.right))
        {
            movement = new Vector3(-y, x, z);
        }
        else if (Equals(groundVector, Vector3.left))
        {
            movement = new Vector3(y, x, z);
        }
        else if (Equals(groundVector, Vector3.forward))
        {
            movement = new Vector3(x, z, -y);
        }
        else if (Equals(groundVector, Vector3.back))
        {
            movement = new Vector3(x, -z, y);
        }

        return movement;
    }

    void Update()
    {


        getGroundVector();

        if (controller.isGrounded)
        {
            playerVelocity = translateMovement(0, 0, 0);
        }

        // This updates the left/right up/down movement
        Vector3 move = translateMovement(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);

        // Some debug garbage
        debugTextField.text = "";
        debugTextField.text += "Down: " + checkGround(Vector3.down) + "\n";
        debugTextField.text += "Up: " + checkGround(Vector3.up) + "\n";
        debugTextField.text += "Left: " + checkGround(Vector3.left) + "\n";
        debugTextField.text += "Right: " + checkGround(Vector3.right) + "\n";
        debugTextField.text += "Forward: " + checkGround(Vector3.forward) + "\n";
        debugTextField.text += "Backward: " + checkGround(Vector3.back) + "\n";
        debugTextField.text += "Move vector: " + move.x.ToString() + ", " + move.y.ToString() + ", " + move.z.ToString() + "\n";
        debugTextField.text += "Ground vector: " + groundVector.x.ToString() + ", " + groundVector.y.ToString() + ", " + groundVector.z.ToString() + "\n";

        // This specifies which way the character is pointing
        if (move != Vector3.zero)
        {
            //gameObject.transform.forward = move;
        }

        // This updates the vertical movement (player in the game shouldn't actually move vertically unless it comes over the edge)
        // It is done separately from above to not influence the character rotation when it's moving vertically.
        playerVelocity += translateMovement(0, -1f * Time.deltaTime, 0);
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
