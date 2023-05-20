using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class GestureAI : Agent
{
    [SerializeField]  private GestureManager gestureManager;

    public override void Initialize()
    {
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        GestureType resultDogGesture = (GestureType)actions.DiscreteActions[0];

        // 정답만 SetReward(10f);
        // 기본값은 SetReward(0f);
        // 틀린 건 SetReward(-10f);
        switch (resultDogGesture)
        {
            case GestureType.None:
                break;
            case GestureType.SitDown:
                break;
            case GestureType.SitSide:
                break;
            case GestureType.Lie:
                break;
            case GestureType.Jump:
                break;
            case GestureType.Die:
                break;
            case GestureType.Attack:
                break;
            default:
                break;
        }
        
        EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionOut = actionsOut.DiscreteActions;
        // 강아지 행동 부여

        //if (Input.GetKey(KeyCode.A))
        //{
        //    rb.AddForce(speed * Vector3.right);
        //    discreteActionOut[0] = 1;
        //}
        //else if (Input.GetKey(KeyCode.D))
        //{
        //    rb.AddForce(speed * Vector3.left);
        //    discreteActionOut[0] = 2;
        //}
        //else
        //{
        //    discreteActionOut[0] = 0;
        //}
    }

    // 관찰할 요소
    public override void CollectObservations(VectorSensor sensor)
    {
        // Space size = 2
        // +) 강아지 행동 결정
        sensor.AddObservation((int)gestureManager.CurrentType);
    }

    public override void OnEpisodeBegin()
    {
        // 강아지 원래 포즈
    }
}
