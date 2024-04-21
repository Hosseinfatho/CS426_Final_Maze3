using TMPro;
using UnityEngine;

public class TutorialRobot2 : MonoBehaviour
{
    [SerializeField] GameObject player;
    GameObject cloudCanvas;

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
        cloudCanvas.SetActive(true);
        lastCloudUpdate = Time.fixedTime;
    }
    void clearCloud()
    {
        cloudEnabled = false;
        cloudCanvas.SetActive(false);
    }
    bool isCloudDonePrinting()
    {
        return cloudTextTargetIndex == cloudTextTargetIndexLength;
    }

    void Start()
    {
        cloudCanvas = GameObject.Find("Canvas");
        cloudText = GetComponentInChildren<TMP_Text>();
        cloudCanvas.SetActive(false);
        cloudEnabled = false;
        lastModeUpdate = -3; //so it starts printing right away
    }

    // returns true if player character is within the distance
    // Has offset = Vector3.up * 3 by default, to avoid hitting the floor, need to use
    // Different vector for different sides of the cube
    bool isPlayerClose(float distance)
    {
        RaycastHit hit;
        Vector3 offset = Vector3.left * 3;
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


    string[] mode0Text = { "Hello PLAYER_CANDIDATE_7465.", "I see TUTORIAL-BOT-8TWNR3 V3 managed to do its job for once.", "Or we actually got a CADET worth something." };
    int mode0TextIndex = 0;

    string[] mode1Text = { "Go UP if you want to learn about basic enemies.", "Go LEFT if you want to learn about the story", "Press ESC and go to MAIN MENU if you're done." };
    int mode1TextIndex = 0;

    void Update()
    {
        // If needed, it adds characters to the cloud, so it appears animated.
        if (cloudEnabled && cloudTextTargetIndex != cloudTextTargetIndexLength && Time.fixedTime - lastCloudUpdate > cloudUpdateDelay)
        {
            cloudText.text += cloudTextTarget.Substring(cloudTextTargetIndex, 1);
            cloudTextTargetIndex++;
            lastCloudUpdate = Time.fixedTime;
        }



        if (mode == 0)
        {
            // update the text cloud
            if (!isCloudDonePrinting())
            {
                lastModeUpdate = Time.fixedTime;
            }
            else if (Time.fixedTime - lastModeUpdate > 5)
            {
                if (!isPlayerClose(5))
                {
                    lastModeUpdate = Time.fixedTime;

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
                if (!isPlayerClose(10))
                {
                    lastModeUpdate = Time.fixedTime;
                }
                else
                {
                    setCloudText(mode1Text[mode1TextIndex]);
                    lastModeUpdate = Time.fixedTime;

                    mode1TextIndex++;
                    if (mode1TextIndex >= mode1Text.Length)
                    {
                        mode1TextIndex = 0;
                        lastModeUpdate = Time.fixedTime;
                    }
                }

            }
        }

    }
}
