using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform objectToFollow;
    public Camera cam;

    [Header("Basic Properties")]
    public float deltaY;
    public float deltaZ;
    public float declinationDeltaY;

    public float smoothing, rotSmoothing;
    public float deadZone;

    private Vector3 vel;
    private Vector3 oldPos;

    [Header("Lookaround")]
    public float lookAroundDistance;
    public float lookAroundSpeed;
    private Vector3 lookAroundVector;

    [Header("Orbit Camera Properties")]
    public float lookDirection;
    public float lookDeclination;

    public float lookDirectionSpeed, lookDeclinationSpeed;

    private bool freezeLookaround;

    [Header("Zoom")]
    public float minZoom;
    public float maxZoom;
    public float zoomSpeed;
    public float zoomSmoothing;

    private float normalizedZoom = .4f;
    private float currentFOV;

    private void Awake()
    {
        oldPos = transform.position;
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (objectToFollow == null) return;

        MouseControl();

        Vector3 targetPos = CalculateTargetPos() + CalculateOrbitCameraDelta();
        Vector3 newPos = Vector3.SmoothDamp(transform.position, targetPos, ref vel, smoothing * Time.deltaTime);
        transform.position = newPos;

        Quaternion targetRot = CalculateTargetRot();
        Quaternion newRot = Quaternion.Slerp(transform.rotation, targetRot, rotSmoothing * Time.deltaTime);
        transform.rotation = newRot;

        SetZoom();
    }

    void MouseControl()
    {
        if (freezeLookaround = Input.GetMouseButton(1))
        {
            lookDirection += -Input.GetAxis("Mouse X") * lookDirectionSpeed;
            lookDeclination += -Input.GetAxis("Mouse Y") * lookDeclinationSpeed;
            lookDeclination = Mathf.Clamp(lookDeclination, 0, 89);
        }

        normalizedZoom -= Input.mouseScrollDelta.y * zoomSpeed;
        normalizedZoom = Mathf.Clamp01(normalizedZoom);
    }

    Vector3 CalculateTargetPos()
    {
        if (freezeLookaround) lookAroundVector = Vector3.zero;
        else
        {
            lookAroundVector = new Vector3(Input.mousePosition.x / Screen.width - .5f, 0, Input.mousePosition.y / Screen.height - .5f) * lookAroundSpeed;
            lookAroundVector = Quaternion.AngleAxis(-lookDirection, Vector3.up) * lookAroundVector;
            lookAroundVector = Vector3.ClampMagnitude(lookAroundVector, lookAroundDistance);
        }       

        Vector3 targetPos = objectToFollow.position + lookAroundVector + Vector3.up * deltaY;

        return targetPos;
    }

    Vector3 CalculateOrbitCameraDelta()
    {
        Vector3 targetPos = Vector3.zero;
        targetPos += new Vector3(Mathf.Sin(lookDirection * Mathf.Deg2Rad) * deltaZ, 0, Mathf.Cos(lookDirection * Mathf.Deg2Rad + Mathf.PI) * deltaZ);
        targetPos += new Vector3(-Mathf.Sin(lookDeclination * Mathf.Deg2Rad) * deltaZ * Mathf.Sin(lookDirection * Mathf.Deg2Rad), Mathf.Sin(lookDeclination * Mathf.Deg2Rad) * declinationDeltaY, Mathf.Sin(lookDeclination * Mathf.Deg2Rad) * deltaZ * Mathf.Cos(lookDirection * Mathf.Deg2Rad));
        return targetPos;
    }

    Quaternion CalculateTargetRot()
    {
        Quaternion targetRot = Quaternion.AngleAxis(-lookDirection, Vector3.up) * Quaternion.AngleAxis(Mathf.Atan((deltaY + Mathf.Sin(lookDeclination * Mathf.Deg2Rad) * declinationDeltaY) / (Mathf.Pow(Mathf.Cos(lookDeclination * Mathf.Deg2Rad), Mathf.Exp(1)) * deltaZ)) * Mathf.Rad2Deg, Vector3.right);

        return targetRot;
    }

    void SetZoom()
    {
        currentFOV = Mathf.Lerp(minZoom, maxZoom, normalizedZoom);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, currentFOV, zoomSmoothing * Time.deltaTime);
    }
}
