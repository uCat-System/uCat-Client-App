using UnityEngine;
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
    
        // External classes
        private LevelTransition _levelTransition;
        private LevelManager _levelManager;
        private ConversationManager _conversationManager;
        private UIManager _uiManager;
        private WordReciteManager _wordReciteManager;
        private WitListeningStateManager _witListeningStateManager;
    
        // 3D Text
        private Modular3DText partialText3D;
        private Modular3DText reciteText3D;
        private Modular3DText confirmationText3D;
        private Modular3DText subtitleText3D;

        // Used to cache the text when we are in a confirmation state
        public string cachedText = "";

        public bool isCurrentlyCountingTowardsTimeout;

        public float currentTimeoutTimerInSeconds;

        public int timeoutInSeconds;

        void Start()
        {   
            _wordReciteManager = GetComponent<WordReciteManager>();
            _witListeningStateManager = GetComponent<WitListeningStateManager>();
            _conversationManager = GetComponent<ConversationManager>();
            _uiManager = GetComponent<UIManager>();
            _levelManager = GetComponent<LevelManager>();
            _levelTransition = FindObjectOfType<LevelTransition>();
            reciteText3D = GameObject.FindWithTag("ReciteText3D").GetComponent<Modular3DText>();
            partialText3D = GameObject.FindWithTag("PartialText3D").GetComponent<Modular3DText>();
            confirmationText3D = GameObject.FindWithTag("ConfirmationText3D").GetComponent<Modular3DText>();
            subtitleText3D = GameObject.FindWithTag("SubtitleText3D").GetComponent<Modular3DText>();
        }

        public void HandlePartialTranscription(string text)
        {

            if (_witListeningStateManager.RecitingWordsIsAllowed()) {
                partialText3D.UpdateText(text);
            }

            if (_witListeningStateManager.currentListeningState == EListeningState.ListeningForFreestyleResponse) {
                reciteText3D.UpdateText(text);
            }

            if (_witListeningStateManager.currentListeningState == EListeningState.ListeningForConversationModeInput) {
                _conversationManager.HandlePartialSpeech(text);
            }
        }

        public void HandleMenuActivationResponse(EMenuActivationResponseType menuActivationResponse) {
            switch (menuActivationResponse) {
                case EMenuActivationResponseType.POSITIVE_ACTIVATE_MENU_RESPONSE:
                    _uiManager.ActivateMenu();
                    break;
                case EMenuActivationResponseType.UNKNOWN_ACTIVATION_RESPONSE:
                    break;
                default:
                    break;
            }
        }

        public void HandleFullTranscription(string text)
        {
            // Listen for menu activation
            if (_witListeningStateManager.MenuActivationCommandsAreAllowed()) {
                EMenuActivationResponseType menuActivationResponse = UICommandHandler.CheckIfMenuActivationCommandsWereSpoken(text);
                HandleMenuActivationResponse(menuActivationResponse);
            }

            // Listen for commands within the menu
            if (_witListeningStateManager.MenuNavigationCommandsAreAllowed()) {
                subtitleText3D.UpdateText(text);
                EMenuNavigationResponseType menuNavigationResponse = UICommandHandler.CheckIfMenuNavigationCommandsWereSpoken(text);
                _uiManager.ActivateMenuNavigationCommandsBasedOnResponse(menuNavigationResponse);
            }

            if (_witListeningStateManager.currentListeningState == EListeningState.ListeningForConfirmation) {
                //Listen for 'yes' or 'no?' (confirmation)
                EConfirmationResponseType confirmationResponse = ConfirmationHandler.CheckIfConfirmationWasSpoken(text);
                StartCoroutine(ProceedBasedOnConfirmation(confirmationResponse));
            }

            if (_witListeningStateManager.currentListeningState == EListeningState.ListeningForNextOrRepeat) {
                // Listen for 'next' or 'repeat' (word recite)
                EProceedResponseType proceedResponse = ConfirmationHandler.CheckIfProceedPhraseWasSpoken(text);
                HandleProceedResponse(proceedResponse);
            }
            if (_witListeningStateManager.RecitingWordsIsAllowed()) {
                // Activate Tasks (recite words, etc) if in any valid reciting states
                ActivateTasksBasedOnTranscription(text);
            }

            if (_witListeningStateManager.currentListeningState == EListeningState.ListeningForFreestyleResponse) {
                // Level 3 Task
                HandleFreestyleResponse(text);
            }

            // Add use case for conversation mode
            if (_witListeningStateManager.currentListeningState == EListeningState.ListeningForConversationModeInput) {
                // if we have just received something the user said and they are allowed to speak in convo mode
                _conversationManager.HandleUserSpeech(text);
            }

            else {
                // Turn mic back on if we are in the menu and it didn't recognise anything
                 if (_witListeningStateManager.MenuNavigationCommandsAreAllowed()) {
                    _witListeningStateManager.ReactivateToTryMenuNavigationCommandsAgain();
                 }
            }
        }

        public void HandleProceedResponse(EProceedResponseType proceedResponse) { 
        switch (proceedResponse) {
            case EProceedResponseType.POSITIVE_PROCEED_RESPONSE:
                _levelTransition.BeginLevelTransition();
                break;
            case EProceedResponseType.NEGATIVE_PROCEED_RESPONSE:
                _levelManager.RepeatLevel();
                break;
            case EProceedResponseType.UNKNOWN_PROCEED_RESPONSE:
                Debug.LogError("Unknown proceed response type: " + proceedResponse);
                _witListeningStateManager.TransitionToState(EListeningState.ListeningForConfirmation);
                break;}
    }

        private IEnumerator ProceedBasedOnConfirmation(EConfirmationResponseType confirmationResponse) {

            partialText3D.UpdateText("");
            yield return new WaitForSeconds(ConfirmationHandler.confirmationWaitTimeInSeconds);
            confirmationText3D.UpdateText("");

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
            if (_witListeningStateManager.TimeoutCountingIsAllowed()) {
                isCurrentlyCountingTowardsTimeout = true;
                currentTimeoutTimerInSeconds = 0;
            }
        }

        public void UserSaidSomething() {
            currentTimeoutTimerInSeconds = 0;
            isCurrentlyCountingTowardsTimeout = false;
        }

        public void OnStoppedListening() {
            isCurrentlyCountingTowardsTimeout = false;
        }

        public void OnTimeOut() {
            isCurrentlyCountingTowardsTimeout = false;
        }

        public void OnInactivity() {
            // Do not time out if we are in the menu, etc. Only when reciting.
            if (_witListeningStateManager.TimeoutCountingIsAllowed()) {
                _witListeningStateManager.TransitionToState(EListeningState.NotListening);
                isCurrentlyCountingTowardsTimeout = false;
                currentTimeoutTimerInSeconds = 0;
                _wordReciteManager.OnMicrophoneTimeOut();
            }
        }

        void Update() {
            // Only count seconds if the mic is active and we are reciting words (not in menu etc)
            if (isCurrentlyCountingTowardsTimeout && _witListeningStateManager.TimeoutCountingIsAllowed()) {
                currentTimeoutTimerInSeconds += Time.deltaTime;
            }

            if (currentTimeoutTimerInSeconds > timeoutInSeconds) {
                OnInactivity();
            }
        }

        public void ActivateTasksBasedOnTranscription(string text)
        {        
            if (_levelManager.CurrentLevel != "Level3") 
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

        public void HandleFreestyleResponse(string text) {
            reciteText3D.UpdateText(text);
            ConfirmWhatUserSaid(text);
        }
        
        public void ConfirmWhatUserSaid(string originallyUtteredText) {
            _witListeningStateManager.TransitionToState(EListeningState.ListeningForConfirmation);
            // Ask them to confirm
            confirmationText3D.UpdateText("Did you say \"" + originallyUtteredText + "\"?\n(Yes/No)");
        }

     void CalculateCachedText(string newText) {
        // Prevent the text log becoming too long
        int maxLengthBasedOnScene = 0;
        switch (_levelManager.CurrentLevel) {
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

