using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager instance;
    public Material[] skyboxes;

    public GameObject sunLight;
    public GameObject rain;

    public int curWeather;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        int tmp = UnityEngine.Random.Range(0, 10);
        if (tmp < 2)
            ChangeWeather(1);
        else if (6 < DateTime.Now.Hour && DateTime.Now.Hour < 18)
            ChangeWeather(0);
        else
            ChangeWeather(2);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            ChangeWeather(0);
        if (Input.GetKeyDown(KeyCode.F2))
            ChangeWeather(1);
        if (Input.GetKeyDown(KeyCode.F3))
            ChangeWeather(2);
    }

    public void ChangeWeather(int i)
    {
        RenderSettings.skybox = skyboxes[i];
        if (i == 1) rain.SetActive(true);
        else rain.SetActive(false);

        if (i == 2)
        {
            sunLight.SetActive(false);
            RenderSettings.fog = false;
        }
        else
        {
            sunLight.SetActive(true);
            RenderSettings.fog = true;
        }
        SoundManager.instance.ChangeMusic(i);
    }
}
