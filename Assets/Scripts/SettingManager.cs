using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingManager : MonoBehaviour
{
    public static SettingManager instance;
    public Coroutine saveCor;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToDailyMode()
    {   //훈련모드로
        if (saveCor != null) return;
        PlayerPrefs.SetInt("playMode", 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToTrainingMode()
    {   //일상모드로
        if (saveCor != null) return;
        PlayerPrefs.SetInt("playMode", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToPetSelect()
    {   //펫 선택 모드로
        if (saveCor != null) return;
        if (GameObject.Find("NftManager"))
            NftManager.instance.DestroyThis();
        SceneManager.LoadScene("PetSelect");
    }
    public void Save()
    {   //수동 세이브 클릭
        if (saveCor != null) return;
        saveCor = StartCoroutine(SaveData());
    }

    public void ExitGame()
    {   //종료
        if (saveCor != null) return;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
    public IEnumerator ExitCor()
    {
        yield return StartCoroutine(SaveData());
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
    public IEnumerator SaveData()
    {
        Debug.Log(1);
        yield return StartCoroutine(RequestManager.Instance.SavePetProperty());
        yield return StartCoroutine(RequestManager.Instance.SaveSettings());
        saveCor = null;
    }
}
