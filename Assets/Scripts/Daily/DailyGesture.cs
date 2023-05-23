using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Policies;
using UnityEngine;

// 느낌표 오브젝트에 부착
public class DailyGesture : MonoBehaviour
{
    [SerializeField] private GestureManager gestureManager;
    [SerializeField] private GestureAI ai;
    [SerializeField] private BehaviorParameters param;

    private IEnumerator curCoroutine;

    private void OnEnable()
    {
        curCoroutine = DetectionCoroutine();
        StartCoroutine(curCoroutine);
    }

    private void OnDisable()
    {
        StopCoroutine(curCoroutine);
    }

    private IEnumerator DetectionCoroutine()
    {
        while (true)
        {
            gestureManager.StartValidate();
            yield return new WaitUntil(() => gestureManager.CurrentBehaviorType != BehaviorType.Undecided);

            var behavior = gestureManager.CurrentBehaviorType;


            // 서버에서 해당 행동에 대한 .onnx파일을 가져와야 함.
            // param.Model = 


            ai.enabled = true;
            yield return new WaitUntil(() => ai.Decision != -1);
            ai.EndEpisode();
            ai.enabled = false;

            if (ai.Decision == 0)   // 정답
            {
                DogAnimator.instance.ActPose((int)behavior);
            }
            else if (ai.Decision == 1)  // 기본 행동 (갸우뚱?)
            {
                DogAnimator.instance.ActPose(-1);
            }
            else // 무시
            {
                DogAnimator.instance.ActPose(-1);
            }

            yield return new WaitForSeconds(1f);
            ai.enabled = false;
        }
    }
}
