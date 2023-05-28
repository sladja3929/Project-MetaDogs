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

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;


        for (int i = 0; i < effectSounds.Length; i++)
        {
            effectSounds[i].source = gameObject.AddComponent<AudioSource>();
            effectSounds[i].source.clip = effectSounds[i].clip;
            effectSounds[i].source.loop = false;
            //effectSounds[i].source.volume = tmp;

        }

        //slider.value = tmp;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetEffectVolume(Slider slider)
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            float value = Mathf.Round(slider.value * 100) * 0.01f;
            effectSounds[i].source.volume = value;
            
        }
        /*if (Time.timeScale == 0f)
            effectSounds[1].source.Play();*/
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
