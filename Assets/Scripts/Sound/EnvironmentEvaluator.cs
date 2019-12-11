using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class EnvironmentEvaluator : MonoBehaviour
{
    public int rayCount = 32;

    List<Ray> rays;

    public float evaluationsPerSecond = 2;
    float lastEvaluationTime;

    public float maxDistance = 100;
    public LayerMask mask;

    float currentRoomSize;

    public AudioMixerGroup audioMixerGroup;
    public float parameterChangeSpeed;

    private void Awake()
    {
        lastEvaluationTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - lastEvaluationTime > 1 / evaluationsPerSecond)
        {
            SetupRays(transform.position);
            EvaluateEnvironment();

            lastEvaluationTime = Time.time;
        }

        SetReverbParameters();
    }

    void EvaluateEnvironment()
    {
        List<Vector3> hitPoints = new List<Vector3>();

        for (int i = 0; i < rayCount; i++)
        {
            if (Physics.Raycast(rays[i], out RaycastHit hit, maxDistance, mask))
                hitPoints.Add(hit.point);
            //Debug.DrawLine(rays[i].origin, hit.point, Color.green, 1 / evaluationsPerSecond);
        }

        float averageDistance = 0;
        foreach (Vector3 point in hitPoints)
            averageDistance += (transform.position - point).sqrMagnitude;
        averageDistance /= hitPoints.Count;

        currentRoomSize = averageDistance;
    }

    void SetupRays(Vector3 origin)
    {
        if (rays == null || rays.Count != rayCount)
        {
            rays = new List<Ray>();
            for (int i = 0; i < rayCount; i++)
            {
                float angle = ((float)i / rayCount) * 2 * Mathf.PI;

                float x = Mathf.Cos(angle);
                float y = Mathf.Sin(angle);

                rays.Add(new Ray(origin, new Vector3(x, 0, y)));
            }
        }
        else
        {
            for (int i = 0; i < rayCount; i++)
                rays[i] = new Ray(origin, rays[i].direction);
        }
    }

    void SetReverbParameters()
    {
        audioMixerGroup.audioMixer.GetFloat("SFX Room", out float currentRoomParam);

        //Debug.Log(currentRoomSize);
        float newRoomParam = -Mathf.Clamp(10000 - currentRoomSize * (currentRoomSize * .67f), 0, 10000);
        //Debug.Log(newRoomParam);
        audioMixerGroup.audioMixer.SetFloat("SFX Room", Mathf.Lerp(currentRoomParam, newRoomParam, parameterChangeSpeed * Time.deltaTime));
    }
}
