using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpLadderAligner : MonoBehaviour
{
    public float checkRadius = 1;

    void AlignToWall(int resolution = 8)
    {
        RaycastHit bestHit = new RaycastHit()
        {
            distance = Mathf.Infinity
        };

        for (int i = 0; i < resolution; i++)
        {
            float angle = i * (2 * Mathf.PI / resolution);
            Vector3 vec = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Ray ray = new Ray(transform.position, vec * checkRadius);
            if (Physics.Raycast(ray, out RaycastHit hit, checkRadius, LayerMask.GetMask("Mineable")))
                if (bestHit.distance > hit.distance)
                    bestHit = hit;
        }

        float finalAngle = Mathf.Atan2(bestHit.normal.x, bestHit.normal.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, finalAngle, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        AlignToWall();
    }
}
