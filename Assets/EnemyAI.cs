using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private Transform[] waypoints;
    [SerializeField]
    private float viewDistance;

    [SerializeField]
    private GameObject player;

    NavMeshAgent agent;
    int waypointIndex;
    Vector3 target;
    Vector3 lastPlayerPosition;
    bool wasPlayerSeen;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        waypointIndex = 0;
        lastPlayerPosition = Vector3.zero;
        wasPlayerSeen = false;

        UpdateDestination();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInSight())
        {
            agent.SetDestination(lastPlayerPosition);
            wasPlayerSeen = true;
        }
        else if (wasPlayerSeen)
        {
            if (Vector3.Distance(transform.position, lastPlayerPosition) < 0.5)
            {
                wasPlayerSeen = false;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, target) < 1)
                nextWaypoint();

            UpdateDestination();
        }

        if (Vector3.Distance(player.transform.position, this.transform.position) < 1)
        {
            Debug.Log("Player caught!");
            Destroy(gameObject);
        }
    }

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
