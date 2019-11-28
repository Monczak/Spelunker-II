using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BetterTagParser;

public class Testing : MonoBehaviour
{
    void Start()
    {
        string testDialogue = "{'lines':[{'elements':[{'text':'blah'}]}]}";

        foreach (string str in DialogueParser.Parse(testDialogue))
            Debug.Log(str);
    }
}
