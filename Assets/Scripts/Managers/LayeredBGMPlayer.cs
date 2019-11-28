using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class LayeredBGMPlayer : MonoBehaviour
{
    public static LayeredBGMPlayer Instance;

    public LayeredBGM currentBGM;

    public AudioMixerGroup mixer;

    [Range(0, 1)]
    public float intensity;

    Dictionary<AudioSource, LayeredBGMTrack> tracks;

    private void Awake()
    {
        #region Singleton
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(gameObject);
        #endregion

        tracks = new Dictionary<AudioSource, LayeredBGMTrack>();
    }
    private void Update()
    {
        foreach (KeyValuePair<AudioSource, LayeredBGMTrack> track in tracks)
        {
            track.Key.volume = track.Value.loudnessCurve.Evaluate(intensity);
        }
    }

    private AudioSource CreateSoundSource(LayeredBGMTrack track)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        tracks.Add(source, track);

        source.outputAudioMixerGroup = mixer;
        source.clip = track.clip;
        source.loop = track.loop;
        source.maxDistance = Mathf.Infinity;

        return source;
    }

    private void RemoveAudioSource(AudioSource source)
    {
        Destroy(source);
    }

    public void LoadBGM()
    {
        foreach (LayeredBGMTrack track in currentBGM.tracks)
        {
            AudioSource source = CreateSoundSource(track);
        }
    }

    public void UnloadBGM()
    {
        foreach (AudioSource source in tracks.Keys)
        {
            RemoveAudioSource(source);
        }
        tracks.Clear();
    }

    public void Play()
    {
        foreach (AudioSource source in tracks.Keys)
        {
            source.Play();
        }
    }

    public void Stop()
    {
        foreach (AudioSource source in tracks.Keys)
        {
            source.Stop();
        }
    }

    private void Start()
    {
        LoadBGM();
        Play();
    }
}
