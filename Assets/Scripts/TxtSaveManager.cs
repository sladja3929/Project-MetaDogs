using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class TxtSaveManager : MonoBehaviour
{
    public static TxtSaveManager instance;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //WriteTxt(Application.streamingAssetsPath + @"\save.txt");
        //SetVolume(ReadTxt(Application.streamingAssetsPath + @"\save.txt"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void WriteTxt(string filePath)
    {
        StreamWriter writer;
        writer = File.CreateText((filePath));
        writer.WriteLine("#Sound");
        writer.WriteLine("background=" + (Mathf.Round(SoundManager.instance.soundVolume * 1000) * 0.001f));
        writer.WriteLine("effect=" + (Mathf.Round(EffectManager.instance.effectVolume * 1000) * 0.001f));
        writer.Close();
        /*
        DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filePath));

        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }

        FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);

        StreamWriter writer = new StreamWriter(fileStream);

        writer.WriteLine("#Sound");
        writer.WriteLine("background=" + (Mathf.Round(SoundManager.instance.soundVolume * 1000) * 0.001f));
        writer.WriteLine("effect=" + (Mathf.Round(EffectManager.instance.effectVolume * 1000) * 0.001f));
        writer.Flush();
        writer.Dispose();
        writer.Close();
        fileStream.Close();
        */
    }
    public string ReadTxt(string filePath)
    {
        FileInfo fileInfo = new FileInfo(filePath);
        string value = "";

        if (fileInfo.Exists)
        {
            StreamReader reader = new StreamReader(filePath);
            value = reader.ReadToEnd();
            reader.Close();
        }

        /*else
            value = "������ �����ϴ�.";*/

        return value;
    }
    public void SetVolume(string txtString)
    {
        string[] lines = txtString.Split('\n'); //�ٴ����� ���ڿ� �迭�� �Ͼ���� 
        string[] soundWords = lines[1].Split('=');
        string[] effectWords = lines[2].Split('=');
        //Debug.Log(soundWords[1]);
        //Debug.Log(effectWords[1]);

        SoundManager.instance.soundVolume = float.Parse(soundWords[1]);
        SoundManager.instance.InitVolume();

        EffectManager.instance.effectVolume = float.Parse(effectWords[1]);
        EffectManager.instance.InitVolume();
    }
}
