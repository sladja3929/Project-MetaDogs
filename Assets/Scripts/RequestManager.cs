using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class RequestManager : Singleton<RequestManager>
{
    public IEnumerator LoadSettings()
    {
        string url = "http://203.250.148.33:20080/db/load_settings"; // Replace with your API endpoint        
        string json = "{\"wallet_id\":\"" + NftManager.instance.walletID + "\"}"; // Replace with your JSON data

        using (UnityWebRequest www = UnityWebRequest.Post(url, json))
        {
            byte[] jsonBytes = new System.Text.UTF8Encoding().GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(jsonBytes);
            www.SetRequestHeader("Content-Type", "application/json");
            www.downloadHandler = new DownloadHandlerFile(Application.streamingAssetsPath + @"\save.txt");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {

            }

            www.Dispose();
        }
    }

    public IEnumerator LoadPetList()
    {
        string url = "http://203.250.148.33:20080/db/load_pet_list"; // Replace with your API endpoint
        string json = "{\"wallet_id\":\"" + NftManager.instance.walletID + "\"}"; // Replace with your JSON data
        
        using (UnityWebRequest www = UnityWebRequest.Post(url, json))
        {
            byte[] jsonBytes = new System.Text.UTF8Encoding().GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(jsonBytes);
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }

            else
            {
                NftManager.instance.petJson = JsonUtility.FromJson<PetJson>("{\"nftList\":" + www.downloadHandler.text + "}");
            }

            www.Dispose();
        }
    }

    public IEnumerator LoadPetProperty(int index)
    {
        string url = "http://203.250.148.33:20080/db/load_pet_property"; // Replace with your API endpoint
        string json = "{\"pet_token\":\"" + NftManager.instance.petJson.nftList[index].pet_token + "\"}"; // Replace with your JSON data
        
        using (UnityWebRequest www = UnityWebRequest.Post(url, json))
        {
            byte[] jsonBytes = new System.Text.UTF8Encoding().GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(jsonBytes);
            www.SetRequestHeader("Content-Type", "application/json");
            Debug.Log("{\"pet_token\":\"" + NftManager.instance.petJson.nftList[index].pet_token + "\"}");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }

            else
            {
                NftManager.instance.petJson.nftList[index] = JsonUtility.FromJson<PetNFT>(www.downloadHandler.text);
            }

            www.Dispose();
        }
    }

    public IEnumerator LoadPetTexture(int index)
    {
        string url = "http://203.250.148.33:20080/db/load_pet_texture"; // Replace with your API endpoint
        string json = "{\"pet_token\":\"" + NftManager.instance.petJson.nftList[index].pet_token + "\"}"; // Replace with your JSON data
        Debug.Log("{\"pet_token\":\"" + NftManager.instance.petJson.nftList[index].pet_token + "\"}");
        using (UnityWebRequest www = UnityWebRequest.Post(url, json))
        {
            byte[] jsonBytes = new System.Text.UTF8Encoding().GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(jsonBytes);
            www.SetRequestHeader("Content-Type", "application/json");
            www.downloadHandler = new DownloadHandlerFile(Application.streamingAssetsPath + "\\" + NftManager.instance.petJson.nftList[index].pet_token + "_texture.png");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {

            }

            www.Dispose();
        }
    }

    public IEnumerator SaveSettings()
    {
        string url = "http://203.250.148.33:20080/db/save_settings"; // Replace with your API endpoint
        string filePath = Application.streamingAssetsPath + @"\save.txt"; // Replace with your text file path

        WWWForm form = new WWWForm();

        // Add form data
        form.AddField("wallet_id", NftManager.instance.walletID);

        // Add file
        byte[] fileBytes = File.ReadAllBytes(filePath);
        form.AddBinaryData("savedata", fileBytes, "save.txt", "text/plain");

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }

            www.Dispose();
        }
    }

    public IEnumerator SavePetProperty()
    {
        string url = "http://203.250.148.33:20080/db/save_pet_property"; // Replace with your API endpoint
        string filePath = Application.streamingAssetsPath + @"\" + NftManager.instance.selected.pet_token + "_texture.png"; // Replace with your text file path

        WWWForm form = new WWWForm();

        // Add form data
        form.AddField("pet_token", NftManager.instance.selected.pet_token);
        form.AddField("pet_name", NftManager.instance.selected.pet_name);
        form.AddField("pet_age", NftManager.instance.selected.pet_age);
        form.AddField("pet_sex", NftManager.instance.selected.pet_sex);
        form.AddField("pet_emotion", NftManager.instance.selected.pet_emotion.ToString());

        // Add file
        byte[] fileBytes = File.ReadAllBytes(filePath);
        form.AddBinaryData("pet_texture", fileBytes, NftManager.instance.selected.pet_token + "_texture.png", "image/png");

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }

            www.Dispose();
        }
    }
}
