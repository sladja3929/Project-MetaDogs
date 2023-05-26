using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PetNFT
{
    public int pet_token;
    public string pet_name;
    public int pet_age;
    public string pet_sex; //(f�� ����, m�� ����)
    //pet_color: png ����
    public Texture pet_color;
    //model: onnx ����
    //property: yaml ����
};
public class NftManager : MonoBehaviour
{
    public static NftManager instance;
    public PetNFT[] nftList;
    public PetNFT selected;
    public int listLen;

    public bool jumpScroll;
    public bool attackScroll;

    public bool autoToy;

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
    public void NftSetting()
    {
        SpawnManager.instance.corgiMesh.materials[0].SetTexture("_BaseMap", selected.pet_color);

        if (jumpScroll)
            TrainManager.instance.petPosesEnabled[2] = true;
        if (attackScroll)
            TrainManager.instance.petPosesEnabled[5] = true;

        if (autoToy)
            SpawnManager.instance.autoToy.SetActive(true);
    }
}