using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PetNFT
{
    public string pet_token;
    public string pet_name;
    public int pet_age;
    public string pet_sex; //(f는 암컷, m은 수컷)
    //pet_color: png 파일
    public Texture pet_color;
    public float pet_emotion;
    //model: onnx 파일
    //property: yaml 파일
};

[System.Serializable]
public class PetJson
{
    public PetNFT[] nftList;
};

public class NftManager : MonoBehaviour
{
    public static NftManager instance;

    public string walletID;

    public PetJson petJson;
    public PetNFT[] nftList;
    public PetNFT selected;
    public int listLen;

    public bool jumpScroll;
    public bool attackScroll;

    public bool autoToy;

    public int weather;

    private void Awake()
    {
        instance = this;
        nftList = new PetNFT[listLen];
    }
    // Start is called before the first frame update
    void Start()
    {


        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }
    public void LoadTextures()  //펫 NFT 프로퍼티들 가져올 때 같이 호출해주세요
    {
        for (int i = 0; i < nftList.Length; i++)
        {
            nftList[i].pet_color = Resources.Load<Texture2D>(nftList[i].pet_token + "_texture");
        }
    }
    public void NftSetting()
    {
        SpawnManager.instance.corgiMesh.materials[0].SetTexture("_BaseMap", selected.pet_color);

        if (jumpScroll)
            TrainManager.instance.petPosesEnabled[2] = true;
        if (attackScroll)
            TrainManager.instance.petPosesEnabled[5] = true;

        if (autoToy)
            SpawnManager.instance.autoToy.SetActive(true);

        WeatherManager.instance.ChangeWeather(weather);
    }

    
}
