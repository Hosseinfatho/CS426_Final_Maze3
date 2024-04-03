using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "FSM/Actions/Chase")]
public class ChaseAction : FSMAction
{
    public override void Execute(BaseStateMachine stateMachine)
    {
        var navMeshAgent = stateMachine.GetComponent<NavMeshAgent>();
        var enemySightSensor = stateMachine.GetComponent<EnemySightSensor>();

        navMeshAgent.SetDestination(enemySightSensor.Player.position);
    }
}
