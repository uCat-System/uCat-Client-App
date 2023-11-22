using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.WitAi;
using Meta.WitAi.TTS.Utilities;

public class TTSTest : MonoBehaviour
{
    public TTSSpeaker _uCatSpeaker;
    // Start is called before the first frame update
    void Start()
    {
        _uCatSpeaker.Speak("Hello there");
    }

    public void Loaded()
    {
        Debug.Log("Loaded");
    }

        public void Ready()
    {
        Debug.Log("Ready");
    }


    public void Started ()
    {
        Debug.Log("Started");
    }

    public void Finished() {
        Debug.Log("Finished");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
