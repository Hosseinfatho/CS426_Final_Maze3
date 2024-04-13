using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PayloadPickup : MonoBehaviour
{
  private Animator animator;
  private NavMeshAgent navMeshAgent;

  private void OnTriggerEnter(Collider other)
  {
    // Debug.Log("Collision detected!");

    if (other.CompareTag("DeliveryBot"))
    {
      // Debug.Log("Setting animation trigger for box up!");
      animator = other.transform.parent.GetComponentInChildren<Animator>();
      navMeshAgent = other.transform.parent.GetComponentInChildren<NavMeshAgent>();

      animator.ResetTrigger("Walking");
      navMeshAgent.disable;

      animator.SetTrigger("BoxDown");
      animator.SetTrigger("BoxUp");
    }
  }
  
}