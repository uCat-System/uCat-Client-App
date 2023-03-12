using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.WitAi;
using Meta.WitAi.Lib;
using Meta.WitAi.Configuration;
using Meta.WitAi.Inspectors;
using UnityEngine.SceneManagement;

namespace MText //necessary for the 3D text to work
{ 
    public class FreeSpeechManager : MonoBehaviour
    {
        [SerializeField] private Wit wit;

        public WordReciteManager wordReciteManager;
        public SentenceReciteManager sentenceReciteManager;

        //Legacy. When TextMeshPro was used as a proof of concept.
        //public TMPro.TextMeshPro partialText;
        //public TMPro.TextMeshPro fullText;

        //Modular 3D Text objects.
        public Modular3DText partialText3D;
        public Modular3DText fullText3D;

        public TMPro.TextMeshPro debugText;

        private WitRuntimeConfiguration runtimeConfig;
        private WitInspector inspector;

        private Mic micInfo;

        //Quick hack while I figure out if you can add text to the 3D text
        private string cachedText = "";

        Scene scene;

        void Start()
        {
            scene = SceneManager.GetActiveScene();
            runtimeConfig = wit.RuntimeConfiguration;
            //partialText.text = "(Listening)";
            partialText3D.UpdateText("(Listening)");
            StartCoroutine(StartListeningAgain());
        }
        public void MicActivityDetected()
        {
            Debug.Log("Mic activity!");
        }
    
        public void StoppedListeningDueToInactivity()
        {
            Debug.Log("Stopped due to inactivity!");
            HandleInactivityFailure();
        }

        public void StoppedListeningDueToTimeout()
        {
            Debug.Log("Stopped due to timeout!");
            HandleInactivityFailure();
        }
        public void StoppedListening()
        {
            Debug.Log("Stopped!");
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
        }

        public void HandlePartialTranscription(string text)
        {
            Debug.Log("Partial");
            Debug.Log(text);
            //partialText.text = text;
            partialText3D.UpdateText(text);
        }

        public void HandleFullTranscription(string text)
        {
            StartCoroutine(HandleTranscriptionThenWait(text));
        }

        void ActivateReciteTask(string text)
        {
            Scene scene = SceneManager.GetActiveScene();
            switch (scene.name)
            {
                case "Level1":
                    wordReciteManager.StartWordCheck(text);
                    break;
                case "Level2":
                    sentenceReciteManager.StartSentenceCheck(text);
                    break;
                default:
                    break;
            }
        }

        void HandleInactivityFailure()
        {
            switch (scene.name)
            {
                case "Level1":
                    wordReciteManager.OnMicrophoneTimeOut();
                    break;
                case "Level2":
                    sentenceReciteManager.OnMicrophoneTimeOut();
                    break;
                default:
                    break;
            }
        }

        public IEnumerator HandleTranscriptionThenWait(string text)
        {
            Debug.Log("Full");
            Debug.Log(text);
        
            // If Level 1 or 2, start checking the appropriate task
            ActivateReciteTask(text);
            //fullText.text = fullText.text + '\n' + ' ' + text;
            cachedText = cachedText + '\n' + ' ' + text;
            fullText3D.UpdateText(cachedText);
            yield return new WaitForSeconds(0.000001f);
            Debug.Log("Full");
            //partialText.text = "(Listening)";
            partialText3D.UpdateText("(Listening)");
            wit.Activate();

        }
   
    }

}
