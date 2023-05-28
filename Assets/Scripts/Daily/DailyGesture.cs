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
        DogAnimator.instance.animator.SetBool("dailyRecordStart", true);
        //Debug.Log("왜 안나와!!!!!!!");
        curCoroutine = DetectionCoroutine();
        StartCoroutine(curCoroutine);
    }

    private void OnDisable()
    {
        gestureManager.InitText();
        StopCoroutine(curCoroutine);
        DogAnimator.instance.animator.SetBool("dailyRecordStart", false);
        if (DogAnimator.instance.animator.GetInteger("petPose") != -1)
            DogAnimator.instance.animator.SetTrigger("loopEnd");
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

            ai.Decision = -1;
            ai.IsStart = true;
            while (ai.Decision == -1)
            {
                ai.RequestDecision();
                yield return Time.fixedDeltaTime;
            }
            ai.IsStart = false;

            Debug.Log(ai.Decision);
            if (ai.Decision == 0)   // 정답
            {
                DogAnimator.instance.animator.SetInteger("petPose", (int)behavior);
                Debug.Log(behavior.ToString());
            }
            else if (ai.Decision == 1)  // 기본 행동 (갸우뚱?)
            {
                gestureManager.SetTextUnrecognizable();
                DogAnimator.instance.animator.SetInteger("petPose", -1);
            }
            else // 무시
            {
                gestureManager.SetTextUnrecognizable();
                DogAnimator.instance.animator.SetInteger("petPose", -1);
            }

            yield return new WaitForSeconds(3f);
            DogAnimator.instance.animator.SetTrigger("loopEnd");
        }
    }
}
