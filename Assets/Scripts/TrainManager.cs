using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainManager : MonoBehaviour
{
    public static TrainManager instance;
    public bool trainMode;

    public GestureManager gestureManager;
    private BehaviorType behaviorType;

    public float maxHamJudgeTime;
    public bool hamNoticed;

    public bool[] petPosesEnabled = new bool[6];
    public GameObject[] hitBoxes = new GameObject[6];

    public GameObject table;

    //public BoxCollider[] hamArea = new BoxCollider[6];

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        trainMode = DogAnimator.instance.animator.GetBool("trainMode");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))    //훈련모드 수동 전환 (권장하진 않음)
        {
            if (DogAnimator.instance.animator.GetBool("trainMode"))
            {
                TrainModeDisable();
            }
            else
            {
                TrainModeEnable();
            }
        }
        if (Input.GetKeyDown(KeyCode.G))
            HitBoxOnOff();
    }

    public void TrainModeEnable()
    {
        DogAnimator.instance.animator.SetBool("trainMode", true);
        trainMode = true;
    }

    public void TrainModeDisable()
    {
        DogAnimator.instance.animator.SetBool("trainMode", false);
        trainMode = false;
    }

    public void HamNotice(int i)
    {   //훈련모드에서 강아지가 간식을 인식했다는 뜻, 즉시 포즈를 취하고 간식이 다른 곳에 나타나도 쳐다보지 않음
        //i는 이게 어떤 포즈인지 표시
        if (!trainMode) return;

        hamNoticed = true;
        DogAnimator.instance.ActPose(i);
        behaviorType = (BehaviorType)i;
    }

    public void RecordStart()   //녹화 시작
    {
        DogAnimator.instance.trainUIAnimator.SetBool("appear", false);
        Player.instance.laser.SetActive(false);
        gestureManager.StartSensing(behaviorType);
    }

    public void RecordFin(bool didFeedHam, bool isNewGesture = false) //녹화 종료
    {
        DogAnimator.instance.animator.SetBool("petEat", didFeedHam);
        DogAnimator.instance.ActPose(-1);

        if (didFeedHam)
        {
            if (isNewGesture)
            {
                // 서버에 behaviorType 보내기
                // 기존에 폴더가 존재하면 삭제하기
                // 동기로 .onnx 받아오기
                // 제스처에 따른 강아지 행동 저장 (새로쓰기 or 덮어쓰기)
            }
            else
            {
                // 서버에 behaviorType 보내기
                // 동기로 .onnx 받아오기
                // 제스처에 따른 강아지 행동 저장 (덮어쓰기)
            }
        }
        
    }

    public void Wrong() //아뇨 다른 자세 할게요
    {
        DogAnimator.instance.ActPose(-1);
        DogAnimator.instance.trainUIAnimator.SetBool("appear", false);
        Player.instance.laser.SetActive(false);

    }

    public void HitBoxOnOff()
    {
        if (hitBoxes[0].activeSelf)
            for (int i = 0; i < 6; i++)
            {
                hitBoxes[i].SetActive(false);
            }
        else
            for (int i = 0; i < 6; i++)
            {
                hitBoxes[i].SetActive(true);
            }
    }
}
