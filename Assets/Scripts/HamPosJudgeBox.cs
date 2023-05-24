using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HamPosJudgeBox : MonoBehaviour
{
    public int boxNum;
    public Image guage;
    bool entered;
    float t;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!TrainManager.instance.trainMode) return;

        if (TrainManager.instance.hamNoticed)
        {
            entered = false;
            if (t > 0 && guage.fillAmount != 0)
            {
                guage.fillAmount = 0;
                t = TrainManager.instance.maxHamJudgeTime;
            }
        }
        if (DogAnimator.instance.animator.GetInteger("petPose") == -1)
            if (entered)
            {
                t -= Time.deltaTime;
                guage.fillAmount = 1 - (t / TrainManager.instance.maxHamJudgeTime);
            }
        if (t < 0)
        {
            TrainManager.instance.HamNotice(boxNum);
            Invoke("DisappearGuage", 1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!TrainManager.instance.trainMode) return;

        if (other.CompareTag("Ham"))
        {
            if (!TrainManager.instance.hamNoticed)
            {
                entered = true;
                t = TrainManager.instance.maxHamJudgeTime;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!TrainManager.instance.trainMode) return;

        if (other.CompareTag("Ham"))
        {
            entered = false;
            t = TrainManager.instance.maxHamJudgeTime;
            guage.fillAmount = 0;
        }
    }
    void DisappearGuage()
    {
        guage.fillAmount = 0;
        t = TrainManager.instance.maxHamJudgeTime;
    }
}
