using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.WitAi;
using Meta.WitAi.Data;
using Meta.WitAi.Json;
public class FreeSpeechManager : MonoBehaviour
{
    [SerializeField] private Wit wit;

    public TMPro.TextMeshPro speechText;

    // Start is called before the first frame update
    void Start()
    {
        wit.Activate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    [MatchIntent("recite_sentence")]
    public void StartSentenceCheck(WitResponseNode response)
    {
        Debug.Log(response);
        var transcription = response.GetTranscription();
        Debug.Log("Transcription");
        Debug.Log(transcription);

        speechText.text = transcription.ToString();
        wit.Activate();
        //StartCoroutine(CheckRecitedSentence(transcription));
    }
}
