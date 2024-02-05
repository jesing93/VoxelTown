using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIControl : MonoBehaviour {

    private Vector3 targetLocation;
    private NavMeshAgent agent;
    private float ms = 0.2f;
    Animator anim;


    void Update() {
        if (agent.remainingDistance < 0.5)
        {
            CrowdManager.Instance.TargetReached(gameObject);
        }
    }

    /// <summary>
    /// Go to specified point
    /// </summary>
    /// <param name="target"></param>
    public void GoTo(Vector3 target, int speedFactor) {
        targetLocation = target;
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(targetLocation);
        //anim = GetComponent<Animator>();
        //anim.SetTrigger("isWalking");
        //anim.SetFloat("wOffset", Random.Range(0.0f, 1.0f));
        ms = Random.Range(0.2f, 0.8f);
        //anim.SetFloat("multSpeed", ms);
        agent.speed = ms * speedFactor;
    }

    /// <summary>
    /// Update the agent speed
    /// </summary>
    /// <param name="newSpeed"></param>
    public void UpdateSpeed(int newSpeed)
    {
        if (newSpeed == 0)
        {
            agent.speed = 0;
        }
        else
        {
            agent.speed = ms * newSpeed;
        }
    }
}