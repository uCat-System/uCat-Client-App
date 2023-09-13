using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuggerClass : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            // IncrementDialogueByOne();
        }
    }

    void IncrementDialogueByOne() {
        Debug.Log("Sup");
        // Increment the current dialogue option index by one
        DialogueHandler.currentDialogueOptionIndex++;
    }
}
