using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class GestureAI : Agent
{
    private int correctCount = 0;
    private int defaultCount = 0;
    private int ignoreCount = 0;

    private int trainingCount = 0;
    private int totalCorrectCount = 0;
    private int totalDefaultCount = 0;
    private int totalIgnoreCount = 0;
    public float CorrectAnswerRate { get; private set; } = 0f;

    public int Decision { get; set; } = -1;

    public bool IsStart { get; set; } = false;

    public override void Initialize()
    {
    }

    // Branch 0 size = 3
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!IsStart) return;
        int resultDogGesture = actions.DiscreteActions[0];

        if (resultDogGesture == 0)  // 정답 행동
        {
            ++correctCount;
            AddReward(1f);

            if (correctCount >= 3)
            {
                AddReward(10f);
                ++totalCorrectCount;
                CorrectAnswerRate = Mathf.Round(totalCorrectCount / (float)trainingCount * 10000f) * 0.01f;
                Decision = 0;
                EndEpisode();
            }
        }
        else if (resultDogGesture == 1) // 기본 행동
        {
            ++defaultCount;
            AddReward(-0.95f);

            if (defaultCount >= 3)
            {
                AddReward(-10f);
                ++totalDefaultCount;
                CorrectAnswerRate = Mathf.Round(totalCorrectCount / (float)trainingCount * 10000f) * 0.01f;
                Decision = 1;
                EndEpisode();
            }
        }
        else if (resultDogGesture == 2)  // 무시 행동
        {
            ++ignoreCount;
            AddReward(-1f);

            if (ignoreCount >= 3)
            {
                AddReward(-10f);
                ++totalIgnoreCount;
                CorrectAnswerRate = Mathf.Round(totalCorrectCount / (float)trainingCount * 10000f) * 0.01f;
                Decision = 2;
                EndEpisode();
            }
        }
    }

    // Space size = 4
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(correctCount);
        sensor.AddObservation(ignoreCount);
        sensor.AddObservation(defaultCount);
        sensor.AddObservation(CorrectAnswerRate);
    }

    public override void OnEpisodeBegin()
    {
        ++trainingCount;
        correctCount = 0;
        defaultCount = 0;
        ignoreCount = 0;
    }

    private void OnDestroy()
    {
        Debug.Log(CorrectAnswerRate);
        Debug.Log("정답: " + totalCorrectCount);
        Debug.Log("기본: " + totalDefaultCount);
        Debug.Log("무시: " + totalIgnoreCount);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
    }

}
