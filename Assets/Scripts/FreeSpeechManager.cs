using System.Collections;
using UnityEngine;
using Meta.WitAi;
using MText;
using Meta.WitAi.Lib;
using Meta.WitAi.Configuration;
using UnityEngine.SceneManagement;

namespace MText
{ 
    public class FreeSpeechManager : MonoBehaviour
    {


/*

    1) 
*/

        [SerializeField] private Wit wit;
        private UIManager uiManager;


        public WordReciteManager wordReciteManager;
        public SentenceReciteManager sentenceReciteManager;

        public Modular3DText partialText3D;
        public Modular3DText fullText3D;

        public string cachedText = "";

        Scene scene;

        void Start()
        {
            uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
            scene = SceneManager.GetActiveScene();
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
            // partialText3D.UpdateText("(Stopped)");
            // StartCoroutine(StartListeningAgain());
        }

        public IEnumerator StartListeningAgain()
        {
            yield return new WaitForSeconds(0.00001f);
            wit.Activate();
        }

        public void HandlePartialTranscription(string text)
        {
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
            partialText3D.UpdateText("(Stopped)");
            uiManager.CheckIfUICommandsWereSpoken(text.ToLower());
        
            // If Level 1 or 2, start checking the appropriate task
            ActivateReciteTask(text);
            CalculateCachedText(text);
            fullText3D.UpdateText(cachedText);

            yield return new WaitForSeconds(2);
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
