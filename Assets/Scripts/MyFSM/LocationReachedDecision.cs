
using Demo.Enemy;
using Demo.FSM;
using UnityEngine;
using UnityEngine.AI;
namespace Demo.MyFSM
{
    [CreateAssetMenu(menuName = "FSM/Decisions/Location Reached")]
    public class LocationReachedDecision : Decision
    {
        public override bool Decide(BaseStateMachine stateMachine)
        {
            var navMeshAgent = stateMachine.GetComponent<NavMeshAgent>();
            var patrolPoints = stateMachine.GetComponent<PatrolPoints>();
            return patrolPoints.HasReached(navMeshAgent);
        }
    }
}