using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    public SkinnedMeshRenderer corgiMesh;
    public GameObject autoToy;

    public Transform[] playerSpawnPoints = new Transform[2];
    public Transform[] petSpawnPoints = new Transform[2];

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("NftManager"))
            NftManager.instance.NftSetting();
        PositionSetting();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void PositionSetting()
    {
        //playMode가 0이면 일상모드, 1이면 훈련모드 위치로 스폰
        //playMode는 설정UI에서 모드 전환 누를 때마다 바뀜
        //PlayerPrefs에 저장되기 때문에 훈련모드인 채로 게임 껐다 키면 훈련모드로 스폰됨
        if (PlayerPrefs.HasKey("playMode"))
        {
            if (PlayerPrefs.GetInt("playMode") == 0)
            {
                Player.instance.transform.position = playerSpawnPoints[0].position;
                Player.instance.transform.rotation = playerSpawnPoints[0].rotation;
                DogAnimator.instance.gameObject.transform.position = petSpawnPoints[0].position;
                DogAnimator.instance.gameObject.transform.rotation = petSpawnPoints[0].rotation;
                TrainManager.instance.TrainModeDisable();
            }
            else if (PlayerPrefs.GetInt("playMode") == 1)
            {
                TrainManager.instance.table.SetActive(false);
                Player.instance.transform.position = playerSpawnPoints[1].position;
                Player.instance.transform.rotation = playerSpawnPoints[1].rotation;
                DogAnimator.instance.gameObject.transform.position = petSpawnPoints[1].position;
                DogAnimator.instance.gameObject.transform.rotation = petSpawnPoints[1].rotation;
                TrainManager.instance.TrainModeEnable();
            }
        }
        else
        {   //게임을 처음 켰으면 일상모드로 스폰
            PlayerPrefs.SetInt("playMode", 0);
            Player.instance.transform.position = playerSpawnPoints[0].position;
            Player.instance.transform.rotation = playerSpawnPoints[0].rotation;
            DogAnimator.instance.gameObject.transform.position = petSpawnPoints[0].position;
            DogAnimator.instance.gameObject.transform.rotation = petSpawnPoints[0].rotation;
        }
    }
}
