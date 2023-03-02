using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.WitAi;
using Meta.WitAi.Data;
using Meta.WitAi.Json;
using Meta.WitAi.Lib;
using Meta.WitAi.Configuration;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Inspectors;

public class FreeSpeechManager : MonoBehaviour
{
    [SerializeField] private Wit wit;

    public TMPro.TextMeshPro partialText;
    public TMPro.TextMeshPro fullText;

    public TMPro.TextMeshPro debugText;

    private WitRuntimeConfiguration runtimeConfig;
    private WitInspector inspector;

    private Mic micInfo;
    // Start is called before the first frame update
    void Start()
    {
        runtimeConfig = wit.RuntimeConfiguration;
        wit.Activate();
    }
    public void MicActivityDetected()
    {

        // Wit should already be activated if mic is detected
  
    }
    
    public void StoppedListening()
    {
        Debug.Log("Stopped!");
        //debugText.text = "Stopped listening";
        StartCoroutine(StartListeningAgain());
    }

    public IEnumerator StartListeningAgain()
    {
        yield return new WaitForSeconds(0.00001f);
        wit.Activate();
    }

    public void StartedListening()
    {
        Debug.Log("Started!");
        debugText.text = "Started listening";
        //wit.Activate();
    }

    public void HandlePartialTranscription(string text)
    {
        Debug.Log("Partial");
        Debug.Log(text);
        partialText.text = text;
    }

    public void HandleFullTranscription(string text)
    {
        StartCoroutine(HandleTranscriptionThenWait(text));
    }

    public IEnumerator HandleTranscriptionThenWait(string text)
    {
        Debug.Log("Full");
        Debug.Log(text);
        fullText.text = fullText.text + '\n' + ' ' + text;
        yield return new WaitForSeconds(0.000001f);
        Debug.Log("Full");
        partialText.text = "(Listening)";
        wit.Activate();

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(wit.MicActive);

    }

    //[MatchIntent("recite_sentence")]
    //public void StartSentenceCheck(WitResponseNode response)
    //{
    //    Debug.Log(response);
    //    var transcription = response.GetTranscription();
    //    Debug.Log("Transcription");
    //    Debug.Log(transcription);

    //    speechText.text = transcription.ToString();
    //    wit.Activate();
    //    //StartCoroutine(CheckRecitedSentence(transcription));
    //}
}
