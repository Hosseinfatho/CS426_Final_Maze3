using UnityEngine;

namespace Demo.Enemy
{
    public class PatrolPoints : MonoBehaviour
    {
        public Transform[] waypoints; // Array of patrol waypoints

        // // NOTE: private scope ideal for integrity, but need game to work
        // public int currentWaypointIndex = 0; // Index of the current waypoint
        private int currentWaypointIndex = 0;

        // Get the next patrol waypoint
        public Transform GetNext()
        {
            // Increment the current waypoint index
            currentWaypointIndex++;

            // Wrap around to the beginning if reached the end of the array
            if (currentWaypointIndex >= waypoints.Length)
                currentWaypointIndex = 0;

            // Return the next waypoint
            return waypoints[currentWaypointIndex];
        }

        // Check if the AI has reached the current waypoint
        public bool HasReached(UnityEngine.AI.NavMeshAgent navMeshAgent)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                return true;
            else
                return false;
        }
    }
}
