using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using System.Web;


public class NewBehaviourScript : MonoBehaviour
{
    private void Start()
    {
        string link = "https://a2a-api.klipwallet.com/v2/a2a/prepare";
        string jsonData = "{\"bapp\": { \"name\" : \"My BApp\" }, \"callback\": { \"success\": \"mybapp:\\/\\/klipwallet\\/success\", \"fail\": \"mybapp:\\/\\/klipwallet\\/fail\" }, \"type\": \"auth\"}";

        string encodedJsonData = HttpUtility.UrlEncode(jsonData);
        string combinedLink = link + "?data=" + encodedJsonData;
        Debug.Log(combinedLink);
    }
    
}
