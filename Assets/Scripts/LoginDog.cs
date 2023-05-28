using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LoginDog : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform toy;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance <= agent.stoppingDistance) //가까워지면 스톱
        {
            animator.SetInteger("moveSpeed", -1);
            //Debug.Log(0);
        }

        if (!animator.GetBool("toyOn"))
            animator.SetBool("toyOn", true);
        if ((transform.position - toy.position).magnitude > agent.stoppingDistance + 1f)
        {
            agent.SetDestination(toy.position);
            agent.stoppingDistance = 1f;
            agent.speed = 3f;
            animator.SetInteger("moveSpeed", 2);
        }
    }
}
