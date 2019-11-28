using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigboxController : MonoBehaviour
{
    public Coord tileCoord;
    public bool isVisible;
    public float smoothing = 14;
    public float alphaSmoothing = 35;

    public float defaultAlpha = .1f;
    private float currentAlpha;

    public float defaultPulsePower = .05f;

    public Material enabledMat;

    private void Awake()
    {
        currentAlpha = defaultAlpha;
    }

    // Update is called once per frame
    void Update()
    {
        Square square;
        try
        {
            square = GameManager.Instance.meshGenerator.squareGrid.squares[tileCoord.tileX + GameManager.Instance.mapGenerator.borderSize, tileCoord.tileY + GameManager.Instance.mapGenerator.borderSize];
        }
        catch
        {
            return;
        }

        if (square.configuration == 2
            || square.configuration == 4
            || square.configuration == 6
            || square.configuration == 8
            || square.configuration == 10
            || square.configuration == 12 
            || square.configuration == 14 
            || square.configuration == 0)
            isVisible = false;

        Vector3 pos = GameManager.Instance.mapGenerator.CoordToWorldPoint(tileCoord);
        transform.position = Vector3.Lerp(transform.position, pos + Vector3.up * -.01f, smoothing * Time.deltaTime);

        currentAlpha = Mathf.Lerp(currentAlpha, isVisible ? defaultAlpha : 0, alphaSmoothing * Time.deltaTime);

        enabledMat.SetFloat("_OverlayAlpha", currentAlpha);
        enabledMat.SetFloat("_PulsePower", isVisible ? defaultPulsePower : 0);
    }
}
