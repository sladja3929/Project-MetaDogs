using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingManager : MonoBehaviour
{
    public static SettingManager instance;

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
        PlayerPrefs.SetInt("playMode", 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToTrainingMode()
    {   //일상모드로
        PlayerPrefs.SetInt("playMode", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToPetSelect()
    {   //펫 선택 모드로
        SceneManager.LoadScene("PetSelect");
    }
    public void Save()
    {   //수동 세이브 클릭

    }

    public void ExitGame()
    {   //종료
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
}
