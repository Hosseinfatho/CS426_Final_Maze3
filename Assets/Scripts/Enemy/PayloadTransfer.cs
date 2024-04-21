using System;
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
                    // Debug.Log("Returning nonzero clip length");
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