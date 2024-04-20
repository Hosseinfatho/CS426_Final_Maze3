using System.Collections;
using Demo.Enemy;
using Demo.FSM;
using UnityEngine;
using UnityEngine.AI;

namespace Demo.Enemy
{
    public class PayloadTransfer : MonoBehaviour
    {
        private bool transferComplete = false;
        private bool animationStarted = false;
        public IEnumerator AnimateAndWait(Animator animator)
        {
            animationStarted = true;
            transferComplete = false;
            animator.ResetTrigger("Walking");
            animator.SetTrigger("BoxDown");
            float boxDownLength = GetAnimationClipLength(animator, "BoxDown");
            yield return new WaitForSeconds(boxDownLength);
            animator.SetTrigger("BoxUp");
            float boxUpLength = GetAnimationClipLength(animator, "BoxUp");
            yield return new WaitForSeconds(boxUpLength);
            Debug.Log("Finished waiting for animations. Going to next waypoint...");
            transferComplete = true;
            animationStarted = false;
        }

        float GetAnimationClipLength(Animator animator, string clipName)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].name == clipName)
                {
                    Debug.Log("Returning nonzero clip length");
                    return clips[i].length;
                }
            }
            Debug.Log("Problem! Returning -1 for clip length.");
            return -1;
        }

        public bool AnimationComplete()
        {
            return transferComplete;
        }
        public bool AnimationStarted()
        {
            return animationStarted;
        }
    }
}