using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource audioSource;
    public Slider soundSlider;
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
        slider.value++;
        slider.onValueChanged.Invoke(0);
    }
    public void SetMinus(Slider slider)
    {
        slider.value--;
        slider.onValueChanged.Invoke(0);
    }
}
