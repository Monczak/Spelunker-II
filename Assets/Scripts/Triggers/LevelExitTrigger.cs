using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExitTrigger : MonoBehaviour
{
    BoxCollider col;

    public LevelConnection connection;

    public delegate void OnPlayerEnterTriggerDelegate(LevelConnection connection);
    public event OnPlayerEnterTriggerDelegate OnPlayerEnterTrigger;

    private void Awake()
    {
        col = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger entered!");
        if (other.gameObject.CompareTag("Player"))
        {
            if (OnPlayerEnterTrigger != null && Input.GetKey(KeyCode.Space))
            {
                OnPlayerEnterTrigger.Invoke(connection);
            }
        }
    }
}
