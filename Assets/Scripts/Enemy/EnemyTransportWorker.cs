using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTransportWorker : MonoBehaviour
{

    [SerializeField] float viewDistance;
    [SerializeField] GameObject player;

    GameObject playerBox;
    GameObject boxPickUpLocation;
    GameObject boxDropOffLocation;

    Animator enemyAnimator;
    NavMeshAgent agent;
    AudioSource pickupSound;
    Vector3 target;
    Vector3 lastPlayerPosition;

    bool wasPlayerSeen;
    float lastPlayerSeenTime;
    float waitBeforeResumingAction = 3; //in seconds, how long to idle on last player's position

    /*
        currentAction meaning:
            0 - go to pickup location
            1 - picking up the package, animation playing
            2 - box picked up, go to box drop off location
            3 - box dropping off animation playing
            anything else - do nothing

            any of that + 10 - chasing player, but don't allow to change this var
            if it's equal to 1 or 3 (to not interrupt the animation)
    */
    int currentAction = 2;
    float currentActionTime = 0;
    int wantedLevel = 0; // 0 - just carry box around, 1 - chase player

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lastPlayerPosition = Vector3.zero;
        wasPlayerSeen = false;

        // Get required components
        boxPickUpLocation = transform.parent.Find("PickUpLocation").gameObject;
        boxDropOffLocation = transform.parent.Find("DropOffLocation").gameObject;
        playerBox = transform.Find("Robot/Box").gameObject;

        // Change all boxes inside the pickup location material to match the one set on robot
        Material wantedMaterial = playerBox.GetComponent<Renderer>().material;
        foreach (Transform box in boxPickUpLocation.transform)
        {
            box.gameObject.GetComponent<Renderer>().material = wantedMaterial;
        }
        boxDropOffLocation.transform.Find("DropOffBox").gameObject.GetComponent<Renderer>().material = wantedMaterial;

        // To set animation triggers
        enemyAnimator = GetComponentInChildren<Animator>();

        // To play audio clips
        pickupSound = GetComponent<AudioSource>();
    }

    void setDestination(Transform newDestination)
    {
        if (agent.destination != newDestination.position)
        {
            agent.SetDestination(newDestination.position);
        }
    }

    public GameObject getMyOwnBox()
    {
        return playerBox;
    }

    // Returns true/false depending if player character is in line of sight of the enemy
    public bool playerInSight()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, viewDistance))
        {
            if (hit.collider.tag == "Player")
            {
                lastPlayerPosition = hit.transform.position;
                return true;
            }
        }

        return false;
    }

    public void setWantedLevel(int wantedLevel)
    {
        this.wantedLevel = wantedLevel;
    }

    public int getWantedLevel()
    {
        return wantedLevel;
    }

    bool checkAnimationState(string stateName)
    {
        return enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    // I'm bad at naming stuff
    bool checkIfShouldChasePlayer()
    {
        // if playerInSight returns true, it also updates the lastPlayerPosition variable
        if (wantedLevel > 0 && playerInSight())
        {
            setAnimation("Walking");
            agent.SetDestination(lastPlayerPosition);
            wasPlayerSeen = true;
            lastPlayerSeenTime = 0;
            return true;
        }

        return false;
    }

    string _lastAnimation = "BoxUp";
    void setAnimation(string animation)
    {
        if (_lastAnimation == animation) return;

        enemyAnimator.ResetTrigger(_lastAnimation);
        enemyAnimator.SetTrigger(animation);
        _lastAnimation = animation;
    }

    void Update()
    {
        // if need to go to pickup location
        if (currentAction == 0)
        {
            setDestination(boxPickUpLocation.transform);
            setAnimation("Walking");

            // if reached the pickup
            if (Vector3.Distance(gameObject.transform.position, boxPickUpLocation.transform.position) < 1)
            {
                currentAction = 1;
                currentActionTime = Time.fixedTime;
                setAnimation("BoxUp");
            }
            // if seen player
            else
            {
                if (checkIfShouldChasePlayer())
                {
                    currentAction += 10;
                }
            }
        }
        // if pickup animation is playing, so wait until its done
        else if (currentAction == 1)
        {
            if (checkAnimationState("BoxUp"))
            {
                playerBox.SetActive(true);
            }
            else if (!checkAnimationState("Idle"))
            {
                currentActionTime = Time.fixedTime;
            }
            else if (Time.fixedTime - currentActionTime > 2)
            {
                currentAction = 2;
            }
        }
        // if need to go to destination
        else if (currentAction == 2)
        {
            setDestination(boxDropOffLocation.transform);
            setAnimation("Walking");

            // if reached the pickup
            if (Vector3.Distance(gameObject.transform.position, boxDropOffLocation.transform.position) < 1)
            {
                currentAction = 3;
                currentActionTime = Time.fixedTime;
                setAnimation("BoxDown");
            }
            // if seen player
            else
            {
                if (checkIfShouldChasePlayer())
                {
                    currentAction += 10;
                }
            }
        }
        //waiting for box drop animation to finish
        else if (currentAction == 3)
        {
            // when boxdown is true, and animation is done playing, it will get "stuck" in 
            // BoxDownDoneState state, unless BoxDownDone is set to true
            if (checkAnimationState("BoxDownDoneState"))
            {
                playerBox.SetActive(false);
                setAnimation("BoxDownDone");
                currentActionTime = Time.fixedTime;
            }
            else if (checkAnimationState("IdleBoxDown") && Time.fixedTime > 1.5)
            {
                currentAction = 0;
            }
        }
        // if here, then must be chasing player
        else
        {
            // calling chasePlayer will automatically update the destination to player location
            // if chasePlayer() returns false, then player no longer in sight
            // so go to last location
            if (!checkIfShouldChasePlayer() && wasPlayerSeen)
            {
                if (Vector3.Distance(transform.position, agent.pathEndPosition) < 0.5)
                {
                    if (lastPlayerSeenTime == 0)
                    {
                        lastPlayerSeenTime = Time.fixedTime;
                        setAnimation("Idle");
                    }
                    else if (Time.fixedTime - lastPlayerSeenTime > 4.0f)
                    {
                        wasPlayerSeen = false;
                    }
                }
            }
            // if player not in sight, reached last player destination, then go back to previous routine
            else if (!wasPlayerSeen && Time.fixedTime - lastPlayerSeenTime > waitBeforeResumingAction)
            {
                currentAction -= 10;
            }

        }

        if (Vector3.Distance(player.transform.position, this.transform.position) < 1)
        {
            Debug.Log("Player should die now");
            // enemyAnimator.ResetTrigger("Walking");
            // enemyAnimator.SetTrigger("Die");
            // pickupSound.Play();
        }


    }
}
