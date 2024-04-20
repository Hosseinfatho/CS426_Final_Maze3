using System.Collections;
using Demo.Enemy;
using Demo.FSM;
using UnityEngine;
using UnityEngine.AI;

namespace Demo.MyFSM
{
    [CreateAssetMenu(menuName = "FSM/Actions/PayloadTransfer")]
    public class PayloadTransferAction : FSMAction
    {
        public override void Execute(BaseStateMachine stateMachine)
        {
            var animator = stateMachine.GetComponentInChildren<Animator>();
            var payloadTransfer = stateMachine.GetComponent<PayloadTransfer>();
            if (!payloadTransfer.AnimationStarted())
            {
                stateMachine.StartCoroutine(payloadTransfer.AnimateAndWait(animator));
            }
        }
    }
}