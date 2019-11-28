using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioLowPassFilter), typeof(AudioEchoFilter))]
public class SoundRayTracer : MonoBehaviour
{
    public Collider listenerCollider;

    public int numberOfRays = 1000;
    public LayerMask mask;

    SoundObject soundObject;
    AudioEchoFilter echo;
    AudioLowPassFilter lowPass;

    List<SoundRay> rays;

    private void Awake()
    {
        soundObject = GetComponent<SoundObject>();
        echo = GetComponent<AudioEchoFilter>();
        lowPass = GetComponent<AudioLowPassFilter>();

        listenerCollider = GameManager.Instance.playerController.listenerCollider;
    }

    public EnvironmentEvaluationResult EvaluateEnvironment()
    {
        Debug.Log($"Evaluating environment around {soundObject.sound.name}");

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        int maxIterations = 50;
        int currentIteration = 0;

        List<int> toRemove = new List<int>();

        float minimumDelay = Mathf.Infinity;

        float cumulativeAttenuation = 0;
        int hitPlayer = 0;

        while (rays.Count > 0 && currentIteration++ < maxIterations)
        {
            for (int i = 0; i < rays.Count; i++)
            {
                SoundRayTracingResult result = GetNextRay(rays[i], out SoundRay newRay);

                switch (result)
                {
                    case SoundRayTracingResult.HitNothing:
                        toRemove.Add(i);
                        break;
                    case SoundRayTracingResult.HitPlayer:
                        float delay = newRay.totalDistance / Config.speedOfSound;
                        minimumDelay = delay < minimumDelay ? delay : minimumDelay;

                        float attenuation = 1 - newRay.energy;
                        hitPlayer++;
                        cumulativeAttenuation += attenuation;

                        toRemove.Add(i);
                        break;
                    case SoundRayTracingResult.HitWall:
                        rays[i] = newRay;
                        break;
                    default: break;
                }
            }

            foreach (int i in toRemove.OrderByDescending(e => e))
                rays.RemoveAt(i);
            toRemove.Clear();
        }

        EnvironmentEvaluationResult evaluationResult = new EnvironmentEvaluationResult();

        if (hitPlayer == 0)
        {
            evaluationResult.delay = 0;
            evaluationResult.lowpass = 1;
            evaluationResult.volume = 0;
        }
        else
        {
            float meanAttenuation = cumulativeAttenuation / hitPlayer;

            evaluationResult.delay = minimumDelay;
            evaluationResult.lowpass = (1 - meanAttenuation) * (1 - meanAttenuation);
            evaluationResult.volume = 1 - meanAttenuation;
        }

        stopwatch.Stop();
        Debug.Log($"Evaluation complete - took {stopwatch.ElapsedMilliseconds} ms");

        return evaluationResult;
    }

    SoundRayTracingResult GetNextRay(SoundRay ray, out SoundRay newRay)
    {
        bool collided = Physics.Raycast(ray.ray, out RaycastHit hit, ray.energy / Config.soundAttenuationPerUnit, mask);

        newRay = new SoundRay
        {
            // Subtract .1 * ray direction to make the origin of the new ray not be inside a collider, otherwise bad things happen
            ray = new Ray(hit.point - ray.ray.direction * .1f, Vector3.Reflect(ray.ray.direction, hit.normal)),
            totalDistance = ray.totalDistance + hit.distance
        };
        newRay.energy = ray.energy - (hit.distance * Config.soundAttenuationPerUnit);

        //Debug.DrawRay(ray.ray.origin, ray.ray.direction * hit.distance, new Color(ray.energy, 0, 0), 2000);

        if (!collided) return SoundRayTracingResult.HitNothing;
        if (ray.energy <= 0) return SoundRayTracingResult.HitNothing;
        if (hit.collider == GameManager.Instance.playerController.listenerCollider) return SoundRayTracingResult.HitPlayer;

        newRay.energy *= 1 - Config.materialAbsorption[hit.collider.sharedMaterial.name];

        return SoundRayTracingResult.HitWall;
    }

    public void SetupRays()
    {
        rays = new List<SoundRay>();
        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = Random.value * Mathf.PI * 2;
            float x = Mathf.Cos(angle);
            float z = Mathf.Sin(angle);
            rays.Add(new SoundRay(new Ray(transform.position, new Vector3(x, 0, z))));
        }
    }

    public void SetupFilters(EnvironmentEvaluationResult result)
    {
        echo.delay = result.delay;
        lowPass.cutoffFrequency = result.lowpass * 22000;
        soundObject.source.volume = result.volume;
    }
}

public struct EnvironmentEvaluationResult
{
    public float delay;
    public float lowpass;
    public float volume;
}

public enum SoundRayTracingResult
{
    HitPlayer,
    HitNothing,
    HitWall
}