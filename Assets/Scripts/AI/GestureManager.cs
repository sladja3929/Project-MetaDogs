using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum BehaviorType
{
    Undecided = -1,
    None,
    SitDown,
    SitSide,
    Lie,
    Jump,
    Die,
    Attack,
}

public class GestureManager : MonoBehaviour
{
    private enum ManagerMode
    {
        None,
        Sensing,
        Validating,
    }

    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Transform target;
    [SerializeField] private float errorRatePerCount = 3750f;
    [SerializeField] [Range(0f, 1f)] private float curveDelay = 0.15f;
    [SerializeField] [Range(100f, 10000f)] private float countPenalty = 3000f;

    private readonly Dictionary<BehaviorType, List<Vector3>> dataSet = new();
    private float errorRate = 0f;
    private bool isObserving = false;
    private bool isStopped = false;
    private bool isWorking = false;

    private int isAllowedChangingGesture = 0;
    private BehaviorType givenBehavior;
    private ManagerMode mode = ManagerMode.None;




    public BehaviorType CurrentBehaviorType { private set; get; } = BehaviorType.None;
    public void StartSensing(BehaviorType type)
    {
        givenBehavior = type;
        CurrentBehaviorType = type;
        mode = ManagerMode.Sensing;
    }
    public void StartValidate()
    {
        CurrentBehaviorType = BehaviorType.Undecided;
        mode = ManagerMode.Validating;
    }


    void Update()
    {
        if (mode == ManagerMode.None) return;

        else if (mode == ManagerMode.Sensing)
        {
            if (!isWorking && OVRInput.GetDown(OVRInput.Button.One))
            {
                isWorking = true;
                resultText.text = "start";
                StartCoroutine(AddGestureCoroutine(givenBehavior));
            }
        }
        else if (mode == ManagerMode.Validating)
        {
            if (!isWorking && OVRInput.GetDown(OVRInput.Button.Two))
            {
                isWorking = true;
                resultText.text = "validate start";
                StartCoroutine(MatchGestureCoroutine());
            }
        }

        // 제스처 관찰 중지
        if (isObserving && OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
        {
            isStopped = true;
        }
    }

    private IEnumerator AddGestureCoroutine(BehaviorType type)
    {
        ObserveGesture(out var inputGesture);
        yield return new WaitUntil(() => !isObserving);

        if (dataSet.ContainsKey(type))
        {
            // 기존 행동에 다른 제스처 감지 시
            if (!IsMatched(inputGesture, type))
            {
                yield return new WaitUntil(() => isAllowedChangingGesture != 0);

                // 제스처 변경을 허용했을 때, 기존 데이터 초기화
                if (isAllowedChangingGesture == 1)
                {
                    dataSet[type] = inputGesture;
                }
                // 제스처 변경을 하지 않았을 때, 현재 입력 무시
                else if (isAllowedChangingGesture == -1)
                {

                }
                isAllowedChangingGesture = 0;
            }
        }
        else
        {
            // 새로 제스처 추가
            dataSet[type] = inputGesture;
        }

        mode = ManagerMode.None;
        isWorking = false;
    }

    private IEnumerator MatchGestureCoroutine()
    {
        ObserveGesture(out var curGesture);
        yield return new WaitUntil(() => !isObserving);

        foreach(var item in dataSet)
        {
            if (IsMatched(curGesture, item.Key))
            {
                CurrentBehaviorType = item.Key;
                break;
            }
        }

        isWorking = false;
    }

    private bool IsMatched(List<Vector3> curGesture, BehaviorType type)
    {
        // Set long & short vector
        List<Vector3> longVec = curGesture;
        List<Vector3> shortVec = dataSet[type];
        if (longVec.Count < shortVec.Count)
        {
            longVec = dataSet[type];
            shortVec = curGesture;
        }

        // Check Error
        float dirError = 0f;
        int shortIdx = 0;
        for (int longIdx = 0; longIdx < longVec.Count; ++longIdx)
        {
            // Cosine similarity를 이용해 MSE 계산
            float curError = Mathf.Pow(Vector3.Angle(longVec[longIdx], shortVec[shortIdx]), 2);
            if (shortIdx + 1 < shortVec.Count)
            {
                float nextError = Mathf.Pow(Vector3.Angle(longVec[longIdx], shortVec[shortIdx + 1]), 2);
                if (curError < nextError)
                {
                    dirError += curError;
                }
                else
                {
                    dirError += nextError;
                    ++shortIdx;
                }
            }
            else
            {
                dirError += curError;
            }
        }

        // Penalty
        for (; shortIdx < shortVec.Count; ++shortIdx)
        {
            dirError += countPenalty;
        }

        if (dirError > errorRate)
        {
            resultText.text = "diff... | " + dirError + " / " + errorRate;
            return false;
        }
        else
        {
            resultText.text = "correct!!: " + type.ToString() + " | " + dirError + " / " + errorRate;
            return true;
        }
    }


    // 함수 마무리: isObserving == false
    private void ObserveGesture(out List<Vector3> newList)
    {
        newList = new();
        isObserving = true;
        StartCoroutine(ObserveGestureCoroutine(newList));
    }

    private IEnumerator ObserveGestureCoroutine(List<Vector3> newList)
    {
        var observeDelay = new WaitForSeconds(0.1f);
        var curveDelay = new WaitForSeconds(this.curveDelay);

        Vector3 startPos = target.position;
        Vector3 endPos;
        Vector3 prevVec;

        isStopped = false;

        // 초기에 0.1 이상 움직여야 인식 시작
        do
        {
            yield return observeDelay;
            endPos = target.position;
            prevVec = endPos - startPos;
            if (isStopped)
            {
                resultText.text = "Move More !!";
            }
        } while (prevVec.magnitude < 0.1f);
        startPos = endPos;
        newList.Add(prevVec);

        while (!isStopped)
        {
            yield return observeDelay;
            endPos = target.position;
            Vector3 diff = endPos - startPos;
            if (Mathf.Abs(Vector3.Angle(prevVec, diff)) > 15f)
            {
                // 곡선 구간 무시
                yield return curveDelay;
                endPos = target.position;
                diff = endPos - startPos;
                if (Mathf.Abs(Vector3.Angle(prevVec, diff)) > 15f)
                {
                    newList.Add(diff);
                    prevVec = diff;
                }
            }
            else
            {
                newList[^1] = newList[^1].normalized * diff.magnitude;
            }
            startPos = endPos;
        }
        countText.text = $"{newList.Count}";
        errorRate = newList.Count * errorRatePerCount;
        isObserving = false;
    }
}
