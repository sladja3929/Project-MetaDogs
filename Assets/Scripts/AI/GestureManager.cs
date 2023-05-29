using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum BehaviorType
{
    None = -2,
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

    [Header("GUID 참조")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject gestureChangePanel;
    [SerializeField] private Transform target;

    [Header("제스처 인식 설정")]
    [SerializeField] [Range(0f, 45f)] private float penaltyAngle = 15f;
    [SerializeField] [Range(0f, 1f)] private float curveDelay = 0.15f;
    [SerializeField] [Range(0.1f, 20f)] private float shakingCorrectionAngle = 10f;

    private Dictionary<BehaviorType, List<Vector3>> dataSet;
    private bool isObserving = false;
    private bool isStopped = false;
    private bool isWorking = false;
    private bool isTraining = false;

    private int isAllowedChangingGesture = 0;
    private BehaviorType givenBehavior;
    private ManagerMode mode = ManagerMode.None;

    private void Awake()
    {
        text.gameObject.SetActive(false);
        gestureChangePanel.SetActive(false);
        SaveDataManager.Instance.LoadTrainingData(out dataSet);
    }

    private void UpdateData()
    {
        SaveDataManager.Instance.SaveTrainingData(dataSet);
    }

    private IEnumerator TrainingCoroutine(BehaviorType type)
    {
        isTraining = true;
        yield return RequestManager.Instance.StartCoroutine("LoadMLAgent", (int)type);
        isTraining = false;
    }


    public BehaviorType CurrentBehaviorType { private set; get; } = BehaviorType.Undecided;
    public void StartSensing(BehaviorType type)
    {
        text.gameObject.SetActive(true);
        gestureChangePanel.SetActive(false);
        text.SetText("오른손 A버튼으로\n훈련 시작하기");
        //EffectManager.instance.PlayEffect(3);
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
            text.SetText("");
            return;
        }
        text.SetText("오른손 A버튼으로\n제스처 입력하기");
        //EffectManager.instance.PlayEffect(3);

        CurrentBehaviorType = BehaviorType.Undecided;
        mode = ManagerMode.Validating;
    }
    
    public void SetTextUnrecognizable()
    {
        text.SetText("강아지가 이해하지 못한 것 같아요.");
        EffectManager.instance.PlayEffect(1);
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
                EffectManager.instance.PlayEffect(3);
            }
        }
        else if (mode == ManagerMode.Validating)
        {
            if (!isWorking && OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.Z))
            {
                isWorking = true;
                StartCoroutine(MatchGestureCoroutine());
                EffectManager.instance.PlayEffect(3);
            }
        }

        // 제스처 관찰 중지
        if (isObserving && OVRInput.GetDown(OVRInput.Button.Two) || Input.GetKeyDown(KeyCode.X))
        {
            isStopped = true;
            //EffectManager.instance.PlayEffect(2);
        }
    }

    private IEnumerator AddGestureCoroutine(BehaviorType type)
    {
        ObserveGesture(out var inputGesture);
        yield return new WaitUntil(() => !isObserving);
        text.SetText("");
        //text.gameObject.SetActive(false);

        // 기존 학습 데이터가 있다면
        if (dataSet.ContainsKey(type))
        {
            // 기존 제스처와 일치하면 학습 완료
            if (IsMatched(inputGesture, type) < 1f)
            {
                StartCoroutine(TrainingCoroutine(type));
                TrainManager.instance.RecordFin(true, false);
            }
            // 다른 제스처 감지 시
            else
            {
                BehaviorType resultType = MatchAllGesture(inputGesture);

                // 처음 보는 제스처라면, 제스처 변경을 할 것인지 묻는다.
                if (resultType == BehaviorType.Undecided)
                {
                    gestureChangePanel.SetActive(true);
                    Player.instance.laser.SetActive(true);
                    yield return new WaitUntil(() => isAllowedChangingGesture != 0);
                    gestureChangePanel.SetActive(false);
                    Player.instance.laser.SetActive(false);

                    // 제스처 변경을 허용했을 때, 기존 데이터 초기화
                    if (isAllowedChangingGesture == 1)
                    {
                        isAllowedChangingGesture = 0;
                        dataSet[type] = inputGesture;
                        UpdateData();
                        StartCoroutine(TrainingCoroutine(type));
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
                }
                // 다른 행동이 이미 쓰고 있는 동작이라면 무시
                else
                {
                    isWorking = false;
                    isAllowedChangingGesture = 0;
                    text.SetText($"해당 제스처는 {resultType}와 유사합니다!");
                    EffectManager.instance.PlayEffect(1);
                    yield return new WaitForSeconds(2f);

                    StartSensing(givenBehavior);
                    yield break;
                }
            }
        }
        else
        {
            BehaviorType resultType = MatchAllGesture(inputGesture);

            // 새로 제스처 추가
            if (resultType == BehaviorType.Undecided)
            {
                dataSet[type] = inputGesture;
                UpdateData();
                StartCoroutine(TrainingCoroutine(type));
                TrainManager.instance.RecordFin(true, true);
            }
            //  다른 행동이 해당 제스처를 쓰고 있다면 무시
            else
            {
                isWorking = false;
                isAllowedChangingGesture = 0;
                text.SetText($"해당 제스처는 {resultType}와 유사합니다!");
                EffectManager.instance.PlayEffect(1);
                yield return new WaitForSeconds(2f);

                StartSensing(givenBehavior);
                yield break;
            }
        }

        mode = ManagerMode.None;
        isWorking = false;
    }

    private IEnumerator MatchGestureCoroutine()
    {
        DogAnimator.instance.animator.ResetTrigger("loopEnd");
        ObserveGesture(out var curGesture);
        yield return new WaitUntil(() => !isObserving);

        BehaviorType resultType = MatchAllGesture(curGesture);

        if (resultType == BehaviorType.Undecided)
        {
            text.SetText("일치하는 제스처가 없습니다!");
            EffectManager.instance.PlayEffect(1);
            yield return new WaitForSeconds(1f);
            CurrentBehaviorType = BehaviorType.None;
        }
        else
        {
            CurrentBehaviorType = resultType;
            text.SetText($"{CurrentBehaviorType}\n가져오는 중");
            EffectManager.instance.PlayEffect(0);
            mode = ManagerMode.None;
        }
        isWorking = false;
    }

    private BehaviorType MatchAllGesture(List<Vector3> gesture)
    {
        float minError = float.MaxValue;
        BehaviorType resultType = BehaviorType.Undecided;
        foreach (var item in dataSet)
        {
            float curError = IsMatched(gesture, item.Key);
            if (curError < 1f && curError < minError)
            {
                minError = curError;
                resultType = item.Key;
            }
        }
        return resultType;
    }

    private float IsMatched(List<Vector3> curGesture, BehaviorType type)
    {
        // Set long & short vector
        List<Vector3> longVec = curGesture;
        List<Vector3> shortVec = dataSet[type];

        if (longVec.Count < shortVec.Count)
        {
            longVec = dataSet[type];
            shortVec = curGesture;
        }

        if (longVec.Count - shortVec.Count > 4) return float.MaxValue;


        // Check Error
        float gestureError = 0f;
        int shortIdx = 0;
        for (int longIdx = 0; longIdx < longVec.Count; ++longIdx)
        {
            // Cosine similarity
            float curError = Mathf.Abs(Vector3.Angle(longVec[longIdx], shortVec[shortIdx]));
            if (shortIdx + 1 < shortVec.Count)
            {
                float nextError = Mathf.Abs(Vector3.Angle(longVec[longIdx], shortVec[shortIdx + 1]));
                if (curError < nextError)
                {
                    gestureError += curError;
                }
                else
                {
                    gestureError += nextError;
                    ++shortIdx;
                }
            }
            else
            {
                gestureError += curError;
            }
        }

        int totalCount = longVec.Count;

        // Penalty
        for (; shortIdx < shortVec.Count; ++shortIdx)
        {
            gestureError += penaltyAngle;
            ++totalCount;
        }

        gestureError /= totalCount;

        // [10, 55) 범위를 단조증가하는 함수
        float dirError = -45 * Mathf.Exp(-0.15f * totalCount) + 55;

        Debug.Log($"행동: {type} | {Mathf.Round(gestureError)} / {Mathf.Round(dirError)}");
        return gestureError / dirError;
    }


    // 함수 마무리: isObserving == false
    private void ObserveGesture(out List<Vector3> newList)
    {
        text.SetText("제스처 감지중...\n오른손 A버튼으로 중지하기");
        EffectManager.instance.PlayEffect(3);
        newList = new();
        isObserving = true;
        StartCoroutine(ObserveGestureCoroutine(newList));
    }

    private IEnumerator ObserveGestureCoroutine(List<Vector3> newList)
    {
        var observeDelay = new WaitForSeconds(0.1f);
        var curveDelay = new WaitForSeconds(this.curveDelay);

        Vector3 startPos = target.localPosition;
        Vector3 endPos;
        Vector3 prevVec;

        isStopped = false;

        // 초기에 약간 움직여야 인식 시작
        do
        {
            yield return observeDelay;
            endPos = target.localPosition;
            prevVec = endPos - startPos;
            if (isStopped)
            {
                text.SetText("제스처가 너무 작습니다!\n더 움직여주세요.");
                EffectManager.instance.PlayEffect(1);
            }
        } while (prevVec.magnitude < 0.1f);
        startPos = endPos;
        newList.Add(prevVec);

        while (!isStopped)
        {
            yield return observeDelay;
            endPos = target.localPosition;
            Vector3 diff = endPos - startPos;

            // 떨림, 곡선 보정
            if (Mathf.Abs(Vector3.Angle(prevVec, diff)) > shakingCorrectionAngle)
            {
                yield return curveDelay;
                endPos = target.localPosition;
                diff = endPos - startPos;
                if (Mathf.Abs(Vector3.Angle(prevVec, diff)) > shakingCorrectionAngle)
                {
                    newList.Add(diff);
                    prevVec = diff;
                }
            }
            else
            {
                newList[^1] += newList[^1].normalized * diff.magnitude;
            }
            startPos = endPos;
        }
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
    public void InitText()
    {
        text.gameObject.SetActive(false);
    }
}
