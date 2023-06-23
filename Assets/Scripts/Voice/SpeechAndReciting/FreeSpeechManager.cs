using System.Collections;
using UnityEngine;
using Meta.WitAi;
using UnityEngine.SceneManagement;
using EState = WitListeningStateMachine.State;

namespace MText
{ 
    public class FreeSpeechManager : MonoBehaviour
    {
        [SerializeField] private Wit wit;
        private UIManager _uiManager;

        public WordReciteManager _wordReciteManager;
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
            _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
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
            if (_witListeningStateManager.currentListeningState == EState.ListeningForEverything) {
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
            _uiManager.CheckIfUICommandsWereSpoken(text.ToLower());

            bool isInConfirmationMode = _witListeningStateManager.currentListeningState == EState.ListeningForConfirmation;

            bool isInReciteMode = _witListeningStateManager.currentListeningState == EState.ListeningForEverything ||
            _witListeningStateManager.currentListeningState == EState.ListeningForRecitedWordsOnly;

            if (isInConfirmationMode) {
                ConfirmationHandler confirmationHandler = new ConfirmationHandler();
                confirmationHandler.CheckIfConfirmationWasSpoken(text, partialText3D, _wordReciteManager, this, originallyUtteredText);
                // StartCoroutine(CheckIfConfirmationWasSpoken(text.ToLower()));
            }
            // 2) Activate Tasks if in recite mode
            else if (isInReciteMode)
                {
                    Debug.Log("activating word task or free recite");
                    ActivateTasksBasedOnTranscription(text);
                }
            else {
                Debug.Log("WRONG state - did not activate word task. You are robably in the menu." + _witListeningStateManager.currentListeningState);
            }
        }

        public void HandleInactivityFailure()
        {
            _wordReciteManager.OnMicrophoneTimeOut();
        }

        public void ActivateTasksBasedOnTranscription(string text)
        {        
            if (SceneManager.GetActiveScene().name != "Level3") 
            {
                Debug.Log("activating word task");
                _wordReciteManager.StartWordCheck(text);
          
            } else {
                // Run level 3 task
                Debug.Log("should be transcribing");
                 // Update the spoken text
                CalculateCachedText(text);
                // Only confirm yes/no if the 'next/proceed' prompt is not active
                if (_wordReciteManager.isDeciding) {
                    Debug.Log("going to check recited word");
                    StartCoroutine(_wordReciteManager.CheckRecitedWord(text));
                } else {
                    ConfirmWhatUserSaid(text.ToLower());
                }
            }

        }

        public void ConfirmWhatUserSaid(string text) {
            Debug.Log("Confirming what user said" + text);
            originallyUtteredText = text;
            _witListeningStateManager.TransitionToState(EState.ListeningForConfirmation);
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
