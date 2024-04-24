using TMPro;
using UnityEngine;

public class TutorialRobot4 : MonoBehaviour
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
                "Now hold up before you do anything. This guy there is armed and ready.",
                "As you can see, he's delivering orange boxes - those are data packets.",
                "Stopping him would be possible, but EvilAICorp AI would know something's missing.",
                "So instead we will deliver a incorrect packet, so everything looks fine...",
                "And then BOOM! Whatever EvilAICorp is building now won't work when needed.",
                "There is another of his kind on the other side of the cube with different color box.",
                "Deliver boxes to wrong destinations to complete the level and defeat the EvilAICorp",
                "Just remember. Once you deliver box to wrong location they will be pissed and will chase you.",
                "But they have attention span of a goldfish, so just stay out of sight and they will go back to their work.",
                "If you're scared press ESC. Good luck soldier!"
            };
    int mode0TextIndex = 0;

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
                if (!isPlayerClose(4))
                {
                    lastModeUpdate = Time.fixedTime - 2.5f; //so the delay is less than during normal conversation
                    clearCloud();
                }
                else
                {
                    setCloudText(mode0Text[mode0TextIndex]);
                    lastModeUpdate = Time.fixedTime;

                    mode0TextIndex++;
                    if (mode0TextIndex >= mode0Text.Length)
                    {
                        mode = 1;
                        clearCloud();
                    }
                }
            }
        }
    }
}
