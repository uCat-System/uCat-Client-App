using UnityEngine;
using Oculus.Voice;
using UnityEngine.SceneManagement;
using EListeningState = WitListeningStateManager.ListeningState;
using EConfirmationResponseType = ConfirmationHandler.ConfirmationResponseType;
using EMenuActivationResponseType = UICommandHandler.MenuActivationResponseType;
using EMenuNavigationResponseType = UICommandHandler.MenuNavigationResponseType;
using EProceedResponseType = ConfirmationHandler.ProceedResponseType;
using System.Collections;
using MText;


    public class FreeSpeechManager : MonoBehaviour
    {

        public Material greenText;
        private LevelTransition _levelTransition;
        private LevelManager _levelManager;
        private DialogueManager _dialogueManager;

        private UIManager _uiManager;
        private WordReciteManager _wordReciteManager;
        private WitListeningStateManager _witListeningStateManager;
        private Modular3DText partialText3D;

        private Modular3DText confirmationText3D;
        private Modular3DText subtitleText3D;

        // Used to cache the text when we are in a confirmation state
        private string originallyUtteredText;


        public string cachedText = "";

        public bool isCurrentlyCountingTowardsTimeout;

        public float currentTimeoutTimerInSeconds;

        private AudioSource catAudioSource;

        public int timeoutInSeconds;

        Scene scene;

        void Start()
        {   
            _wordReciteManager = GetComponent<WordReciteManager>();
            _dialogueManager = GetComponent<DialogueManager>();
            _witListeningStateManager = GetComponent<WitListeningStateManager>();
            _uiManager = GetComponent<UIManager>();
            _levelManager = GetComponent<LevelManager>();
            _levelTransition = FindObjectOfType<LevelTransition>();
            partialText3D = GameObject.FindWithTag("PartialText3D").GetComponent<Modular3DText>();
            confirmationText3D = GameObject.FindWithTag("ConfirmationText3D").GetComponent<Modular3DText>();
            subtitleText3D = GameObject.FindWithTag("SubtitleText3D").GetComponent<Modular3DText>();
            catAudioSource = GameObject.FindWithTag("uCat").GetComponent<AudioSource>();
            scene = SceneManager.GetActiveScene();
        }

        public void HandlePartialTranscription(string text)
        {
            // Always update subtitles when attempting speech
            subtitleText3D.UpdateText(text);
            if (_witListeningStateManager.RecitingWordsIsAllowed()) {
                partialText3D.UpdateText(text);
            }
        }

        public void HandleMenuActivationResponse(EMenuActivationResponseType menuActivationResponse) {
            switch (menuActivationResponse) {
                case EMenuActivationResponseType.POSITIVE_ACTIVATE_MENU_RESPONSE:
                    _uiManager.ActivateMenu();
                    break;
                case EMenuActivationResponseType.UNKNOWN_ACTIVATION_RESPONSE:
                    Debug.Log("Activating menu was allowed but phrase was invalid: " + menuActivationResponse);
                    break;
                default:
                    break;
            }
        }

        public void HandleFullTranscription(string text)
        {
            subtitleText3D.UpdateText(text);
            Debug.Log("Handling full transcription: " + text);
            if (_witListeningStateManager.MenuActivationCommandsAreAllowed()) {
                Debug.Log("Menu command allowed: " + text);  
                // Listen for menu activation
                EMenuActivationResponseType menuActivationResponse = UICommandHandler.CheckIfMenuActivationCommandsWereSpoken(text);
                HandleMenuActivationResponse(menuActivationResponse);
            }

            if (_witListeningStateManager.MenuNavigationCommandsAreAllowed()) {
                // Listen for commands within the menu
                Debug.Log("Menu navigation command allowed: " + text);
                EMenuNavigationResponseType menuNavigationResponse = UICommandHandler.CheckIfMenuNavigationCommandsWereSpoken(text);
                _uiManager.ActivateMenuNavigationCommandsBasedOnResponse(menuNavigationResponse);
            }

            if (_witListeningStateManager.currentListeningState == EListeningState.ListeningForConfirmation) {
                Debug.Log("Checking if confirmation was spoken: " + text);
                //Listen for 'yes' or 'no?' (confirmation)
                EConfirmationResponseType confirmationResponse = ConfirmationHandler.CheckIfConfirmationWasSpoken(text);
                StartCoroutine(ProceedBasedOnConfirmation(confirmationResponse, originallyUtteredText));
            }

            if (_witListeningStateManager.currentListeningState == EListeningState.ListeningForNextOrRepeat) {
                // Listen for 'next' or 'repeat' (word recite)
                Debug.Log("Checking if next or repeat was spoken: " + text);
                EProceedResponseType proceedResponse = ConfirmationHandler.CheckIfProceedPhraseWasSpoken(text);
                HandleProceedResponse(proceedResponse, text);
            }
            if (_witListeningStateManager.RecitingWordsIsAllowed()) {
                Debug.Log("About to activate recite stuff");
                // Activate Tasks (recite words, etc) if in any valid reciting states
                ActivateTasksBasedOnTranscription(text);
            }

            else {
                // Turn mic back on if we are in the menu and it didn't recognise anything
                Debug.LogError("Did not activate word task with phrase " + text + " . You are probably in the menu: " + _witListeningStateManager.currentListeningState);
                 if (_witListeningStateManager.MenuNavigationCommandsAreAllowed()) {
                    _witListeningStateManager.ReactivateToTryMenuNavigationCommandsAgain();
                 }
            }
        }

        public void HandleProceedResponse(EProceedResponseType proceedResponse, string originallyUtteredText) { 
        switch (proceedResponse) {
            case EProceedResponseType.POSITIVE_PROCEED_RESPONSE:
                _levelTransition.BeginLevelTransition();
                break;
            case EProceedResponseType.NEGATIVE_PROCEED_RESPONSE:
                _levelManager.RepeatLevel();
                break;
            case EProceedResponseType.UNKNOWN_PROCEED_RESPONSE:
                partialText3D.UpdateText(ConfirmationHandler.proceedResponses[proceedResponse]);
                _witListeningStateManager.TransitionToState(EListeningState.ListeningForConfirmation);
                break;}
    }

        private IEnumerator ProceedBasedOnConfirmation(EConfirmationResponseType confirmationResponse, string originallyUtteredText) {

            string confirmationText = ConfirmationHandler.confirmationResponses[confirmationResponse];
            confirmationText3D.UpdateText(confirmationText);
            yield return new WaitForSeconds(ConfirmationHandler.confirmationWaitTimeInSeconds);

            switch (confirmationResponse) {
                case EConfirmationResponseType.POSITIVE_CONFIRMATION_RESPONSE:
                    _wordReciteManager.MoveOnIfMoreWordsInList();
                    break;
                case EConfirmationResponseType.NEGATIVE_CONFIRMATION_RESPONSE:
                    _wordReciteManager.RepeatSameWord();
                    break;
                case EConfirmationResponseType.UNKNOWN_CONFIRMATION_RESPONSE:
                    // Listen again
                    _witListeningStateManager.TransitionToState(EListeningState.ListeningForConfirmation);
                    break;
                default:
                    Debug.LogError("ERROR: Confirmation response type not recognised");
                    break;
            }
        }

        public void OnStartListening() {
            // Clear the text
            // subtitleText3D.UpdateText("STARTED LISTENING");
            isCurrentlyCountingTowardsTimeout = true;
            if (_witListeningStateManager.TimeoutCountingIsAllowed()) {
                currentTimeoutTimerInSeconds = 0;
            }
        }

        public void UserSaidSomething() {
            // Clear the text
            // subtitleText3D.UpdateText("MIC DATA SENT");
            Debug.Log("MIC DATA SENT");
            currentTimeoutTimerInSeconds = 0;
            isCurrentlyCountingTowardsTimeout = false;
        }

        public void OnStoppedListening() {
            // Clear the text
            // subtitleText3D.UpdateText("STOPPED LISTENING: " + currentTimeoutTimerInSeconds + " seconds");
            isCurrentlyCountingTowardsTimeout = false;
        }

        public void OnTimeOut() {
            // Clear the text
            // subtitleText3D.UpdateText("TIMED OUT " + currentTimeoutTimerInSeconds + " seconds");
            isCurrentlyCountingTowardsTimeout = false;
        }

        public void OnInactivity() {
            // Do not time out if we are in the menu, etc. Only when reciting.
            if (_witListeningStateManager.TimeoutCountingIsAllowed()) {
                _witListeningStateManager.TransitionToState(EListeningState.NotListening);
                isCurrentlyCountingTowardsTimeout = false;
                subtitleText3D.UpdateText("INACTIVITY" + currentTimeoutTimerInSeconds + " seconds");
                currentTimeoutTimerInSeconds = 0;
                _wordReciteManager.OnMicrophoneTimeOut();
            }
        }

        void Update() {
            // Only count seconds if the mic is active and we are reciting words (not in menu etc)
            if (isCurrentlyCountingTowardsTimeout && _witListeningStateManager.TimeoutCountingIsAllowed()) {
                currentTimeoutTimerInSeconds += Time.deltaTime;
                // subtitleText3D.UpdateText("TIMEOUT: " + currentTimeoutTimerInSeconds + " seconds");
            }

            if (currentTimeoutTimerInSeconds > timeoutInSeconds) {
                OnInactivity();
            }
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
                if (_witListeningStateManager.currentListeningState != EListeningState.ListeningForConfirmation) {
                    ConfirmWhatUserSaid(text);
                }
            }
        }
        
        public void ConfirmWhatUserSaid(string originallyUtteredText) {
            Debug.Log("Setting state to confirtmation mode ");
            _witListeningStateManager.TransitionToState(EListeningState.ListeningForConfirmation);
            // Ask them to confirm
            confirmationText3D.UpdateText("Did you say \"" + originallyUtteredText + "\"?\n(Yes/No)");
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

