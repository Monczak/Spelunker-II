using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCoordCube : MonoBehaviour
{
    public int tileX, tileY;

    // Update is called once per frame
    void Update()
    {
        transform.position = GameManager.Instance.mapGenerator.CoordToWorldPoint(new Coord(tileX, tileY));
    }
}
