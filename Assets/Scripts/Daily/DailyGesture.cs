using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Policies;
using UnityEngine;

// 느낌표 오브젝트에 부착
public class DailyGesture : MonoBehaviour
{
    [SerializeField] private GestureManager gestureManager;
    [SerializeField] private GestureAI ai;

    private IEnumerator curCoroutine;

    private void OnEnable()
    {
        curCoroutine = DetectionCoroutine();
        StartCoroutine(curCoroutine);
    }

    private void OnDisable()
    {
        StopCoroutine(curCoroutine);
        DogAnimator.instance.animator.SetBool("dailyRecordStart", false);
    }

    private IEnumerator DetectionCoroutine()
    {
        while (true)
        {
            gestureManager.StartValidate();
            yield return new WaitUntil(() => gestureManager.CurrentBehaviorType != BehaviorType.Undecided);

            var behavior = gestureManager.CurrentBehaviorType;

            if (behavior == BehaviorType.None)  // 해당 제스처가 없으면 무시
            {
                DogAnimator.instance.animator.SetInteger("petPose", -1);
                continue;
            }

            // 서버에서 해당 행동에 대한 .onnx파일을 가져와야 함.
            // param.Model = 

            ai.Decision = -1;
            int count = 0;
            while (ai.Decision == -1)
            {
                ai.RequestDecision();
                yield return Time.fixedDeltaTime;
                ++count;
                if (count > 100) break;
            }

            Debug.Log(ai.Decision);
            if (ai.Decision == 0)   // 정답
            {
                DogAnimator.instance.animator.SetInteger("petPose", (int)behavior);
                Debug.Log(behavior.ToString());
            }
            else if (ai.Decision == 1)  // 기본 행동 (갸우뚱?)
            {
                DogAnimator.instance.animator.SetInteger("petPose", -1);
            }
            else // 무시
            {
                DogAnimator.instance.animator.SetInteger("petPose", -1);
            }

            yield return new WaitForSeconds(3f);
            DogAnimator.instance.animator.SetBool("loopEnd", true);
        }
    }
}
