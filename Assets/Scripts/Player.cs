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
            if (OVRInput.GetDown(OVRInput.Button.Start))
                if (canvas.gameObject.activeSelf)
                {
                    canvas.gameObject.SetActive(false);
                    if (!DogAnimator.instance.trainUIAnimator.GetBool("appear"))
                        laser.SetActive(false);
                }
                else
                {
                    EffectManager.instance.PlayEffect(1);
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
                EffectManager.instance.PlayEffect(1);
                //canvas.transform.SetParent(transform);
                canvas.transform.position = canvasPos.position;
                canvas.transform.rotation = canvasPos.rotation;
                canvas.transform.rotation *= Quaternion.Euler(0, 1, 0);
                //canvas.transform.SetParent(transform.parent);
                canvas.gameObject.SetActive(true);
                laser.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.E) || OVRInput.GetDown(OVRInput.Button.Three))    //간식 키기
        {
            EffectManager.instance.PlayEffect(2);
            if (!ham.activeSelf)
            {
                ham.SetActive(true);
                controller.SetActive(false);
            }
            else
                ham.SetActive(false);
        }

        if (DogMove.instance.toy.gameObject.activeSelf)
            if (Input.GetKeyDown(KeyCode.R) || OVRInput.GetDown(OVRInput.Button.Four))    //장난감 키기
            {
                EffectManager.instance.PlayEffect(2);
                if (!controller.activeSelf)
                {
                    controller.SetActive(true);
                    ham.SetActive(false);
                }
                else
                    controller.SetActive(false);
            }
    }
    void FixedUpdate()
    {

    }
}
