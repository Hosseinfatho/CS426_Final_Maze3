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
            // Debug.Log("Executing payload transfer action!");

            var animator = stateMachine.GetComponentInChildren<Animator>();
            var payloadTransfer = stateMachine.GetComponent<PayloadTransfer>();
            if (!payloadTransfer.AnimationStarted())
            {
                Debug.Log("Starting coroutine for payload transfer animation...");
                stateMachine.StartCoroutine(payloadTransfer.AnimateAndWait(animator));
            }
            else
            {
                // Debug.Log("Animation already started!");
            }
        }
    }
}