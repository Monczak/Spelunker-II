using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SoundObject : MonoBehaviour
{
    public AudioSource source;
    //SoundRayTracer rayTracer;

    public Sound sound;

    private void OnClipEnd()
    {
        Destroy(gameObject);
    }

    public void Setup()
    {
        source = GetComponent<AudioSource>();

        source.clip = sound.clip;
        source.volume = sound.volume;
        source.pitch = sound.pitch;
        source.playOnAwake = false;

        gameObject.name = $"Sound ({source.clip.name})";

        //rayTracer = GetComponent<SoundRayTracer>();
        //rayTracer.SetupRays();

        //var result = rayTracer.EvaluateEnvironment();

        //rayTracer.SetupFilters(result);

        Invoke("OnClipEnd", source.clip.length + 1);
    }
}
