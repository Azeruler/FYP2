using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ragdoll : MonoBehaviour
{

    Rigidbody[] rigidBodies;
    Animator animator;
    public NavMeshAgent agent; // Reference to NavMeshAgent

    // Method to set the NavMeshAgent reference
    public void SetNavMeshAgent(UnityEngine.AI.NavMeshAgent navMeshAgent)
    {
        agent = navMeshAgent;
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();
        DeactivateRagdoll();
    }

    // Update is called once per frame
    public void DeactivateRagdoll()
    {
        foreach (var rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = true;
        }
        animator.enabled = true;
    }

    public void ActivateRagdoll()
    {
        foreach (var rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = false;
        }
        animator.enabled = false;
        agent.enabled = false; // Disable the NavMeshAgent
      
    }
}
