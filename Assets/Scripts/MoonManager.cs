using System;
using Unity.Mathematics;
using UnityEngine;

[ExecuteAlways]
public class MoonManager : MonoBehaviour
{
    [SerializeField] public SunManager Sun;
    [SerializeField] public Light Moon;

    private void Update()
    {
        updatetime(Sun.hours / 24f);
    }

    private void updatetime(float timeporcent)
    {
        if (Moon != null)
        {
            Moon.transform.localRotation = Quaternion.Euler(new Vector3((timeporcent * -360f) + 90f, 170f, 0));
        }
    }

}
