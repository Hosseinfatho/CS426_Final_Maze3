using System.Collections;
using Demo.Enemy;
using Demo.FSM;
using UnityEngine;
using UnityEngine.AI;

namespace Demo.MyFSM
{
    [CreateAssetMenu(menuName = "FSM/Actions/Patrol")]
    public class PatrolAction : FSMAction
    {
        public override void Execute(BaseStateMachine stateMachine)
        {
            var navMeshAgent = stateMachine.GetComponent<NavMeshAgent>();
            var patrolPoints = stateMachine.GetComponent<PatrolPoints>();
            var animator = stateMachine.GetComponentInChildren<Animator>();

            if (patrolPoints.HasReached(navMeshAgent))
            {
                stateMachine.StartCoroutine(AnimateAndNavigate(animator, navMeshAgent, patrolPoints));
            }
        }

        public IEnumerator AnimateAndNavigate(Animator animator, NavMeshAgent navMeshAgent, PatrolPoints patrolPoints)
        {
            Debug.Log("Setting flags: animationStarted = true, transferComplete = false");
            Debug.Log("Reset trigger Walking, set trigger BoxDown");
            animator.ResetTrigger("Walking");
            animator.SetTrigger("BoxDown");
            float boxDownLength = GetAnimationClipLength(animator, "BoxDown");
            Debug.Log("Waiting for BoxDown anim to complete...");
            yield return new WaitForSeconds(boxDownLength);
            Debug.Log("Reset trigger BoxDown, set trigger BoxUp");
            animator.ResetTrigger("BoxDown");
            animator.SetTrigger("BoxUp");
            Debug.Log("Waiting for BoxUp animation...");
            float boxUpLength = GetAnimationClipLength(animator, "BoxUp");
            yield return new WaitForSeconds(boxUpLength);
            Debug.Log("Finished waiting for animations.");
            Debug.Log("Setting flags: transferComplete = true, animationStarted = false");
            navMeshAgent.SetDestination(patrolPoints.GetNext().position);
        }
        
        float GetAnimationClipLength(Animator animator, string clipName)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].name == clipName)
                {
                    // Debug.Log("Returning nonzero clip length");
                    return clips[i].length;
                }
            }
            Debug.Log("Problem! Returning -1 for clip length.");
            return -1;
        }
    }
}