using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MText;

public class DebuggerClass : MonoBehaviour
{

    public Animator boardAnimator;
    public FreeSpeechManager _freeSpeechManager;

    public WitListeningStateManager _witListeningStateManager;

    public Modular3DText debugText;
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

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            _freeSpeechManager.HandleFullTranscription("word practice");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            _freeSpeechManager.HandleFullTranscription("phrase practice");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            _freeSpeechManager.HandleFullTranscription("Level 3");
        }

        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            Time.timeScale = 1f;
        }

        if (Input.GetKeyDown(KeyCode.F5)) {
            _witListeningStateManager.TurnWitActivationOffAndOn();
        }
    }

    void ActivateMenuViaVoice() {
        _freeSpeechManager.HandleFullTranscription("menu");
    }
}
