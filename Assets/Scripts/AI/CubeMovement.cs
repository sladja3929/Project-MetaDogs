using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CubeMovement : Agent
{
    public float speed = 5f;
    public GameObject bigObj;
    public GameObject smallObj;
    private Rigidbody rb;
    private Vector3 initPos;
    private GameObject wall;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        initPos = transform.position;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int discrete = actions.DiscreteActions[0];
        //float continuous = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);

        switch (discrete)
        {
            case 0: AddReward(-0.001f); break;
            case 1: rb.AddForce(speed * Vector3.right); break;
            case 2: rb.AddForce(speed * Vector3.left); break;
        }
        //Debug.Log(continuous);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(speed * Vector3.right);
            discreteActionOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(speed * Vector3.left);
            discreteActionOut[0] = 2;
        }
        else
        {
            discreteActionOut[0] = 0;
        }
    }

    // °üÂûÇÒ ¿ä¼Ò
    public override void CollectObservations(VectorSensor sensor)
    {
        // Space size = 2
        sensor.AddObservation(gameObject.transform.position.x);
        sensor.AddObservation(rb.velocity.x);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Respawn"))
        {
            AddReward(-10f);
            EndEpisode();
        }
        else if (collision.gameObject.CompareTag("Finish"))
        {
            AddReward(10f);
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            AddReward(10f);
            wall = other.gameObject;
            other.gameObject.SetActive(false);
        }
    }

    public override void OnEpisodeBegin()
    {
        gameObject.transform.position = initPos;
        if (wall is not null) wall.SetActive(true);
    }
}
