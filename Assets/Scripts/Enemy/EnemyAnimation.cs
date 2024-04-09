using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Demo.Enemy
{
    public class EnemyAnimation : MonoBehaviour
    {
        private Animator animator;

        void Start()
        {
            animator = GetComponentInChildren<Animator>();
            animator.SetTrigger("Walking");
        }

        void Update()
        {

        }
    }
}