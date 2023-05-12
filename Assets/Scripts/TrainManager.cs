using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainManager : MonoBehaviour
{
    public static TrainManager instance;
    public bool trainMode;

    public float maxHamJudgeTime;
    public bool hamNoticed;

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
    }

    public void RecordStart()   //녹화 시작, 오민 '해 줘'
    {
        DogAnimator.instance.trainUIAnimator.SetBool("appear", false);
        Player.instance.laser.SetActive(false);

    }

    public void RecordFin() //녹화 종료
    {

        DogAnimator.instance.animator.SetBool("petEat", true); //냠냠 시작
        DogAnimator.instance.ActPose(-1);
    }

    public void Wrong() //아뇨 다른 자세 할게요
    {
        DogAnimator.instance.ActPose(-1);
        DogAnimator.instance.trainUIAnimator.SetBool("appear", false);
        Player.instance.laser.SetActive(false);

    }
}
