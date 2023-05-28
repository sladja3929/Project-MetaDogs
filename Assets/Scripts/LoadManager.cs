using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadManager : MonoBehaviour
{
    public int loadType;
    // Start is called before the first frame update
    void Start()
    {
        if (loadType == 0)  //펫 선택 화면 로딩
        {
            RequestManager.Instance.StartLoadingTitleToPetSelect();
        }
        else if (loadType == 1) //인게임 씬 로딩
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
