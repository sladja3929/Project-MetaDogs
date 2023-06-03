using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource audioSource;
    public Slider soundSlider;
    public float soundVolume;

    public AudioClip[] sounds;
    void Awake()
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
    public void SetBgmVolume(Slider slider)
    {
        audioSource.volume = slider.value / 6;
    }
    public void SetPlus(Slider slider)
    {
        if (SettingManager.instance.saveCor == null) return;
        slider.value++;
        soundVolume = slider.value / 6;
        TxtSaveManager.instance.WriteTxt(Application.streamingAssetsPath + @"\save.txt");
        slider.onValueChanged.Invoke(0);
    }
    public void SetMinus(Slider slider)
    {
        if (SettingManager.instance.saveCor == null) return;
        slider.value--;
        soundVolume = slider.value / 6;
        TxtSaveManager.instance.WriteTxt(Application.streamingAssetsPath + @"\save.txt");
        slider.onValueChanged.Invoke(0);
    }
    public void ChangeMusic(int i)
    {
        audioSource.clip = sounds[i];
        audioSource.Play();
    }
    public void InitVolume()
    {
        audioSource.volume = soundVolume;
        soundSlider.value = Mathf.Round(soundVolume * 6);
    }
}
