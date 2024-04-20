using Demo.Enemy;
using Demo.FSM;
using UnityEngine;
namespace Demo.MyFSM
{
    [CreateAssetMenu(menuName = "FSM/Decisions/Animation Complete")]
    public class AnimationCompleteDecision : Decision
    {
        public override bool Decide(BaseStateMachine stateMachine)
        {
            var payloadTransfer = stateMachine.GetComponent<PayloadTransfer>();
            return payloadTransfer.AnimationComplete();
        }
    }
}