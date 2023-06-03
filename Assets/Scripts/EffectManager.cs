using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class Effect
{
    public string soundName;
    public AudioClip clip;
    public AudioSource source;
}
public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    public Slider slider;
    public Effect[] effectSounds;
    public float effectVolume;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;


        for (int i = 0; i < effectSounds.Length; i++)
        {
            effectSounds[i].source = gameObject.AddComponent<AudioSource>();
            effectSounds[i].source.clip = effectSounds[i].clip;
            effectSounds[i].source.loop = false;
            if (i > 4)
                effectSounds[i].source.loop = true;
            //effectSounds[i].source.volume = effectVolume;

        }

        //slider.value = tmp;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitVolume()
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            effectSounds[i].source.volume = effectVolume;
            //if (i > 4)
            effectSounds[i].source.volume = effectVolume * 0.5f;
        }
        slider.value = Mathf.Round(effectVolume * 6);
    }

    public void SetEffectVolume(Slider slider)
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            effectSounds[i].source.volume = slider.value / 6;
            //if (i > 4)
            effectSounds[i].source.volume = effectVolume * 0.5f;
            //
        }
        /*if (Time.timeScale == 0f)
            effectSounds[1].source.Play();*/
    }
    public void SetPlus(Slider slider)
    {
        if (SettingManager.instance.saveCor == null) return;
        slider.value++;
        effectVolume = slider.value / 6;
        TxtSaveManager.instance.WriteTxt(Application.streamingAssetsPath + @"\save.txt");
        slider.onValueChanged.Invoke(0);
    }
    public void SetMinus(Slider slider)
    {
        if (SettingManager.instance.saveCor == null) return;
        slider.value--;
        effectVolume = slider.value / 6;
        TxtSaveManager.instance.WriteTxt(Application.streamingAssetsPath + @"\save.txt");
        slider.onValueChanged.Invoke(0);
    }

    public void StopAll()
    {
        for (int i = 4; i < effectSounds.Length; i++)
        {
            effectSounds[i].source.Stop();
        }
    }

    public void PlayEffect(int i)
    {
        effectSounds[i].source.Play();
    }

    public void OnPointerDown(BaseEventData eventData)
    {
        //effectSounds[34].source.Play();
    }
    public void OnPointerUp(BaseEventData eventData)
    {
        //effectSounds[34].source.Stop();
    }

}
