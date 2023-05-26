using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PetNFT
{
    public int pet_token;
    public string pet_name;
    public int pet_age;
    public string pet_sex; //(f는 암컷, m은 수컷)
    //pet_color: png 파일
    public Texture pet_color;
    //model: onnx 파일
    //property: yaml 파일
};

public class PetSelectManager : MonoBehaviour
{
    public static PetSelectManager instance;
    public string sampleWallet_i;   //wallet id를 전달받았다 가정

    public PetNFT[] sampleNFTs; //n개의 nft 파일을 전달받았다 가정, 인스펙터창에서 수정 가능

    public TextMeshProUGUI boyuPet, nftId, petName, age, gender; //클립보드에 나타나는 펫 정보들

    public SkinnedMeshRenderer corgiMesh;   //펫 스킨 적용 대상
    public Texture[] petTextures;

    public int petArrIdx;   //현재 화면에 보여지는 강아지가 몇 번째 강아지인지 (0부터 시작)

    public RawImage[] fades = new RawImage[2];   //컷 신 넘어갈 때 페이드아웃 연출
    public AudioSource audioSource; //배경음
    



    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        PetChange();
    }

    // Update is called once per frame
    void Update()
    {   //펫 넘기기, 펫 선택
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            PrevPet();
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            NextPet();
        if (Input.GetKeyDown(KeyCode.Space))
            PetSelect();
    }

    public void PetChange() //펫 텍스처 변경, 클립보드의 텍스트 변경
    {
        corgiMesh.materials[0].SetTexture("_BaseMap", sampleNFTs[petArrIdx].pet_color);

        boyuPet.text = "보유 펫 " + (petArrIdx + 1) + "/" + sampleNFTs.Length;
        nftId.text = "펫 NFT ID\n" + sampleNFTs[petArrIdx].pet_token;
        petName.text = "이름: " + sampleNFTs[petArrIdx].pet_name;
        age.text = "나이: " + sampleNFTs[petArrIdx].pet_age + "months";

        if (sampleNFTs[petArrIdx].pet_sex == "f")
            gender.text = "성별: ♀";
        else if (sampleNFTs[petArrIdx].pet_sex == "m")
            gender.text = "성별: ♂";
        else
            gender.text = "성별: ?";
    }

    public void PrevPet()   //펫 목록 한 칸 전으로 이동
    {
        petArrIdx--;
        if (petArrIdx < 0)
            petArrIdx += sampleNFTs.Length;
        PetChange();
    }
    public void NextPet()   //펫 목록 한 칸 뒤로 이동
    {
        petArrIdx++;
        if (petArrIdx >= sampleNFTs.Length)
            petArrIdx -= sampleNFTs.Length;
        PetChange();
    }
    public void PetSelect() //펫 선택
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()   //페이드아웃, 씬 전환
    {
        float tmpVol = audioSource.volume;
        while (fades[0].transform.localScale.x < 9)
        {
            fades[0].transform.localScale += new Vector3(9 * Time.deltaTime, 0, 0);
            fades[1].transform.localScale += new Vector3(9 * Time.deltaTime, 0, 0);
            audioSource.volume -= tmpVol * Time.deltaTime;

            yield return null;
        }
        //DontDestroyOnLoad(gameObject);
        //yield return new WaitForSeconds(1);
        //SceneManager.LoadScene("CutScene");
        ToIngameScene();
    }

    public void ToIngameScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}
