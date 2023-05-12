using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum GestureType
{
    SitDown
}

public class GestureManager : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI countText;

    [SerializeField] private Transform target;
    [SerializeField] private float errorRatePerCount = 3750f;
    [SerializeField] [Range(0f, 1f)] private float curveDelay = 0.15f;
    [SerializeField] [Range(100f, 10000f)] private float countPenalty = 3000f;

    private readonly Dictionary<GestureType, List<Vector3>> dataSet = new();
    private float errorRate = 0f;
    private bool isObserving = false;
    private bool isMatched = false;
    private bool isStopped = false;

    private bool flag = true;

    void Update()
    {
        if (flag && OVRInput.GetDown(OVRInput.Button.One))
        {
            flag = false;
            resultText.text = "start";
            StartCoroutine(AddGestureCoroutine(GestureType.SitDown));
        }
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
        {
            isStopped = true;
        }
        if (flag && OVRInput.GetDown(OVRInput.Button.Two))
        {
            flag = false;
            resultText.text = "validate start";
            StartCoroutine(MatchGestureCoroutine());
        }
    }

    private IEnumerator AddGestureCoroutine(GestureType type)
    {
        ObserveGesture(out var inputGesture);
        yield return new WaitUntil(() => !isObserving);
        dataSet[type] = inputGesture;
        flag = true;
    }

    private IEnumerator MatchGestureCoroutine()
    {
        ObserveGesture(out var curGesture);
        yield return new WaitUntil(() => !isObserving);

        foreach(var item in dataSet)
        {
            isMatched = MatchGesture(curGesture, item.Key);
            if (isMatched) break;
        }

        flag = true;
    }

    private bool MatchGesture(List<Vector3> curGesture, GestureType type)
    {
        // Set long & short Vector
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

        // penalty
        for (; shortIdx < shortVec.Count; ++shortIdx)
        {
            dirError += countPenalty;
        }

        if (dirError > errorRate)
        {
            resultText.text = "diff... | " + dirError + " / " + errorRate;
            Debug.Log("에러율로 제스처 다름 | " + dirError + " / " + errorRate);
            return false;
        }
        else
        {
            resultText.text = "correct!!: " + type.ToString() + " | " + dirError + " / " + errorRate;
            Debug.Log("제스처 일치: " + type.ToString() + " | " + dirError + " / " + errorRate);
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

        // 초기에 0.5 이상 움직여야 인식 시작
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
        //Debug.Log("저징된 데이터 개수: " + newList.Count);
        countText.text = $"{newList.Count}";
        errorRate = newList.Count * errorRatePerCount;
        isObserving = false;
    }
}
