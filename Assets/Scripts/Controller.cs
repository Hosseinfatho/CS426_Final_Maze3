using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    [SerializeField] float playerSpeed;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float mouseWheelSensitivity;
    [SerializeField] float cameraSnapBackSpeed;
    [SerializeField] GameObject[] transportEnemies;
    [SerializeField] GameObject pauseMenu;

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

    GameObject playerBox;
    GameObject playerModel;
    GameObject cameraContainer;
    PauseMenu pauseMenuScript;

    GameObject eKey;


    bool snapBackCamera = false; //if set to true, camera will center around the player, when done it will be set to false

    /*
        -2 - player dead, screen was shown
        -1 - player dead, but didn't show the end game screen
         0 - no box, can walk, can pickup
         1 - pickup animation playing, can't walk
         2 - has box, can walk, can pickup
         3 - drop animation playing, can't walk
    */
    int playerMode = 0;
    int setTargetToDestroy = -1; // if >= 0, target with this ID will be set to be destroyed
    float setTargetToDestroyTime = 0; //also used to time the dead animation !
    float targetDestroyDelay = 3f;
    int targetsAmount = 0;
    int targetsDestroyed = 0;


    //Box locations
    GameObject[] pickUpLocations, dropOffLocations, boxesUsedAtThisLocation;

    // Used to animate the player rotation
    bool rotatePlayer = false;
    Quaternion playerRotationTarget;

    float playerGravity = -2f;
    float lerpTime = 0;
    float smoothing = 0.25f;

    private void resetPos()
    {
        foreach (GameObject worker in transportEnemies)
        {
            EnemyTransportWorker script = worker.transform.Find("EnemyTransportWorker").GetComponent<EnemyTransportWorker>();
            script.setWantedLevel(0);
        }
    }

    private void testButtonPressed()
    {
        foreach (GameObject worker in transportEnemies)
        {
            EnemyTransportWorker script = worker.transform.Find("EnemyTransportWorker").GetComponent<EnemyTransportWorker>();
            script.setWantedLevel(script.getWantedLevel() + 1);
        }
    }

    string _lastAnimation = "BoxUp";
    void setAnimation(string animation)
    {
        if (_lastAnimation == animation) return;

        playerAnimator.ResetTrigger(_lastAnimation);
        playerAnimator.SetTrigger(animation);
        _lastAnimation = animation;
    }
    bool checkAnimationState(string stateName)
    {
        return playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
    void resetAllAnimationTriggers()
    {
        foreach (var param in playerAnimator.parameters)
        {
            playerAnimator.ResetTrigger(param.name);
        }
    }

    // returns -1 if not in proximity, returns index of location if in proximity
    int isBoxInProximity(GameObject[] locations, float distance = 1.5f)
    {
        //string debug = "Locations: ";
        for (int i = 0; i < locations.Length; i++)
        {
            float distanceToBoxes = Vector3.Distance(gameObject.transform.position, locations[i].transform.position);
            //debug += distanceToBoxes.ToString() + ", ";
            if (distanceToBoxes <= distance)
            {
                return i;
            }
        }

        //Debug.Log(debug);
        return -1;
    }

    private void log(string msg)
    {
        debugTextField.text += msg + "\n";
    }

    public void killPlayer()
    {
        playerMode = -1;
        resetAllAnimationTriggers();
        playerAnimator.SetTrigger("Die");
        controller.enabled = false;
        setTargetToDestroyTime = Time.fixedTime;
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
        targetsAmount = transportEnemies.Length;
        controller = gameObject.AddComponent<CharacterController>();
        controller.radius = 1;
        controller.height = 1;
        resetButton.onClick.AddListener(resetPos);
        testButton.onClick.AddListener(testButtonPressed);
        playerAnimator = transform.Find("RobotModel/Robot").gameObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        eKey = transform.Find("RobotModel/Robot/e_key").gameObject;
        eKey.SetActive(false);

        pauseMenuScript = pauseMenu.transform.Find("PauseMenuScript").gameObject.GetComponent<PauseMenu>();

        playerBox = transform.Find("RobotModel/Robot/Box").gameObject;
        playerModel = transform.Find("RobotModel").gameObject;
        cameraContainer = transform.Find("CameraContainer").gameObject;
        mainCamera = cameraContainer.transform.Find("MainCamera").gameObject;
        playerBox.SetActive(false);
        playerAnimator.SetTrigger("GoToIdleBoxDown"); // remember to reset it later

        // populate the location arrays
        pickUpLocations = new GameObject[targetsAmount];
        dropOffLocations = new GameObject[targetsAmount];
        boxesUsedAtThisLocation = new GameObject[targetsAmount];

        for (int i = 0; i < targetsAmount; i++)
        {
            GameObject enemyContainer = transportEnemies[i];

            pickUpLocations[i] = enemyContainer.transform.Find("PickUpLocation").gameObject;
            dropOffLocations[i] = enemyContainer.transform.Find("DropOffLocation").gameObject;
            boxesUsedAtThisLocation[i] = enemyContainer.transform.Find("EnemyTransportWorker/Robot/Box").gameObject;
        }

        Debug.Log("Start function completed.");
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

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Enemy")
        {
            killPlayer();
        }
    }

    void Update()
    {
        debugTextField.text = "";

        // If game paused, don't do any checks
        if (Time.timeScale == 0) return;

        // if specified some target to mark as destroyed, this will execute
        if (setTargetToDestroy >= 0 && Time.fixedTime - setTargetToDestroyTime > targetDestroyDelay)
        {
            EnemyTransportWorker enemyScript = transportEnemies[setTargetToDestroy].transform.Find("EnemyTransportWorker").GetComponent<EnemyTransportWorker>();

            if (!enemyScript.isTargetDestroyed())
            {
                enemyScript.destroyTarget();
                setTargetToDestroy = -1;
                targetsDestroyed++;

                // notify all workers about change in wanted level
                foreach (GameObject worker in transportEnemies)
                {
                    EnemyTransportWorker script = worker.transform.Find("EnemyTransportWorker").GetComponent<EnemyTransportWorker>();
                    script.setWantedLevel(script.getWantedLevel() + 1);
                }
            }

            // level finished
            if (targetsDestroyed >= targetsAmount)
            {
                pauseMenuScript.showMessage("Level cleared !", 0.2f, 1f, 0.2f);
            }
        }

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


        /*



                    @!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@
                    IF PLAYER IS DEAD ANYTHING AFTER THIS POINT DOESN'T EXECUTE
                    @!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@!@



        */
        if (playerMode < 0)
        {
            if (playerMode == -1)
            {
                if (checkAnimationState("Die") && Time.fixedTime - setTargetToDestroyTime > 3f)
                {
                    playerMode = -2;
                    pauseMenuScript.showMessage("You failed.", 1f, 0.2f, 0.2f);
                }
            }

            return; // !!!!!!!!!!!!!!!!!!!!!!!
        }





        // Animations and unlocking the movement when they are done
        // no box, playing the pick up animation, enable box at the start:
        if (playerMode == 1)
        {
            if (checkAnimationState("BoxUp"))
            {
                playerBox.SetActive(true);
            }
            else if (checkAnimationState("Idle"))
            {
                playerMode = 2;
                resetAllAnimationTriggers();
            }
        }
        else if (playerMode == 3)
        {
            if (checkAnimationState("BoxDownDoneState"))
            {
                playerAnimator.SetTrigger("BoxDownDone");
                playerBox.SetActive(false);
                playerMode = 0;
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

            float x = 0;
            float z = 0;

            float x_raw = Input.GetAxisRaw("Horizontal");
            float z_raw = Input.GetAxisRaw("Vertical");

            // All input should be done here
            // Because if here, then player alive and touching ground
            if (playerMode == 0 || playerMode == 2)
            {
                x = Input.GetAxis("Horizontal");
                z = Input.GetAxis("Vertical");

                int boxLocationIndex = -1;

                if (targetsAmount > 0)
                {
                    if (playerMode == 0)
                        boxLocationIndex = isBoxInProximity(pickUpLocations);
                    else if (playerMode == 2)
                        boxLocationIndex = isBoxInProximity(dropOffLocations);
                }

                // if near pickup or dropoff display e key on the player
                eKey.SetActive(boxLocationIndex != -1);

                // if trying to pick up / drop off something
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if ((playerMode == 0 || playerMode == 2) && boxLocationIndex > 0)
                    {
                        x = 0;
                        z = 0;
                    }

                    // 0 - no box, can walk, can pickup
                    if (playerMode == 0)
                    {
                        if (boxLocationIndex >= 0)
                        {
                            playerMode = 1;
                            resetAllAnimationTriggers();
                            playerAnimator.SetTrigger("BoxUp");
                            playerBox.GetComponent<Renderer>().material = boxesUsedAtThisLocation[boxLocationIndex].GetComponent<Renderer>().material;
                        }
                    }
                    else if (playerMode == 2)
                    {
                        if (boxLocationIndex >= 0)
                        {
                            playerMode = 3;
                            resetAllAnimationTriggers();
                            playerAnimator.SetTrigger("BoxDown");

                            //set target to be destroyed only if material differs
                            //first 10 letters should be enough to compare
                            //as unity changes the suffix / adds stuff
                            string material1Name = playerBox.GetComponent<Renderer>().material.name;
                            material1Name = material1Name.Substring(0, material1Name.IndexOf(" "));
                            string material2Name = boxesUsedAtThisLocation[boxLocationIndex].GetComponent<Renderer>().material.name;
                            material2Name = material2Name.Substring(0, material2Name.IndexOf(" "));

                            if (material1Name != material2Name)
                            {
                                setTargetToDestroy = boxLocationIndex;
                                setTargetToDestroyTime = Time.fixedTime;
                            }
                        }
                    }
                }
            }

            movementVector.Set(x, 0, z);

            if (x != 0 || z != 0)
            {
                // camera is rotated by 90* so we need to remove that
                movementVector = cameraContainer.transform.localRotation * Quaternion.Euler(-90, 0, 0) * movementVector;
                movementVector.y = 0;
            }

            if ((x_raw != 0 || z_raw != 0) && (playerMode == 0 || playerMode == 2))
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
