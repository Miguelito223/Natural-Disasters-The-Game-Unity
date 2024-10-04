using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "LightPreset", menuName = "Scripts/LightPreset", order = 1)]
public class LightPresent : ScriptableObject
{
   public Gradient Ambientcolor;
   public Gradient Direcionalcolor;
   public Gradient FogColor;
}
