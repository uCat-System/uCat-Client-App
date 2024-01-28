using System;
using UnityEngine;
using UnityEngine.UI;
using Meta.WitAi.TTS.Utilities;

public class TTSSpeakerInput : MonoBehaviour
{
    [SerializeField] private TTSSpeaker _speaker;

    
    // Stop speaking
    // private void StopClick() => _speaker.Stop();
    
    // Speak phrase
    public void VocalizeTTS(string trascription)
    {
        _speaker.Speak(trascription);
    }
}

