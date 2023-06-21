using System.Collections;
using UnityEngine;
using Meta.WitAi;
using UnityEngine.SceneManagement;

namespace MText
{ 
    public class FreeSpeechManager : MonoBehaviour
    {
        [SerializeField] private Wit wit;
        private UIManager uiManager;

        public WordReciteManager wordReciteManager;
        public Modular3DText partialText3D;
        public Modular3DText subtitleText3D;
        public Modular3DText fullText3D;

        // Used to cache the text when we are in a confirmation state
        private string originallyUtteredText;

        public WitListeningStateManager _witListeningStateManager;

        public string cachedText = "";

        Scene scene;

        void Start()
        {
            subtitleText3D = GameObject.FindWithTag("SubtitleText3D").GetComponent<Modular3DText>();
            uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
            scene = SceneManager.GetActiveScene();
        }
    
        public void StoppedListeningDueToInactivity()
        {
            Debug.Log("Stopped listening due to inactivity");
            HandleInactivityFailure();
        }

        public void StoppedListeningDueToDeactivation()
        {
            Debug.Log("Stopped listening due to Deactivation");
            // HandleInactivityFailure();
        }

        public void StoppedListeningDueToTimeout()
        {
            Debug.Log("Stopped listening due to timeout");
            HandleInactivityFailure();
        }

        public void HandlePartialTranscription(string text)
        {
            Debug.Log("Partial received: " + text);
            // Always update subtitles when attempting speech
            subtitleText3D.UpdateText(text);
            if (_witListeningStateManager.currentListeningState == "ListeningForEverything") {
                partialText3D.UpdateText(text);
            }
        }

         public void MinimumWakeThresholdHit()
        {
            partialText3D.UpdateText("Heard something");
            Debug.Log("HIT MINIMUM: ");
            
        }

        public void HandleFullTranscription(string text)
        {
            Debug.Log("Receiving full text of " + text);

            // Clear subtitle speech
            // 1) Always listen for menu
            uiManager.CheckIfUICommandsWereSpoken(text.ToLower());

            bool isInConfirmationMode = _witListeningStateManager.currentListeningState == "ListeningForConfirmation";

            bool isInReciteMode = _witListeningStateManager.currentListeningState == "ListeningForEverything" ||
            _witListeningStateManager.currentListeningState == "ListeningForRecitedWordsOnly";

            if (isInConfirmationMode) {
                StartCoroutine(CheckIfConfirmationWasSpoken(text.ToLower()));
            }
            // 2) Activate Tasks if in recite mode
            else if (isInReciteMode)
                {
                    Debug.Log("activating word task or free recite");
                    ActivateTasksBasedOnTranscription(text);
                }
            else {
                Debug.Log("WRONG state - did not activate word task. You are robably in the menu.");
            }
        }

         public IEnumerator CheckIfConfirmationWasSpoken(string text) {
            // text is input as lower case from FreeSpeechManager
            Debug.Log("Checking for confirmation: " + text);
            switch (text)
            {
                case "yes":
                    Debug.Log("Yes was spoken");
                    partialText3D.UpdateText("Cool!");
                    yield return new WaitForSeconds(1);
                    wordReciteManager.MoveOnIfMoreWordsInList();
                    break;
                case "no":
                    Debug.Log("No was spoken");
                    partialText3D.UpdateText("Oops, let's try again.");
                    yield return new WaitForSeconds(1);
                    wordReciteManager.RepeatSameWord();
                    break;
                default:
                    Debug.Log("Something else was spoken");
                    partialText3D.UpdateText("Sorry, I didn't understand that. Please say yes or no.");
                    yield return new WaitForSeconds(2);
                    ConfirmWhatUserSaid(originallyUtteredText);
                    break;
            }
        }

        public void HandleInactivityFailure()
        {
            wordReciteManager.OnMicrophoneTimeOut();
        }

        public void ActivateTasksBasedOnTranscription(string text)
        {        
            if (SceneManager.GetActiveScene().name != "Level3") 
            {
                Debug.Log("activating word task");
                wordReciteManager.StartWordCheck(text);
          
            } else {
                // Run level 3 task
                Debug.Log("should be transcribing");
                 // Update the spoken text
                CalculateCachedText(text);
                // Only confirm yes/no if the 'next/proceed' prompt is not active
                if (wordReciteManager.isDeciding) {
                    Debug.Log("going to check recited word");
                    StartCoroutine(wordReciteManager.CheckRecitedWord(text));
                } else {
                    ConfirmWhatUserSaid(text.ToLower());
                }
            }

        }

         void ConfirmWhatUserSaid(string text) {
            Debug.Log("Confirming what user said" + text);
            originallyUtteredText = text;
            _witListeningStateManager.ChangeState("ListeningForConfirmation");
            // Display what they said on screen
            // partialText3D.UpdateText(text);

            // Ask them to confirm
            partialText3D.UpdateText("Did you say " + text + "?");

            // Change state to listening for confirmation
            // this state should only accept yes or no
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
