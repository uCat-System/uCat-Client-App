using System;
using UnityEngine;
using MText;
using Meta.WitAi;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class WitListeningStateManager : MonoBehaviour
{
    public enum ListeningState
    // Turn into a struct? TODO
    // maps {ListeningState => appropriate boolean}
    // maybe use quirins boilerplate for static classes
    // eg {ListeningForEverything --> {ableToListen: true, micIcon: true}}

    /*

        struct ListeningState
        {
            public EListeningState Id; --> ListeningForEverything
            public bool AbleToListen; --> 
            public bool ShowMicIcon;
            public Enum[] AllowedTransitions; --> deal with permuations of transitions
        }


        readonly var ListeningState ListeningForEverything = new() {
            Id = EListeningState.ListeningForEverything,
            AbleToListen = true,
            ShowMicIcon = true

        }

        // ListeningForEverything.AbleToListen --> true
        // CurrentState = ListeningForEverything --> fire an event to show mic etc

    */

    {
        NotListening,
        ListeningForEverything,
        ListeningForMenuActivationCommandsOnly, // brings up menu
        ListeningForRecitedWordsOnly,
        ListeningForTaskMenuCommandsOnly, // navigates within the menu
        ListeningForConfirmation,
        ListeningForNextOrRepeat,

        ListeningForLobbyMenuCommandsOnly, // including memo mode
        ListeningForFreestyleResponse,
        ListeningForConversationModeInput,
        WaitingForConversationResponse
    }
    private string scene;
    private UIManager _uiManager;
    public ListeningState currentListeningState;
    private WordReciteManager _wordReciteManager;
    private GameObject wit;

    public float witAutomaticReactivationTimer;

    public GameObject micIcon;

    // This dict will return true if we are in any of the allowed reciting states

    private static Dictionary<ListeningState, bool> validRecitingStates = new Dictionary<ListeningState, bool>
    {
        { ListeningState.ListeningForEverything, true},
        { ListeningState.ListeningForRecitedWordsOnly, true },
    };

    private static Dictionary<ListeningState, bool> validMenuNavigationStates = new Dictionary<ListeningState, bool>
    {
        { ListeningState.ListeningForLobbyMenuCommandsOnly, true },
        { ListeningState.ListeningForTaskMenuCommandsOnly, true },
    };

    private static Dictionary<ListeningState, bool> validMenuActivationStates = new Dictionary<ListeningState, bool>
    {
        { ListeningState.ListeningForEverything, true },
        { ListeningState.ListeningForMenuActivationCommandsOnly, true },
    };

    private static Dictionary<ListeningState, bool> validTimeoutCountingStates = new Dictionary<ListeningState, bool>
    {
        { ListeningState.ListeningForEverything, true },
        { ListeningState.ListeningForRecitedWordsOnly, true },
    };

    private void Start()
    {   
        _uiManager = GetComponent<UIManager>();
        _wordReciteManager = GetComponent<WordReciteManager>();
        wit = GameObject.FindWithTag("Wit");
        scene = SceneManager.GetActiveScene().name;
        if (scene == "ConvoMode") {
            TransitionToState(ListeningState.ListeningForConversationModeInput);
        } else {
            TransitionToState(ListeningState.ListeningForTaskMenuCommandsOnly);
        }
        InvokeRepeating("EnableWitEverySoOften", 0f, witAutomaticReactivationTimer);
    }

    public bool CurrentStateIsAllowedInDictionary(Dictionary<ListeningState, bool> dictToSearch) {
        // Go through every enum value
        foreach (ListeningState state in Enum.GetValues(typeof(ListeningState)))
        {
            if (!dictToSearch.ContainsKey(state))
            {
                // If the state is not in the dictionary, add it and set it to false
                dictToSearch[state] = false;
                // ListeningForTaskMenuCommandsOnly
            }
        }
        // If the current state is in the dictionary, return true or false depending on if it is allowed
        return dictToSearch[currentListeningState];
    }

    public bool RecitingWordsIsAllowed()
    {
        // If the current state is in the dictionary, return true or false depending on if it is allowed
        // This prevents the accidental activation of reciting logic when the user is in a menu or elsewhere
        return CurrentStateIsAllowedInDictionary(validRecitingStates);
    }

    public void ReactivateToTryMenuNavigationCommandsAgain() {
        // This is called when the user says something that is not a valid menu navigation command
        // We want to reactivate the wit listening state manager to try again
        // This is a coroutine because we want to wait a few seconds before reactivating
        StartCoroutine(ReactivateToTryMenuNavigationCommandsAgainCoroutine());
    }

    IEnumerator ReactivateToTryMenuNavigationCommandsAgainCoroutine() {
        // Wait a few seconds before reactivating
        yield return new WaitForSeconds(1);
        // Reactivate
        TransitionToRelevantMenuNavigationStateBasedOnLevel();
    }

    public bool MenuNavigationCommandsAreAllowed() {
        return CurrentStateIsAllowedInDictionary(validMenuNavigationStates);
    }

    public bool MenuActivationCommandsAreAllowed() {
        return CurrentStateIsAllowedInDictionary(validMenuActivationStates);
    }

    public bool TimeoutCountingIsAllowed() {
        return CurrentStateIsAllowedInDictionary(validTimeoutCountingStates);
    }

    public void TransitionToRelevantMenuNavigationStateBasedOnLevel() {
        if (scene != "Lobby") {
            TransitionToState(ListeningState.ListeningForTaskMenuCommandsOnly);
        } else {
            TransitionToState(ListeningState.ListeningForLobbyMenuCommandsOnly);
        }
    }

    public void StoppedListening() {
        Debug.LogError("Stopped! Reactivating");
        StartCoroutine(ResetToCurrentListeningState());
    }

    public IEnumerator ResetToCurrentListeningState() {
           
        // Turn it back on again
        // TODO - improve this - I don't know why it doesn't work without the delay
        yield return new WaitForSeconds(0.00001f);
        TransitionToState(currentListeningState);
    }

    public IEnumerator TurnWitActivationOffAndOn() {
        // Turn it off and on
        // transition was made: this fires, works.
        // wit stops listening, but no transition was requested. 

        wit.SetActive(true);
        Wit witComponent = wit.GetComponent<Wit>();
        witComponent.Deactivate();
        yield return new WaitForSeconds(0.0000001f);
        witComponent.Activate();
    }

    void DisableWit() {
        wit.SetActive(false);
    }

    void EnableWitEverySoOften(){
        // Activate it again.

        if (currentListeningState != ListeningState.WaitingForConversationResponse) {
        Debug.Log("Enabling Wit on timer");
            wit.SetActive(true);
            Wit witComponent = wit.GetComponent<Wit>();
            witComponent.Activate();
        }
    }


    void Update() {
       
    }
 
    // This is called from the WitListeningStateMachine script using actual enum values.

    public void TransitionToState(ListeningState nextState)
        {
            // Once struct in place, switch here using the ListeningState.Id enum
            // micIcon.SetActive(ListeningState.ShowMicIcon);
            
            // Maybe also check the current state to catch bugs
            // eg if transition from a to b is invalid, make it throw something
            switch (nextState)
            {
                case ListeningState.NotListening:
                    DisableWit();
                    micIcon.SetActive(false);
                    break;
                case ListeningState.ListeningForMenuActivationCommandsOnly:
                    // turn on wit here only TODO if AbleToListen is true for this id in the struct.
                    StartCoroutine(TurnWitActivationOffAndOn());
                    micIcon.SetActive(false);
                    break;
                case ListeningState.ListeningForEverything:
                    StartCoroutine(TurnWitActivationOffAndOn());
                    micIcon.SetActive(true);
                    break;
                case ListeningState.ListeningForTaskMenuCommandsOnly:
                    StartCoroutine(TurnWitActivationOffAndOn());
                    micIcon.SetActive(true);
                    break;
                case ListeningState.ListeningForConfirmation:
                    StartCoroutine(TurnWitActivationOffAndOn());
                    micIcon.SetActive(true);
                    break;
                case ListeningState.ListeningForLobbyMenuCommandsOnly:
                    StartCoroutine(TurnWitActivationOffAndOn());
                    micIcon.SetActive(true);
                    break;
                case ListeningState.ListeningForNextOrRepeat:
                    StartCoroutine(TurnWitActivationOffAndOn());
                    micIcon.SetActive(true);
                    break;

                 case ListeningState.ListeningForFreestyleResponse:
                    StartCoroutine(TurnWitActivationOffAndOn());
                    micIcon.SetActive(true);
                    break;

                case ListeningState.ListeningForConversationModeInput:
                    StartCoroutine(TurnWitActivationOffAndOn());
                    micIcon.SetActive(true);
                    break;

                case ListeningState.WaitingForConversationResponse:
                    DisableWit();
                    micIcon.SetActive(false);
                    break;
                default:
                    Debug.LogError("Invalid state transition." + nextState);
                    return;
            }

            currentListeningState = nextState;

            Debug.Log("WitListeningStateMachine transitioned to state: " + nextState);
        }
}
