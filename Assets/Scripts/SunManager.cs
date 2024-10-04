using System;
using Unity.Mathematics;
using UnityEngine;

[ExecuteAlways]
public class SunManager : MonoBehaviour
{
    [SerializeField] public Light SunDirLight;
    [SerializeField] public LightPresent Present;

    [SerializeField, Range(0,24)] public float hours;
    [SerializeField, Range(0, 60)] public float minutes;
    [SerializeField, Range(0, 60)] public float seconds;
    [SerializeField] public float days;
    [SerializeField, Range(0, 1000)] public float speed;
    [SerializeField] public bool PM_or_AM;

    private void Start()
    {
        GlobalsVariables.instance.SunDirLight = this;
    }
    private void Update()
    {
        if (Present == null) 
            return;

        if (Application.isPlaying)
        {
            seconds += speed * Time.deltaTime;

            if (seconds > 60)
            {
                seconds = 0;
                minutes++;
            }
            if (minutes > 60)
            {
                minutes = 0;
                hours++;
            }

            if (!PM_or_AM)
            {
                if (hours > 24)
                {
                    hours = 0;
                    days++;
                }
            }
            else
            {
                if (hours > 12)
                {
                    hours = 0;
                    days++;
                }
            }

            updatetime(hours / 24f);
        }
        else
        {
            updatetime(hours / 24f);
        }
    }

    private void updatetime(float timeporcent)
    {
        RenderSettings.ambientLight = Present.Ambientcolor.Evaluate(timeporcent);
        RenderSettings.fogColor = Present.FogColor.Evaluate(timeporcent);
        

        if (SunDirLight != null)
        {
            SunDirLight.color = Present.Direcionalcolor.Evaluate(timeporcent);
            SunDirLight.transform.localRotation = Quaternion.Euler(new Vector3((timeporcent * 360f) - 90f, 170f, 0));
        }
    }
    private void OnValidate()
    {
        if (SunDirLight != null)
            return;
        

        if (RenderSettings.sun != null)
        {
            SunDirLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsByType<Light>(FindObjectsSortMode.None);

            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    SunDirLight = light;
                    return;

                }
            }
        }
    }
}
