using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.WitAi;
using MText;
using Meta.WitAi.Lib;
using Meta.WitAi.Configuration;
using UnityEngine.SceneManagement;

namespace MText //necessary for the 3D text to work
{ 
    public class FreeSpeechManager : MonoBehaviour
    {


        [SerializeField] private Wit wit;
        [SerializeField] private UIManager uiManager;


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

        private Mic micInfo;

        //Quick hack while I figure out if you can add text to the 3D text
        public string cachedText = "";

        Scene scene;

        void Start()
        {
            scene = SceneManager.GetActiveScene();
            runtimeConfig = wit.RuntimeConfiguration;
            partialText3D.UpdateText("(Listening)");
            StartCoroutine(StartListeningAgain());
        }
        public void MicActivityDetected()
        {
            //Debug.Log("Mic activity!");
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

        public void HandlePartialTranscription(string text)
        {
           // Debug.Log("Partial");
           // Debug.Log(text);
            //partialText.text = text;
            partialText3D.UpdateText(text);
        }

        public void HandleFullTranscription(string text)
        {
            StartCoroutine(HandleTranscriptionThenWait(text));
        }

        void ActivateReciteTask(string text)
        {
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

            wit.Deactivate();
            Debug.Log("Full");
            Debug.Log(text.ToLower());

            if (text.ToLower() == "hey ucat" || text.ToLower() == "hey you cat" || text.ToLower() == "hey you kat")
            {
                Debug.Log("MENU ACTIVATE");
                uiManager.ActivateMenu();
                yield break;
            }
        
            Debug.Log("Continuing");
            // If Level 1 or 2, start checking the appropriate task
            ActivateReciteTask(text);
            CalculateCachedText(text);
            fullText3D.UpdateText(cachedText);
            yield return new WaitForSeconds(0.000001f);
            Debug.Log("Full");
            partialText3D.UpdateText("(Listening)");
            wit.Activate();
        }

     void CalculateCachedText(string newText) {
        // Prevent the text log becoming too long
        int maxLengthBasedOnScene = 0;
        Scene scene = SceneManager.GetActiveScene();
        switch (scene.name)
        {
            case "Level1":
                maxLengthBasedOnScene = 40;
                break;
            case "Level2":
                 maxLengthBasedOnScene = 70;
                 break;
            case "Level3":
                maxLengthBasedOnScene = 120;
                break;
            default:
                maxLengthBasedOnScene = 30;
                break;
        }

        if (cachedText.Length > maxLengthBasedOnScene) {
            cachedText = newText;
        } else {
            cachedText = cachedText + '\n' + ' ' + newText;
        }
    }
    }

   

}
