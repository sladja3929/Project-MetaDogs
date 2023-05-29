using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Policies;
using UnityEngine;
using Unity.Barracuda;
using Unity.Barracuda.ONNX;
using System.IO;

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

            //db에서 모델 가져오기
            yield return RequestManager.Instance.StartCoroutine("LoadAIModel", (int)behavior);

            var ONNXPath = Application.streamingAssetsPath + @"\model_" + (int)behavior + ".onnx";

            // 파일이 존재하지 않으면 무시
            if (!File.Exists(ONNXPath)) continue;

            var curModel = LoadNNModel(ONNXPath, "AITest");
            ai.SetModel("AITest", curModel);

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

    private NNModel LoadNNModel(string modelPath, string modelName)
    {
        ONNXModelConverter converter = new(true);
        Model model = converter.Convert(modelPath);

        NNModelData modelData = ScriptableObject.CreateInstance<NNModelData>();
        using (var memoryStream = new MemoryStream())
        using (var writer = new BinaryWriter(memoryStream))
        {
            ModelWriter.Save(writer, model);
            modelData.Value = memoryStream.ToArray();
        }
        modelData.name = "Data";
        modelData.hideFlags = HideFlags.HideInHierarchy;

        NNModel result = ScriptableObject.CreateInstance<NNModel>();
        result.modelData = modelData;
        result.name = modelName;
        return result;
    }
}
