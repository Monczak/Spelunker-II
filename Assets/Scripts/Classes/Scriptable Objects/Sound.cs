using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Sound", menuName = "Scriptable Objects/Sound")]
public class Sound : ScriptableObject
{
    public AudioClip clip;
    public float volume = 1, pitch = 1;
    public SoundType type;
    public bool enableRayTracing;
}
