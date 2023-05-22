using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using TMPro;

public class GestureAI : Agent
{
    [SerializeField] private TextMeshProUGUI text;

    private int correctCount = 0;
    private int defaultCount = 0;
    private int ignoreCount = 0;

    private int trainingCount = 0;
    private int totalCorrectCount = 0;
    private int totalDefaultCount = 0;
    private int totalIgnoreCount = 0;
    private float correctAnswerRate = 0f;

    public override void Initialize()
    {
        //GetComponent<DecisionRequester>().DecisionPeriod = Random.Range(5, 12);
    }

    // Branch 0 size = 3
    public override void OnActionReceived(ActionBuffers actions)
    {
        int resultDogGesture = actions.DiscreteActions[0];

        if (resultDogGesture == 0)  // 정답 행동
        {
            ++correctCount;
            AddReward(1f);

            if (correctCount >= 20)
            {
                AddReward(10f);
                ++totalCorrectCount;
                correctAnswerRate = Mathf.Round(totalCorrectCount / (float)trainingCount * 10000f) * 0.01f;
                text.SetText($"correct | {correctAnswerRate}%");
                EndEpisode();
            }
        }
        else if (resultDogGesture == 1) // 기본 행동
        {
            ++defaultCount;
            AddReward(-0.95f);

            if (defaultCount >= 20)
            {
                AddReward(-10f);
                ++totalDefaultCount;
                correctAnswerRate = Mathf.Round(totalCorrectCount / (float)trainingCount * 10000f) * 0.01f;
                text.SetText($"default | {correctAnswerRate}%");
                EndEpisode();
            }
        }
        else if (resultDogGesture == 2)  // 무시 행동
        {
            ++ignoreCount;
            AddReward(-1f);

            if (ignoreCount >= 20)
            {
                AddReward(-10f);
                ++totalIgnoreCount;
                correctAnswerRate = Mathf.Round(totalCorrectCount / (float)trainingCount * 10000f) * 0.01f;
                text.SetText($"ignore | {correctAnswerRate}%");
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
        sensor.AddObservation(correctAnswerRate);
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
        Debug.Log(correctAnswerRate);
        Debug.Log("정답: " + totalCorrectCount);
        Debug.Log("기본: " + totalDefaultCount);
        Debug.Log("무시: " + totalIgnoreCount);
    }
}
