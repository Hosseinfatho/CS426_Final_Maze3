using TMPro;
using UnityEngine;

public class TutorialRobot3 : MonoBehaviour
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
                "G'day mate, I'm Greg",
                "Right behind me, we've got a bloody interesting specimen.",
                "This bloke over there is a representation of the process that run in EvilAiCorp.",
                "And they chuck data around EvilAICorp OS like nobody's business.",
                "Those blokes will keep on doin' that forever, mate.",
                "If ya don't go stirrin' 'em up, they'll give ya a fair go and leave ya be, mate.",
                "Fair dinkum, mate. Just keep yer mitts off 'em, or it's all over like a snag on the barbie.",
                "If you wanna learn how to smash 'em to bits, head left and have a yarn with me coworker.",
                "Cheers mate!"
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
