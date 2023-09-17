using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuggerClass : MonoBehaviour
{

    public Animator boardAnimator;
    public FreeSpeechManager _freeSpeechManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ActivateMenuViaVoice();
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            _freeSpeechManager.HandleFullTranscription("resume");
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            boardAnimator.SetTrigger("Open");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            boardAnimator.SetTrigger("Close");
        }
    }

    void ActivateMenuViaVoice() {
        _freeSpeechManager.HandleFullTranscription("menu");
    }
}
