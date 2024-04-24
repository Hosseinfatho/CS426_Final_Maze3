using TMPro;
using UnityEngine;

public class TutorialRobot2 : MonoBehaviour
{
    [SerializeField] GameObject player;
    Canvas cloudCanvas;
    AudioSource audioSource;
    Transform rayCastSource;

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
        rayCastSource = transform.Find("RaycastSource");
        clearCloud();
        lastModeUpdate = -3; //so it starts printing right away
    }

    // returns true if player character is within the distance
    // Has offset = Vector3.up * 3 by default, to avoid hitting the floor, need to use
    // Different vector for different sides of the cube
    bool isPlayerClose(float distance)
    {
        RaycastHit hit;
        if (Physics.Raycast(rayCastSource.position, player.transform.position - rayCastSource.position, out hit, distance))
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
                "Hello PLAYER_CANDIDATE_7465.",
                "I see TUTORIAL-BOT-8TWNR3 V3 managed to do its job for once.",
                "Or we actually got a CADET worth something."
            };
    int mode0TextIndex = 0;

    string[] mode1Text = {
                "As you can see, we are inside the operating system",
                "All you can see is a visualization to make your work easier",
                "(don't think about it)",
                "When you complete your training, we will give you some work",
                "That will be to sabotage the EvilAICorp production line",
                "How to do that? Go to the TUTORIAL-BOT-82333 (UP)",
                "He's going to explain the rest",
                "Just bear in mind, he's not from around here"
            };
    int mode1TextIndex = 0;

    void Update()
    {
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



        if (mode == 0)
        {
            // update the text cloud
            if (!isCloudDonePrinting())
            {
                lastModeUpdate = Time.fixedTime;
            }
            else if (Time.fixedTime - lastModeUpdate > 3)
            {
                if (!isPlayerClose(5))
                {
                    lastModeUpdate = Time.fixedTime - 4.5f; //so the delay is less than during normal conversation
                }
                else
                {
                    setCloudText(mode0Text[mode0TextIndex]);
                    lastModeUpdate = Time.fixedTime;

                    mode0TextIndex++;
                    if (mode0TextIndex >= mode0Text.Length)
                        mode = 1;
                }
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
                if (!isPlayerClose(5))
                {
                    lastModeUpdate = Time.fixedTime;
                    clearCloud();
                }
                else
                {
                    setCloudText(mode1Text[mode1TextIndex]);
                    lastModeUpdate = Time.fixedTime;

                    mode1TextIndex++;
                    if (mode1TextIndex >= mode1Text.Length)
                    {
                        mode = 2; //this disables this robot
                    }
                }

            }
        }

    }
}
