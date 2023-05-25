using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public Canvas canvas;
    public Transform canvasPos;
    public GameObject laser;
    public bool vrMode;
    public GameObject ham;
    public GameObject controller;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //if (vrMode) GetComponent<CapsuleCollider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (vrMode) //vrMode 켜져있을 때 메뉴창 열기
        {
            if (OVRInput.GetDown(OVRInput.Button.Four))
                if (canvas.gameObject.activeSelf)
                {
                    canvas.gameObject.SetActive(false);
                    if (!DogAnimator.instance.trainUIAnimator.GetBool("appear"))
                        laser.SetActive(false);
                }
                else
                {
                    //canvas.transform.SetParent(transform);
                    canvas.transform.position = canvasPos.position;
                    canvas.transform.rotation = canvasPos.rotation;
                    canvas.transform.rotation *= Quaternion.Euler(0, 1, 0);
                    //canvas.transform.SetParent(transform.parent);
                    canvas.gameObject.SetActive(true);
                    laser.SetActive(true);
                }
        }
        else if (Input.GetKeyDown(KeyCode.Q))   //vrMode 꺼져있을 때 메뉴창 열기
        {
            if (canvas.gameObject.activeSelf)
            {
                canvas.gameObject.SetActive(false);
                if (!DogAnimator.instance.trainUIAnimator.GetBool("appear"))
                    laser.SetActive(false);
            }
            else
            {
                //canvas.transform.SetParent(transform);
                canvas.transform.position = canvasPos.position;
                canvas.transform.rotation = canvasPos.rotation;
                canvas.transform.rotation *= Quaternion.Euler(0, 1, 0);
                //canvas.transform.SetParent(transform.parent);
                canvas.gameObject.SetActive(true);
                laser.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.E) || OVRInput.GetDown(OVRInput.Button.Three))    //빈 손, 간식, 장난감 스왑
        {
            if (!TrainManager.instance.trainMode)
                if (!ham.activeSelf && !controller.activeSelf)
                    ham.SetActive(true);
                else if (!controller.activeSelf)
                {
                    ham.SetActive(false);
                    controller.SetActive(true);
                }
                else
                    controller.SetActive(false);
            else if (!ham.activeSelf)
                ham.SetActive(true);
            else
                ham.SetActive(false);
        }

        /*if (Input.GetKeyDown(KeyCode.R) || OVRInput.GetDown(OVRInput.Button.Three))    //장난감 키기
        {
            if (!controller.activeSelf)
                controller.SetActive(true);
            else
                controller.SetActive(false);
        }*/
    }
    void FixedUpdate()
    {

    }
}
