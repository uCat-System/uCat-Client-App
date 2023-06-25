using UnityEngine;
using Meta.WitAi;
using UnityEngine.SceneManagement;
using EListeningState = WitListeningStateManager.ListeningState;
using EConfirmationResponseType = ConfirmationHandler.ConfirmationResponseType;
using System.Collections;

namespace MText
{
    public class FreeSpeechManager : MonoBehaviour
    {
        public UIManager _uiManager;

        public WordReciteManager _wordReciteManager;
        public Modular3DText partialText3D;
        public Modular3DText subtitleText3D;

        // Used to cache the text when we are in a confirmation state
        private string originallyUtteredText;

        public WitListeningStateManager _witListeningStateManager;

        public string cachedText = "";

        Scene scene;

        void Start()
        {      
            subtitleText3D = GameObject.FindWithTag("SubtitleText3D").GetComponent<Modular3DText>();
            scene = SceneManager.GetActiveScene();
        }
    
        public void StoppedListeningDueToInactivity()
        {
            HandleInactivityFailure();
        }

        public void StoppedListeningDueToDeactivation()
        {
        }

        public void StoppedListeningDueToTimeout()
        {
            HandleInactivityFailure();
        }

        public void HandlePartialTranscription(string text)
        {
            // Always update subtitles when attempting speech
            subtitleText3D.UpdateText(text);
            if (_witListeningStateManager.currentListeningState == EListeningState.ListeningForEverything) {
                partialText3D.UpdateText(text);
            }
        }

         public void MinimumWakeThresholdHit()
        {
            // partialText3D.UpdateText("Heard something");            
        }

        public void HandleFullTranscription(string text)
        {
            // Clear subtitle speech
            // 1) Always listen for menu
            _uiManager.CheckIfUICommandsWereSpoken(text.ToLower());

            bool isInConfirmationMode = _witListeningStateManager.currentListeningState == EListeningState.ListeningForConfirmation;

            bool isInReciteMode = _witListeningStateManager.currentListeningState == EListeningState.ListeningForEverything ||
            _witListeningStateManager.currentListeningState == EListeningState.ListeningForRecitedWordsOnly;

            if (isInConfirmationMode) {
                // "yes"
                EConfirmationResponseType confirmation = ConfirmationHandler.CheckIfConfirmationWasSpoken(text);
                // EConfirmationResponseType (confirmation) == ConfirmationResponseType.POSITIVE_CONFIRMATION_RESPONSE
                StartCoroutine(ProceedBasedOnConfirmation(confirmation, originallyUtteredText));
            }
            // 2) Activate Tasks if in recite mode
            else if (isInReciteMode)
                {
                    ActivateTasksBasedOnTranscription(text);
                }
            else {
                Debug.LogError("WRONG state - did not activate word task. You are robably in the menu." + _witListeningStateManager.currentListeningState);
            }
        }

        private IEnumerator ProceedBasedOnConfirmation(EConfirmationResponseType responseType, string originallyUtteredText) {

            string confirmationText = ConfirmationHandler.confirmationResponses[responseType];
            partialText3D.UpdateText(confirmationText);
            yield return new WaitForSeconds(ConfirmationHandler.confirmationWaitTimeInSeconds);

            // if yes, move on
            if (responseType == EConfirmationResponseType.POSITIVE_CONFIRMATION_RESPONSE) {
                _wordReciteManager.MoveOnIfMoreWordsInList();
            }
            // If no, repeat the word
            else if (responseType == EConfirmationResponseType.NEGATIVE_CONFIRMATION_RESPONSE) {
                _wordReciteManager.RepeatSameWord();
            }
            // If neither, repeat confirmation attempt
            else if (responseType == EConfirmationResponseType.UNKNOWN_CONFIRMATION_RESPONSE) {
                ConfirmWhatUserSaid(originallyUtteredText);
            }

            else {
                Debug.LogError("ERROR: Confirmation response type not recognised");
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
                _wordReciteManager.StartWordCheck(text);
          
            } else {
                // Run level 3 task
                 // Update the spoken text
                CalculateCachedText(text);
                // Only confirm yes/no if the 'next/proceed' prompt is not active
                if (_wordReciteManager.isDecidingToProceedOrNot) {
                    StartCoroutine(_wordReciteManager.CheckRecitedWord(text));
                } else {
                    ConfirmWhatUserSaid(text.ToLower());
                }
            }

        }

        public void ConfirmWhatUserSaid(string text) {
            originallyUtteredText = text;
            Debug.Log("Setting state to confirtmation mode ");
            _witListeningStateManager.TransitionToState(EListeningState.ListeningForConfirmation);

            // Ask them to confirm
            partialText3D.UpdateText("Did you say " + text + "?");
        }

     void CalculateCachedText(string newText) {
        // Prevent the text log becoming too long
        int maxLengthBasedOnScene = 0;
        Scene scene = SceneManager.GetActiveScene();
        switch (scene.name) {
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
                break;}

        if (cachedText.Length > maxLengthBasedOnScene) {
            cachedText = newText;
        } else {
            cachedText = cachedText + '\n' + ' ' + newText;
        }
    }
    }
}
