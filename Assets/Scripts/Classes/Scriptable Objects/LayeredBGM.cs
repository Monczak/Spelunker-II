using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Layered BGM", menuName = "Scriptable Objects/Layered BGM")]
public class LayeredBGM : ScriptableObject
{
    public List<LayeredBGMTrack> tracks;
}

[Serializable]
public struct LayeredBGMTrack
{
    public AudioClip clip;
    public AnimationCurve loudnessCurve;

    public bool loop;
}
