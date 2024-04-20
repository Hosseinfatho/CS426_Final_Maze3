using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    [SerializeField] Transform[] waypoints;
    [SerializeField] float viewDistance;
    [SerializeField] GameObject player;

    Animator enemyAnimator;
    NavMeshAgent agent;
    AudioSource deathSound;
    int waypointIndex;
    Vector3 target;
    Vector3 lastPlayerPosition;
    bool wasPlayerSeen;
    float lastPlayerSeenTime;
    bool movementEnabled;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        waypointIndex = 0;
        lastPlayerPosition = Vector3.zero;
        wasPlayerSeen = false;
        movementEnabled = true;

        // To set animation triggers
        enemyAnimator = GetComponentInChildren<Animator>();

        // To play audio clips
        deathSound = GetComponent<AudioSource>();

        UpdateDestination();
    }

    void Update()
    {
        if (movementEnabled)
        {
            // If player in sight, go to the player's location
            if (playerInSight())
            {
                enemyAnimator.SetTrigger("Walking");
                agent.SetDestination(lastPlayerPosition);
                wasPlayerSeen = true;
                lastPlayerSeenTime = 0;
            }
            // if player no longer in sight, go to the last player's known location
            else if (wasPlayerSeen)
            {
                if (Vector3.Distance(transform.position, agent.pathEndPosition) < 0.5)
                {
                    if (lastPlayerSeenTime == 0)
                    {
                        lastPlayerSeenTime = Time.fixedTime;
                        enemyAnimator.ResetTrigger("Walking");
                    }
                    else if (Time.fixedTime - lastPlayerSeenTime > 4.0f)
                    {
                        wasPlayerSeen = false;
                    }
                }
            }
            // otherwise, follow waypoints
            else
            {
                enemyAnimator.SetTrigger("Walking");

                if (Vector3.Distance(transform.position, target) < 1)
                    nextWaypoint();

                UpdateDestination();
            }
        }


        if (Vector3.Distance(player.transform.position, this.transform.position) < 1)
        {
            movementEnabled = false;
            enemyAnimator.ResetTrigger("Walking");
            enemyAnimator.SetTrigger("Die");
            deathSound.Play();
        }
    }

    // Returns true/false depending if player character is in line of sight of the enemy
    bool playerInSight()
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

    // Sets destination of the agent to the waypoint
    void UpdateDestination()
    {
        target = waypoints[waypointIndex].position;
        agent.SetDestination(target);
    }

    void nextWaypoint()
    {
        waypointIndex++;
        if (waypointIndex >= waypoints.Length)
            waypointIndex = 0;
    }
}
