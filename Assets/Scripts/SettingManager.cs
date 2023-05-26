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
    {   //�Ʒø���
        PlayerPrefs.SetInt("playMode", 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToTrainingMode()
    {   //�ϻ����
        PlayerPrefs.SetInt("playMode", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToPetSelect()
    {   //�� ���� ����
        if (GameObject.Find("NftManager"))
            NftManager.instance.DestroyThis();
        SceneManager.LoadScene("PetSelect");
    }
    public void Save()
    {   //���� ���̺� Ŭ��

    }

    public void ExitGame()
    {   //����
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // ���ø����̼� ����
#endif
    }
}