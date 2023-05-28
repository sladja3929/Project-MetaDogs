using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.Data;
using static QRcodegenerate;
using System.Collections.Generic;
using static System.Net.WebRequestMethods;
using System;

public class QRcodegenerate : MonoBehaviour
{
    //
    public RawImage qrCodeImage;
    public string data = "https://klipwallet.com/?target=/a2a?request_key=";
    public string getData = "https://a2a-api.klipwallet.com/v2/a2a/result?request_key=";

    public class Status
    {
        public string request_key;
        public string result;
        public string status;
        public string expiration_time;
    }

    private void Start()
    {
        // Post prepare request키 받아오기
        StartCoroutine(UnityWebRequestPostTest());
    }

    IEnumerator UnityWebRequestPostTest()
    {
        string url = "https://a2a-api.klipwallet.com/v2/a2a/prepare";
        string result;
        // Create the JSON data to send
        string json = "{\"bapp\": {\"name\": \"METADOGS\"}, \"callback\": {\"success\": \"mybapp://klipwallet/success\", \"fail\": \"mybapp://klipwallet/fail\"}, \"type\": \"auth\"}";

        // Create a UnityWebRequest
        UnityWebRequest www = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        // Wait for the response and then get our data
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Status convertJson = JsonUtility.FromJson<Status>(www.downloadHandler.text);
            data += convertJson.request_key;    // Request Key가 생성되면 해당 키를 이용하여 사용자 인증 링크를 생성
            getData += convertJson.request_key;  // 사용자 인증이 완료되었는지 검증하는 링크에도 Request Key가 필요함.
        }
    }

    public void buttonClick()
    {
        GenerateQR();   // 클립으로 로그인 버튼을 누르면 QR코드를 생성을 함
        StartCoroutine(GET());  // QR코드 생성과 동시에 코루틴을 실행하여 사용자 인증을 확인함
    }

    private void GenerateQR()   // QR코드 생성 함수
    {
        int width = 256;
        int height = 256;
        Debug.Log(getData);
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };

        Color32[] colorArray = writer.Write(data);
        Texture2D qrCodeTexture = new Texture2D(width, height);
        qrCodeTexture.SetPixels32(colorArray);
        qrCodeTexture.Apply();

        qrCodeImage.texture = qrCodeTexture;
    }

    private IEnumerator GET()  // 사용자가 인증을 했는지 확인하는 것을 GET통신으로 구현
    {
        string result;
        UnityWebRequest www = UnityWebRequest.Get(getData);
        yield return www.SendWebRequest();
        result = www.downloadHandler.text;
        Debug.Log(result);
        Status convertJson = JsonUtility.FromJson<Status>(result);  // 결과값(string)을 json 형태로 변환
        Debug.Log(convertJson.status);

        if (convertJson.status == "prepared")   // 사용자가 인증을 하지 않으면 계속 prepared 상태의 값이 돌아옴
        {
            yield return new WaitForSeconds(5.0f);  // 약간의 시간 지연을 주어서
            StartCoroutine(GET());                  // 재귀를 이용하여 계속 호출.
        }
        else
        {
            Debug.Log("Completed");  // 사용자 인증이 완료되면 완료 로그 출력
            // 이후에 NFT 강아지 선택 씬으로 이동하는 코드 작성 예정.
        }
    }

}
