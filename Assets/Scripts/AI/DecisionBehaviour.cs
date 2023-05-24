using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;

public class DecisionBehaviour : MonoBehaviour
{
    private GestureAI ai;
    private BehaviorParameters behavior;
    private DecisionRequester adecision;

    // Start is called before the first frame update
    void Awake()
    {
        ai = GetComponent<GestureAI>();
        behavior = GetComponent<BehaviorParameters>();
        adecision = GetComponent<DecisionRequester>();
        ai.enabled = false;
        behavior.enabled = false;
        adecision.enabled = false;
    }

    public void SetDogBehavior(BehaviorType type)
    {
        StartCoroutine(GetBehaviorCoroutine(type));
    }

    private IEnumerator GetBehaviorCoroutine(BehaviorType type)
    {
        // 서버에서 해당 제스처의 정답 행동에 대한 모델을 가져온다.
        // 동기 후 아래 실행

        ai.enabled = true;
        behavior.enabled = true;
        adecision.enabled = true;
        yield return new WaitUntil(() => ai.Decision != -1);

        if (ai.Decision == 0)   // 정답
        {
            // 강아지 행동
        }
        else if (ai.Decision == 1)  // 갸우뚱
        {
            // 강아지 행동
        }
        else  // 무시
        {
            // 강아지 행동
        }
        ai.enabled = false;
        behavior.enabled = false;
        adecision.enabled = false;
    }
}
