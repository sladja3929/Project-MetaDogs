using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public Camera cam;
    public GameObject dog;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.LookAt(dog.transform);
    }

    public void OnClickLogIn()
    {
        PlayerPrefs.SetInt("devLogin", 0);  //devLogin 여부에 따라 펫 선택 화면이 바뀜

        /*
         * 
         *
         *
         */

        //ToPetSelect();
    }

    public void OnClickDevLogin()
    {
        PlayerPrefs.SetInt("devLogin", 1);  //devLogin 여부에 따라 펫 선택 화면이 바뀜

        ToPetSelect();
    }

    public void ToPetSelect()
    {
        SceneManager.LoadScene("PetSelect");
    }
}
