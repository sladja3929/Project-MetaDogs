using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum BehaviorType
{
    Undecided = -1,
    SitSide,
    Die,
    Jump,
    Lie,
    SitDown,
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

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject gestureChangePanel;
    [SerializeField] private Transform target;
    [SerializeField] private float errorRatePerCount = 3750f;
    [SerializeField] [Range(0f, 1f)] private float curveDelay = 0.15f;
    [SerializeField] [Range(100f, 10000f)] private float countPenalty = 3000f;

    private Dictionary<BehaviorType, List<Vector3>> dataSet = new();
    private float errorRate = 0f;
    private bool isObserving = false;
    private bool isStopped = false;
    private bool isWorking = false;

    private int isAllowedChangingGesture = 0;
    private BehaviorType givenBehavior;
    private ManagerMode mode = ManagerMode.None;

    private void Awake()
    {
        text.gameObject.SetActive(false);
        gestureChangePanel.SetActive(false);
        //SaveDataManager.Instance.LoadTrainingData(out dataSet);
    }

    private void UpdateData()
    {
        //SaveDataManager.Instance.SaveTrainingData(dataSet);
    }


    public BehaviorType CurrentBehaviorType { private set; get; } = BehaviorType.Undecided;
    public void StartSensing(BehaviorType type)
    {
        text.gameObject.SetActive(true);
        gestureChangePanel.SetActive(false);
        text.SetText("오른손 A버튼으로 훈련 시작하기");
        givenBehavior = type;
        CurrentBehaviorType = type;
        mode = ManagerMode.Sensing;
    }
    public void StartValidate()
    {
        text.gameObject.SetActive(true);
        gestureChangePanel.SetActive(false);

        if (dataSet.Count == 0)
        {
            text.SetText("제스처가 없어요..");
            return;
        }
        text.SetText("오른손 A버튼으로 제스처 입력하기");


        CurrentBehaviorType = BehaviorType.Undecided;
        mode = ManagerMode.Validating;
    }




    void Update()
    {
        if (mode == ManagerMode.None) return;

        else if (mode == ManagerMode.Sensing)
        {
            if (!isWorking && (OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.Z)))
            {
                isWorking = true;
                StartCoroutine(AddGestureCoroutine(givenBehavior));
            }
        }
        else if (mode == ManagerMode.Validating)
        {
            if (!isWorking && OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.Z))
            {
                isWorking = true;
                StartCoroutine(MatchGestureCoroutine());
            }
        }

        // 제스처 관찰 중지
        if (isObserving && OVRInput.GetDown(OVRInput.Button.Two) || Input.GetKeyDown(KeyCode.X))
        {
            isStopped = true;
        }
    }

    private IEnumerator AddGestureCoroutine(BehaviorType type)
    {
        ObserveGesture(out var inputGesture);
        yield return new WaitUntil(() => !isObserving);
        text.SetText("");
        text.gameObject.SetActive(false);

        if (dataSet.ContainsKey(type))
        {
            // 기존 행동에 다른 제스처 감지 시
            if (!IsMatched(inputGesture, type))
            {
                gestureChangePanel.SetActive(true);
                Player.instance.laser.SetActive(true);
                yield return new WaitUntil(() => isAllowedChangingGesture != 0);
                gestureChangePanel.SetActive(false);
                Player.instance.laser.SetActive(false);

                // 제스처 변경을 허용했을 때, 기존 데이터 초기화
                if (isAllowedChangingGesture == 1)
                {
                    dataSet[type] = inputGesture;
                    UpdateData();
                    TrainManager.instance.RecordFin(true, true);
                }
                // 제스처 변경을 하지 않았을 때, 현재 입력 무시
                else if (isAllowedChangingGesture == -1)
                {
                    isWorking = false;
                    isAllowedChangingGesture = 0;
                    StartSensing(givenBehavior);
                    yield break;
                }
                isAllowedChangingGesture = 0;
            }
            // 기존 제스처 학습
            else
            {
                TrainManager.instance.RecordFin(true, false);
            }
        }
        else
        {
            // 새로 제스처 추가
            dataSet[type] = inputGesture;
            UpdateData();
            TrainManager.instance.RecordFin(true, true);

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
            return false;
        }
        else
        {
            return true;
        }
    }


    // 함수 마무리: isObserving == false
    private void ObserveGesture(out List<Vector3> newList)
    {
        text.SetText("제스처 감지중...\n 오른손 A버튼으로 중지하기");
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
                text.SetText("제스처가 너무 작습니다!\n 더 움직여주세요.");
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
        errorRate = newList.Count * errorRatePerCount;
        isObserving = false;
    }


    public void OnClickChangeOK()
    {
        isAllowedChangingGesture = 1;
    }
    public void OnClickChangeNo()
    {
        isAllowedChangingGesture = -1;
    }
}
