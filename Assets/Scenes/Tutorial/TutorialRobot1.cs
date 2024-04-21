using TMPro;
using UnityEngine;

public class TutorialRobot1 : MonoBehaviour
{
    [SerializeField] GameObject player;
    Canvas cloudCanvas;
    AudioSource audioSource;

    /*
            Text cloud stuff
    */
    TMP_Text cloudText;
    string cloudTextTarget;
    int cloudTextTargetIndex;
    int cloudTextTargetIndexLength;
    float lastCloudUpdate;
    bool cloudEnabled;
    float cloudUpdateDelay = 0.05f;
    void setCloudText(string newText)
    {
        cloudTextTarget = newText;
        cloudText.text = "";
        cloudTextTargetIndex = 0;
        cloudTextTargetIndexLength = newText.Length;
        cloudEnabled = true;
        cloudCanvas.enabled = true;
        lastCloudUpdate = Time.fixedTime;
    }
    void clearCloud()
    {
        cloudEnabled = false;
        cloudCanvas.enabled = false;
    }
    bool isCloudDonePrinting()
    {
        return cloudTextTargetIndex == cloudTextTargetIndexLength;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        cloudCanvas = GetComponentInChildren<Canvas>();
        cloudText = GetComponentInChildren<TMP_Text>();
        clearCloud();
        lastModeUpdate = -3; //so it starts printing right away
    }

    // returns true if player character is within the distance
    // Has offset = Vector3.up * 3 by default, to avoid hitting the floor, need to use
    // Different vector for different sides of the cube
    bool isPlayerClose(float distance)
    {
        RaycastHit hit;
        Vector3 offset = Vector3.up * 3;
        if (Physics.Raycast(gameObject.transform.position + offset, player.transform.position - (gameObject.transform.position + offset), out hit, distance))
        {
            return hit.transform == player.transform;
        }
        return false;
    }

    /*
        FSM / Control stuff
        This basically is a FSM in code.
        The mode is used to keep track of messages / actions and state of the tutorial NPC.
    */
    int mode = 0;
    float lastModeUpdate = 0;


    string[] mode0Text = {
                            "Oh hey buddy, come over here for a sec.",
                            "(You can use WASD to move)",
                            "Come quick, gotta tell you what's going on.",
                            "Like really close, don't want to yell."
                        };
    int mode0TextIndex = 0;

    string[] mode1Text = {
                            "Good.",
                            "My name is TUTORIAL-BOT-8TWNR3 V3",
                            "But my friends just call me Greg.",
                            "Anyways, I'm gonna tell you a little bit about the controls.",
                            "You already know how to move",
                            "If I'm speaking too slow, hold E",
                            "You can rotate the camera using the mouse",
                            "And zoom in and out using mouse wheel",
                            "And re-center the camera using right mouse button.",
                            "If you didn't notice, this world is a cube",
                            "And you can move over the edge as well.",
                            "So practice that, and go left to meet my best buddy Tom",
                            "(also remind him that he owes me $5)"
                        };
    int mode1TextIndex = 1; // 0 is displayed within mode 0

    string[] mode2Text = {
                            "Just go left until you come over the edge",
                            "Tom will take over from now",
                            "He's gonna explain what we're doing here",
                            "(I'm done with my part)"
                        };
    int mode2TextIndex = 100; //so it doesn't start printing right away

    string[] mode1Error = {
                            "Hey I wasn't done yet.",
                            "Oh you're back. Where was I..."
    };

    void Update()
    {
        // Used to ship delay
        bool skipCloudDelay = Input.GetKey(KeyCode.E);

        // If needed, it adds characters to the cloud, so it appears animated.
        if (cloudEnabled && cloudTextTargetIndex != cloudTextTargetIndexLength && (Time.fixedTime - lastCloudUpdate > cloudUpdateDelay || skipCloudDelay))
        {
            cloudText.text += cloudTextTarget.Substring(cloudTextTargetIndex, 1);
            cloudTextTargetIndex++;
            lastCloudUpdate = Time.fixedTime;

            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else if (audioSource.isPlaying && cloudTextTargetIndex == cloudTextTargetIndexLength)
        {
            audioSource.Stop();
        }

        // Rotates the cloud so it faces the camera
        // if (cloudEnabled)
        // {
        //     cloudCanvas.transform.LookAt(Camera.main.transform);
        //     cloudCanvas.transform.rotation *= Quaternion.Euler(new Vector3(180, 0, 0));
        // }



        if (mode == 0)
        {
            // update the text cloud
            if (!isCloudDonePrinting())
            {
                lastModeUpdate = Time.fixedTime;
            }
            else if (Time.fixedTime - lastModeUpdate > 5)
            {
                setCloudText(mode0Text[mode0TextIndex]);
                lastModeUpdate = Time.fixedTime;

                mode0TextIndex++;
                if (mode0TextIndex >= mode0Text.Length)
                    mode0TextIndex = 1;
            }

            // check if player close enough
            if (isPlayerClose(3))
            {
                mode = 1;
                setCloudText(mode1Text[0]);
            }
        }
        else if (mode == 1)
        {
            // update the text cloud
            if (!isCloudDonePrinting())
            {
                lastModeUpdate = Time.fixedTime;
            }
            else if (Time.fixedTime - lastModeUpdate > 2)
            {
                if (isPlayerClose(6))
                {
                    if (cloudTextTarget == mode1Error[0])
                    {
                        setCloudText(mode1Error[1]);
                        lastModeUpdate = Time.fixedTime;
                    }
                    else
                    {
                        setCloudText(mode1Text[mode1TextIndex]);
                        lastModeUpdate = Time.fixedTime;

                        mode1TextIndex++;
                        if (mode1TextIndex >= mode1Text.Length)
                        {
                            mode = 2;
                            lastModeUpdate = Time.fixedTime;
                        }
                    }



                }
                else
                {
                    if (cloudTextTarget != mode1Error[0])
                        setCloudText(mode1Error[0]);
                    else
                    {
                        clearCloud();
                    }

                    lastModeUpdate = Time.fixedTime;
                }


            }
        }
        else if (mode == 2)
        {
            // update the text cloud
            if (!isCloudDonePrinting())
            {
                lastModeUpdate = Time.fixedTime;
            }
            else if (Time.fixedTime - lastModeUpdate > 1)
            {
                if (!isPlayerClose(10))
                {
                    lastModeUpdate = Time.fixedTime;
                    clearCloud();
                }
                else if (mode2TextIndex < mode2Text.Length)
                {
                    setCloudText(mode2Text[mode2TextIndex]);
                    mode2TextIndex++;
                    lastModeUpdate = Time.fixedTime;
                }
                else if (Time.fixedTime - lastModeUpdate > 6)
                {
                    mode2TextIndex = 0;
                }
                else
                {
                    clearCloud();
                }
            }
        }




    }
}
