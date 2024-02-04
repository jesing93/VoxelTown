using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIControl : MonoBehaviour {

    private GameObject[] goalLocations;
    private NavMeshAgent agent;
    Animator anim;

    void Start() {

        agent = GetComponent<NavMeshAgent>();
        goalLocations = GameObject.FindGameObjectsWithTag("goal");
        int i = Random.Range(0, goalLocations.Length);
        agent.SetDestination(goalLocations[i].transform.position);
        anim = GetComponent<Animator>();
        anim.SetTrigger("isWalking");
        anim.SetFloat("wOffset", Random.Range(0.0f, 1.0f));
        float ms = Random.Range(0.5f, 2f);
        anim.SetFloat("multSpeed", ms);
        agent.speed *= ms; 
    }


    void Update() {
        if (agent.remainingDistance < 1)
        {
            int i = Random.Range(0, goalLocations.Length);
            agent.SetDestination(goalLocations[i].transform.position);
        }
    }
}